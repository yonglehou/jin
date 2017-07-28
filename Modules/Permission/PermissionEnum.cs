//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Tunynet.Common
{
    /// <summary>
    /// 权限许可类型
    /// </summary>
    public enum OwnerType
    {
        [Display(Name = "用户")]
        /// <summary>
        /// 用户
        /// </summary>
         User = 1,
         [Display(Name = "角色")]
        /// <summary>
        /// 角色
        /// </summary>
        Role = 11
      
    }

    /// <summary>
    /// 权限许可类型
    /// </summary>
    public enum PermissionType
    {
        [Display(Name = "未设置")]
        /// <summary>
        /// 未设置
        /// </summary>
        NotSet = 0,
        [Display(Name = "允许")]
        /// <summary>
        /// 允许
        /// </summary>
        Allow = 1,
        [Display(Name = "拒绝")]
        /// <summary>
        /// 拒绝
        /// </summary>
        Refuse = 2
    }

}
