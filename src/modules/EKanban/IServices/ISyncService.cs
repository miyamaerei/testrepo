using System.Threading.Tasks;
using VOL.Core.BaseProvider;

namespace EKanban.IServices
{
    public interface ISyncService
    {
        Task SyncFromAzureBoardsAsync();
    }
}
