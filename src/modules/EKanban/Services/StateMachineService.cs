using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EKanban.IRepositories;
using EKanban.IServices;
using EKanban.Models;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.Services;

public class StateMachineService : IStateMachineService, IDependency
{
    private readonly IExecutionCardRepository _executionCardRepositories;
    private readonly ILogger<StateMachineService> _logger;

    // Define allowed transitions
    private readonly Dictionary<ExecutionCardStatus, HashSet<ExecutionCardStatus>> _allowedTransitions = new()
    {
        { ExecutionCardStatus.New, new HashSet<ExecutionCardStatus> { ExecutionCardStatus.Ready } },
        { ExecutionCardStatus.Ready, new HashSet<ExecutionCardStatus> { ExecutionCardStatus.InProgress, ExecutionCardStatus.Failed } },
        { ExecutionCardStatus.InProgress, new HashSet<ExecutionCardStatus> { ExecutionCardStatus.Submitted, ExecutionCardStatus.Failed } },
        { ExecutionCardStatus.Submitted, new HashSet<ExecutionCardStatus> { ExecutionCardStatus.Completed, ExecutionCardStatus.Ready, ExecutionCardStatus.Failed } },
        { ExecutionCardStatus.Completed, new HashSet<ExecutionCardStatus> { } }, // Terminal state
        { ExecutionCardStatus.Failed, new HashSet<ExecutionCardStatus> { ExecutionCardStatus.Ready } } // Can retry from failed
    };

    public StateMachineService(
        IExecutionCardRepository executionCardRepositories,
        ILogger<StateMachineService> logger)
    {
        _executionCardRepositories = executionCardRepositories;
        _logger = logger;
    }

    public Task<bool> CanTransitionAsync(ExecutionCard card, ExecutionCardStatus newStatus)
    {
        var currentStatus = card.Status;
        var allowed = _allowedTransitions.ContainsKey(currentStatus)
            && _allowedTransitions[currentStatus].Contains(newStatus);

        return Task.FromResult(allowed);
    }

    public async Task TransitionToAsync(ExecutionCard card, ExecutionCardStatus newStatus)
    {
        var canTransition = await CanTransitionAsync(card, newStatus);
        if (!canTransition)
        {
            var currentStatus = card.Status;
            _logger.LogWarning($"Invalid transition from {currentStatus} to {newStatus} for card {card.Id}");
            throw new InvalidOperationException($"Cannot transition from {currentStatus} to {newStatus}");
        }

        card.Status = newStatus;
        card.LastUpdated = DateTime.UtcNow;

        if (newStatus == ExecutionCardStatus.InProgress)
        {
            card.InProgressStartTime = DateTime.UtcNow;
        }

        _executionCardRepositories.Update<EKanban.Models.ExecutionCard>(card, (string[])null!, false);
        await _executionCardRepositories.SaveChangesAsync();
        _logger.LogInformation($"Card {card.Id} transitioned from {card.Status} to {newStatus}");
    }

    public async Task StartAiExecutionAsync(ExecutionCard card)
    {
        await TransitionToAsync(card, ExecutionCardStatus.InProgress);
    }

    public async Task CompleteAiExecutionAsync(ExecutionCard card, bool isSpecPassed)
    {
        if (isSpecPassed)
        {
            await TransitionToAsync(card, ExecutionCardStatus.Completed);
        }
        else
        {
            card.FailureCount++;
            await TransitionToAsync(card, ExecutionCardStatus.Ready);
        }

        _executionCardRepositories.Update<EKanban.Models.ExecutionCard>(card, (string[])null!, false);
        await _executionCardRepositories.SaveChangesAsync();
    }
}
