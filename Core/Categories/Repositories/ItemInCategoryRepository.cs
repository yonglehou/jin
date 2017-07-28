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
    /// 分类同内容项关联的仓储实现
    /// </summary>
    public class ItemInCategoryRepository : Repository<ItemInCategory>, IItemInCategoryRepository
    {
        /// <summary>
        /// 批量为内容项设置类别
        /// </summary>
        /// <param name="itemIds">内容项Id集合</param>
        /// <param name="categoryId">类别Id</param>
        public int AddItemsToCategory(IEnumerable<long> itemIds, long categoryId)
        {
            //要执行的sql集合
            List<Sql> sqls = new List<Sql>();

            //组装sql
            foreach (long itemId in itemIds)
            {
                if (itemId > 0)
                {
                    var sql = Sql.Builder;
                    sql.Append("insert tn_ItemsInCategories (CategoryId,ItemId) values (@0,@1)", categoryId, itemId);
                    sqls.Add(sql);
                }
            }

            //执行语句
            int effectLineCount = CreateDAO().Execute(sqls);

            #region 处理缓存

            if (effectLineCount > 0)
            {
                //处理，分类下内容项集合缓存
                RealTimeCacheHelper.IncreaseAreaVersion("CategoryId", categoryId);

                //处理，内容项设置的分类集合缓存
                foreach (long itemId in itemIds)
                {
                    RealTimeCacheHelper.IncreaseAreaVersion("ItemId", itemId);
                }
            }

            #endregion

            return effectLineCount;
        }

        private bool isExistByCategoyIdItemId(long categoryId, long itemId)
        {
            var sql = Sql.Builder;
            sql.Append("select COUNT(Id) from tn_ItemsInCategories where CategoryId=@0 and ItemId= @1", categoryId, itemId);

            int value = CreateDAO().First<int>(sql);

            return value > 0;
        }

        /// <summary>
        /// 为内容项批量设置类别
        /// </summary>
        /// <param name="categoryIds">类别Id集合</param>
        /// <param name="itemId">内容项Id</param>
        /// <param name="ownerId">类别拥有者Id</param>
        public void AddCategoriesToItem(IEnumerable<long> categoryIds, long itemId)
        {
            //要执行的sql集合
            List<Sql> sqls = new List<Sql>();

            //组装sql
            foreach (long categoryId in categoryIds)
            {
                if (categoryId > 0)
                {
                    if (!isExistByCategoyIdItemId(categoryId, itemId))
                    {
                        //声明NPoco的SqlBuilder
                        var sql = Sql.Builder;
                        sql.Append("insert tn_ItemsInCategories (CategoryId,ItemId) values (@0,@1)", categoryId, itemId);
                        sqls.Add(sql);
                    }
                }
            }

            //执行语句
            int effectLineCount = CreateDAO().Execute(sqls);

            #region 处理缓存

            if (effectLineCount > 0)
            {
                //分类的区域缓存
                foreach (long categoryId in categoryIds)
                {
                    RealTimeCacheHelper.IncreaseAreaVersion("CategoryId", categoryId);
                }
                RealTimeCacheHelper.IncreaseAreaVersion("CategoryId", 0);
                //item的分区缓存
                RealTimeCacheHelper.IncreaseAreaVersion("ItemId", itemId);
            }

            #endregion
        }

        /// <summary>
        /// 清除内容项设置的所有分类
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="tenantTypeId">租户Id</param>
        public int ClearCategoriesFromItem(long itemId, long? ownerId, string tenantTypeId)
        {
            var dao = CreateDAO();
            //为清除缓存做准备
            IEnumerable<long> categoryIds = GetCategoriesOfItem(itemId, ownerId, tenantTypeId);

            //声明PetaPoco的SqlBuilder
            var sql = Sql.Builder;

            //组装sql
            sql.Append("select Id from tn_ItemsInCategories inner Join tn_Categories on tn_ItemsInCategories.CategoryId = tn_Categories.CategoryId where tn_ItemsInCategories.ItemId=@0 and tn_Categories.OwnerId = @1 and tn_Categories.TenantTypeId=@2", itemId, ownerId ?? 0, tenantTypeId);
            IEnumerable<object> ids_object = dao.FetchFirstColumn(sql);
            IEnumerable<long> ids = ids_object.Cast<long>();

            int effectLineCount = 0;
            if (ids.Count() > 0)
                effectLineCount = dao.Execute(Sql.Builder.Append("delete from tn_ItemsInCategories  where Id in (@ids)", new { ids = ids }));

            #region 处理缓存

            if (effectLineCount > 0)
            {
                //ItemId分区缓存
                RealTimeCacheHelper.IncreaseAreaVersion("ItemId", itemId);

                //本分区缓存，作用是：供其他服务或者应用来更新缓存
                RealTimeCacheHelper.IncreaseAreaVersion("OwnerId", ownerId);

                //CategoryId分区缓存
                if (categoryIds != null && categoryIds.Count() > 0)
                {
                    foreach (long categorId in categoryIds)
                    {
                        RealTimeCacheHelper.IncreaseAreaVersion("CategoryId", categorId);
                    }
                }
            }
            #endregion

            return effectLineCount;
        }

        /// <summary>
        /// 删除分类下的所有的关联项
        /// </summary>
        /// <param name="categoryId">分类Id</param>
        public int ClearItemsFromCategory(long categoryId)
        {
            #region 清除缓存做准备

            long totalCount;
            IList<long> categoryIds = new List<long>();
            categoryIds.Add(categoryId);
            IEnumerable<long> itemIds = GetItemIdsOfCategory(categoryId, categoryIds, int.MaxValue, 1, out totalCount);

            #endregion

            //声明PetaPoco的SqlBuilder
            var sql = Sql.Builder;

            //组装sql
            sql.Append("delete from tn_ItemsInCategories where CategoryId=@0", categoryId);

            //执行语句
            int effectLineCount = CreateDAO().Execute(sql);

            #region 处理缓存
            if (effectLineCount > 0)
            {
                //CategoryId 分区缓存
                RealTimeCacheHelper.IncreaseAreaVersion("CategoryId", categoryId);

                //列表缓存
                foreach (long id in itemIds)
                {
                    RealTimeCacheHelper.IncreaseAreaVersion("ItemId", id);
                }
            }
            #endregion

            return effectLineCount;
        }

        /// <summary>
        /// 获取类别的内容项集合
        /// </summary>
        /// <param name="categoryId">分类的Id集合</param>
        /// <returns>内容项的ID集合</returns>
        public IEnumerable<long> GetItemsOfCategory(long categoryId, IEnumerable<long> categorieIds)
        {
            var dao = CreateDAO();
            //获取缓存
            StringBuilder categoriesIdsStringBuilder = new StringBuilder();
            foreach (long id in categorieIds)
            {
                categoriesIdsStringBuilder.AppendFormat("-{0}", id);
            }
            StringBuilder cacheKey = new StringBuilder(RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "CategoryId", categoryId));
            cacheKey.AppendFormat("allCategoryIds-{0}", categoriesIdsStringBuilder.ToString());

            IEnumerable<long> itemIds = null;
            itemIds = cacheService.Get<IEnumerable<long>>(cacheKey.ToString());
            if (itemIds == null)
            {
                var sql = PetaPoco.Sql.Builder;
                sql.Select("ItemId").From("tn_ItemsInCategories").Where(" CategoryId in (@ids)", new { ids = categorieIds });
                itemIds = dao.FetchFirstColumn(sql).Cast<long>();
                cacheService.Set(cacheKey.ToString(), itemIds, CachingExpirationType.UsualObjectCollection);
            }

            return itemIds;
        }

        /// <summary>
        /// 获取类别的内容项集合
        /// </summary>
        /// <param name="categoryId">分类的Id集合</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="totalRecords">输出参数：总记录数</param>
        /// <returns>当页内容项的ID集合</returns>
        public IEnumerable<long> GetItemIdsOfCategory(long categoryId, IEnumerable<long> categorieIds, int pageSize, int pageIndex, out long totalRecords)
        {
            var sql = Sql.Builder;
            sql.Where(" CategoryId in (@ids)", new { ids = categorieIds });

            PagingDataSet<ItemInCategory> itemInCategories = GetPagingEntities(pageSize, pageIndex, sql);

            totalRecords = itemInCategories.TotalRecords;
            //返回CategoryId集合
            return itemInCategories.Where(n => n != null).Select(n => n.ItemId).ToList();
        }

        /// <summary>
        /// 获取内容项的所有分类Id集合
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <param name="ownerId">分类所有者</param>
        /// <param name="tenantTypeId">租户Id</param>
        /// <returns>返回内容项的类别Id集合</returns>
        public IEnumerable<long> GetCategoriesOfItem(long itemId, long? ownerId, string tenantTypeId)
        {
            StringBuilder cacheKey = new StringBuilder(RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "ItemId", itemId));
            cacheKey.Append("-GetCategoriesOfItem:" + itemId);
            cacheKey.Append("-ownerId:" + (ownerId ?? 0));
            cacheKey.Append("-tenantTypeId:" + tenantTypeId);
                        
            IEnumerable<long> categoryIds = cacheService.Get<IEnumerable<long>>(cacheKey.ToString());
            if (categoryIds == null)
            {
                var sql = Sql.Builder;

                sql.Select("*")
               .From("tn_Categories")
               .InnerJoin("tn_ItemsInCategories C").On("tn_Categories.CategoryId = C.CategoryId")
               .Where(" C.ItemId = @0", itemId)
               .Where("tn_Categories.OwnerId = @0 and tn_Categories.TenantTypeId=@1", ownerId ?? 0, tenantTypeId);
                categoryIds = CreateDAO().FetchFirstColumn(sql).Cast<long>();

                cacheService.Set(cacheKey.ToString(), categoryIds, CachingExpirationType.ObjectCollection);
            }
            return categoryIds;

          

        }

        /// <summary>
        /// 删除分类同内容的关联项
        /// </summary>
        /// <param name="categoryId">分类Id</param>
        /// <param name="itemId">内容项Id</param>
        /// <param name="ownerId">拥有者Id</param>
        public int DeleteItemInCategory(long categoryId, long itemId)
        {
            //声明PetaPoco的SqlBuilder
            var sql = Sql.Builder;

            //组装sql
            sql.Append("delete from tn_ItemsInCategories  where CategoryId=@0 and ItemId=@1", categoryId, itemId);


            //执行语句
            int effectLineCount = CreateDAO().Execute(sql);

            #region 处理缓存

            if (effectLineCount > 0)
            {
                //ItemId分区缓存
                RealTimeCacheHelper.IncreaseAreaVersion("ItemId", itemId);
                //CategoryId分区缓存
                RealTimeCacheHelper.IncreaseAreaVersion("CategoryId", categoryId);
            }
            #endregion

            return effectLineCount;
        }
        /// <summary>
        /// 根据ItemId获取分类
        /// </summary>
        /// <param name="itemId">成员Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <returns></returns>
        public ItemInCategory GetItems(long itemId,string tenantTypeId)
        {
            Sql sql = Sql.Builder;
            sql.Select("tn_ItemsInCategories.*") .From("tn_ItemsInCategories ").InnerJoin("tn_Categories").On("tn_Categories.CategoryId=tn_ItemsInCategories.CategoryId")
               .Where("tn_ItemsInCategories.ItemId = @0  And tn_Categories.TenantTypeId=@1", itemId, tenantTypeId);

            return CreateDAO().SingleOrDefault<ItemInCategory>(sql);



        }
        /// <summary>
        /// 获取分类及其成员项的关系集合
        /// </summary>
        /// <param name="itemId">成员Id</param>
        /// <param name="categoryId">分类Id</param>
        /// <returns></returns>
        public IEnumerable<ItemInCategory> GetItemsInCategories(long itemId, long categoryId)
        {
            Sql sql = Sql.Builder;
            sql.From("tn_ItemsInCategories")
               .Where("CategoryId = @0", categoryId)
               .Where("ItemId = @0", itemId);

            return Gets(sql);
        }
    }
}
