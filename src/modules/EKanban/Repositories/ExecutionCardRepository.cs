using SqlSugar;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using EKanban.Models;
using EKanban.IRepositories;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.Repositories;

public class ExecutionCardRepository : RepositoryBase<ExecutionCard>, IExecutionCardRepository, IDependency
{
    public ExecutionCardRepository(VOLContext dbContext) : base(dbContext)
    {
    }

    public async Task<Dictionary<ExecutionCardStatus, List<ExecutionCard>>> GetCardsGroupedByStatusAsync()
    {
        var allCards = await DbContext.Queryable<ExecutionCard>()
            .OrderByDescending(c => c.LastUpdated)
            .ToListAsync();
        
        return allCards.GroupBy(c => c.Status)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<List<ExecutionCard>> GetInProgressAiCardsAsync()
    {
        return await DbContext.Queryable<ExecutionCard>()
            .Where(c => c.Status == ExecutionCardStatus.InProgress)
            .Where(c => c.ExecutorType == ExecutorType.AI)
            .ToListAsync();
    }

    public async Task<List<ExecutionCard>> GetReadyAiCardsAsync()
    {
        return await DbContext.Queryable<ExecutionCard>()
            .Where(c => c.Status == ExecutionCardStatus.Ready)
            .Where(c => c.ExecutorType == ExecutorType.AI)
            .Where(c => !c.NeedsManualIntervention)
            .ToListAsync();
    }
}
