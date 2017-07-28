//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 第三方登录编辑
    /// </summary>
    public class AccountTypeEditModel
    {
        /// <summary>
        ///第三方帐号类型标识
        /// </summary>
        [Display(Name = "AccountTypeKey")]
        public string AccountTypeKey { get; set; }

        /// <summary>
        /// 网站接入应用标识
        /// </summary>
        [Display(Name = "AppKey")]
        [Required(ErrorMessage = "请输入网站接入应用标识")]
        public string AppKey { get; set; }

        /// <summary>
        /// 网站接入应用加密串
        /// </summary>
        [Display(Name = "AppSecret")]
        [Required(ErrorMessage = "请输入网站接入应用加密串")]
        public string AppSecret { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        public bool IsEnabled { get; set; }
    }
}
