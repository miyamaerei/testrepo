using SqlSugar;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using EKanban.Models;
using EKanban.IRepositories;

namespace EKanban.Repositories;

public class TaskFileChangeRepository : RepositoryBase<TaskFileChange>, ITaskFileChangeRepository
{
    public TaskFileChangeRepository(VOLContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<TaskFileChange>> GetByExecutionCardIdAsync(int executionCardId)
    {
        return await DbContext.Queryable<TaskFileChange>()
            .Where(f => f.ExecutionCardId == executionCardId)
            .OrderByDescending(f => f.ChangedAt)
            .ToListAsync();
    }
}
