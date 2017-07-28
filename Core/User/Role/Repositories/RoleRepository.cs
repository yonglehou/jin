//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Repositories;
using PetaPoco;
using Tunynet.Caching;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 用户角色Repository
    /// </summary>
    public class RoleRepository : Repository<Role>
    {
        /// <summary>
        /// 删除用户角色
        /// </summary>
        /// <param name="role">待删除的用户角色</param>
        /// <returns>删除成功返回1，否则返回0</returns>
        public override int Delete(Role role)
        {
            var dao = CreateDAO();

            if (role == null)
                return 0;

            var sql = Sql.Builder;
            sql.Select("Id")
                .From("tn_UsersInRoles")
                .Where("RoleId = @0", role.RoleId);
            IEnumerable<long> userInRoleIds = dao.FetchFirstColumn(sql).Cast<long>();

            var sqlUserInRolesDelete = Sql.Builder;
            var sqlRolesDelete = Sql.Builder;
            int count;
            sqlUserInRolesDelete.Append("Delete from tn_UsersInRoles where RoleId = @0", role.RoleId);
            sqlRolesDelete.Append("Delete from tn_Roles where RoleId =@0", role.RoleId);
            base.OnDeleted(Get(role.RoleId));
            using (var scope = dao.GetTransaction())
            {
                dao.Execute(sqlUserInRolesDelete);
                count = dao.Execute(sqlRolesDelete);
                scope.Complete();
            }
            OnDeleted(role);

            
            Caching.RealTimeCacheHelper userInRoleCacheHelper = EntityData.ForType(typeof(UserInRole)).RealTimeCacheHelper;
            foreach (var userInRoleId in userInRoleIds)
            {
                UserInRole userInRole = cacheService.Get<UserInRole>(userInRoleCacheHelper.GetCacheKeyOfEntity(userInRoleId));
                if (userInRole != null)
                    userInRoleCacheHelper.MarkDeletion(userInRole);
            }

            return count;
        }

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="role">待更新的用户角色</param>
        public override void Update(Role role)
        {
            var sql = Sql.Builder;
            sql.Append("Update tn_Roles set RoleName = @0,IsPublic = @1,Description = @2, RoleImageAttachmentId = @3,ConnectToUser=@4  where RoleId = @5", role.RoleName, role.IsPublic, role.Description, role.RoleImageAttachmentId,role.ConnectToUser, role.RoleId);
            CreateDAO().Execute(sql);

            role = Get(role.RoleId);
            base.OnUpdated(role);
        }

    }
}
