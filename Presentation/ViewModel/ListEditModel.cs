using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tunynet.Common;

namespace Tunynet.Common
{
    /// <summary>
    /// 列表编辑实体类
    /// </summary>
    public class ListEditModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [Display(Name = "编码")]
        [Required(ErrorMessage = "请填写编码")]
        [Remote("ValidateListCode", "ControlPanel", ErrorMessage ="列表编码已存在请重新输入")]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请填写名称")]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Description { get; set; }

        /// <summary>
        /// 是否多层级
        /// </summary>
        [Display(Name = "是否多层级")]
        public int IsMultilevel { get; set; }

      
    }
   
}
