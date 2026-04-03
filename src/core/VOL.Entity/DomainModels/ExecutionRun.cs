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
    [Entity(TableCnName = "执行记录", TableName = "ExecutionRuns")]
    public partial class ExecutionRun : BaseEntity
    {
        /// <summary>
        ///主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Key]
        [Display(Name = "ExecutionRunId")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int ExecutionRunId { get; set; }

        /// <summary>
        ///关联执行卡片ID
        /// </summary>
        [Display(Name = "ExecutionCardId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int ExecutionCardId { get; set; }

        /// <summary>
        ///关联执行任务ID
        /// </summary>
        [Display(Name = "ExecutionTaskId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? ExecutionTaskId { get; set; }

        /// <summary>
        ///执行者名称
        /// </summary>
        [Display(Name = "ExecutorName")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string ExecutorName { get; set; }

        /// <summary>
        ///执行者类型
        /// </summary>
        [Display(Name = "ExecutorType")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int ExecutorType { get; set; }

        /// <summary>
        ///输入提示词
        /// </summary>
        [Display(Name = "InputPrompt")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        public string InputPrompt { get; set; }

        /// <summary>
        ///输出结果
        /// </summary>
        [Display(Name = "OutputResult")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        public string OutputResult { get; set; }

        /// <summary>
        ///执行证据
        /// </summary>
        [Display(Name = "Evidence")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        public string Evidence { get; set; }

        /// <summary>
        ///开始时间
        /// </summary>
        [Display(Name = "StartTime")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime StartTime { get; set; }

        /// <summary>
        ///结束时间
        /// </summary>
        [Display(Name = "EndTime")]
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        ///执行耗时毫秒
        /// </summary>
        [Display(Name = "DurationMs")]
        [Column(TypeName = "bigint")]
        public long? DurationMs { get; set; }

        /// <summary>
        ///是否成功
        /// </summary>
        [Display(Name = "IsSuccess")]
        [Column(TypeName = "bit")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public bool IsSuccess { get; set; }

        /// <summary>
        ///错误信息
        /// </summary>
        [Display(Name = "ErrorMessage")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        [Display(Name = "CreatedDate")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime CreatedDate { get; set; }
    }
}
