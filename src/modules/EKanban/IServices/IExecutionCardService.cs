using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using VOL.Core.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using EKanban.IRepositories;

namespace EKanban.IServices
{
    public partial interface IExecutionCardService : IService<ExecutionCard>
    {
        Task<List<ExecutionCard>> GetInProgressAiCardsAsync();
        Task TriggerReExecuteAsync(int cardId);
    }
}
