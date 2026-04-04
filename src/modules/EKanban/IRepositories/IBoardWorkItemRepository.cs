using EKanban.Models;
using VOL.Core.BaseProvider;

namespace EKanban.IRepositories;

public interface IBoardWorkItemRepository : IRepository<BoardWorkItem>
{
    Task<BoardWorkItem?> FindByExternalIdAsync(int externalId);
}
