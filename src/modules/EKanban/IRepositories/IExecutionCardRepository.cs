using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EKanban.IRepositories
{
    public partial interface IExecutionCardRepository : IRepository<ExecutionCard>
    {
        Task<List<ExecutionCard>> GetInProgressAiCardsAsync();
        Task<List<ExecutionCard>> GetReadyAiCardsAsync();
        Task<List<ExecutionCard>> GetAllAsync();
        Task<ExecutionCard> FindOneAsync(int id);
        Task AddAsync(ExecutionCard card);
        Task UpdateAsync(ExecutionCard card);
    }
}
