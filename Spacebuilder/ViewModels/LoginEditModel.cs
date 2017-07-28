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
using System.Web.Mvc;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 登录模板
    /// </summary>
    public class LoginEditModel
    {
        /// <summary>
        /// 用户名、手机号
        /// </summary>
        [Required(ErrorMessage = "请输登录帐号")]
        //[Remote("CheckUser", "Account", ErrorMessage = "请输入有效手机号或邮箱")]
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "请输入密码")]
        //[RegularExpression(pattern: "[\\S]{4,}", ErrorMessage = "密码格式不正确！")]
        public string PassWord { get; set; }


        /// <summary>
        /// 是否记得密码
        /// </summary>
        [Display(Name = "下次自动登录")]
        public bool RememberPassword { get; set; }
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
