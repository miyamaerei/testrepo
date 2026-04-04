using SqlSugar;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using EKanban.Models;
using EKanban.IRepositories;

namespace EKanban.Repositories;

public class TaskPhaseProgressRepository : RepositoryBase<TaskPhaseProgress>, ITaskPhaseProgressRepository
{
    public TaskPhaseProgressRepository(VOLContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<TaskPhaseProgress>> GetByExecutionCardIdAsync(int executionCardId)
    {
        return await DbContext.Queryable<TaskPhaseProgress>()
            .Where(p => p.ExecutionCardId == executionCardId)
            .OrderBy(p => p.Phase)
            .ToListAsync();
    }

    public async Task<TaskPhaseProgress?> GetByExecutionCardAndPhaseAsync(int executionCardId, DevelopmentPhase phase)
    {
        return await DbContext.Queryable<TaskPhaseProgress>()
            .Where(p => p.ExecutionCardId == executionCardId)
            .Where(p => p.Phase == phase)
            .FirstAsync();
    }
}
