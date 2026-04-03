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
    [Entity(TableCnName = "Azure工作项", TableName = "BoardWorkItems")]
    public partial class BoardWorkItem : BaseEntity
    {
        /// <summary>
        ///主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Key]
        [Display(Name = "BoardWorkItemId")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int BoardWorkItemId { get; set; }

        /// <summary>
        ///Azure Boards 原始工作项ID
        /// </summary>
        [Display(Name = "AzureWorkItemId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int AzureWorkItemId { get; set; }

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
        ///Azure 原始状态
        /// </summary>
        [Display(Name = "AzureState")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string AzureState { get; set; }

        /// <summary>
        ///Azure 工作项链接
        /// </summary>
        [Display(Name = "Url")]
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        [Editable(true)]
        public string Url { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        [Display(Name = "CreatedDate")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///最后同步时间
        /// </summary>
        [Display(Name = "LastSyncDate")]
        [Column(TypeName = "datetime")]
        [Required(AllowEmptyStrings = false)]
        public DateTime LastSyncDate { get; set; }
    }
}
