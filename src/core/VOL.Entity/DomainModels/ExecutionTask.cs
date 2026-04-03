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
    [Entity(TableCnName = "执行任务", TableName = "ExecutionTasks")]
    public partial class ExecutionTask : BaseEntity
    {
        /// <summary>
        ///主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Key]
        [Display(Name = "ExecutionTaskId")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int ExecutionTaskId { get; set; }

        /// <summary>
        ///关联执行卡片ID
        /// </summary>
        [Display(Name = "ExecutionCardId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int ExecutionCardId { get; set; }

        /// <summary>
        ///任务名称
        /// </summary>
        [Display(Name = "TaskName")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string TaskName { get; set; }

        /// <summary>
        ///任务提示词
        /// </summary>
        [Display(Name = "TaskPrompt")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        public string TaskPrompt { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        [Display(Name = "IsEnabled")]
        [Column(TypeName = "bit")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        ///排序索引
        /// </summary>
        [Display(Name = "OrderIndex")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int OrderIndex { get; set; } = 0;

        /// <summary>
        ///创建时间
        /// </summary>
        [Display(Name = "CreatedDate")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime CreatedDate { get; set; }
    }
}
