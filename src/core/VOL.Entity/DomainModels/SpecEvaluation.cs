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
    [Entity(TableCnName = "Spec评估", TableName = "SpecEvaluations")]
    public partial class SpecEvaluation : BaseEntity
    {
        /// <summary>
        ///主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Key]
        [Display(Name = "SpecEvaluationId")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int SpecEvaluationId { get; set; }

        /// <summary>
        ///关联 Spec ID
        /// </summary>
        [Display(Name = "SpecId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int SpecId { get; set; }

        /// <summary>
        ///关联执行记录 ID
        /// </summary>
        [Display(Name = "ExecutionRunId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int ExecutionRunId { get; set; }

        /// <summary>
        ///评估时的 Spec 内容快照
        /// </summary>
        [Display(Name = "SpecContent")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string SpecContent { get; set; }

        /// <summary>
        ///评估时的证据
        /// </summary>
        [Display(Name = "Evidence")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string Evidence { get; set; }

        /// <summary>
        ///是否通过
        /// </summary>
        [Display(Name = "IsPassed")]
        [Column(TypeName = "bit")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public bool IsPassed { get; set; }

        /// <summary>
        ///评估结果描述
        /// </summary>
        [Display(Name = "EvaluationResult")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        public string EvaluationResult { get; set; }

        /// <summary>
        ///评估耗时毫秒
        /// </summary>
        [Display(Name = "EvaluationDurationMs")]
        [Column(TypeName = "bigint")]
        public long? EvaluationDurationMs { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        [Display(Name = "CreatedDate")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime CreatedDate { get; set; }
    }
}
