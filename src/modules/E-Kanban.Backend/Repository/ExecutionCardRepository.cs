using SqlSugar;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class ExecutionCardRepository : BaseRepository<ExecutionCard>, IExecutionCardRepository
{
    public ExecutionCardRepository(SqlSugarClient db) : base(db)
    {
    }

    public async Task<Dictionary<ExecutionCardStatus, List<ExecutionCard>>> GetCardsGroupedByStatusAsync()
    {
        var allCards = await _db.Queryable<ExecutionCard>()
            .OrderByDescending(c => c.LastUpdated)
            .ToListAsync();
        
        return allCards.GroupBy(c => c.Status)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<List<ExecutionCard>> GetInProgressAiCardsAsync()
    {
        return await _db.Queryable<ExecutionCard>()
            .Where(c => c.Status == ExecutionCardStatus.InProgress)
            .Where(c => c.ExecutorType == ExecutorType.AI)
            .ToListAsync();
    }

    public async Task<List<ExecutionCard>> GetReadyAiCardsAsync()
    {
        return await _db.Queryable<ExecutionCard>()
            .Where(c => c.Status == ExecutionCardStatus.Ready)
            .Where(c => c.ExecutorType == ExecutorType.AI)
            .Where(c => !c.NeedsManualIntervention)
            .ToListAsync();
    }
}
