using E_Kanban.Backend.IServices;
using Microsoft.Extensions.Logging;

namespace E_Kanban.Backend.Jobs;

public class SyncFromAzureBoardsJob
{
    private readonly ISyncService _syncService;
    private readonly ILogger<SyncFromAzureBoardsJob> _logger;

    public SyncFromAzureBoardsJob(
        ISyncService syncService,
        ILogger<SyncFromAzureBoardsJob> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Starting scheduled sync from Azure Boards");
        try
        {
            await _syncService.SyncFromAzureBoardsAsync();
            _logger.LogInformation("Scheduled sync from Azure Boards completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scheduled sync from Azure Boards failed");
            // Don't throw - Hangfire will mark it as failed
        }
    }
}
