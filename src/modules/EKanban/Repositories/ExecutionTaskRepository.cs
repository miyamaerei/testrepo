using SqlSugar;
using EKanban.Models;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using VOL.Entity.DomainModels;
using EKanban.IRepositories;

namespace EKanban.Repositories
{
    public partial class ExecutionTaskRepository : RepositoryBase<EKanban.Models.ExecutionTask>, IExecutionTaskRepository
    {
        public ExecutionTaskRepository(VOLContext dbContext) : base(dbContext)
        {
        }
    }
}
