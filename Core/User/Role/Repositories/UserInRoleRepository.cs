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
using Tunynet.Caching;
using PetaPoco;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 用户角色与用户关联Repository
    /// </summary>
    public class UserInRoleRepository : Repository<UserInRole>, IUserInRoleRepository
    {

        /// <summary>
        /// 把用户加入到一组角色中
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleNames">赋予用户的用户角色</param>
        public void AddUserToRoles(long userId, List<long> roleIds)
        {
            var dao = CreateDAO();

            dao.OpenSharedConnection();
            RemoveUserRoles(userId);
            var sqlInsert = Sql.Builder;
            UserInRole userInRole = new UserInRole();
            userInRole.UserId = userId;
            foreach (var roleId in roleIds)
            {
                userInRole.RoleId = roleId;
                dao.Insert(userInRole);
            }
            dao.CloseSharedConnection();

            //增加版本
            RealTimeCacheHelper.IncreaseAreaVersion("UserId", userId);
            foreach (var roleId in roleIds)
            {
                RealTimeCacheHelper.IncreaseAreaVersion("roleId", roleId);
            }
        }

        /// <summary>
        /// 删除用户的一个角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        public void Delete(long userId, long roleId)
        {
            var sql = Sql.Builder;
            sql.Append("delete from tn_UsersInRoles")
                .Where("UserId= @0 and RoleId=@1", userId, roleId);
            CreateDAO().Execute(sql);

            //增加版本
            RealTimeCacheHelper.IncreaseAreaVersion("UserId", userId);
            RealTimeCacheHelper.IncreaseAreaVersion("RoleId", roleId);
        }


        /// <summary>
        /// 获取用户角色名称
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="onlyPublic">是否仅获取对外公开的角色</param>
        /// <returns>用户的所有角色，如果该用户没有用户角色返回空集合</returns>
        public IEnumerable<string> GetRoleNamesOfUser(long userId, bool onlyPublic = false)
        {
            var sqlRole = Sql.Builder;
            var sql = PetaPoco.Sql.Builder;
            sql.Select("r.RoleName")
                .From("tn_UsersInRoles ur").LeftJoin("tn_Roles r").On("ur.RoleId=r.RoleId")
                .Where("ur.UserId = @0", userId);
            if (onlyPublic)
            {
                sql.Where("r.IsPublic=@0", onlyPublic);
            }
            return CreateDAO().Fetch<string>(sql);
        }

        /// <summary>
        /// 获取用户的角色ID
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="onlyPublic">是否仅获取对外公开的角色</param>
        /// <returns>用户的所有角色，如果该用户没有用户角色返回空集合</returns>
        public IEnumerable<long> GetRoleIdsOfUser(long userId, bool onlyPublic = false)
        {
            string cacheKeyUserInRole = RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "UserId", userId) + onlyPublic;
            List<long> roleIds = cacheService.Get<List<long>>(cacheKeyUserInRole);

            var sqlRole = Sql.Builder;
            if (roleIds == null)
            {
                var sql = PetaPoco.Sql.Builder;
                sql.Select("RoleId")
                    .From("tn_UsersInRoles")
                    .Where("UserId = @0", userId);
                if (onlyPublic)
                {
                    sql.Where("IsPublic=@0", onlyPublic);
                }

                roleIds = CreateDAO().Fetch<long>(sql);

                cacheService.Set(cacheKeyUserInRole, roleIds, CachingExpirationType.UsualObjectCollection);
            }
            return roleIds;
        }

        /// <summary>
        /// 查询拥有管理员角色的用户Id集合
        /// </summary>
        /// <param name="administratorRoleName">管理员角色名称</param>
        /// <returns></returns>
        public IEnumerable<long> GetUserIdsOfRole(long administratorRoleId)
        {
            string cacheKeyUserInRole = RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "RoleName", administratorRoleId);
            List<long> userIds = cacheService.Get<List<long>>(cacheKeyUserInRole);
            if (userIds == null)
            {
                var sql = PetaPoco.Sql.Builder;
                sql.Select("UserId")
                    .From("tn_UsersInRoles")
                    .Where("RoleId = @0", administratorRoleId);
                userIds = CreateDAO().Fetch<long>(sql);
                cacheService.Set(cacheKeyUserInRole, userIds, CachingExpirationType.UsualObjectCollection);
            }
            return userIds;
        }


        /// <summary>
        /// 移除用户的所有角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        public void RemoveUserRoles(long userId)
        {
            var sqlDelete = Sql.Builder;
            sqlDelete.Append("Delete from tn_UsersInRoles where UserId = @0", userId);
            CreateDAO().Execute(sqlDelete);
            RealTimeCacheHelper.IncreaseAreaVersion("UserId", userId);
        }


        /// <summary>
        /// 根据角色ID除所有关联
        /// </summary>
        /// <param name="roleId">角色Id</param>
        public void RemoveRoles(long roleId)
        {
            var sqlDelete = Sql.Builder;
            sqlDelete.Append("Delete from tn_UsersInRoles where RoleId = @0", roleId);
            CreateDAO().Execute(sqlDelete);
        }
    }
}
