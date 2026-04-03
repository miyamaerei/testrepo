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
    [Entity(TableCnName = "Spec定义", TableName = "Specs")]
    public partial class Spec : BaseEntity
    {
        /// <summary>
        ///主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Key]
        [Display(Name = "SpecId")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int SpecId { get; set; }

        /// <summary>
        ///关联执行卡片ID
        /// </summary>
        [Display(Name = "ExecutionCardId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int ExecutionCardId { get; set; }

        /// <summary>
        ///Spec 内容
        /// </summary>
        [Display(Name = "SpecContent")]
        [Column(TypeName = "nvarchar(max)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string SpecContent { get; set; }

        /// <summary>
        ///版本号
        /// </summary>
        [Display(Name = "Version")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int Version { get; set; } = 1;

        /// <summary>
        ///创建时间
        /// </summary>
        [Display(Name = "CreatedDate")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime CreatedDate { get; set; }
    }
}
