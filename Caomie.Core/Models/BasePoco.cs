using System;
using System.ComponentModel.DataAnnotations;

namespace Caomei.Core
{
    public interface IBasePoco
    {
        DateTime? CreateTime { get; set; }
        string CreateBy { get; set; }
        DateTime? UpdateTime { get; set; }
        string UpdateBy { get; set; }
    }

    /// <summary>
    /// Model层的基类，所有的model都应该继承这个类。这会使所有的model层对应的数据库表都有一个自增主键
    /// </summary>
    public class BasePoco : TopBasePoco, IBasePoco
    {
        /// <summary>
        /// CreateTime
        /// </summary>
        [Display(Name = "Sys.CreateTime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// CreateBy
        /// </summary>
        [Display(Name = "Sys.CreateBy")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string CreateBy { get; set; }

        /// <summary>
        /// UpdateTime
        /// </summary>
        [Display(Name = "Sys.UpdateTime")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// UpdateBy
        /// </summary>
        [Display(Name = "Sys.UpdateBy")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string UpdateBy { get; set; }
    }
}