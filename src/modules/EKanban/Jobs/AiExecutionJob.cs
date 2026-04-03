using System.Linq;
using System;
using System.Threading.Tasks;
using Quartz;
using EKanban.AiExecution;
using EKanban.IRepositories;
using Microsoft.Extensions.Logging;
using VOL.Entity.DomainModels;

namespace EKanban.Jobs
{
    public class AiExecutionJob : IJob
    {
        private readonly IExecutionCardRepository _executionCardRepository;
        private readonly IAiExecutionService _aiExecutionService;
        private readonly ILogger<AiExecutionJob> _logger;

        public AiExecutionJob(
            IExecutionCardRepository executionCardRepository,
            IAiExecutionService aiExecutionService,
            ILogger<AiExecutionJob> logger)
        {
            _executionCardRepository = executionCardRepository;
            _aiExecutionService = aiExecutionService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Starting scheduled AI execution for ready tasks");

            try
            {
                // Get all AI tasks that are in Ready state
                var readyCards = await _executionCardRepository.GetReadyAiCardsAsync();

                if (!readyCards.Any())
                {
                    _logger.LogInformation("No ready AI tasks to execute");
                    return;
                }

                _logger.LogInformation($"Found {readyCards.Count} ready AI tasks to execute");

                foreach (var card in readyCards)
                {
                    if (card.NeedsManualIntervention)
                    {
                        _logger.LogInformation($"Skipping card {card.ExecutionCardId} - needs manual intervention");
                        continue;
                    }

                    try
                    {
                        await _aiExecutionService.ExecuteAiTaskAsync(card);
                        _logger.LogInformation($"AI execution completed for card {card.ExecutionCardId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"AI execution failed for card {card.ExecutionCardId}");
                        // Continue with next card
                    }
                }

                _logger.LogInformation("Scheduled AI execution completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled AI execution failed");
            }
        }
    }
}
