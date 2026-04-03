using System.Threading.Tasks;

namespace E_Kanban.Backend.IServices;

public interface ISyncService
{
    Task SyncFromAzureBoardsAsync();
}
