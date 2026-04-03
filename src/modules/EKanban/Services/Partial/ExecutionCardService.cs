using EKanban.IServices;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using EKanban.IRepositories;

namespace EKanban.Services
{
    public partial class ExecutionCardService : IExecutionCardService
    {
        public async Task<List<ExecutionCard>> GetInProgressAiCardsAsync()
        {
            return await ((IExecutionCardRepository)repository).GetInProgressAiCardsAsync();
        }

        public async Task TriggerReExecuteAsync(int cardId)
        {
            var card = await ((IExecutionCardRepository)repository).FindOneAsync(cardId);
            if (card == null)
            {
                throw new System.ArgumentException($"Card {cardId} not found");
            }

            // Reset failure count if it was needing manual intervention
            if (card.NeedsManualIntervention)
            {
                card.FailureCount = 0;
                card.NeedsManualIntervention = false;
            }

            // Transition back to Ready to trigger another execution
            if (card.Status == (int)ExecutionCardStatus.InProgress)
            {
                // Already in progress, just reset the start time
                card.InProgressStartTime = System.DateTime.UtcNow;
                await ((IExecutionCardRepository)repository).UpdateAsync(card);
            }
            else if (card.Status != (int)ExecutionCardStatus.Completed)
            {
                // Go back to Ready
                card.Status = (int)ExecutionCardStatus.Ready;
                card.LastUpdated = System.DateTime.UtcNow;
                await ((IExecutionCardRepository)repository).UpdateAsync(card);
            }
        }
    }
}
