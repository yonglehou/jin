//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using PetaPoco;
using Tunynet;
using Tunynet.Caching;
using Tunynet.Repositories;
using Tunynet.Common;
using System.Linq;
using Tunynet.Utilities;

namespace Tunynet.Common
{
    /// <summary>
    ///栏目管理员仓储类
    /// </summary>
    public class CategoryManagerRepository : Repository<CategoryManager>, ICategoryManagerRepository
    {
        /// <summary>
        /// 更新管理员列表
        /// </summary>
        /// <param name="categoryId">(栏目ID)贴吧Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="managerIds">管理员用户Id集合</param>
        /// <param name="referenceCategoryId">从那个栏目继承</param>
        public void UpdateManagerIds(string tenantTypeId, long categoryId, IEnumerable<long> managerIds)
        {
            List<Sql> sqls = new List<Sql>();
            sqls.Add(Sql.Builder.Append("delete from tn_CategoryManagers Where CategoryId=@0 and TenantTypeId=@1", categoryId, tenantTypeId));
            if (managerIds != null)
            {
                foreach (var userId in managerIds)
                {
                    sqls.Add(Sql.Builder.Append("insert into tn_CategoryManagers(categoryId,TenantTypeId,UserId,ReferenceCategoryId) values(@0,@1,@2,@3)", categoryId, tenantTypeId, userId, 0));
                    //移除用户是否管理员判断缓存
                    var cacheKey= GetCacheKey_IsCategoryManager(userId, tenantTypeId);
                    cacheService.Remove(cacheKey);
                }
            }
            CreateDAO().Execute(sqls);
            //更新缓存
            if (managerIds != null)
                cacheService.Set(GetCacheKey_CategoryManagerIds(categoryId, tenantTypeId), managerIds, CachingExpirationType.UsualObjectCollection);

            //移除继承管理员列表缓存
            RemoveCache(tenantTypeId, categoryId);

        }
        /// <summary>
        /// 移除继承管理员列表缓存
        /// </summary>
        /// <param name="tenantTypeId"></param>
        /// <param name="categoryId"></param>
        private void RemoveCache(string tenantTypeId, long categoryId)
        {
            var sql = Sql.Builder.Select("CategoryId").From("tn_CategoryManagers").Where("ReferenceCategoryId=@0 and tenantTypeId=@1", categoryId, tenantTypeId);
            var categoryIds = CreateDAO().Fetch<long>(sql);
            cacheService.Remove(GetCacheKey_CategoryManagerIds(categoryId, tenantTypeId));
            if (categoryIds != null)
            {
                foreach (var item in categoryIds)
                {
                    //移除缓存
                    RemoveCache(tenantTypeId, item);
                }
            }
        }

        /// <summary>
        /// 更新继承栏目管理员记录
        /// </summary>
        /// <param name="categoryId">(栏目ID)贴吧Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="managerIds">管理员用户Id集合</param>
        /// <param name="referenceCategoryId">从那个栏目继承</param>
        public void UpdateReferenceCategoryId(string tenantTypeId, long categoryId, long referenceCategoryId)
        {
            List<Sql> sqls = new List<Sql>();
            sqls.Add(Sql.Builder.Append("delete from tn_CategoryManagers Where CategoryId=@0 and TenantTypeId=@1", categoryId, tenantTypeId));
            sqls.Add(Sql.Builder.Append("insert into tn_CategoryManagers(categoryId,TenantTypeId,UserId,ReferenceCategoryId) values(@0,@1,@2,@3)", categoryId, tenantTypeId, 0, referenceCategoryId));

            CreateDAO().Execute(sqls);

            //todo: by mazq, 2017-03-25, @zhangzh 重复操作，已经在RemoveCache(tenantTypeId, categoryId)移除了 @mazq 已改正

            //移除继承管理员列表缓存
            RemoveCache(tenantTypeId, categoryId);

        }

        /// <summary>
        /// 获取管理员用户Id列表
        /// </summary>
        /// <param name="categoryId">栏目ID/贴吧Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="isReference">是否继承上级管理员列表</param>
        /// <returns>吧管理员用户Id列表</returns>
        public IEnumerable<long> GetCategoryManagerIds(string tenantTypeId, long categoryId, out bool isReference)
        {
            //默认不是继承上一级的管理
            isReference = false;
            string cacheKey = GetCacheKey_CategoryManagerIds(categoryId, tenantTypeId);
            List<long> sectionManagerIds = new List<long>();
            cacheService.TryGetValue<List<long>>(cacheKey, out sectionManagerIds);
            if (sectionManagerIds == null)
            {
                while (sectionManagerIds == null)
                {
                    var sql = Sql.Builder.Where("CategoryId=@0 and TenantTypeId = @1", categoryId, tenantTypeId);
                    var categoryManagers = CreateDAO().Fetch<CategoryManager>(sql);
                    if (categoryManagers.Count == 0)
                        break;
                    var category = categoryManagers.Where(n => n.ReferenceCategoryId > 0).FirstOrDefault();
                    if (category != null)
                    {
                        categoryId = category.ReferenceCategoryId;
                        //是继承的
                        isReference = true;
                    }
                    else
                        sectionManagerIds = categoryManagers.Select(n => n.UserId).ToList();
                }

                if (sectionManagerIds == null)
                    sectionManagerIds = new List<long>();
                cacheService.Set(cacheKey, sectionManagerIds, CachingExpirationType.UsualObjectCollection);
            }
            return sectionManagerIds;
        }

