using SqlSugar;

namespace E_Kanban.Backend.Models;

/// <summary>
/// 执行定义任务
/// </summary>
[SugarTable("ExecutionTasks")]
public class ExecutionTask
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    /// <summary>
    /// 关联的 ExecutionCard ID
    /// </summary>
    public int ExecutionCardId { get; set; }
    
    /// <summary>
    /// 执行者类型
    /// </summary>
    public ExecutorType ExecutorType { get; set; }
    
    /// <summary>
    /// 执行指令（Prompt）
    /// </summary>
    [SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]
    public string? ExecutionInstructions { get; set; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
