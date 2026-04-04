using EKanban.Models;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using VOL.Core.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using EKanban.IRepositories;

namespace EKanban.IServices
{
    public partial interface IExecutionCardService : IService<EKanban.Models.ExecutionCard>
    {
        Task<List<EKanban.Models.ExecutionCard>> GetInProgressAiCardsAsync();
        Task TriggerReExecuteAsync(int cardId);
    }
}
