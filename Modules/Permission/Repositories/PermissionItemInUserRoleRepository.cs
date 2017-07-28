//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using PetaPoco;
using Tunynet.Caching;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 权限项目关联设置的数据访问
    /// </summary>
    public class PermissionItemInUserRoleRepository : IPermissionItemInUserRoleRepository
    {
        /// <summary>
        /// 缓存设置
        /// </summary>
        protected static RealTimeCacheHelper RealTimeCacheHelper { get { return EntityData.ForType(typeof(Permission)).RealTimeCacheHelper; } }

        // 缓存服务
        private ICacheService cacheService ;

        public PermissionItemInUserRoleRepository(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }
        /// <summary>
        /// 默认Database实例
        /// </summary>
        private Database database;
        protected virtual Database CreateDAO()
        {
            if (database == null)
            {
                database =Database.CreateInstance();
            }
            return database;
        }

        /// <summary>
        /// 更新权限项目设置
        /// </summary>
        /// <param name="permissionItemInUserRoles">待更新的权限项目规则集合</param>
        public void UpdatePermissionsInUserRole(IEnumerable<string> permissionItemKeys, long ownerId, OwnerType ownerType)
        {
            var database = CreateDAO();
            database.OpenSharedConnection();

            var sql = Sql.Builder;
            sql.Append("delete from tn_Permissions")
                 .Where(" OwnerId = @0 and OwnerType=@1 and IsLocked=0", ownerId, (int)ownerType);
            //删除所属权限
            database.Execute(sql);

            sql = Sql.Builder.From("tn_Permissions")
                .Where(" OwnerId = @0 and OwnerType=@1", ownerId, (int)ownerType);
            //获取所属的锁定权限
            var permissions= database.Fetch<Permission>(sql);
         
            List<Sql> sqls = new List<Sql>();
            foreach (var permissionItemKey in permissionItemKeys)
            {
                //获取的权限等于空或者不包含当前要更改的权限项
                if (permissions==null||(permissions!=null&& !permissions.Select(n=>n.PermissionItemKey).Contains(permissionItemKey)))
                {
                    sqls.Add(Sql.Builder.Append("INSERT INTO tn_Permissions (PermissionItemKey, OwnerId, OwnerType, IsLocked) VALUES (@0,@1,@2,0)",
                                             permissionItemKey,
                                               ownerId,
                                               (int)ownerType));
                }
              
            }

            database.Execute(sqls);
            database.CloseSharedConnection();
            foreach (var permissionItemKey in permissionItemKeys)
            {
                RealTimeCacheHelper.IncreaseAreaVersion("PermissionItemKey", permissionItemKey);
            }
            RealTimeCacheHelper.IncreaseGlobalVersion();
            string cacheKey = GetCacheKey_GetPermissionsInUserRole(ownerId, ownerType);
            cacheService.Remove(cacheKey);
        }

        /// <summary>
        /// 获取用户角色对应的权限设置
        /// </summary>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="ownerType">拥有者所属类别</param>
        /// <returns>返回roleName对应的权限设置</returns>
        public IEnumerable<Permission> GetPermissionsInUserRole(long ownerId, OwnerType ownerType)
        {


            string cacheKey = GetCacheKey_GetPermissionsInUserRole(ownerId, ownerType);
            List<Permission> permissionItemInUserRoles=null;
            cacheService.TryGetValue<List<Permission>>(cacheKey, out permissionItemInUserRoles);
            if (permissionItemInUserRoles == null)
            {
                var sql = Sql.Builder;
                sql.Where("OwnerId = @0 and  OwnerType=@1", ownerId, ownerType);
                permissionItemInUserRoles = CreateDAO().Fetch<Permission>(sql);
                cacheService.Set(cacheKey, permissionItemInUserRoles, CachingExpirationType.UsualObjectCollection);
            }
            return permissionItemInUserRoles;
        }
        /// <summary>
        /// 获取所有用户角色的权限对应
        /// </summary>
        /// <returns>返回roleName对应的权限设置</returns>
        public  Dictionary<OwnerType, Dictionary<long, IEnumerable<string>>> GetAllPermission()
        {
            Dictionary<OwnerType, Dictionary<long, IEnumerable<string>>> allPermission = new Dictionary<OwnerType, Dictionary<long, IEnumerable<string>>>();
            var sql = Sql.Builder;
            sql.Select("*").From("tn_Permissions");
            var permissions = CreateDAO().Fetch<Permission>(sql);
            //获取所有的角色ID
            foreach (OwnerType ownerType in System.Enum.GetValues(typeof(OwnerType)))
            {
                Dictionary<long, IEnumerable<string>> userpermissions = new Dictionary<long, IEnumerable<string>>();
                var rolepermissions = permissions.Where(m => m.OwnerType == ownerType);
                var ids = rolepermissions.Select(n => n.OwnerId).Distinct();
                foreach (var item in ids)
                {
                    //装进所有的 角色的权限集合
                    var permissionItemKeys = rolepermissions.Where(n => n.OwnerId == item).Select(n => n.PermissionItemKey);
                    userpermissions.Add(item, permissionItemKeys);
                }
                allPermission.Add(ownerType, userpermissions);
            }
            return allPermission;

        }
        /// <summary>
        /// 获取权限项目与角色关联 的CacheKey
        /// </summary>
        /// <param name="roleId">角色名</param>
        private string GetCacheKey_GetPermissionsInUserRole(long ownerId, OwnerType ownerType)
        {
            string cacheKeyPrefix = RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "OwnerId", ownerId);
            return string.Format("{0}::PermissionItemsInUserRole:RoleId:{1}:OwnerType:{2}", cacheKeyPrefix, ownerId, ownerType);
        }
    }
}