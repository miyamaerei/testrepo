using System.Collections.Generic;
using VOL.Core.BaseProvider;
using System.Threading.Tasks;
using VOL.Entity.DomainModels;

namespace EKanban.IServices
{
    public interface IAzureBoardsClient
    {
        Task<List<BoardWorkItem>> GetAllWorkItemsAsync();
        Task<BoardWorkItem> GetWorkItemByIdAsync(int azureWorkItemId);
        Task AddCommentAsync(int azureWorkItemId, string comment);
        Task UpdateStateAsync(int azureWorkItemId, string state);
    }
}
