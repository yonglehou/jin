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

namespace Tunynet.Common
{
    /// <summary>
    /// 站点风格类型
    /// </summary>
    public enum SiteStyleType
    {
        /// <summary>
        /// 简白
        /// </summary>
        [Display(Name = "简白")]
        Default = 0,
        /// <summary>
        /// 雅灰
        /// </summary>
        [Display(Name = "雅灰")]
        Graybg = 1
    }
    
}