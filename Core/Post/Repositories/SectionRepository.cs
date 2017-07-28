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

namespace Tunynet.Post
{
    /// <summary>
    ///贴吧Repository
    /// </summary>
    public class SectionRepository : Repository<Section>, ISectionRepository
    {
        private int pageSize = 20;
        private  CategoryService categoryService  = DIContainer.Resolve<CategoryService>();

        /// <summary>
        /// 依据OwnerId获取单个贴吧（用于OwnerId与贴吧一对一关系）
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>贴吧</returns>
        public Section GetByOwnerId(string tenantTypeId, long ownerId)
        {
            string cacheKey = GetCacheKey_BarSection(ownerId, tenantTypeId);
            Section section = cacheService.Get<Section>(cacheKey);
            if (section == null)
            {
                var sql = Sql.Builder;
                sql.Select("*")
                   .From("tn_Sections")
                   .Where("TenantTypeId=@0", tenantTypeId)
                   .Where("OwnerId=@0", ownerId);
                section = CreateDAO().FirstOrDefault<Section>(sql);
                if (section != null)
                    cacheService.Set(cacheKey, section, CachingExpirationType.UsualSingleObject);
            }
            return section;
        }
        
      

        /// <summary>
        /// 获取拥有者的贴吧列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="userId">吧主UserId</param>
        /// <returns>贴吧列表</returns>
        public IEnumerable<Section> GetsByUserId(string tenantTypeId,long userId)
        {
            //string cacheKey = RealTimeCacheHelper.GetAreaVersion("UserId", userId) + userId + "SectionsOfUser";
            //List<long> sectionIds = cacheService.Get<List<long>>(cacheKey);

            List<long> sectionIds = new List<long>();
            var sql = Sql.Builder;
            sql.Select("SectionId")
               .From("tn_Sections")               
               .Where("UserId=@0", userId);

            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId=@0", tenantTypeId);

            sectionIds = CreateDAO().Fetch<long>(sql);
           
            return PopulateEntitiesByEntityIds<long>(sectionIds);
        }

        /// <summary>
        /// 获取贴吧的排行数据
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">前多少条</param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="sortBy">贴吧排序依据</param>
        /// <returns></returns>
        public IEnumerable<Section> GetTops(string tenantTypeId, int topNumber, long? categoryId, SortBy_BarSection sortBy)
        {
            var sql = GetSql_SectionsByCategoryId(tenantTypeId, string.Empty, categoryId, sortBy);
            return GetTopEntities(topNumber,sql);
        }

        /// <summary>
        /// 获取贴吧列表
        /// </summary>
        /// <remarks>在频道贴吧分类页使用</remarks>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="nameKeyword"></param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="sortBy">贴吧排序依据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴吧列表</returns>
        public PagingDataSet<Section> Gets(string tenantTypeId, string nameKeyword, long? categoryId, SortBy_BarSection sortBy, int pageIndex)
        {
            if (string.IsNullOrEmpty(nameKeyword))
                return GetPagingEntities(pageSize, pageIndex, GetSql_SectionsByCategoryId(tenantTypeId, string.Empty, categoryId, sortBy));
            else
            {
                var sql = GetSql_SectionsByCategoryId(tenantTypeId, nameKeyword, categoryId, sortBy);
                return GetPagingEntities(pageSize, pageIndex, sql);
            }
        }

