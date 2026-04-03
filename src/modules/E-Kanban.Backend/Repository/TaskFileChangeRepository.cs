using SqlSugar;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class TaskFileChangeRepository : BaseRepository<TaskFileChange>, ITaskFileChangeRepository
{
    public TaskFileChangeRepository(SqlSugarClient db) : base(db)
    {
    }

    public async Task<List<TaskFileChange>> GetByExecutionCardIdAsync(int executionCardId)
    {
        return await _db.Queryable<TaskFileChange>()
            .Where(f => f.ExecutionCardId == executionCardId)
            .OrderByDescending(f => f.ChangedAt)
            .ToListAsync();
    }
}
