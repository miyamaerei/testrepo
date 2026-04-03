using SqlSugar;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class BoardWorkItemRepository : BaseRepository<BoardWorkItem>, IBoardWorkItemRepository
{
    public BoardWorkItemRepository(SqlSugarClient db) : base(db)
    {
    }

    public async Task<BoardWorkItem?> FindByExternalIdAsync(int externalId)
    {
        return await _db.Queryable<BoardWorkItem>()
            .Where(w => w.ExternalWorkItemId == externalId)
            .FirstAsync();
    }
}
