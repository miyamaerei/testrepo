using SqlSugar;
using VOL.Core.DbContext;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using EKanban.IRepositories;
using System.Threading.Tasks;

namespace EKanban.Repositories
{
    public partial class ExecutionRunRepository : RepositoryBase<ExecutionRun>, IExecutionRunRepository
    {
        public ExecutionRunRepository(VOLContext dbContext) : base(dbContext)
        {
        }

        public async Task<ExecutionRun> FindOneAsync(int id)
        {
            return await DbContext.Queryable<ExecutionRun>().In(id).FirstAsync();
        }

        public async Task AddAsync(ExecutionRun run)
        {
            await DbContext.Insertable(run).ExecuteCommandAsync();
        }
    }
}
