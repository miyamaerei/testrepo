using System.Threading.Tasks;

namespace EKanban.IServices;

public interface IAiTaskCheckService
{
    Task CheckInProgressTasksAsync();
}
