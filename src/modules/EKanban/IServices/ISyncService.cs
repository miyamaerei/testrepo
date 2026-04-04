using System.Threading.Tasks;

namespace EKanban.IServices;

public interface ISyncService
{
    Task SyncFromAzureBoardsAsync();
}
