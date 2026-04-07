/*
 *代码由框架生成,任何更改都可能导致被代码生成器覆盖
 *如果数据库字段发生变化，请在代码生器重新生成此Model
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SqlSugar;
using VOL.Entity.SystemModels;

namespace VOL.Entity.DomainModels
{
    [Entity(TableCnName = "执行卡片", TableName = "ExecutionCards")]
    public partial class ExecutionCard : BaseEntity
    {
        /// <summary>
        ///主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Key]
        [Display(Name = "ExecutionCardId")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int ExecutionCardId { get; set; }

        /// <summary>
        ///关联 BoardWorkItem ID
        /// </summary>
        [Display(Name = "BoardWorkItemId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int BoardWorkItemId { get; set; }

        /// <summary>
        ///标题
        /// </summary>
        [Display(Name = "Title")]
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        /// <summary>
        ///描述
        /// </summary>
        [Display(Name = "Description")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        public string Description { get; set; }

        /// <summary>
        ///状态：0-New, 1-Ready, 2-InProgress, 3-Submitted, 4-Completed, 5-Failed
        /// </summary>
        [Display(Name = "Status")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int Status { get; set; }

        /// <summary>
        ///执行者类型：0-Manual, 1-AI, 2-System
        /// </summary>
        [Display(Name = "ExecutorType")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int ExecutorType { get; set; }

        /// <summary>
        ///当前 Spec ID
        /// </summary>
        [Display(Name = "CurrentSpecId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? CurrentSpecId { get; set; }

        /// <summary>
        ///InProgress 开始时间
        /// </summary>
        [Display(Name = "InProgressStartTime")]
        [Column(TypeName = "datetime")]
        [Editable(true)]
        public DateTime? InProgressStartTime { get; set; }

        /// <summary>
        ///失败次数
        /// </summary>
        [Display(Name = "FailureCount")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int FailureCount { get; set; } = 0;

        /// <summary>
        ///是否需要人工干预
        /// </summary>
        [Display(Name = "NeedsManualIntervention")]
        [Column(TypeName = "bit")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public bool NeedsManualIntervention { get; set; } = false;

        /// <summary>
        ///创建时间
        /// </summary>
        [Display(Name = "CreatedDate")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///最后更新时间
        /// </summary>
        [Display(Name = "LastUpdated")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///是否手动创建（true: 手动创建, false: Azure同步）
        /// </summary>
        [Display(Name = "IsManualCreated")]
        [Column(TypeName = "bit")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public bool IsManualCreated { get; set; } = false;

        /// <summary>
        ///看板ID
        /// </summary>
        [Display(Name = "BoardId")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        [Editable(true)]
        public string BoardId { get; set; }

        /// <summary>
        ///项目仓库ID
        /// </summary>
        [Display(Name = "ProjectRepositoryId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? ProjectRepositoryId { get; set; }

        /// <summary>
        ///规格ID
        /// </summary>
        [Display(Name = "SpecId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? SpecId { get; set; }
    }

    /// <summary>
    /// 执行卡片状态枚举
    /// </summary>
    public enum ExecutionCardStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        New = 0,
        /// <summary>
        /// 就绪
        /// </summary>
        Ready = 1,
        /// <summary>
        /// 执行中
        /// </summary>
        InProgress = 2,
        /// <summary>
        /// 已提交
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
        Manual = 0,
        /// <summary>
        /// AI 执行
        /// </summary>
        AI = 1,
        /// <summary>
        /// 系统自动执行
        /// </summary>
        System = 2
    }
}
