//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Tunynet.Caching;
using Tunynet.Repositories;
using Tunynet.Utilities;
using PetaPoco;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 标签仓储的具体实现类
    /// </summary>
    public class TagRepository<T> : Repository<T>, ITagRepository<T> where T : Tag
    {
        int pageSize = 20;

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="entity">待创建实体</param>
        /// <returns></returns>
        public override void Insert(T entity)
        {
            Sql sql = Sql.Builder;
            sql.Append("select count(*) from tn_Tags where TagName = @0 and TenantTypeId = @1", entity.TagName, entity.TenantTypeId);

            var dao = CreateDAO();
            dao.OpenSharedConnection();            
            if (dao.ExecuteScalar<int>(sql) == 0)
            {
                base.Insert(entity);
            }
            dao.CloseSharedConnection();
        }     

        /// <summary>
        /// 从数据库删除实体
        /// </summary>
        /// <param name="entity">标签实体</param>
        /// <returns>影响行数</returns>
        public override int Delete(T entity)
        {
            IList<Sql> sqls = new List<Sql>();

            int affectCount = 0;
            var dao = CreateDAO();
            dao.OpenSharedConnection();

            sqls.Add(Sql.Builder.Append("delete from tn_Tags where TagId = @0", entity.TagId));
            sqls.Add(Sql.Builder.Append("delete from tn_ItemsInTags where TenantTypeId = @0 and TagName = @1", entity.TenantTypeId, entity.TagName));
        
            using (var transaction = dao.GetTransaction())
            {
                affectCount = dao.Execute(sqls);
                transaction.Complete();
            }

            if (affectCount > 0)
            {
                //更新实体缓存
                OnDeleted(entity);
                RealTimeCacheHelper.IncreaseGlobalVersion();
            }

            dao.CloseSharedConnection();

            return affectCount;
        }

        /// <summary>
        /// 获取标签实体
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public T Get(string tagName, string tenantTypeId)
        {
            string cacheKey = "TagIdToTagNames::TenantTypeId:" + tenantTypeId;
            Dictionary<string, long> tagNameToIds = cacheService.Get<Dictionary<string, long>>(cacheKey);

            long tagId = 0;
            if (tagNameToIds == null)
                tagNameToIds = new Dictionary<string, long>();
            if (!tagNameToIds.ContainsKey(tagName) || tagNameToIds[tagName] == 0)
            {
                Sql sql = Sql.Builder;
                sql.Select("TagId")
                   .From("tn_Tags")
                   .Where("TagName = @0", StringUtility.StripSQLInjection(tagName))
                   .Where("TenantTypeId = @0", tenantTypeId);

                var dao = CreateDAO();
                tagId = dao.FirstOrDefault<long?>(sql) ?? 0;
                tagNameToIds[tagName] = tagId;

                cacheService.Set(cacheKey, tagNameToIds, CachingExpirationType.UsualObjectCollection);
            }
            else
                tagId = tagNameToIds[tagName];

            return tagId == 0 ? null : Get(tagId);
        }

        /// <summary>
        /// 获取前N个标签
        /// </summary>
        /// <remarks>智能提示时也使用该方法获取数据</remarks>
        ///<param name="tenantTypeId">租户类型Id</param>
        ///<param name="topNumber">前N条数据</param>
        ///<param name="isFeatured">是否为特色标签</param>
        ///<param name="sortBy">标签排序字段</param>

        public IEnumerable<T> GetTopTags(string tenantTypeId, int topNumber, bool? isFeatured, SortBy_Tag? sortBy)
        {
            IEnumerable<T> topTags = new List<T>();
           
                var sql = Sql.Builder;
                var whereSql = Sql.Builder;
                var orderSql = Sql.Builder;
                sql.Select("tn_Tags.*")
                   .From("tn_Tags");
                if (!string.IsNullOrEmpty(tenantTypeId))
                    whereSql.Where("TenantTypeId = @0", tenantTypeId);
                if (isFeatured.HasValue)
                    whereSql.Where("IsFeatured = @0", isFeatured);

           
                CountService countService = new CountService(TenantTypeIds.Instance().Tag());
                StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().Tag());
                switch (sortBy)
                {
                    case SortBy_Tag.OwnerCountDesc:
                        orderSql.OrderBy("OwnerCount desc");
                        break;
                    case SortBy_Tag.ItemCountDesc:
                        orderSql.OrderBy("ItemCount desc");
                        break;
                    case SortBy_Tag.PreDayItemCountDesc:
                        string preDayCountType = stageCountTypeManager.GetStageCountTypes(CountTypes.Instance().ItemCounts()).Min();
                        sql.LeftJoin(string.Format("(select * from tn_CountsPerDay WHERE (tn_CountsPerDay.CountType = '{0}')) c", preDayCountType))
                        .On("TagId = c.ObjectId");
                        orderSql.OrderBy("c.StatisticsCount desc");
                        break;
                    case SortBy_Tag.PreWeekItemCountDesc:
                        string preWeekCountType = stageCountTypeManager.GetStageCountTypes(CountTypes.Instance().ItemCounts()).Max();
                        sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", preWeekCountType))
                        .On("TagId = c.ObjectId");
                        orderSql.OrderBy("c.StatisticsCount desc");
                        break;
                    default:
                        orderSql.OrderBy("TagId desc");
                        break;
                }
                sql.Append(whereSql).Append(orderSql);
                IEnumerable<object> ids = CreateDAO().FetchTopPrimaryKeys<T>(topNumber, sql);
                topTags = PopulateEntitiesByEntityIds(ids);
          

            return topTags;
        }

        /// <summary>
        /// 获取前N个标签名
        /// </summary>
        /// <remarks>用于智能提示</remarks>
        ///<param name="tenantTypeId">租户类型Id</param>
        ///<param name="ownerId">拥有者Id</param>
        ///<param name="keyword">标签名称关键字</param>
        ///<param name="topNumber">前N条数据</param>
        public IEnumerable<string> GetTopTagNames(string tenantTypeId, string keyword, int topNumber)
        {
            IEnumerable<string> topTagNames = new List<string>();
            var sql = Sql.Builder;
            sql.Select("tn_Tags.TagName")
               .From("tn_Tags")
               .Where("tn_Tags.TenantTypeId = @0", tenantTypeId)
               .Where("tn_Tags.TagName like @0", "%" + StringUtility.StripSQLInjection(keyword) + "%")
               .OrderBy("tn_Tags.ItemCount");

            topTagNames = CreateDAO().FetchTop<string>(topNumber, sql);

            return topTagNames;
        }

        /// <summary>
        ///分页检索标签
        /// </summary>
        ///<param name="query">查询条件</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        public PagingDataSet<T> GetTags(TagQuery query, int pageIndex, int pageSize)
        {
            PagingDataSet<T> tags = null;

            var sql = Sql.Builder;
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                //防sql注入
                query.Keyword = StringUtility.StripSQLInjection(query.Keyword);
                sql.Where("TagName like @0", "%" + query.Keyword + "%");
            }
       
            if (!string.IsNullOrEmpty(query.TenantTypeId))
            {
                sql.Where("TenantTypeId = @0", query.TenantTypeId);
            }
            if (query.IsFeatured.HasValue)
            {
                sql.Where("IsFeatured = @0", query.IsFeatured);
            }
            sql.OrderBy("ItemCount Desc");
            sql.OrderBy("DateCreated Desc");
            tags = GetPagingEntities(pageSize, pageIndex, sql);

            return tags;
        }


        /// <summary>
        /// 删除垃圾数据
        /// </summary>
        /// <param name="serviceKey">服务标识</param>
        public void DeleteTrashDatas()
        {
            IEnumerable<TenantType> tenantTypes = new TenantTypeRepository().Gets(MultiTenantServiceKeys.Instance().Tag());
            List<Sql> sqls = new List<Sql>();

            foreach (var tenantType in tenantTypes)
            {

                Type type = Type.GetType(tenantType.ClassType);
                if (type == null)
                    continue;

                var pd = TableInfo.FromPoco(type);

              
                    sqls.Add(Sql.Builder.Append("delete from tn_ItemsInTags")
                                        .Where("not exists (select 1 from (select 1 as c from tn_ItemsInTags," + pd.TableName + " where tn_ItemsInTags.ItemId = " + pd.PrimaryKey + ") as a) and tn_ItemsInTags.TenantTypeId = @0"
                                        , tenantType.TenantTypeId));
            }

            CreateDAO().Execute(sqls);
        }

        /// <summary>
        /// 标签内容数减1（删除资讯时使用）
        /// </summary>
        /// <param name="tagName"></param>
        public void SetItemCount(string tagName)
        {
            var sql = Sql.Builder;
            sql.Append("update tn_Tags set ItemCount=ItemCount-1").Where("TagName=@0",tagName);
            CreateDAO().Execute(sql);
        }
    }
}