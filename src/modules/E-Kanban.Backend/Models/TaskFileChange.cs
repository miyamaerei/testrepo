using SqlSugar;

namespace E_Kanban.Backend.Models;

/// <summary>
/// 文件变更类型枚举
/// </summary>
public enum ChangeType
{
    /// <summary>
    /// 新增文件
    /// </summary>
    Added = 0,
    
    /// <summary>
    /// 修改文件
    /// </summary>
    Modified = 1,
    
    /// <summary>
    /// 删除文件
    /// </summary>
    Deleted = 2
}

/// <summary>
/// 任务文件变更记录实体，记录每个任务新增/修改/删除了哪些文件
/// </summary>
[SugarTable("TaskFileChanges")]
public class TaskFileChange
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    /// <summary>
    /// 关联的执行卡片 ID
    /// </summary>
    public int ExecutionCardId { get; set; }
    
    /// <summary>
    /// 文件路径（相对项目目录）
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// 变更类型
    /// </summary>
    public ChangeType ChangeType { get; set; }
    
    /// <summary>
    /// Git 提交哈希
    /// </summary>
    public string? CommitHash { get; set; }
    
    /// <summary>
    /// 变更时间
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
