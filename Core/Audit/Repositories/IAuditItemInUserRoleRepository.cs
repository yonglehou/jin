//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Tunynet.Repositories;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// AuditItemInUserRole仓储接口
    /// </summary>
    public interface IAuditItemInUserRoleRepository 
    {
        /// <summary>
        /// 获取用户角色对应的审核设置
        /// </summary>
        /// <param name="roleId">角色名称</param>
        /// <returns>返回roleName对应的审核设置</returns>
        IEnumerable<AuditItemInUserRole> GetAuditItemsInUserRole(long roleId);

        /// <summary>
        /// 更新审核项目设置
        /// </summary>
        /// <param name="auditItemInUserRoles">待更新的审核项目规则集合</param>
        void UpdateAuditItemInUserRole(IEnumerable<AuditItemInUserRole> auditItemInUserRoles);
        /// <summary>
        /// 获取所有用户审核规则
        /// </summary>
        /// <returns>返回roleName对应的权限设置</returns>
        Dictionary<long, IEnumerable<AuditItemInUserRole>> GetAllAuditItemsInUserRole();

    }
}