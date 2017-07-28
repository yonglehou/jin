//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Common.Repositories;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 为IUser扩展与角色相关的功能
    /// </summary>
    public static class UserExtensionByRole
    {
        /// <summary>
        /// 判断用户是否为超级管理员
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsSuperAdministrator(this IUser user)
        {
            if (user == null)
                return false;
            RoleService roleService = DIContainer.Resolve<RoleService>();
            return roleService.IsUserInRoles(user.UserId, RoleIds.Instance().SuperAdministrator());
        }

        /// <summary>
        /// 判断用户是否为受信任用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsTrustedUser(this IUser user)
        {
            if (user == null)
                return false;
            RoleService roleService = DIContainer.Resolve<RoleService>();
            return roleService.IsUserInRoles(user.UserId, RoleIds.Instance().TrustedUser());
        }

        /// <summary>
        /// 获取用户的角色ID集合
        /// </summary>
        /// <param name="user"></param>
        /// <param name="onlyPublic">是否仅获取对外公开的角色</param>
        /// <returns></returns>
        public static IEnumerable<long> UserRoleNames(this IUser user, bool onlyPublic = false)
        {

            RoleService roleService = DIContainer.Resolve<RoleService>();
            return roleService.GetRoleIdsOfUser(user.UserId, onlyPublic);
        }

        /// <summary>
        /// 判断用户是否至少含有requiredRoleNames的一个用户角色
        /// </summary>
        /// <param name="user"><see cref="IUser"/></param>
        /// <param name="requiredRoleNames">待检测用户角色集合</param>
        /// <returns></returns>
        public static bool IsInRoles(this IUser user, params long[] roleIds)
        {
            if (user == null)
                return false;
            RoleService roleService = DIContainer.Resolve<RoleService>();
            return roleService.IsUserInRoles(user.UserId, roleIds);
        }

        
    }
}