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
    /// 注册模板
    /// </summary>
    public class RegisterEditModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage ="请输入手机号码")]
        [RegularExpression(pattern: "^1[3-8][\\d]{9}$", ErrorMessage = "输入的手机号码格式不正确")]
        [Remote("CheckUniqueMobile", "Account", ErrorMessage ="手机号已被注册")]
        [Display(Name = "手机号码")]
        public string AccountMobile { get; set; }


        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Required(ErrorMessage = "请输入电子邮箱")]
        [RegularExpression(pattern: "^([a-zA-Z0-9_.-]+)@([0-9A-Za-z.-]+).([a-zA-Z.]{2,6})$", ErrorMessage = "输入的电子邮箱格式不正确")]
        [Remote("CheckUniqueEmail", "Account", ErrorMessage = "电子邮箱已被注册")]
        [Display(Name = "邮箱地址")]
        public string AccountEmail { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "请输入密码")]
        [StringLength(16, MinimumLength = 4, ErrorMessage = "字母、数字至少{2}位,并且不能超过{1}位")]
        [Remote("CheckPassword", "Account", ErrorMessage = "不合法的密码格式")]
        [Display(Name = "密码")]
        public string PassWord { get; set; }

        /// <summary>
        /// 激活码
        /// </summary>
        [Required(ErrorMessage ="请输入验证码")]
        [Display(Name = "激活码")]
        public string VerfyCode { get; set; }





    }
}
