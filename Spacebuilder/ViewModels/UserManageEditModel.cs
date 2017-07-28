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
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    /// <summary> 
    /// 后台用户管理Model
    /// </summary>
    public class UserManageEditModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        ///密码
        /// </summary>
        [Required(ErrorMessage = "请输入密码")]
        [StringLength(64, MinimumLength = 6, ErrorMessage = "字母、数字至少{2}位,并且不能超过{1}位")]
        [Display(Name ="密码")]
        public string Password { get; set; }

        /// <summary>
        ///手机号码
        /// </summary>
        [RegularExpression(pattern: "^1[3-8][\\d]{9}$", ErrorMessage = "输入的手机号码格式不正确")]
        [Display(Name ="手机号码")]
        public string AccountMobile { get; set; }

        /// <summary>
        ///帐号邮箱
        /// </summary>
        [RegularExpression(pattern: "^([a-zA-Z0-9_.-]+)@([0-9A-Za-z.-]+).([a-zA-Z.]{2,6})$", ErrorMessage = "输入的电子邮箱格式不正确")]
        [Display(Name = "邮箱")]
        public string AccountEmail { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [Required(ErrorMessage = "请输入昵称")]
        [StringLength(16,MinimumLength =3,ErrorMessage = "昵称长度为3-16位")]
        [RegularExpression(pattern: "[\u4e00-\u9fa5-_a-zA-Z0-9]+$", ErrorMessage = "昵称仅包含中文、字母、数字以及下划线")]
        [Display(Name = "昵称")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 上次登录日期
        /// </summary>
        public DateTime LastActivityTime { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 用户类别
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 是否封禁
        /// </summary>
        public bool IsBanned { get; set; }

        /// <summary>
        /// 封禁原因
        /// </summary>

        public string BanReason { get; set; }

        /// <summary>
        ///用户是否被监管
        /// </summary>
        [Display(Name ="管制状态")]
        public int ModerateState { get; set; }

        #region 拓展方法

        /// <summary>
        /// 将编辑模型转为新的IUser(后台添加新用户时使用)
        /// </summary>
        /// <returns></returns>
        public IUser GetNewUser()
        {
            User user = User.New();
            if (this.AccountEmail!=null)
            {
                user.IsEmailVerified = true;
            }
            if (this.AccountMobile != null)
            {
                user.IsMobileVerified = true;
            }
            user.AccountEmail = this.AccountEmail;
            user.AccountMobile = this.AccountMobile;
            user.UserName = this.UserName;
            user.Password = this.Password;
            user.Status =   Common.UserStatus.IsActivated;
            return user;
        }

        #endregion
    }
}
