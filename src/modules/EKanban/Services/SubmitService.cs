using System;
using System.Threading.Tasks;
using EKanban.IRepositories;
using EKanban.IServices;
using EKanban.Models;
using EKanban.Specs;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.Services;

public class SubmitService : ISubmitService, IDependency
{
    private readonly IExecutionCardRepository _executionCardRepositories;
    private readonly IExecutionRunRepository _executionRunRepositories;
    private readonly ITaskPhaseProgressRepository _taskPhaseProgressRepositories;
    private readonly ITaskFileChangeRepository _taskFileChangeRepositories;
    private readonly IStateMachineService _stateMachineService;
    private readonly ISpecGenerator _specGenerator;
    private readonly ISpecEvaluator _specEvaluator;
    private readonly ILogger<SubmitService> _logger;

    public SubmitService(
        IExecutionCardRepository executionCardRepositories,
        IExecutionRunRepository executionRunRepositories,
        ITaskPhaseProgressRepository taskPhaseProgressRepositories,
        ITaskFileChangeRepository taskFileChangeRepositories,
        IStateMachineService stateMachineService,
        ISpecGenerator specGenerator,
        ISpecEvaluator specEvaluator,
        ILogger<SubmitService> logger)
    {
        _executionCardRepositories = executionCardRepositories;
        _executionRunRepositories = executionRunRepositories;
        _taskPhaseProgressRepositories = taskPhaseProgressRepositories;
        _taskFileChangeRepositories = taskFileChangeRepositories;
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

        var card = await _executionCardRepositories.FindFirstAsync(p => p.Id == cardId);
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

        _executionRunRepositories.Add(run, true);

        // Complete current phase and move to next phase if any phase is in progress
        var allPhases = await _taskPhaseProgressRepositories.GetByExecutionCardIdAsync(cardId);
        var currentPhase = allPhases.FirstOrDefault(p => p.Status == PhaseStatus.InProgress);
        if (currentPhase != null)
        {
            // Mark current phase as completed
            currentPhase.Status = PhaseStatus.Completed;
            currentPhase.CompletedAt = DateTime.UtcNow;
            currentPhase.OutputDocPath = output;
            _taskPhaseProgressRepositories.Update<TaskPhaseProgress>(currentPhase, (string[])null!, false);
            await _taskPhaseProgressRepositories.SaveChangesAsync();
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
                _taskPhaseProgressRepositories.Update<TaskPhaseProgress>(nextPhase, (string[])null!, false);
                await _taskPhaseProgressRepositories.SaveChangesAsync();
                _logger.LogInformation($"Auto-started next phase {nextPhase.Phase} for card {cardId}");
            }
        }

        // If no Spec exists yet, generate one
        if (!card.SpecId.HasValue)
        {
            _logger.LogInformation($"Generating initial Spec for card {cardId}");
            var spec = await _specGenerator.GenerateSpecAsync(card);
            card.SpecId = spec.Id;
            _executionCardRepositories.Update<ExecutionCard>(card, (string[])null!, false);
            await _executionCardRepositories.SaveChangesAsync();
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
