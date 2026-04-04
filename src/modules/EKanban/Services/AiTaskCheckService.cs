using System;
using System.Threading.Tasks;
using EKanban.IRepositories;
using EKanban.IServices;
using EKanban.Models;
using EKanban.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.Services;

public class AiTaskCheckService : IAiTaskCheckService, IDependency
{
    private readonly IExecutionCardRepository _executionCardRepositories;
    private readonly IStateMachineService _stateMachineService;
    private readonly ILogger<AiTaskCheckService> _logger;
    private readonly int _timeoutMinutes;
    private readonly int _maxAutoRetries;

    public AiTaskCheckService(
        IExecutionCardRepository executionCardRepositories,
        IStateMachineService stateMachineService,
        IConfiguration configuration,
        ILogger<AiTaskCheckService> logger)
    {
        _executionCardRepositories = executionCardRepositories;
        _stateMachineService = stateMachineService;
        _logger = logger;

        _timeoutMinutes = configuration.GetValue<int>("AiExecution:InProgressTimeoutMinutes", 120);
        _maxAutoRetries = configuration.GetValue<int>("AiExecution:MaxAutoRetries", 3);
    }

    public async Task CheckInProgressTasksAsync()
    {
        _logger.LogInformation("Starting check for overdue AI tasks...");

        var inProgressCards = await _executionCardRepositories.GetInProgressAiCardsAsync();
        _logger.LogInformation($"Found {inProgressCards.Count} AI tasks in progress");

        int timedOutCount = 0;
        int retryCount = 0;
        int manualInterventionCount = 0;

        foreach (var card in inProgressCards)
        {
            if (!card.InProgressStartTime.HasValue)
            {
                continue;
            }

            var timeInProgress = DateTime.UtcNow - card.InProgressStartTime.Value;
            if (timeInProgress.TotalMinutes > _timeoutMinutes)
            {
                timedOutCount++;
                _logger.LogWarning($"Card {card.Id} timed out after {timeInProgress.TotalMinutes:F1} minutes in progress");

                if (card.FailureCount < _maxAutoRetries)
                {
                    // Auto retry: transition back to Ready for another attempt
                    card.FailureCount++;
                    card.NeedsManualIntervention = false;
                    await _stateMachineService.TransitionToAsync(card, ExecutionCardStatus.Ready);
                    retryCount++;
                    _logger.LogInformation($"Auto retry scheduled for card {card.Id} (failure {card.FailureCount} of {_maxAutoRetries})");
                }
                else
                {
                    // Exceeded max retries, need manual intervention
                    card.NeedsManualIntervention = true;
                    _executionCardRepositories.Update<EKanban.Models.ExecutionCard>(card, (string[])null!, false);
                    await _executionCardRepositories.SaveChangesAsync();
                    manualInterventionCount++;
                    _logger.LogWarning($"Card {card.Id} exceeded max retries ({_maxAutoRetries}), needs manual intervention");
                }
            }
        }

        _logger.LogInformation(
            "AI task check completed. " +
            "Timed out: {TimedOut}, Retried: {Retried}, Need manual intervention: {Manual}",
            timedOutCount, retryCount, manualInterventionCount);
    }
}
