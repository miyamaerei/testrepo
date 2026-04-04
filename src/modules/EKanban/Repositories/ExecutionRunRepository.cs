using SqlSugar;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using EKanban.Models;
using EKanban.IRepositories;

namespace EKanban.Repositories;

public class ExecutionRunRepository : RepositoryBase<ExecutionRun>, IExecutionRunRepository
{
    public ExecutionRunRepository(VOLContext dbContext) : base(dbContext)
    {
    }
}
