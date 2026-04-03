using SqlSugar;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class TaskPhaseProgressRepository : BaseRepository<TaskPhaseProgress>, ITaskPhaseProgressRepository
{
    public TaskPhaseProgressRepository(SqlSugarClient db) : base(db)
    {
    }

    public async Task<List<TaskPhaseProgress>> GetByExecutionCardIdAsync(int executionCardId)
    {
        return await _db.Queryable<TaskPhaseProgress>()
            .Where(p => p.ExecutionCardId == executionCardId)
            .OrderBy(p => p.Phase)
            .ToListAsync();
    }

    public async Task<TaskPhaseProgress?> GetByExecutionCardAndPhaseAsync(int executionCardId, DevelopmentPhase phase)
    {
        return await _db.Queryable<TaskPhaseProgress>()
            .Where(p => p.ExecutionCardId == executionCardId)
            .Where(p => p.Phase == phase)
            .FirstAsync();
    }
}
