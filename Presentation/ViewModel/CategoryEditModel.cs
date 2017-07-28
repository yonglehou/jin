//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Tunynet.Utilities;
using Tunynet.Common;
using Tunynet.CMS;
using Tunynet.Common.Repositories;

namespace Tunynet.Spacebuilder
{
    [Serializable]
    public class CategoryEditModel
    {

        /// <summary>
        ///主键-类别Id
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        ///拥有者Id
        /// </summary>
        /// 2017-02-22 LiQG 修改 返回类型从int修改为long
        public long OwnerId { get; set; }

        /// <summary>
        ///租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }
        

        /// <summary>
        ///类别名称
        /// </summary>
        [Display(Name ="类别名称")]
        [Required(ErrorMessage = "请输入名称")]
        [StringLength(100, ErrorMessage = "最多可以输入{1}个字符")]
        public string CategoryName { get; set; }



        

        #region 扩展

        /// <summary>
        /// 父类别Id
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 父类别名称
        /// </summary>
        [Display(Name ="父级类别")]
        public string ParentName
        {
            get
            {
                if (ParentId > 0)
                { 
                    var parentCategory = DIContainer.Resolve<CategoryService>().Get(ParentId);
                    if (parentCategory != null)
                    {
                        return parentCategory.CategoryName;
                    }
                }
                return string.Empty;
            }

        }



        /// <summary>
        ///类别深度 顶级类别 Depth=0
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        ///类别描述
        /// </summary>
        [Display(Name ="类别描述")]
        [StringLength(255, ErrorMessage = "最多可以输入{1}个字符")]
        public string Description { get; set; }



            
        #endregion






    }
}