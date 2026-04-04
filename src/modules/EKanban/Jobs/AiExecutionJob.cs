using EKanban.AiExecution;
using EKanban.IRepositories;
using EKanban.Models;
using Microsoft.Extensions.Logging;

namespace EKanban.Jobs;

public class AiExecutionJob
{
    private readonly IExecutionCardRepository _executionCardRepositories;
    private readonly IAiExecutionService _aiExecutionService;
    private readonly ILogger<AiExecutionJob> _logger;

    public AiExecutionJob(
        IExecutionCardRepository executionCardRepositories,
        IAiExecutionService aiExecutionService,
        ILogger<AiExecutionJob> logger)
    {
        _executionCardRepositories = executionCardRepositories;
        _aiExecutionService = aiExecutionService;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Starting scheduled AI execution for ready tasks");

        try
        {
            // Get all AI tasks that are in Ready state
            var readyCards = await _executionCardRepositories.GetReadyAiCardsAsync();

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
                    _logger.LogInformation($"Skipping card {card.Id} - needs manual intervention");
                    continue;
                }

                try
                {
                    await _aiExecutionService.ExecuteAiTaskAsync(card);
                    _logger.LogInformation($"AI execution completed for card {card.Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"AI execution failed for card {card.Id}");
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
