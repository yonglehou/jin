//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// PermissionItemInUserRole仓储接口
    /// </summary>
    public interface IPermissionItemInUserRoleRepository
    {
        /// <summary>
        /// 更新权限项目设置
        /// </summary>
        /// <param name="permissionItemKeys">待更新的权限项目规则集合</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="ownerType">拥有者所属类别</param>
        void UpdatePermissionsInUserRole(IEnumerable<string> permissionItemKeys, long ownerId, OwnerType ownerType);

        /// <summary>
        /// 获取用户角色对应的权限设置
        /// </summary>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="ownerType">拥有者所属类别</param>
        /// <returns></returns>
        IEnumerable<Permission> GetPermissionsInUserRole(long ownerId,OwnerType ownerType);


        /// <summary>
        /// 获取所有用户角色的权限对应
        /// </summary>
        /// <returns>返回roleName对应的权限设置</returns>
        Dictionary<OwnerType, Dictionary<long,IEnumerable<string>>> GetAllPermission();

    }
}
