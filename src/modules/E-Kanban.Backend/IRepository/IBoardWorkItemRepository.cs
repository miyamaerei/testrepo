using E_Kanban.Backend.Models;

namespace E_Kanban.Backend.IRepository;

public interface IBoardWorkItemRepository : IBaseRepository<BoardWorkItem>
{
    Task<BoardWorkItem?> FindByExternalIdAsync(int externalId);
}
