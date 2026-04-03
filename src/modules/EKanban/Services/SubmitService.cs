using System.Linq;
using System;
using System.Threading.Tasks;
using EKanban.IRepositories;
using EKanban.IServices;
using EKanban.Specs;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public class SubmitService : ISubmitService, IDependency
    {
        private readonly IExecutionCardRepository _executionCardRepository;
        private readonly IExecutionRunRepository _executionRunRepository;
        private readonly IStateMachineService _stateMachineService;
        private readonly ISpecGenerator _specGenerator;
        private readonly ISpecEvaluator _specEvaluator;
        private readonly ILogger<SubmitService> _logger;

        public SubmitService(
            IExecutionCardRepository executionCardRepository,
            IExecutionRunRepository executionRunRepository,
            IStateMachineService stateMachineService,
            ISpecGenerator specGenerator,
            ISpecEvaluator specEvaluator,
            ILogger<SubmitService> logger)
        {
            _executionCardRepository = executionCardRepository;
            _executionRunRepository = executionRunRepository;
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

            var card = await _executionCardRepository.FindOneAsync(cardId);
            if (card == null)
            {
                throw new ArgumentException($"Execution card {cardId} not found");
            }

            // Create execution run record
            var run = new ExecutionRun
            {
                ExecutionCardId = cardId,
                ExecutionTaskId = null,
                ExecutorName = executorName,
                ExecutorType = executorType,
                InputPrompt = null,
                OutputResult = output,
                Evidence = evidence,
                StartTime = card.InProgressStartTime ?? DateTime.UtcNow,
                EndTime = DateTime.UtcNow,
                DurationMs = (long)(DateTime.UtcNow - (card.InProgressStartTime ?? DateTime.UtcNow)).TotalMilliseconds,
                IsSuccess = true,
                ErrorMessage = null,
                CreatedDate = DateTime.UtcNow
            };

            await _executionRunRepository.AddAsync(run);

            // If no Spec exists yet, generate one
            if (!card.CurrentSpecId.HasValue)
            {
                _logger.LogInformation($"Generating initial Spec for card {cardId}");
                var spec = await _specGenerator.GenerateSpecAsync(card);
                card.CurrentSpecId = spec.SpecId;
                await _executionCardRepository.UpdateAsync(card);
            }

            // Evaluate against Spec
            var evaluationResult = await _specEvaluator.EvaluateAsync(
                card.CurrentSpecId.Value,
                run.ExecutionRunId,
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
                NewStatus = (ExecutionCardStatus)card.Status
            };
        }
    }
}
