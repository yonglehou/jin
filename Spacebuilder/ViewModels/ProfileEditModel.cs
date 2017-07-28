//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Tunynet;
using Tunynet.Caching;
using Tunynet.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Tunynet.Spacebuilder
{
  /// <summary>
  /// 更改信息用户资料
  /// </summary>
    public class ProfileEditModel 
    {
       

        /// <summary>
        ///UserId
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        ///用户名
        /// </summary>
        [Required(ErrorMessage = "请输入昵称")]
        [RegularExpression(pattern: "[\u4e00-\u9fa5-_a-zA-Z0-9]{3,16}", ErrorMessage = "请输入有效的昵称")]
        [Remote("CheckUserName", "Account", ErrorMessage = "昵称已经存在 ")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "请输入原密码")]
       [Display(Name ="原密码")]

        public string PassWord { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "请输入新密码")]
        [StringLength(64, MinimumLength = 6, ErrorMessage = "字母、数字至少{2}位,并且不能超过{1}位")]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }


    }


}