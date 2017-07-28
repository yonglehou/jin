//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Tunynet.Common.Configuration;
using Tunynet.Settings;

namespace Tunynet.Common
{
    /// <summary>
    /// 权限验证服务类
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
      
        private PermissionService permissionService;
        private ConcurrentDictionary<string, ITenantAuthorizationHandler> tenantAuthorizationHandlerDictionary = new ConcurrentDictionary<string, ITenantAuthorizationHandler>();
        private ISettingsManager<UserSettings> userSettingsManager = DIContainer.Resolve<ISettingsManager<UserSettings>>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public AuthorizationService(PermissionService permissionService)
        {
            tenantAuthorizationHandlerDictionary = new ConcurrentDictionary<string, ITenantAuthorizationHandler>();
            foreach (var tenantAuthorizationHandler in DIContainer.Resolve<IEnumerable<ITenantAuthorizationHandler>>())
            {
                tenantAuthorizationHandlerDictionary[tenantAuthorizationHandler.TenantTypeId] = tenantAuthorizationHandler;
            }

            UserSettings userSettings = DIContainer.Resolve<ISettingsManager<UserSettings>>().Get();

        
            this.permissionService = permissionService;

        }
       

        /// <summary>
        /// 当前用户是不是超级管理员
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <returns>是超级管理员返回true，否则返回false</returns>
        public bool IsSuperAdministrator(IUser currentUser)
        {
            if (currentUser == null)
                return false;
            return currentUser.IsSuperAdministrator();
        }

        /// <summary>
        /// 是不是拥有者
        /// </summary>
        /// <remarks>
        /// 拥有者一般对自己的内容有管理权限
        /// </remarks>
        /// <param name="currentUser">当前用户</param>
        /// <param name="userIds">可能作为拥着有的多个用户Id</param>
        /// <returns>是拥有者返回true，否则返回false</returns>
        public bool IsOwner(IUser currentUser, params long[] userIds)
        {
            if (currentUser == null)
                return false;

            if (userIds.Contains(currentUser.UserId))
                return true;

            return false;
        }

        /// <summary>
        /// 是不是租户管理者
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="tenantOwnerId">租户的OwnerId</param>
        /// <returns>是租户拥有者返回true，否则返回false</returns>
        public bool IsTenantManager(IUser currentUser, string tenantTypeId, long tenantOwnerId)
        {
            ITenantAuthorizationHandler tenantAuthorizationHandler;

            if (tenantAuthorizationHandlerDictionary.TryGetValue(tenantTypeId, out tenantAuthorizationHandler))
            {
                return tenantAuthorizationHandler.IsTenantManager(currentUser, tenantOwnerId);
            }

            return false;
        }

        /// <summary>
        /// 是不是租户普通成员
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="tenantOwnerId">租户的OwnerId</param>
        /// <returns>是租户拥有者返回true，否则返回false</returns>
        public bool IsTenantMember(IUser currentUser, string tenantTypeId, long tenantOwnerId)
        {
            ITenantAuthorizationHandler tenantAuthorizationHandler;

            if (tenantAuthorizationHandlerDictionary.TryGetValue(tenantTypeId, out tenantAuthorizationHandler))
            {
                return tenantAuthorizationHandler.IsTenantMember(currentUser, tenantOwnerId);
            }

            return false;
        }


       

        /// <summary>
        /// 检查用户是否有权限进行某项操作
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="permissionItemKey">权限项目标识</param>
        /// <returns>有权限操作返回true，否则返回false</returns>
        public bool Check(IUser currentUser, string permissionItemKey)
        {
            if (currentUser == null)
                return false;

            if (IsSuperAdministrator(currentUser))
                return true;
            //获取用户的所有权限
            var resolvedUserPermission = permissionService.ResolveUserPermission(currentUser.UserId);
            if (resolvedUserPermission==null)
                return false;
            //判断用户的所有权限里有没有当前权限
            return resolvedUserPermission.Select(n => n.PermissionItemKey).Contains(permissionItemKey);

        }
    }
}