        /// <summary>
        /// 获取用户是否是任一栏目管理员(后台进入时判断用)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="categoryId">栏目ID</param>
        /// <returns>吧管理员用户Id列表</returns>
        public bool IsCategoryManager(string tenantTypeId, long userId, long? categoryId)
        {
            //todo: by mazq, 2017-03-25, @zhangzh 鉴于该方法调用频繁 categoryId有值时应该调用GetCategoryManagerIds()判断，categoryId无值时也应该有缓存  @mazq 已改正
            if (categoryId.HasValue)
            {
                bool isReference;
                var categoryManagerIds=GetCategoryManagerIds(tenantTypeId, categoryId.Value, out isReference);
                return (categoryManagerIds.Any() && categoryManagerIds.Contains(userId));
            }
            bool? result;
            var cacheKey = GetCacheKey_IsCategoryManager(userId, tenantTypeId);
            cacheService.TryGetValue<bool?>(cacheKey, out result);
            if (!result.HasValue)
            {
                var sql = Sql.Builder.Where("UserId=@0 and TenantTypeId = @1", userId, tenantTypeId);
                var categoryManagers = CreateDAO().Fetch<CategoryManager>(sql);
                if (categoryManagers.Count() > 0)
                    result = true;
                else
                    result = false;
                cacheService.Set(cacheKey, result, CachingExpirationType.UsualSingleObject);

            }
            return result.Value;
        }

        /// <summary>
        /// 删除垃圾数据
        /// </summary>
        public void DeleteTrashDatas()
        {
            IEnumerable<TenantType> tenantTypes = new TenantTypeRepository().Gets(MultiTenantServiceKeys.Instance().CategoryManager());
            foreach (var tenantType in tenantTypes)
            {
                Type type = Type.GetType(tenantType.ClassType);
                if (type == null)
                    continue;

                var pd = TableInfo.FromPoco(type);
                var sql = Sql.Builder.Append("select tn_CategoryManagers.CategoryId from tn_CategoryManagers where not exists (select 1 from (select 1 as c from tn_CategoryManagers," + pd.TableName + " where tn_CategoryManagers.CategoryId = " + pd.TableName+"."+pd.PrimaryKey + ") as a) and tn_CategoryManagers.TenantTypeId = @0"
                                      , tenantType.TenantTypeId);
                var categoryIds = CreateDAO().Fetch<long>(sql);

                //删除继承垃圾数据的垃圾数据
                foreach (var item in categoryIds)
                {
                    DeleteReferenceCategoryId(tenantType.TenantTypeId, item);
                }
            }
        }

        /// <summary>
        /// 删除继承垃圾数据
        /// </summary>
        /// <param name="tenantTypeId"></param>
        /// <param name="categoryId"></param>
        private void DeleteReferenceCategoryId(string tenantTypeId, long categoryId)
        {
            var sql = Sql.Builder.Append("delete from tn_CategoryManagers where categoryId=@0 and  tenantTypeId=@1", categoryId, tenantTypeId);
            CreateDAO().Execute(sql);
            sql = Sql.Builder.Select("CategoryId").From("tn_CategoryManagers").Where("ReferenceCategoryId=@0 and tenantTypeId=@1", categoryId, tenantTypeId);
            var categoryIds = CreateDAO().Fetch<long>(sql);
            if (categoryIds != null)
            {
                foreach (var item in categoryIds)
                {
                    //删除垃圾继承数据
                    DeleteReferenceCategoryId(tenantTypeId, item);
                }
            }

        }

        #region CacheKey

        /// <summary>
        /// 管理员缓存
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="tenantTypeId"></param>
        /// <returns></returns>
        private string GetCacheKey_CategoryManagerIds(long categoryId, string tenantTypeId)
        {
            return string.Format("CategoryManagerIds::C-{0}T-{1}", categoryId, tenantTypeId);
        }
        /// <summary>
        /// 判断是否管理员的缓存Key
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tenantTypeId"></param>
        /// <returns></returns>
        private string GetCacheKey_IsCategoryManager(long userId, string tenantTypeId)
        {
            return string.Format("IsCategoryManager::U-{0}T-{1}", userId, tenantTypeId);
        }

        #endregion

    }
}