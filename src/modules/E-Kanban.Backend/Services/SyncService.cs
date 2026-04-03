using System;
using System.Threading.Tasks;
using E_Kanban.Backend.IRepository;
using E_Kanban.Backend.IServices;
using E_Kanban.Backend.Models;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace E_Kanban.Backend.Services;

public class SyncService : ISyncService, IDependency
{
    private readonly IBoardWorkItemRepository _boardWorkItemRepository;
    private readonly IExecutionCardRepository _executionCardRepository;
    private readonly AzureBoardsClient _azureBoardsClient;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        IBoardWorkItemRepository boardWorkItemRepository,
        IExecutionCardRepository executionCardRepository,
        AzureBoardsClient azureBoardsClient,
        ILogger<SyncService> logger)
    {
        _boardWorkItemRepository = boardWorkItemRepository;
        _executionCardRepository = executionCardRepository;
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
                var existing = await _boardWorkItemRepository.FindByExternalIdAsync(azureItem.Id);

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

                    var newBoardWorkItemId = await _boardWorkItemRepository.InsertAsync(boardWorkItem);
                    boardWorkItem.Id = newBoardWorkItemId;
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

                    var newCardId = await _executionCardRepository.InsertAsync(card);
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

                    await _boardWorkItemRepository.UpdateAsync(existing);
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
