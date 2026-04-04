using EKanban.IServices;
using Microsoft.Extensions.Logging;

namespace EKanban.Jobs;

public class AiTaskCheckJob
{
    private readonly IAiTaskCheckService _aiTaskCheckService;
    private readonly ILogger<AiTaskCheckJob> _logger;

    public AiTaskCheckJob(
        IAiTaskCheckService aiTaskCheckService,
        ILogger<AiTaskCheckJob> logger)
    {
        _aiTaskCheckService = aiTaskCheckService;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Starting scheduled AI task timeout check");
        try
        {
            await _aiTaskCheckService.CheckInProgressTasksAsync();
            _logger.LogInformation("Scheduled AI task timeout check completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scheduled AI task check failed");
        }
    }
}
