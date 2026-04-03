using SqlSugar;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class ExecutionRunRepository : BaseRepository<ExecutionRun>, IExecutionRunRepository
{
    public ExecutionRunRepository(SqlSugarClient db) : base(db)
    {
    }
}
