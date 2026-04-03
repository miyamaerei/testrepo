using SqlSugar;

namespace E_Kanban.Backend.Models;

/// <summary>
/// Azure Boards 工作项（外部引用）
/// </summary>
[SugarTable("BoardWorkItems")]
public class BoardWorkItem
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    /// <summary>
    /// Azure Boards 中的工作项 ID
    /// </summary>
    public int ExternalWorkItemId { get; set; }
    
    /// <summary>
    /// 所属 Board ID
    /// </summary>
    public string BoardId { get; set; } = string.Empty;
    
    /// <summary>
    /// 工作项标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 工作项描述
    /// </summary>
    [SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]
    public string? Description { get; set; }
    
    /// <summary>
    /// 外部状态
    /// </summary>
    public string ExternalState { get; set; } = string.Empty;
    
    /// <summary>
    /// 最后同步时间
    /// </summary>
    public DateTime LastSyncedAt { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
