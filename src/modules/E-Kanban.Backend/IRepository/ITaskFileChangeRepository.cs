using E_Kanban.Backend.Models;

namespace E_Kanban.Backend.IRepository;

public interface ITaskFileChangeRepository : IBaseRepository<TaskFileChange>
{
    /// <summary>
    /// 根据执行卡片 ID 获取所有文件变更记录
    /// </summary>
    Task<List<TaskFileChange>> GetByExecutionCardIdAsync(int executionCardId);
}
