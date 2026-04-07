using EKanban.Models;
using VOL.Core.BaseProvider;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.IRepositories;

public interface IBoardWorkItemRepository : IRepository<BoardWorkItem>, IDependency
{
    Task<BoardWorkItem?> FindByExternalIdAsync(int externalId);
}
