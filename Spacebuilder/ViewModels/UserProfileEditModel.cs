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
  /// 完善信息用户资料
  /// </summary>
    public class UserProfileEditModel 
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
        [Remote("CheckUserName", "Account", ErrorMessage = "昵称已经存在")]
        [Display(Name ="昵称")]
        public string UserName { get; set; }

        /// <summary>
        ///真实姓名
        /// </summary>
        [StringLength(16, ErrorMessage = "姓名长度超出限制")]
        [Display (Name ="真实姓名")]
        public string TrueName { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public int HasAvatar { get; set; }
        /// <summary>
        ///性别1=男,2=女,0=未设置
        /// </summary>
        [Display (Name ="性别")]
        public GenderType Gender { get; set; }

        /// <summary>
        ///所在地
        /// </summary>
        [Display (Name = "所在地")]
        public string NowAreaCode { get; set; }


        /// <summary>
        ///自我介绍
        /// </summary>
        [StringLength(255, ErrorMessage = "最长文本不能超过{1}个字")]
        [Display (Name ="自我介绍")]
        [AllowHtml]
        public string Introduction { get; set; }

       
    }


}