using System;
using System.Threading.Tasks;
using E_Kanban.Backend.IRepository;
using E_Kanban.Backend.IServices;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.Specs;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace E_Kanban.Backend.Services;

public class SubmitService : ISubmitService, IDependency
{
    private readonly IExecutionCardRepository _executionCardRepository;
    private readonly IExecutionRunRepository _executionRunRepository;
    private readonly ITaskPhaseProgressRepository _taskPhaseProgressRepository;
    private readonly ITaskFileChangeRepository _taskFileChangeRepository;
    private readonly IStateMachineService _stateMachineService;
    private readonly ISpecGenerator _specGenerator;
    private readonly ISpecEvaluator _specEvaluator;
    private readonly ILogger<SubmitService> _logger;

    public SubmitService(
        IExecutionCardRepository executionCardRepository,
        IExecutionRunRepository executionRunRepository,
        ITaskPhaseProgressRepository taskPhaseProgressRepository,
        ITaskFileChangeRepository taskFileChangeRepository,
        IStateMachineService stateMachineService,
        ISpecGenerator specGenerator,
        ISpecEvaluator specEvaluator,
        ILogger<SubmitService> logger)
    {
        _executionCardRepository = executionCardRepository;
        _executionRunRepository = executionRunRepository;
        _taskPhaseProgressRepository = taskPhaseProgressRepository;
        _taskFileChangeRepository = taskFileChangeRepository;
        _stateMachineService = stateMachineService;
        _specGenerator = specGenerator;
        _specEvaluator = specEvaluator;
        _logger = logger;
    }

    public async Task<SubmitResult> SubmitExecutionResultAsync(
        int cardId,
        int executorType,
        string executorName,
        string evidence,
        string output)
    {
        _logger.LogInformation($"Submitting execution result for card {cardId}");

        var card = await _executionCardRepository.GetByIdAsync(cardId);
        if (card == null)
        {
            throw new ArgumentException($"Execution card {cardId} not found");
        }

        // Create execution run record
        var run = new ExecutionRun
        {
            ExecutionCardId = cardId,
            SubmittedBy = executorName,
            Evidence = evidence,
            StartTime = card.InProgressStartTime ?? DateTime.UtcNow,
            EndTime = DateTime.UtcNow,
            SubmittedAt = DateTime.UtcNow
        };

        if (card.InProgressStartTime.HasValue)
        {
            run.DurationMs = (long)(DateTime.UtcNow - card.InProgressStartTime.Value).TotalMilliseconds;
        }

        await _executionRunRepository.InsertAsync(run);

        // Complete current phase and move to next phase if any phase is in progress
        var allPhases = await _taskPhaseProgressRepository.GetByExecutionCardIdAsync(cardId);
        var currentPhase = allPhases.FirstOrDefault(p => p.Status == PhaseStatus.InProgress);
        if (currentPhase != null)
        {
            // Mark current phase as completed
            currentPhase.Status = PhaseStatus.Completed;
            currentPhase.CompletedAt = DateTime.UtcNow;
            currentPhase.OutputDocPath = output;
            await _taskPhaseProgressRepository.UpdateAsync(currentPhase);
            _logger.LogInformation($"Completed phase {currentPhase.Phase} for card {cardId}");

            // Check if there's a next phase and start it
            var nextPhase = allPhases
                .Where(p => p.Phase > currentPhase.Phase && p.Status == PhaseStatus.NotStarted)
                .OrderBy(p => p.Phase)
                .FirstOrDefault();
            
            if (nextPhase != null)
            {
                nextPhase.Status = PhaseStatus.InProgress;
                nextPhase.StartedAt = DateTime.UtcNow;
                await _taskPhaseProgressRepository.UpdateAsync(nextPhase);
                _logger.LogInformation($"Auto-started next phase {nextPhase.Phase} for card {cardId}");
            }
        }

        // If no Spec exists yet, generate one
        if (!card.SpecId.HasValue)
        {
            _logger.LogInformation($"Generating initial Spec for card {cardId}");
            var spec = await _specGenerator.GenerateSpecAsync(card);
            card.SpecId = spec.Id;
            await _executionCardRepository.UpdateAsync(card);
        }

        // Evaluate against Spec
        var evaluationResult = await _specEvaluator.EvaluateAsync(
            card.SpecId.Value,
            run.Id,
            evidence);

        // Transition state based on evaluation
        if (evaluationResult.IsPassed)
        {
            await _stateMachineService.CompleteAiExecutionAsync(card, true);
            _logger.LogInformation($"Spec evaluation passed for card {cardId}, marking as completed");
        }
        else
        {
            await _stateMachineService.CompleteAiExecutionAsync(card, false);
            _logger.LogInformation($"Spec evaluation failed for card {cardId}, back to ready for next iteration");
        }

        return new SubmitResult
        {
            IsSuccess = true,
            IsSpecPassed = evaluationResult.IsPassed,
            EvaluationResult = evaluationResult.EvaluationResult,
            NewStatus = card.Status
        };
    }
}
