using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Common;

namespace Tunynet.Common
{  /// <summary>
   /// 列表项编辑实体类
   /// </summary>
    public class ListItemEditModel
    {
        /// <summary>
        /// 列表项Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 项编码
        /// </summary>
        [Required(ErrorMessage = "请填写项编码")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 列表编码
        /// </summary>
        public string ListCode { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "请填写名称")]
        public string Name { get; set; }

        /// <summary>
        /// 子级数目
        /// </summary>
        public int ChildrenCount { get; set; }

        /// <summary>
        /// 深度（从0开始）
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// 排列顺序
        /// </summary>
        public int DisplayOrder { get; set; }

    }

   
}
