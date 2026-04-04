using SqlSugar;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using EKanban.Models;
using EKanban.IRepositories;

namespace EKanban.Repositories;

public class BoardWorkItemRepository : RepositoryBase<BoardWorkItem>, IBoardWorkItemRepository
{
    public BoardWorkItemRepository(VOLContext dbContext) : base(dbContext)
    {
    }

    public async Task<BoardWorkItem?> FindByExternalIdAsync(int externalId)
    {
        return await DbContext.Queryable<BoardWorkItem>()
            .Where(w => w.ExternalWorkItemId == externalId)
            .FirstAsync();
    }
}
