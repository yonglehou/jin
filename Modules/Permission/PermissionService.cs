//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Tunynet.Caching;
using Tunynet.Repositories;
using Tunynet.Common.Repositories;
using Tunynet.Events;
using System;

namespace Tunynet.Common
{
    /// <summary>
    /// 权限管理服务类
    /// </summary>
    public class PermissionService
    {
        #region 构造器
        private IRepository<PermissionItem> permissionItemRepository;
        private IPermissionItemInUserRoleRepository permissionItemInUserRoleRepository;
        private ICacheService cacheService;
        private RoleService roleService;
        private IUserService userService;
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="permissionItemRepository">PermissionItem仓储</param>
        /// <param name="permissionItemInUserRoleRepository"><see cref="IPermissionItemInUserRoleRepository"/></param>
        public PermissionService(IRepository<PermissionItem> permissionItemRepository, IPermissionItemInUserRoleRepository permissionItemInUserRoleRepository, ICacheService cacheService, RoleService roleService, IUserService userService)
        {
            this.permissionItemRepository = permissionItemRepository;
            this.permissionItemInUserRoleRepository = permissionItemInUserRoleRepository;
            this.cacheService = cacheService;
            this.roleService = roleService;
            this.userService = userService;
        }
        #endregion


        #region 权限项目

        /// <summary>
        /// 获取权限项集合
        /// </summary>
        /// <param name="applicationId">应用程序ID</param>
        /// <returns>权限项集合</returns>
        public IEnumerable<PermissionItem> GetPermissionItems()
        {
            IEnumerable<PermissionItem> allPermissionItems = permissionItemRepository.GetAll("DisplayOrder");

            return allPermissionItems;
        }

        /// <summary>
        /// 获取PermissionItem
        /// </summary>
        /// <param name="itemKey">权限项标识</param>
        /// <returns></returns>
        public PermissionItem GetPermissionItem(string itemKey)
        {
            return permissionItemRepository.Get(itemKey);
        }

        /// <summary>
        /// 获取用户角色对应的权限设置
        /// </summary>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="ownerType">拥有者所属类别</param>
        /// <returns>返回roleName对应的权限设置</returns>
        public IEnumerable<Permission> GetPermissionsInUserRole(long ownerId, OwnerType ownerType)
        {
            return permissionItemInUserRoleRepository.GetPermissionsInUserRole(ownerId, ownerType);
        }

        /// <summary>
        /// 更新权限规则
        /// </summary>
        /// <param name="permissionItemKeys">待更新的权限项目规则集合</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="ownerType">拥有者所属类别</param>
        public void UpdatePermissionsInUserRole(IEnumerable<string> permissionItemKeys, long ownerId, OwnerType ownerType)
        {
            var oldpermissions = GetPermissionsInUserRole(ownerId, ownerType);
            permissionItemInUserRoleRepository.UpdatePermissionsInUserRole(permissionItemKeys, ownerId, ownerType);
            var permissions = GetPermissionsInUserRole(ownerId, ownerType);

            if (!(permissionItemKeys.Count() > 0))
            {
                foreach (var permission in oldpermissions)
                {
                    EventBus<Permission>.Instance().OnAfter(permission, new CommonEventArgs(EventOperationType.Instance().Delete()));
                }
            }
            else
            {
                foreach (var permission in permissions)
                {
                    EventBus<Permission>.Instance().OnAfter(permission, new CommonEventArgs(EventOperationType.Instance().Update()));
                }
            }




            //EventBus<Permission, CommonEventArgs>.Instance().OnBatchAfter(permissionItemInUserRoles, new CommonEventArgs(EventOperationType.Instance().Update()));

        }

        /// <summary>
        /// 获取所有用户角色的权限对应
        /// </summary>
        /// <returns>返回roleName对应的权限设置</returns>
        public Dictionary<OwnerType, Dictionary<long, IEnumerable<string>>> GetAllPermission()
        {
            return permissionItemInUserRoleRepository.GetAllPermission();

        }
        #endregion

        #region 获取用户权限

        /// <summary>
        /// 解析用户的所有权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<Permission> ResolveUserPermission(long userId)
        {

            IEnumerable<Permission> permissions = new List<Permission>();
            var user = userService.GetUser(userId);
            //匿名用户
            if (user == null)
                return permissions;

            IList<long> roleIdsOfUser = roleService.GetRoleIdsOfUser(userId).ToList();
            roleIdsOfUser.Add(RoleIds.Instance().RegisteredUsers());
            if (user.IsModerated)
                roleIdsOfUser.Add(RoleIds.Instance().ModeratedUser());

            foreach (var roleId in roleIdsOfUser)
            {
                var rolePermissions = GetPermissionsInUserRole(roleId, OwnerType.Role);
                permissions = permissions.Union(rolePermissions);
            }
            var userPermissions = GetPermissionsInUserRole(userId, OwnerType.User);
            permissions = permissions.Union(userPermissions);
            return permissions;
        }
     
        #endregion

    }
}
