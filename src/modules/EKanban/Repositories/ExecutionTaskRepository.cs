using SqlSugar;
using VOL.Core.DbContext;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using EKanban.IRepositories;

namespace EKanban.Repositories
{
    public partial class ExecutionTaskRepository : RepositoryBase<ExecutionTask>, IExecutionTaskRepository
    {
        public ExecutionTaskRepository(VOLContext dbContext) : base(dbContext)
        {
        }
    }
}
