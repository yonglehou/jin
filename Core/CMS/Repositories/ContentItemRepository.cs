//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Utilities;
using PetaPoco;
using Tunynet.Common;
using System.Data;
using Tunynet.Caching;
using Tunynet.Repositories;
using Tunynet.Settings;

namespace Tunynet.CMS
{
    public class ContentItemRepository : Repository<ContentItem>, IContentItemRepository
    {
        private ISettingsManager<SiteSettings> siteSettings = DIContainer.Resolve<ISettingsManager<SiteSettings>>();
        /// <summary>
        /// 获取所有后代栏目
        /// </summary>
        /// <param name="categoryId">栏目ID</param>
        /// <returns></returns>
        public IEnumerable<ContentCategory> GetCategoryDescendants(int categoryId)
        {
            var contentCategoryRepository = new Repository<ContentCategory>();
            ContentCategory contentcategory = contentCategoryRepository.Get(categoryId);
            if (contentcategory == null || contentcategory.ChildCount == 0)
                return null;
            else
            {
                string descendantParentIdListPrefix = "," + contentcategory.CategoryId.ToString();
                return contentCategoryRepository.GetAll().Where(x => x.ParentIdList.Contains(descendantParentIdListPrefix));
            }
        }

        /// <summary>
        /// 创建ContentItem
        /// </summary>
        /// <param name="contentItem">内容项</param>
        /// <returns></returns>
        public override void Insert(ContentItem contentItem)
        {
            base.Insert(contentItem);

            ContentModel contentModel = contentItem.ContentModel;
            if (contentModel != null)
            {
                List<Sql> sqls = new List<Sql>();

                StringBuilder sqlBuilder = new StringBuilder();
                List<object> values = new List<object>();

                if (!string.IsNullOrEmpty(contentModel.AdditionalTableName))
                {
                    sqlBuilder.AppendFormat("INSERT INTO {0} (", contentModel.AdditionalTableName);

                    sqlBuilder.Append(ContentModel.AdditionalTableForeignKey);
                    values.Add(contentItem.ContentItemId);

                    foreach (var field in contentModel.AdditionalFields)
                    {
                        if (contentItem.AdditionalProperties.Keys.Contains(field.FieldName))
                        {
                            sqlBuilder.Append("," + field.FieldName);
                            if (contentItem.AdditionalProperties[field.FieldName] == null)
                                values.Add(field.DefaultValue);
                            else
                                values.Add(contentItem.AdditionalProperties[field.FieldName]);
                        }
                        else
                        {
                            sqlBuilder.Append("," + field.FieldName);
                            values.Add(field.DefaultValue);
                        }
                    }
                    sqlBuilder.Append(")");

                    sqls.Add(Sql.Builder.Append(sqlBuilder.Append("values (@0)").ToString(), values));
                }

                sqls.Add(Sql.Builder.Append("UPDATE tn_ContentCategories SET ContentCount=ContentCount+1 WHERE CategoryId=@0", contentItem.ContentCategoryId));

                CreateDAO().Execute(sqls);
            }
        }

        /// <summary>
        /// 更新ContentItem
        /// </summary>
        /// <param name="contentItem">内容项</param>
        public override void Update(ContentItem contentItem)
        {
            ContentModel contentModel = contentItem.ContentModel;
            if (contentModel != null && !string.IsNullOrEmpty(contentModel.AdditionalTableName))
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.AppendFormat("UPDATE {0} SET ", contentModel.AdditionalTableName);

                List<object> values = new List<object>();

                int columnIndex = 0;
                foreach (var field in contentModel.AdditionalFields)
                {
                    //todo @wanglei 当附表中未配置 字段时 报错
                    if (!contentItem.AdditionalProperties.ContainsKey(field.FieldName))
                        continue;
                    sqlBuilder.AppendFormat(" {0}=@{1},", field.FieldName, columnIndex);
                    values.Add(contentItem.AdditionalProperties[field.FieldName]);
                    columnIndex++;
                }
                //todo @wanglei 当附表中未配置 字段时 报错
                if (contentModel.AdditionalFields.Count() > 0 && columnIndex > 0)
                {
                    //去除末尾","
                    sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
                    sqlBuilder.AppendFormat(" WHERE {0} = @{1}", ContentModel.AdditionalTableForeignKey, columnIndex);
                    values.Add(contentItem.ContentItemId);

                    Sql sql = Sql.Builder.Append(sqlBuilder.ToString(), values.ToArray());
                    CreateDAO().Execute(sql);
                }
            }

