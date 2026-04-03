using E_Kanban.Backend.Models;

namespace E_Kanban.Backend.IRepository;

public interface ITaskPhaseProgressRepository : IBaseRepository<TaskPhaseProgress>
{
    /// <summary>
    /// 根据执行卡片 ID 获取所有阶段进度
    /// </summary>
    Task<List<TaskPhaseProgress>> GetByExecutionCardIdAsync(int executionCardId);
    
    /// <summary>
    /// 根据执行卡片 ID 和阶段获取进度记录
    /// </summary>
    Task<TaskPhaseProgress?> GetByExecutionCardAndPhaseAsync(int executionCardId, DevelopmentPhase phase);
}
