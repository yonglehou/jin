//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;
using System.Text;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 审核设置数据访问
    /// </summary>
    public class AuditItemInUserRoleRepository : IAuditItemInUserRoleRepository
    {
        /// <summary>
        /// 缓存设置
        /// </summary>
        protected static RealTimeCacheHelper RealTimeCacheHelper { get { return EntityData.ForType(typeof(AuditItemInUserRole)).RealTimeCacheHelper; } }

        // 缓存服务
        ICacheService cacheService = DIContainer.Resolve<ICacheService>();

        /// <summary>
        /// 默认NPocoDatabase实例
        /// </summary>
        private Database database;
        protected virtual Database CreateDAO()
        {
            if (database == null)
            {
                database = Database.CreateInstance();
            }
            return database;
        }

        /// <summary>
        /// 获取用户角色对应的审核设置
        /// </summary>
        /// <param name="RoleId">角色名称</param>
        /// <returns>返回RoleId对应的审核设置</returns>
        public IEnumerable<AuditItemInUserRole> GetAuditItemsInUserRole(long roleId)
        {
            string cacheKey = GetCacheKey_GetAuditItemsInUserRole(roleId);
            List<AuditItemInUserRole> auditItemInUserRoles = cacheService.Get<List<AuditItemInUserRole>>(cacheKey);
            if (auditItemInUserRoles == null)
            {
                var sql = Sql.Builder;
                sql.Where("RoleId = @0", roleId);
                auditItemInUserRoles = CreateDAO().Fetch<AuditItemInUserRole>(sql);
                cacheService.Set(cacheKey, auditItemInUserRoles, CachingExpirationType.UsualObjectCollection);
            }
            return auditItemInUserRoles;
        }

        /// <summary>
        /// 更新审核项目设置
        /// </summary>
        /// <param name="auditItemInUserRoles">待更新的审核项目规则集合</param>
        public void UpdateAuditItemInUserRole(IEnumerable<AuditItemInUserRole> auditItemInUserRoles)
        {
            if (auditItemInUserRoles == null)
                return;
            var database = CreateDAO();
            database.OpenSharedConnection();

            List<Sql> sqls = new List<Sql>();

            foreach (var auditItemInUserRole in auditItemInUserRoles)
            {
                AuditItemInUserRole tempAuditItemInUserRole = null;

                var sql = Sql.Builder;
                sql.From("tn_AuditItemsInUserRoles")
                   .Where("RoleId = @0 and ItemKey = @1", auditItemInUserRole.RoleId, auditItemInUserRole.ItemKey);

                //获取是否存在记录
                tempAuditItemInUserRole = database.FirstOrDefault<AuditItemInUserRole>(sql);

                //检测是否存在、锁定
                if (tempAuditItemInUserRole != null)
                {
                    if (!tempAuditItemInUserRole.IsLocked)
                    {
                        sqls.Add(Sql.Builder.Append(" update tn_AuditItemsInUserRoles ")
                                            .Append(" set StrictDegree = @0, IsLocked= @1 ", auditItemInUserRole.StrictDegree, auditItemInUserRole.IsLocked)
                                            .Append(" where RoleId = @0 and ItemKey = @1", auditItemInUserRole.RoleId, auditItemInUserRole.ItemKey));
                    }
                }
                else
                {
                    sqls.Add(Sql.Builder.Append("INSERT INTO tn_AuditItemsInUserRoles (RoleId, ItemKey, StrictDegree, IsLocked) VALUES (@0,@1,@2,@3)",
                                                 auditItemInUserRole.RoleId,
                                                 auditItemInUserRole.ItemKey,
                                                 auditItemInUserRole.StrictDegree,
                                                 auditItemInUserRole.IsLocked));
                }
            }
            
            database.Execute(sqls);

            database.CloseSharedConnection();

            IEnumerable<long> roleIds = auditItemInUserRoles.Select(n => n.RoleId).Distinct();
            foreach (var roleId in roleIds)
            {
                RealTimeCacheHelper.IncreaseAreaVersion("roleId", roleId);
            }

            RealTimeCacheHelper.IncreaseGlobalVersion();

        }

        /// <summary>
        /// 获取所有用户审核规则
        /// </summary>
        /// <returns>返回RoleId对应的权限设置</returns>
       public  Dictionary<long, IEnumerable<AuditItemInUserRole>> GetAllAuditItemsInUserRole()
        {
            Dictionary<long, IEnumerable<AuditItemInUserRole>> allAuditItems = new Dictionary<long, IEnumerable<AuditItemInUserRole>>();
            var sql = Sql.Builder;
            sql.Select("*").From("tn_AuditItemsInUserRoles");
            var auditItems = CreateDAO().Fetch<AuditItemInUserRole>(sql);
            var ids = auditItems.Select(n => n.RoleId).Distinct();
            foreach (var item in ids)
            {
                //装进所有的 角色的权限集合
                var auditItem = auditItems.Where(n => n.RoleId == item);
                allAuditItems.Add(item, auditItem);
            }
            return allAuditItems;
        }

        /// <summary>
        /// 获取审核项目与角色关联 的CacheKey
        /// </summary>
        /// <param name="roleId">角色名</param>
        private string GetCacheKey_GetAuditItemsInUserRole(long roleId)
        {
            string cacheKeyPrefix = RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "roleId", roleId);
            return string.Format("{0}::AuditItemsInUserRole:roleId:{1}", cacheKeyPrefix, roleId);
        }
    }
}