            base.Update(contentItem);
        }

        /// <summary>
        /// 删除ContentItem
        /// </summary>
        /// <param name="contentItem">内容项</param>
        /// <returns></returns>
        public override int Delete(ContentItem contentItem)
        {
            ContentModel contentModel = contentItem.ContentModel;
            List<Sql> sqls = new List<Sql>();
            if (contentModel != null && !string.IsNullOrEmpty(contentModel.AdditionalTableName))
            {
                sqls.Add(Sql.Builder.Append(string.Format("DELETE FROM {0} WHERE {1}=@0", contentModel.AdditionalTableName, ContentModel.AdditionalTableForeignKey), contentItem.ContentItemId));
            }
            sqls.Add(Sql.Builder.Append("UPDATE tn_ContentCategories SET ContentCount=ContentCount-1 WHERE CategoryId=@0", contentItem.ContentCategoryId));
            CreateDAO().Execute(sqls);
            return base.Delete(contentItem);
        }

        /// <summary>
        /// 依据查询条件获取ContentItem列表
        /// </summary>
        /// <param name="query">查询的条件</param>
        /// <param name="pageSize">每页的个数</param>
        /// <param name="pageIndex">第几页</param>
        /// <returns>用于分页的ContentItem列表</returns>
        public PagingDataSet<ContentItem> GetContentItems(ContentItemQuery query, int pageSize, int pageIndex)
        {
            var sql = Sql.Builder.Select("tn_ContentItems.*").From("tn_ContentItems");
            var whereSql = Sql.Builder;
            var orderSql = Sql.Builder;

            if (query.CategoryId.HasValue && query.CategoryId.Value > 0)
            {
                if (query.IncludeCategoryDescendants.HasValue && query.IncludeCategoryDescendants.Value)
                {
                    IEnumerable<ContentCategory> contentCategorys = GetCategoryDescendants(query.CategoryId.Value);

                    IEnumerable<int> descendantcategoryids = contentCategorys == null ? new List<int>() : contentCategorys.Where(n => n.IsEnabled = true).Select(f => f.CategoryId);

                    List<int> categoryids = new List<int>(descendantcategoryids);
                    categoryids.Add(query.CategoryId.Value);

                    whereSql.Where("tn_ContentItems.ContentCategoryId in (@CategoryId)", new { CategoryId = categoryids });
                }
                else
                {
                    whereSql.Where("tn_ContentItems.ContentCategoryId = @0", query.CategoryId.Value);
                }
            }

            if (query.IsAdmin)
            {
                //后台不显示草稿状态的数据
                whereSql.Where("tn_ContentItems.ApprovalStatus !=0");
                if (query.AuditStatus.HasValue)
                {
                    //审核
                    whereSql.Where("tn_ContentItems.ApprovalStatus=@0", (int)query.AuditStatus);
                }
            }
            else
            {
                if (query.IsContextUser)
                {
                    if (query.AuditStatus.HasValue)
                    {
                        whereSql.Where("tn_ContentItems.ApprovalStatus=@0", (int)query.AuditStatus);
                    }
                    else
                    {
                        whereSql.Where("tn_ContentItems.ApprovalStatus !=0");
                    }
                }
                else
                    whereSql = AuditSqls(whereSql);
            }

            if (query.ContentModelId.HasValue && query.ContentModelId.Value > 0)
                whereSql.Where("tn_ContentItems.ContentModelId=@0", query.ContentModelId.Value);

            if (query.UserId.HasValue && query.UserId.Value > 0)
                whereSql.Where("tn_ContentItems.UserId=@0", query.UserId.Value);

            query.SubjectKeyword = StringUtility.StripSQLInjection(query.SubjectKeyword);
            //todo @wanglei 模糊查询需要优化
            if (!string.IsNullOrWhiteSpace(query.SubjectKeyword))
                whereSql.Where("tn_ContentItems.Subject like @0 or tn_ContentItems.Author like @0", "%" + query.SubjectKeyword + "%");
            //whereSql.Where("tn_ContentItems.Subject like @0", "%" + query.SubjectKeyword + "%");+ " or tn_ContentItems.Author like %'" + query.SubjectKeyword + "'%"

            if (query.MinDate != null)
            {
                whereSql.Where("tn_ContentItems.DatePublished >= @0", query.MinDate);
            }
            DateTime maxDate = DateTime.Now;
            if (query.MaxDate != null)
            {
                maxDate = query.MaxDate.Value.AddDays(1);
            }
            whereSql.Where("tn_ContentItems.DatePublished <= @0", maxDate);

            //CountService countService = new CountService(TenantTypeIds.Instance().ContentItem());
            //string countTableName = countService.GetTableName_Counts();
            StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().ContentItem());
            int stageCountDays = 0;
            string stageCountType = string.Empty;
            if (query.OrderBySticky)
                orderSql.OrderBy("tn_ContentItems.IsSticky desc");
            switch (query.SortBy)
            {
                case ContentItemSortBy.DatePublished_Desc:
                    orderSql.OrderBy("tn_ContentItems.DatePublished desc");
                    //todo @wanglei 这个地方如果在下面的几种情况下会添加两个where
                    sql.Append(whereSql).Append(orderSql);
                    break;
                case ContentItemSortBy.HitTimes:
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", CountTypes.Instance().HitTimes(), TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.StageHitTimes:
                    stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().HitTimes());
                    stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().HitTimes(), stageCountDays);
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", stageCountType, TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.CommentCount:
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", CountTypes.Instance().CommentCount(), TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.StageCommentCount:
                    stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().CommentCount());
                    stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().CommentCount(), stageCountDays);
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", stageCountType, TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
            }
            PagingDataSet<ContentItem> pds = null;
            pds = GetPagingEntities(pageSize, pageIndex, sql);
            return pds;
        }


        /// <summary>
        /// 获取前topNumber条ContentItem
        /// </summary>
        /// <param name="topNumber">前几条</param>
        /// <param name="categoryId">栏目ID</param>
        /// <param name="includeCategoryDescendants">是否包含CategoryId的后代</param>
        /// <param name="minDate">最小时间</param>
        /// <param name="sortBy">排序方式</param>
        /// <returns>前多少条的数据</returns>
        public IEnumerable<ContentItem> GetTopContentItems(int topNumber = 10, int? categoryId = null, bool? includeCategoryDescendants = true, DateTime? minDate = null, ContentItemSortBy sortBy = ContentItemSortBy.DatePublished_Desc)
        {
            var sql = Sql.Builder.Select("tn_ContentItems.*").From("tn_ContentItems");
            var whereSql = Sql.Builder;
            var orderSql = Sql.Builder;

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                if (includeCategoryDescendants.HasValue && includeCategoryDescendants.Value)
                {
                    IEnumerable<ContentCategory> contentCategorys = GetCategoryDescendants(categoryId.Value);

                    IEnumerable<int> descendantcategoryids = contentCategorys == null ? new List<int>() : contentCategorys.Where(n => n.IsEnabled = true).Select(f => f.CategoryId);

                    List<int> categoryids = new List<int>(descendantcategoryids);
                    categoryids.Add(categoryId.Value);

                    whereSql.Where("tn_ContentItems.ContentCategoryId in (@CategoryId)", new { CategoryId = categoryids });
                }
                else
                {
                    whereSql.Where("tn_ContentItems.ContentCategoryId = @0", categoryId.Value);
                }
            }
            if (minDate != null)
            {
                whereSql.Where("tn_ContentItems.DatePublished >= @0", minDate);
            }
            ///审核
            whereSql = AuditSqls(whereSql);

            StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().ContentItem());
            int stageCountDays = 0;
            string stageCountType = string.Empty;
            switch (sortBy)
            {
                case ContentItemSortBy.DatePublished_Desc:
                    orderSql.OrderBy("tn_ContentItems.DatePublished desc");
                    break;
                case ContentItemSortBy.HitTimes:
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", CountTypes.Instance().HitTimes(), TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.StageHitTimes:
                    stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().HitTimes());
                    stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().HitTimes(), stageCountDays);
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", stageCountType, TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.CommentCount:
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", CountTypes.Instance().CommentCount(), TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.StageCommentCount:
                    stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().CommentCount());
                    stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().CommentCount(), stageCountDays);
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", stageCountType, TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
            }
            sql.Append(whereSql).Append(orderSql);

            return GetTopEntities(topNumber, sql);
        }

        /// <summary>
        /// 前台根据ModelKey分页获取ContentItem
        /// </summary>
        /// <param name="query">查询的条件</param>
        /// <param name="pageSize">每页的个数</param>
        /// <param name="pageIndex">第几页</param>
        /// <returns>用于分页的ContentItem列表</returns>
        public PagingDataSet<ContentItem> GetContentItemsofModelKey(string ModelKey = "", int pageSize = 20, int pageIndex = 1, ContentItemSortBy? sortBy = ContentItemSortBy.DatePublished_Desc)
        {
            var sql = Sql.Builder.Select("tn_ContentItems.*").From("tn_ContentItems");
            var whereSql = Sql.Builder;
            var orderSql = Sql.Builder;
            whereSql.Where("ContentModelId in (select ModelId from tn_ContentModels WHERE (tn_ContentModels.ModelKey = '" + ModelKey + "'))");
            whereSql = AuditSqls(whereSql);

            StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().ContentItem());
            int stageCountDays = 0;
            string stageCountType = string.Empty;
            orderSql.OrderBy("tn_ContentItems.IsSticky desc");
            switch (sortBy)
            {
                case ContentItemSortBy.DatePublished_Desc:
                    orderSql.OrderBy("tn_ContentItems.DatePublished desc");
                    break;
                case ContentItemSortBy.HitTimes:
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", CountTypes.Instance().HitTimes(), TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.StageHitTimes:
                    stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().HitTimes());
                    stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().HitTimes(), stageCountDays);
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", stageCountType, TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.CommentCount:
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", CountTypes.Instance().CommentCount(), TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.StageCommentCount:
                    stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().CommentCount());
                    stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().CommentCount(), stageCountDays);
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", stageCountType, TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;


            }
            sql.Append(whereSql).Append(orderSql);
            PagingDataSet<ContentItem> pds = null;
            pds = GetPagingEntities(pageSize, pageIndex, sql);
            return pds;
        }
        /// <summary>
        /// 前台根据ModelKey前topNumber条ContentItem
        /// </summary>
        /// <param name="topNumber">数量</param>
        /// <param name="ModelKey">内容模型key</param>
        /// <param name="minDate">最小时间</param>
        /// <param name="sortBy">排序</param>
        /// <returns>前多少条的数据</returns>
        public IEnumerable<ContentItem> GetTopContentItemsofModelKey(int topNumber = 10, string ModelKey = "", DateTime? minDate = null, ContentItemSortBy? sortBy = ContentItemSortBy.DatePublished_Desc)
        {
            var sql = Sql.Builder.Select("tn_ContentItems.*").From("tn_ContentItems");
            var whereSql = Sql.Builder;
            var orderSql = Sql.Builder;

            whereSql.Where("ContentModelId in (select ModelId from tn_ContentModels WHERE (tn_ContentModels.ModelKey = '" + ModelKey + "'))");
            if (minDate != null)
            {
                whereSql.Where("tn_ContentItems.DatePublished >= @0", minDate);
            }
            ///审核
            whereSql = AuditSqls(whereSql);
            StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().ContentItem());
            int stageCountDays = 0;
            string stageCountType = string.Empty;

            orderSql.OrderBy("tn_ContentItems.IsSticky desc");
            switch (sortBy)
            {
                case ContentItemSortBy.DatePublished_Desc:
                    orderSql.OrderBy("tn_ContentItems.DatePublished desc");
                    break;
                case ContentItemSortBy.HitTimes:
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", CountTypes.Instance().HitTimes(), TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.StageHitTimes:
                    stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().HitTimes());
                    stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().HitTimes(), stageCountDays);
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", stageCountType, TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.CommentCount:
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", CountTypes.Instance().CommentCount(), TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case ContentItemSortBy.StageCommentCount:
                    stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().CommentCount());
                    stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().CommentCount(), stageCountDays);
                    sql.InnerJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}'and tn_Counts.TenantTypeId = '{1}'))c", stageCountType, TenantTypeIds.Instance().ContentItem()))
                    .On("ContentItemId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
            }
            sql.Append(whereSql).Append(orderSql);
            return GetTopEntities(topNumber, sql);
        }
        /// <summary>
        /// 资讯计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        public int GetContentItemCount(AuditStatus? approvalStatus ,bool is24Hours)
        {
            Sql sql = Sql.Builder;
            sql.Select(" count(tn_ContentItems.ContentItemId )").From("tn_ContentItems");
            if (approvalStatus.HasValue)
                sql.Where("tn_ContentItems.ApprovalStatus=@0", (int)approvalStatus.Value);
            if (is24Hours)
                sql.Where("tn_ContentItems.DateCreated>@0", DateTime.Now.AddHours(-24));
          return  CreateDAO().SingleOrDefault<int>(sql);
        }

        /// <summary>
        /// 获取上一篇
        /// </summary>
        /// <param name="contentItem">内容项</param>
        /// <returns>返回上一篇内容项</returns>
        public long GetPrevContentItemId(ContentItem contentItem)
        {
            var dao = CreateDAO();
            string cacheKey = string.Format("CmsPrevContentItemId-{0}", contentItem.ContentItemId);
            long prevContentItemId;
            if (!cacheService.TryGetValue<long>(cacheKey, out prevContentItemId))
            {
                var sql = Sql.Builder;
                sql.Select("ContentItemId")
                .From("tn_ContentItems")
                .Where("DatePublished > @0", contentItem.DatePublished)
                .Where("ContentCategoryId = @0", contentItem.ContentCategoryId);

                sql.OrderBy("DatePublished");
                var ids_object = dao.FetchTopPrimaryKeys<ContentItem>(1, sql);
                if (ids_object.Count() > 0)
                    prevContentItemId = ids_object.Cast<long>().First();
                cacheService.Set(cacheKey, prevContentItemId, CachingExpirationType.SingleObject);
            }
            return prevContentItemId;
        }

        /// <summary>
        /// 获取下一篇
        /// </summary>
        /// <param name="contentItem">内容项</param>
        /// <returns>返回下一篇内容项</returns>
        public long GetNextContentItemId(ContentItem contentItem)
        {
            var dao = CreateDAO();
            string cacheKey = string.Format("CmsNextContentItemId-{0}", contentItem.ContentItemId);
            long nextContentItemId;
            if (!cacheService.TryGetValue<long>(cacheKey, out nextContentItemId))
            {
                var sql = Sql.Builder;
                sql.Select("ContentItemId")
                .From("tn_ContentItems")
                .Where("DatePublished < @0", contentItem.DatePublished)
                .Where("ContentCategoryId = @0", contentItem.ContentCategoryId);
                sql.OrderBy("DatePublished desc");
                var ids_object = dao.FetchTopPrimaryKeys<ContentItem>(1, sql);
                if (ids_object.Count() > 0)
                    nextContentItemId = ids_object.Cast<long>().First();
                cacheService.Set(cacheKey, nextContentItemId, CachingExpirationType.SingleObject);
            }
            return nextContentItemId;
        }

        /// <summary>
        /// 获取解析的正文
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns></returns>
        public string GetResolvedBody(long contentItemId)
        {
            ContentItem contentItem = Get(contentItemId);
            if (contentItem == null || string.IsNullOrEmpty(contentItem.AdditionalProperties.Get<string>("Body", string.Empty)))
                return string.Empty;

            string cacheKey = GetCacheKeyOfResolvedBody(contentItemId);
            string resolveBody = cacheService.GetFromFirstLevel<string>(cacheKey);
            if (resolveBody == null)
            {
                resolveBody = contentItem.AdditionalProperties.Get<string>("Body", string.Empty);
                resolveBody = DIContainer.ResolveNamed<IBodyProcessor>(TenantTypeIds.Instance().ContentItem()).Process(resolveBody, TenantTypeIds.Instance().ContentItem(), contentItem.ContentItemId, contentItem.UserId);
                cacheService.Set(cacheKey, resolveBody, CachingExpirationType.SingleObject);
            }
            return resolveBody;
        }

        /// <summary>
        /// 获取解析正文缓存Key
        /// </summary>
        /// <param name="contentItemId">内容项ID</param>
        /// <returns></returns>
        private string GetCacheKeyOfResolvedBody(long contentItemId)
        {
            return "ContentItemResolvedBody" + contentItemId;
        }

        /// <summary>
        /// 获取ContentItem附表数据
        /// </summary>
        /// <param name="contentModelId">模型ID</param>
        /// <param name="contentItemId">内容项主键ID</param>
        /// <returns></returns>
        public Dictionary<string, object> GetContentItemAdditionalProperties(long contentItemId)
        {
            Dictionary<string, object> additionalProperties = null;
            ContentItem contentitem = Get(contentItemId);
            if (contentitem == null)
                return null;
            ContentModel contentType = DIContainer.Resolve<ContentModelService>().Get((long)contentitem.ContentModelId);

            if (contentType != null)
            {
                additionalProperties = new Dictionary<string, object>();

                Database database = CreateDAO();
                database.OpenSharedConnection();
				 //@wangl 修改附表为空时异常
                if (!string.IsNullOrEmpty(contentType.AdditionalTableName))
                {
					try
					{
						using (var cmd = database.CreateCommand(database.Connection, string.Format("SELECT * FROM  {0} WHERE {1} = @0", contentType.AdditionalTableName, ContentModel.AdditionalTableForeignKey), contentitem.ContentItemId))
						{
							using (IDataReader dr = cmd.ExecuteReader())
							{
								if (dr.Read())
								{
									foreach (var column in contentType.AdditionalFields)
									{
										if (dr[column.FieldName] == null)
										additionalProperties.Add(column.FieldName, column.DefaultValue);
									    else
										additionalProperties.Add(column.FieldName, dr[column.FieldName]);
									}
								}
							}
						}
					}
					finally
					{
						database.CloseSharedConnection();
					}
				}
			}
            return additionalProperties;
        }

        /// <summary>
        /// 批量移动ContentItem
        /// </summary>
        /// <param name="contentItems"></param>
        /// <param name="toContentCategoryId"></param>
        public void Move(IEnumerable<ContentItem> contentItems, int toContentCategoryId)
        {
            List<Sql> sqls = new List<Sql>();

            foreach (var contentCategoryId in contentItems.Select(c => c.ContentCategoryId))
            {
                sqls.Add(Sql.Builder.Append("UPDATE tn_ContentCategories Set ContentCount=ContentCount-1").Where("CategoryId = @0", contentCategoryId));
            }
            sqls.Add(Sql.Builder.Append("UPDATE tn_ContentCategories Set ContentCount=ContentCount+@0", contentItems.Count()).Where("CategoryId = @0", toContentCategoryId));

            CreateDAO().Execute(sqls);

            foreach (var contentItem in contentItems)
            {
                contentItem.ContentCategoryId = toContentCategoryId;
                Update(contentItem);
            }
        }

        /// <summary>
        /// 审核语句组装
        /// </summary>
        /// <param name="wheresql">wheresql</param>
        /// <returns></returns>
        private Sql AuditSqls(Sql wheresql)
        {
            var setting = siteSettings.Get();
            if (setting.AuditStatus == PubliclyAuditStatus.Success)
                wheresql.Where("ApprovalStatus=@0", setting.AuditStatus);
            else if (setting.AuditStatus == PubliclyAuditStatus.Again)
                wheresql.Where("ApprovalStatus>@0", PubliclyAuditStatus.Pending);
            else if (setting.AuditStatus == PubliclyAuditStatus.Pending)
                wheresql.Where("ApprovalStatus>@0", PubliclyAuditStatus.Fail);
            return wheresql;

        }
    }
}
