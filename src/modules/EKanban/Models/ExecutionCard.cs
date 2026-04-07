using SqlSugar;
using VOL.Entity.SystemModels;

namespace EKanban.Models;

/// <summary>
/// 执行卡片状态枚举
/// </summary>
public enum ExecutionCardStatus
{
    /// <summary>
    /// 新建，待就绪
    /// </summary>
    New = 0,
    
    /// <summary>
    /// 就绪，可以开始执行
    /// </summary>
    Ready = 1,
    
    /// <summary>
    /// 执行中
    /// </summary>
    InProgress = 2,
    
    /// <summary>
    /// 已提交，等待 Spec 校验
    /// </summary>
    Submitted = 3,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// 失败
    /// </summary>
    Failed = 5
}

/// <summary>
/// 执行者类型枚举
/// </summary>
public enum ExecutorType
{
    /// <summary>
    /// 人工执行
    /// </summary>
    Human = 0,
    
    /// <summary>
    /// AI 执行（GitHub Copilot CLI）
    /// </summary>
    AI = 1,
    
    /// <summary>
    /// 系统自动执行
    /// </summary>
    System = 2
}

/// <summary>
/// Kanban 执行卡片
/// </summary>
[SugarTable("ExecutionCards")]
public class ExecutionCard : BaseEntity
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    /// <summary>
    /// 关联的 BoardWorkItem ID
    /// </summary>
    public int BoardWorkItemId { get; set; }
    
    /// <summary>
    /// 所属 Board ID
    /// </summary>
    public string BoardId { get; set; } = string.Empty;
    
    /// <summary>
    /// 卡片标题（从 BoardWorkItem 同步）
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 卡片描述（从 BoardWorkItem 同步）
    /// </summary>
    [SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]
    public string? Description { get; set; }
    
    /// <summary>
    /// 当前状态
    /// </summary>
    public ExecutionCardStatus Status { get; set; } = ExecutionCardStatus.New;
    
    /// <summary>
    /// 执行者类型
    /// </summary>
    public ExecutorType ExecutorType { get; set; } = ExecutorType.Human;
    
    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 关联的 Spec ID
    /// </summary>
    public int? SpecId { get; set; }
    
    /// <summary>
    /// 失败重试次数
    /// </summary>
    public int FailureCount { get; set; } = 0;
    
    /// <summary>
    /// 是否需要人工干预
    /// </summary>
    public bool NeedsManualIntervention { get; set; } = false;
    
    /// <summary>
    /// 进入 InProgress 状态开始时间（用于超时检测）
    /// </summary>
    public DateTime? InProgressStartTime { get; set; }
    
    /// <summary>
    /// 关联的项目仓库 ID
    /// </summary>
    public int? ProjectRepositoryId { get; set; }
}
