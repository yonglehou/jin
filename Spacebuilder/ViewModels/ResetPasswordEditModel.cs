//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 重设密码ViewModel
    /// </summary>
    public class ResetPasswordEditModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "帐号")]
        [Required(ErrorMessage = "帐号为必填项")]
        //[Remote("CheckUser", "Account", ErrorMessage = "不合法的帐号 ")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Required(ErrorMessage = "请输入密码")]
        [StringLength(64, MinimumLength = 6, ErrorMessage = "字母、数字至少{2}位,并且不能超过{1}位")]
        [Display(Name = "新密码")]
        public string NewPassWord { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "请输入验证码")]
        [Display(Name = "验证码")]
        public string VerfyCode { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string AccountNumber { get; set; }

    }
}
