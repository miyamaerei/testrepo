using SqlSugar;

namespace E_Kanban.Backend.Models;

/// <summary>
/// 开发阶段枚举
/// </summary>
public enum DevelopmentPhase
{
    /// <summary>
    /// 需求结构化分析
    /// </summary>
    RequirementsAnalysis = 1,
    
    /// <summary>
    /// 本地代码资产盘点
    /// </summary>
    CodeInventory = 2,
    
    /// <summary>
    /// 需求-代码映射
    /// </summary>
    RequirementsCodeMapping = 3,
    
    /// <summary>
    /// 增量开发实施
    /// </summary>
    IncrementalDevelopment = 4,
    
    /// <summary>
    /// 验证测试
    /// </summary>
    VerificationTesting = 5,
    
    /// <summary>
    /// 知识沉淀
    /// </summary>
    KnowledgeSettle = 6
}

/// <summary>
/// 阶段状态枚举
/// </summary>
public enum PhaseStatus
{
    /// <summary>
    /// 未开始
    /// </summary>
    NotStarted = 0,
    
    /// <summary>
    /// 进行中
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 2
}

/// <summary>
/// 任务阶段进度跟踪实体，记录每个开发阶段的进度状态
/// </summary>
[SugarTable("TaskPhaseProgress")]
public class TaskPhaseProgress
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
    /// 开发阶段
    /// </summary>
    public DevelopmentPhase Phase { get; set; }
    
    /// <summary>
    /// 阶段状态
    /// </summary>
    public PhaseStatus Status { get; set; }
    
    /// <summary>
    /// 阶段输出文档路径
    /// </summary>
    public string? OutputDocPath { get; set; }
    
    /// <summary>
    /// 阶段日志
    /// </summary>
    [SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]
    public string? Logs { get; set; }
    
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartedAt { get; set; }
    
    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
