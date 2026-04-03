using System.Linq;
using System;
using System.Threading.Tasks;
using Quartz;
using EKanban.IServices;
using Microsoft.Extensions.Logging;

namespace EKanban.Jobs
{
    public class SyncFromAzureBoardsJob : IJob
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

        public async Task Execute(IJobExecutionContext context)
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
                // Don't throw, job will be marked as failed but don't break the scheduler
            }
        }
    }
}
