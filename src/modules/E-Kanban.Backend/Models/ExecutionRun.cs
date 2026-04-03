using SqlSugar;

namespace E_Kanban.Backend.Models;

/// <summary>
/// 执行事实（一次提交记录）
/// </summary>
[SugarTable("ExecutionRuns")]
public class ExecutionRun
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    /// <summary>
    /// 关联的 ExecutionTask ID
    /// </summary>
    public int ExecutionTaskId { get; set; }
    
    /// <summary>
    /// 关联的 ExecutionCard ID
    /// </summary>
    public int ExecutionCardId { get; set; }
    
    /// <summary>
    /// 提交者（用户名 / AI / System）
    /// </summary>
    public string SubmittedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// 提交时间
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 执行证据 / 结果输出
    /// </summary>
    [SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]
    public string? Evidence { get; set; }
    
    /// <summary>
    /// 命令退出码（如果是 CLI 执行）
    /// </summary>
    public int? ExitCode { get; set; }
    
    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long? DurationMs { get; set; }
    
    /// <summary>
    /// 执行开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }
    
    /// <summary>
    /// 执行结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
