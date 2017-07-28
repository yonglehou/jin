//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Tunynet.UI
{
    /// <summary>
    /// 导航类型（1：普通导航；2：栏目）
    /// </summary>
    public enum NavigationTypes
    {
        ///// <summary>
        ///// 平台定义
        ///// </summary>
        //[Display(Name ="平台")]
        //PresentArea = 0,

        /// <summary>
        /// 普通导航
        /// </summary>
        [Display(Name ="普通导航")]
        Application = 1,

        /// <summary>
        /// 栏目
        /// </summary>
        [Display(Name ="栏目")]
        AddCategory = 2,

    }
}
