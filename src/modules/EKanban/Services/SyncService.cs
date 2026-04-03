using System.Linq;
using System;
using System.Threading.Tasks;
using EKanban.IServices;
using EKanban.IRepositories;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public class SyncService : ISyncService, IDependency
    {
        private readonly IBoardWorkItemRepository _boardWorkItemRepository;
        private readonly IExecutionCardRepository _executionCardRepository;
        private readonly IAzureBoardsClient _azureBoardsClient;
        private readonly ILogger<SyncService> _logger;

        public SyncService(
            IBoardWorkItemRepository boardWorkItemRepository,
            IExecutionCardRepository executionCardRepository,
            IAzureBoardsClient azureBoardsClient,
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
                    var existing = await _boardWorkItemRepository.FindSingleAsync(
                        w => w.AzureWorkItemId == azureItem.AzureWorkItemId);

                    if (existing == null)
                    {
                        // New work item - insert
                        await _boardWorkItemRepository.AddAsync(azureItem);
                        _logger.LogInformation($"Added new work item: {azureItem.Title} (ID: {azureItem.AzureWorkItemId})");

                        // Create corresponding ExecutionCard
                        var card = new ExecutionCard
                        {
                            BoardWorkItemId = azureItem.BoardWorkItemId,
                            Title = azureItem.Title,
                            Description = azureItem.Description,
                            Status = (int)ExecutionCardStatus.New,
                            ExecutorType = (int)ExecutorType.AI,
                            FailureCount = 0,
                            NeedsManualIntervention = false,
                            CreatedDate = DateTime.UtcNow,
                            LastUpdated = DateTime.UtcNow
                        };

                        await _executionCardRepository.AddAsync(card);
                        _logger.LogInformation($"Created execution card for work item {azureItem.AzureWorkItemId}");
                    }
                    else
                    {
                        // Existing - update
                        existing.Title = azureItem.Title;
                        existing.Description = azureItem.Description;
                        existing.AzureState = azureItem.AzureState;
                        existing.LastSyncDate = DateTime.UtcNow;

                        await _boardWorkItemRepository.UpdateAsync(existing);
                        _logger.LogInformation($"Updated existing work item: {azureItem.Title} (ID: {azureItem.AzureWorkItemId})");
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
}
