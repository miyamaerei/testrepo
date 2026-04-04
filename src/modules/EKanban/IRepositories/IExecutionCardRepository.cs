using EKanban.Models;
using VOL.Core.BaseProvider;

namespace EKanban.IRepositories;

public interface IExecutionCardRepository : IRepository<ExecutionCard>
{
    /// <summary>
    /// 按状态分组获取所有卡片
    /// </summary>
    Task<Dictionary<ExecutionCardStatus, List<ExecutionCard>>> GetCardsGroupedByStatusAsync();
    
    /// <summary>
    /// 获取所有 InProgress 状态的 AI 任务
    /// </summary>
    Task<List<ExecutionCard>> GetInProgressAiCardsAsync();
    
    /// <summary>
    /// 获取所有 Ready 状态等待执行的 AI 任务
    /// </summary>
    Task<List<ExecutionCard>> GetReadyAiCardsAsync();
}
