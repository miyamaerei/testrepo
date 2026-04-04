using System;
using System.Threading.Tasks;
using EKanban.IRepositories;
using EKanban.IServices;
using EKanban.Models;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.Services;

public class SyncService : ISyncService, IDependency
{
    private readonly IBoardWorkItemRepository _boardWorkItemRepositories;
    private readonly IExecutionCardRepository _executionCardRepositories;
    private readonly AzureBoardsClient _azureBoardsClient;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        IBoardWorkItemRepository boardWorkItemRepositories,
        IExecutionCardRepository executionCardRepositories,
        AzureBoardsClient azureBoardsClient,
        ILogger<SyncService> logger)
    {
        _boardWorkItemRepositories = boardWorkItemRepositories;
        _executionCardRepositories = executionCardRepositories;
        _azureBoardsClient = azureBoardsClient;
        _logger = logger;
    }

    public async Task SyncFromAzureBoardsAsync()
    {
        _logger.LogInformation("Starting sync from Azure Boards...");

        try
        {
            var workItems = await _azureBoardsClient.GetAllWorkItemsAsync();

            foreach (var azureItem in workItems)
            {
                // Check if this work item already exists
                var existing = await _boardWorkItemRepositories.FindByExternalIdAsync(azureItem.Id);

                if (existing == null)
                {
                    // New work item - insert
                    var boardWorkItem = new BoardWorkItem
                    {
                        ExternalWorkItemId = azureItem.Id,
                        Title = azureItem.Title,
                        Description = azureItem.Description,
                        ExternalState = azureItem.State,
                        LastSyncedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _boardWorkItemRepositories.Add(boardWorkItem, true);
                    _logger.LogInformation($"Added new work item: {azureItem.Title} (ID: {azureItem.Id})");

                    // Create corresponding ExecutionCard
                    var card = new ExecutionCard
                    {
                        BoardWorkItemId = boardWorkItem.Id,
                        Title = azureItem.Title,
                        Description = azureItem.Description,
                        Status = ExecutionCardStatus.New,
                        ExecutorType = ExecutorType.AI,
                        FailureCount = 0,
                        NeedsManualIntervention = false,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    _executionCardRepositories.Add(card, true);
                    _logger.LogInformation($"Created execution card for work item {azureItem.Id}");
                }
                else
                {
                    // Existing - update
                    existing.Title = azureItem.Title;
                    existing.Description = azureItem.Description;
                    existing.ExternalState = azureItem.State;
                    existing.LastSyncedAt = DateTime.UtcNow;
                    existing.UpdatedAt = DateTime.UtcNow;

                    _boardWorkItemRepositories.Update<BoardWorkItem>(existing, (string[])null!, true);
                    await _boardWorkItemRepositories.SaveChangesAsync();
                    _logger.LogInformation($"Updated existing work item: {azureItem.Title} (ID: {azureItem.Id})");
                }
            }

            _logger.LogInformation($"Sync completed. Processed {workItems.Count} work items.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync from Azure Boards failed");
            throw;
        }
    }
}
