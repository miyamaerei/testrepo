using System.Threading.Tasks;

namespace E_Kanban.Backend.IServices;

public interface IAiTaskCheckService
{
    Task CheckInProgressTasksAsync();
}
