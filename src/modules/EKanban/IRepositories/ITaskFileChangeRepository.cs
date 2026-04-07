using EKanban.Models;
using VOL.Core.BaseProvider;

namespace EKanban.IRepositories;

public interface ITaskFileChangeRepository : IRepository<TaskFileChange>, IDependency
{
    /// <summary>
    /// 根据执行卡片 ID 获取所有文件变更记录
    /// </summary>
    Task<List<TaskFileChange>> GetByExecutionCardIdAsync(int executionCardId);
}
