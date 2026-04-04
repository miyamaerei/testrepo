using SqlSugar;
using VOL.Entity.SystemModels;

namespace EKanban.Models;

/// <summary>
/// 完成标准规范定义
/// </summary>
[SugarTable("Specs")]
public class Spec : BaseEntity
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
    /// Spec 定义（由 AI 生成）
    /// </summary>
    [SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]
    public string Definition { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
