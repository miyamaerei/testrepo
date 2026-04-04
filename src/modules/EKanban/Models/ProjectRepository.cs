using SqlSugar;
using VOL.Entity.SystemModels;

namespace EKanban.Models;

/// <summary>
/// 项目仓库实体，存储项目配置信息
/// </summary>
[SugarTable("ProjectRepositories")]
public class ProjectRepositories : BaseEntity
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    /// <summary>
    /// 项目名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 本地工作目录
    /// </summary>
    public string LocalWorkingDir { get; set; } = string.Empty;
    
    /// <summary>
    /// Git 远程地址
    /// </summary>
    public string GitRemoteUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// 默认分支，默认为 main
    /// </summary>
    public string DefaultBranch { get; set; } = "main";
    
    /// <summary>
    /// 项目描述
    /// </summary>
    [SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]
    public string? Description { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
