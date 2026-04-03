using System.Linq;
using SqlSugar;
using VOL.Core.DbContext;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using EKanban.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EKanban.Repositories
{
    public partial class ExecutionCardRepository : RepositoryBase<ExecutionCard>, IExecutionCardRepository
    {
        public ExecutionCardRepository(VOLContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<ExecutionCard>> GetInProgressAiCardsAsync()
        {
            return await VOLContext.Set<ExecutionCard>()
                .Where(c => c.Status == (int)ExecutionCardStatus.InProgress)
                .Where(c => c.ExecutorType == (int)ExecutorType.AI)
                .ToListAsync();
        }

        public async Task<List<ExecutionCard>> GetReadyAiCardsAsync()
        {
            return await VOLContext.Set<ExecutionCard>()
                .Where(c => c.Status == (int)ExecutionCardStatus.Ready)
                .Where(c => c.ExecutorType == (int)ExecutorType.AI)
                .Where(c => !c.NeedsManualIntervention)
                .ToListAsync();
        }

        public async Task<ExecutionCard> FindOneAsync(int id)
        {
            return await DbContext.Queryable<ExecutionCard>().In(id).FirstAsync();
        }

        public async Task AddAsync(ExecutionCard card)
        {
            await DbContext.Insertable(card).ExecuteCommandAsync();
        }

        public async Task UpdateAsync(ExecutionCard card)
        {
            await DbContext.Updateable(card).ExecuteCommandAsync();
        }

        public async Task<List<ExecutionCard>> GetAllAsync()
        {
            return await VOLContext.Set<ExecutionCard>().ToListAsync();
        }
    }
}
