using SqlSugar;
using VOL.Entity.SystemModels;

namespace EKanban.Models;

/// <summary>
/// Spec 评估结果
/// </summary>
public enum EvaluationResult
{
    /// <summary>
    /// 通过
    /// </summary>
    Passed = 0,
    
    /// <summary>
    /// 失败
    /// </summary>
    Failed = 1
}

/// <summary>
/// Spec 评估记录
/// </summary>
[SugarTable("SpecEvaluations")]
public class SpecEvaluation : BaseEntity
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    /// <summary>
    /// 关联的 ExecutionRun ID
    /// </summary>
    public int ExecutionRunId { get; set; }
    
    /// <summary>
    /// 关联的 Spec ID
    /// </summary>
    public int SpecId { get; set; }
    
    /// <summary>
    /// 评估结果
    /// </summary>
    public EvaluationResult Result { get; set; }
    
    /// <summary>
    /// 评估消息（为什么通过/失败）
    /// </summary>
    [SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]
    public string? Message { get; set; }
    
    /// <summary>
    /// 评估时间
    /// </summary>
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}