        /// <summary>
        /// 获取类别下贴吧列表的SQL块
        /// </summary>
        /// <param name="tenantTypeId"></param>
        /// <param name="nameKeyword"></param>
        /// <param name="categoryId"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        private Sql GetSql_SectionsByCategoryId(string tenantTypeId, string nameKeyword, long? categoryId, SortBy_BarSection sortBy)
        {
            var sql = Sql.Builder;
            var whereSql = Sql.Builder;
            var orderSql = Sql.Builder;

            sql.Select("tn_Sections.*")
            .From("tn_Sections");

            whereSql.Where("TenantTypeId = @0", tenantTypeId)
            /*.Where("AuditStatus=@0", AuditStatus.Success)*/
            .Where("IsEnabled=@0", true);
            if (!string.IsNullOrEmpty(nameKeyword))
            {
                whereSql.Where("Name like @0", StringUtility.StripSQLInjection(nameKeyword) + "%");
            }
            if (categoryId != null && categoryId.Value > 0)
            {
                
                IEnumerable<Category> categories = categoryService.GetCategoriesOfDescendants(categoryId.Value);
                List<long> categoryIds = new List<long> { categoryId.Value };
                if (categories != null && categories.Count() > 0)
                    categoryIds.AddRange(categories.Select(n => n.CategoryId));
                sql.InnerJoin("tn_ItemsInCategories")
               .On("tn_Sections.SectionId = tn_ItemsInCategories.ItemId");
                whereSql.Where("tn_ItemsInCategories.CategoryId in(@categoryIds)", new { categoryIds = categoryIds });
            }
            CountService countService = new CountService(TenantTypeIds.Instance().Section());

            switch (sortBy)
            {
                case SortBy_BarSection.DateCreated_Desc:
                    orderSql.OrderBy("DisplayOrder,SectionId desc");
                    break;
                case SortBy_BarSection.ThreadCount:
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", CountTypes.Instance().ThreadCount()))
                    .On("SectionId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarSection.ThreadAndPostCount:
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", CountTypes.Instance().ThreadAndPostCount()))
                    .On("SectionId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarSection.StageThreadAndPostCount:
                    StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().Section());
                    int stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().ThreadAndPostCount());
                    string stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().ThreadAndPostCount(), stageCountDays);
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", stageCountType))
                    .On("SectionId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarSection.FollowedCount:
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", CountTypes.Instance().FollowedCount()))
                    .On("SectionId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                default:
                    orderSql.OrderBy("SectionId desc");
                    break;
            }

            sql.Append(whereSql).Append(orderSql);
            return sql;
        }

        /// <summary>
        /// 贴吧管理时查询贴吧分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="query">贴吧查询条件</param>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴吧分页集合</returns>
        public PagingDataSet<Section> Gets(string tenantTypeId, SectionQuery query, int pageSize, int pageIndex)
        {
            var sql = Sql.Builder;
            sql.Select("tn_Sections.*")
            .From("tn_Sections");

            if (query.CategoryId != null && query.CategoryId.Value > 0)
            {               
                IEnumerable<Category> categories = categoryService.GetCategoriesOfDescendants(query.CategoryId.Value);
                List<long> categoryIds = new List<long> { query.CategoryId.Value };
                if (categories != null && categories.Count() > 0)
                    categoryIds.AddRange(categories.Select(n => n.CategoryId));
                sql.InnerJoin("tn_ItemsInCategories")
               .On("tn_Sections.SectionId = tn_ItemsInCategories.ItemId")
               .Where("tn_ItemsInCategories.CategoryId in(@categoryIds)", new { categoryIds = categoryIds });
            }

            sql.Where("TenantTypeId = @0", tenantTypeId);

            if (query.UserId != null && query.UserId.Value > 0)
                sql.Where("UserId = @0", query.UserId);

            if (query.IsEnabled != null)
                sql.Where("IsEnabled = @0", query.IsEnabled.Value);

            if (!string.IsNullOrEmpty(query.NameKeyword))
                sql.Where("Name like @0 or Description like @0 ", "%"+ StringUtility.StripSQLInjection(query.NameKeyword) + "%");


            sql.OrderBy("DisplayOrder,SectionId desc");

            return GetPagingEntities(pageSize, pageIndex, sql);
        }

    
        #region CacheKey

        /// <summary>
        /// 获取贴吧缓存Key
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="tenantTypeId"></param>
        /// <returns></returns>
        private string GetCacheKey_BarSection(long ownerId, string tenantTypeId)
        {
            return string.Format("BarSection::O-{0}:T-{1}", ownerId, tenantTypeId);
        }      

        #endregion
                
    }
}