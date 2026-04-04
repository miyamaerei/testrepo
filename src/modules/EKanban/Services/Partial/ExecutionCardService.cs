using EKanban.Models;
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
        public async Task<List<EKanban.Models.ExecutionCard>> GetInProgressAiCardsAsync()
        {
            return await ((IExecutionCardRepository)repository).GetInProgressAiCardsAsync();
        }

        public async Task TriggerReExecuteAsync(int cardId)
        {
            var card = await ((IExecutionCardRepository)repository).FindFirstAsync(c => c.Id == cardId);
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
            if (card.Status == EKanban.Models.ExecutionCardStatus.InProgress)
            {
                // Already in progress, just reset the start time
                card.InProgressStartTime = System.DateTime.UtcNow;
                ((IExecutionCardRepository)repository).Update<EKanban.Models.ExecutionCard>(card, (string[])null!, false);
                await ((IExecutionCardRepository)repository).SaveChangesAsync();
            }
            else if (card.Status != EKanban.Models.ExecutionCardStatus.Completed)
            {
                // Go back to Ready
                card.Status = EKanban.Models.ExecutionCardStatus.Ready;
                card.LastUpdated = System.DateTime.UtcNow;
                ((IExecutionCardRepository)repository).Update<EKanban.Models.ExecutionCard>(card, (string[])null!, false);
                await ((IExecutionCardRepository)repository).SaveChangesAsync();
            }
        }
    }
}
