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
using Tunynet.Settings;

namespace Tunynet.Post
{
    /// <summary>
    /// 贴子仓储
    /// </summary>
    public class ThreadRepository : Repository<Thread>, IThreadRepository
    {
        IBodyProcessor barBodyProcessor = DIContainer.ResolveNamed<IBodyProcessor>(TenantTypeIds.Instance().Section());
        private int pageSize = 20;
        private ISettingsManager<SiteSettings> siteSettings = DIContainer.Resolve<ISettingsManager<SiteSettings>>();

        /// <summary>
        /// 更新贴子
        /// </summary>
        /// <param name="entity"></param>
        public override void Update(Thread entity)
        {
            base.Update(entity);
            //更新解析正文缓存
            string cacheKey = GetCacheKeyOfResolvedBody(entity.ThreadId);
            string resolveBody = cacheService.Get<string>(cacheKey);
            if (resolveBody != null)
            {
                resolveBody = entity.GetBody();

                resolveBody = barBodyProcessor.Process(resolveBody, TenantTypeIds.Instance().Thread(), entity.ThreadId, entity.UserId);
                cacheService.Set(cacheKey, resolveBody, CachingExpirationType.SingleObject);
            }
        }

        /// <summary>
        /// 移动贴子
        /// </summary>
        /// <param name="threadId"></param>
        /// <param name="moveToSectionId"></param>
        public void MoveThread(long threadId, long moveToSectionId)
        {
            Thread thread = Get(threadId);
            if (thread == null)
                return;
            if (thread.SectionId == moveToSectionId)
                return;
            long oldSectionId = thread.SectionId;
            thread.SectionId = moveToSectionId;

            var sql = Sql.Builder.Append("update tn_Threads set SectionId = @0 where ThreadId = @1", moveToSectionId, threadId);
            CreateDAO().Execute(sql);


        }



        /// <summary>
        /// 删除用户记录（删除用户时使用）
        /// </summary>
        /// <param name="userId">被删除用户</param>
        /// <param name="takeOver">接管用户</param>
        /// <param name="takeOverAll">是否接管被删除用户的所有内容</param>
        public void DeleteUser(long userId, User takeOver, bool takeOverAll)
        {
            List<Sql> sqls = new List<Sql>();
            if (takeOver != null)
            {
                sqls.Add(Sql.Builder.Append("update tn_Sections set UserId = @0 where UserId = @1", takeOver.UserId, userId));
                if (takeOverAll)
                {
                    sqls.Add(Sql.Builder.Append("update tn_Threads set UserId = @0,Author=@1 where UserId = @2", takeOver.UserId, takeOver.DisplayName, userId));
                }
            }

            CreateDAO().Execute(sqls);
        }

        /// <summary>
        /// 删除贴子
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override int Delete(Thread entity)
        {
            //同时删除贴子、回贴、评分记录
            List<Sql> sqls = new List<Sql>();



            //sqls.Add(Sql.Builder.Append("delete from spb_BarRatings where ThreadId = @0", entity.ThreadId));

            CreateDAO().Execute(sqls);
            int affectCount = base.Delete(entity);

            return affectCount;
        }

        /// <summary>
        /// 获取上一篇
        /// </summary>
        /// <param name="barThread"></param>
        /// <returns></returns>
        public long GetPrevThreadId(Thread barThread)
        {
            var dao = CreateDAO();
            string cacheKey = string.Format("BarPrevThreadId-{0}", barThread.ThreadId);
            long prevThreadId;
            if (!cacheService.TryGetValue<long>(cacheKey, out prevThreadId))
            {
                var sql = Sql.Builder;
                sql.Select("ThreadId")
                .From("tn_Threads")
                .Where("LastModified > @0", barThread.LastModified)
                .Where("TenantTypeId = @0", barThread.TenantTypeId)
                .Where("SectionId = @0", barThread.SectionId);



                sql.OrderBy("LastModified");
                var ids_object = dao.FetchTopPrimaryKeys<Thread>(1, sql);
                if (ids_object.Count() > 0)
                    prevThreadId = ids_object.Cast<long>().First();
                cacheService.Set(cacheKey, prevThreadId, CachingExpirationType.SingleObject);
            }
            return prevThreadId;
        }


        /// <summary>
        /// 获取下一篇
        /// </summary>
        /// <param name="barThread"></param>
        /// <returns></returns>
        public long GetNextThreadId(Thread barThread)
        {
            var dao = CreateDAO();
            string cacheKey = string.Format("BarNextThreadId-{0}", barThread.ThreadId);
            long nextThreadId;
            if (!cacheService.TryGetValue<long>(cacheKey, out nextThreadId))
            {
                var sql = Sql.Builder;
                sql.Select("ThreadId")
                .From("tn_Threads")
                .Where("LastModified < @0", barThread.LastModified)
                .Where("TenantTypeId = @0", barThread.TenantTypeId)
                .Where("SectionId = @0", barThread.SectionId);

                sql.OrderBy("LastModified desc");
                var ids_object = dao.FetchTopPrimaryKeys<Thread>(1, sql);
                if (ids_object.Count() > 0)
                    nextThreadId = ids_object.Cast<long>().First();
                cacheService.Set(cacheKey, nextThreadId, CachingExpirationType.SingleObject);
            }
            return nextThreadId;
        }

        /// <summary>
        /// 获取某个贴吧下的所有贴子（用于删除贴子）
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public IEnumerable<Thread> GetAllThreadsOfSection(long sectionId)
        {
            var sql = Sql.Builder;
            sql.Select("*")
            .From("tn_Threads")
            .Where("SectionId=@0", sectionId);
            IEnumerable<Thread> threads = CreateDAO().Fetch<Thread>(sql);
            return threads;
        }

        /// <summary>
        /// 获取某个用户的所有贴子（用于删除用户）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Thread> GetAllThreadsOfUser(long userId)
        {
            var sql = Sql.Builder;
            sql.Select("*")
            .From("tn_Threads")
            .Where("UserId=@0", userId);
            IEnumerable<Thread> threads = CreateDAO().Fetch<Thread>(sql);
            return threads;
        }
        /// <summary>
        /// 获取某个拥有者时删除所有贴子（用于删除对象）
        /// </summary>
        /// <param name="OwnerId"></param>
        /// <returns></returns>
        public IEnumerable<Thread> GetAllThreadsOfOwner(long OwnerId)
        {
            var sql = Sql.Builder;
            sql.Select("*")
            .From("tn_Threads")
            .Where("OwnerId=@0", OwnerId);
            IEnumerable<Thread> threads = CreateDAO().Fetch<Thread>(sql);
            return threads;
        }

        /// <summary>
        /// 获取解析的正文
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public string GetResolvedBody(long threadId)
        {
            Thread barThread = Get(threadId);
            if (barThread == null)
                return string.Empty;

            string cacheKey = GetCacheKeyOfResolvedBody(threadId);
            string resolveBody = cacheService.Get<string>(cacheKey);
            if (resolveBody == null)
            {
                resolveBody = barThread.GetBody();
                resolveBody = barBodyProcessor.Process(resolveBody, TenantTypeIds.Instance().Thread(), barThread.ThreadId, barThread.UserId);
                cacheService.Set(cacheKey, resolveBody, CachingExpirationType.SingleObject);
            }
            return resolveBody;
        }


        /// <summary>
        /// 获取BarThread内容
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public string GetBody(long threadId)
        {
            string cacheKey = RealTimeCacheHelper.GetCacheKeyOfEntityBody(threadId);
            string body = cacheService.Get<string>(cacheKey);
            if (body == null)
            {
                Thread barThread = CreateDAO().SingleOrDefault<Thread>("Where ThreadId = @0", threadId);
                body = barThread != null ? barThread.Body : string.Empty;
                cacheService.Set(cacheKey, body, CachingExpirationType.SingleObject);
            }
            return body;
        }

        /// <summary>
        /// 获取用户的主题贴分页集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="ignoreAudit">是否忽略审核状态（作者或管理员查看时忽略审核状态）</param>
        /// <param name="isPosted">是否是取我回复过的</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns>主题贴列表</returns>
        public PagingDataSet<Thread> GetUserThreads(string tenantTypeId, long userId, bool ignoreAudit, bool isPosted, int pageSize, int pageIndex, long? sectionId)
        {
            //不必筛选审核状态
            //缓存周期：对象集合，需要维护即时性
            //排序：发布时间（倒序）
            var sql = Sql.Builder;
            sql.Select("ThreadId")
            .From("tn_Threads")
            .Where("UserId = @0", userId)
            .Where("TenantTypeId = @0", tenantTypeId);
            if (sectionId.HasValue)
                sql.Where("SectionId=@0", sectionId.Value);
            //过滤审核状态
            if (!ignoreAudit)
                sql = AuditSqls(sql);
            sql.OrderBy("ThreadId desc");
            return GetPagingEntities(pageSize, pageIndex, sql);
        }

        /// <summary>
        /// 获取主题贴的排行数据
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">前多少条</param>
        /// <param name="sortBy">主题贴排序依据</param>
        /// <returns></returns>
        public IEnumerable<Thread> GetTops(string tenantTypeId, int topNumber, bool? isEssential, SortBy_BarThread sortBy)
        {
            //只获取可对外显示审核状态的主题贴
            //缓存周期：常用对象集合，不用维护即时性
            var sql = Sql.Builder;
            var whereSql = Sql.Builder;
            var orderSql = Sql.Builder;
            sql.Select("tn_Threads.*")
            .From("tn_Threads");
            whereSql.Where("tn_Threads.TenantTypeId = @0", tenantTypeId);

            if (isEssential.HasValue)
                whereSql.Where("IsEssential = @0", isEssential.Value);
            //审核
            whereSql = AuditSqls(whereSql);

            switch (sortBy)
            {
                case SortBy_BarThread.DateCreated_Desc:
                    orderSql.OrderBy("ThreadId desc");
                    break;
                case SortBy_BarThread.LastModified_Desc:
                    orderSql.OrderBy("LastModified desc");
                    break;
                case SortBy_BarThread.HitTimes:
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", CountTypes.Instance().HitTimes()))
                    .On("ThreadId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarThread.StageHitTimes:
                    StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().Thread());
                    int stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().HitTimes());
                    string stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().HitTimes(), stageCountDays);
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", stageCountType))
                    .On("ThreadId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarThread.PostCount:
                    orderSql.OrderBy("PostCount desc");
                    break;
                default:
                    orderSql.OrderBy("ThreadId desc");
                    break;
            }
            sql.Append(whereSql).Append(orderSql);
            return GetTopEntities(topNumber, sql);
        }
        /// <summary>
        /// 根据标签名获取主题贴排行分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="tagName">标签名</param>
        /// <param name="isEssential">是否为精华贴</param>
        /// <param name="sortBy">贴子排序依据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>主题贴列表</returns>
        public PagingDataSet<Thread> Gets(string tenantTypeId, string tagName, bool? isEssential, SortBy_BarThread sortBy, int pageIndex)
        {
            //只获取可对外显示审核状态的主题贴
            //缓存周期：对象集合，不用维护即时性
            var sql = Sql.Builder;
            sql.Select("tn_Threads.*")
            .From("tn_Threads");
            var whereSql = Sql.Builder;
            var orderSql = Sql.Builder;

            whereSql.Where("tn_Threads.TenantTypeId = @0", tenantTypeId);
            if (!string.IsNullOrEmpty(tagName))
            {
                sql.InnerJoin("tn_ItemsInTags")
                .On("tn_Threads.ThreadId = tn_ItemsInTags.ItemId");

                whereSql.Where("tn_ItemsInTags.TagName=@0", tagName)
                .Where("tn_ItemsInTags.TenantTypeId = @0", TenantTypeIds.Instance().Thread());
            }

            if (isEssential.HasValue)
                whereSql.Where("IsEssential = @0", isEssential.Value);
            //审核
            whereSql = AuditSqls(whereSql);

            CountService countService = new CountService(TenantTypeIds.Instance().Thread());

            switch (sortBy)
            {
                case SortBy_BarThread.DateCreated_Desc:
                    orderSql.OrderBy("ThreadId desc");
                    break;
                case SortBy_BarThread.LastModified_Desc:
                    orderSql.OrderBy("LastModified desc");
                    break;


                case SortBy_BarThread.HitTimes:
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", CountTypes.Instance().HitTimes()))
                              .On("ThreadId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarThread.StageHitTimes:
                    StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().Thread());
                    int stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().HitTimes());
                    string stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().HitTimes(), stageCountDays);
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", stageCountType))
                    .On("ThreadId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarThread.PostCount:
                    orderSql.OrderBy("PostCount desc");
                    break;
                default:
                    orderSql.OrderBy("ThreadId desc");
                    break;
            }
            sql.Append(whereSql).Append(orderSql);
            return GetPagingEntities(pageSize, pageIndex, sql);
        }


        /// <summary>
        /// 根据贴吧获取主题贴分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="sectionId">贴吧Id</param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="orderBySticky">是否置顶</param>
        /// <param name="sortBy">贴子排序依据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>主题贴列表</returns>
        public PagingDataSet<Thread> Gets(long sectionId, long? ownerId, long? categoryId, bool? orderBySticky, SortBy_BarThread sortBy, int pageSize, int pageIndex, SortBy_BarDateThread BarDate)
        {
            //只获取可对外显示审核状态的主题贴
            //缓存周期：对象集合，需要维护即时性
            var sql = Sql.Builder;
            var whereSql = Sql.Builder;
            var orderSql = Sql.Builder;
            sql.Select("tn_Threads.*")
            .From("tn_Threads");
            whereSql.Where("SectionId = @0", sectionId);
            if (ownerId.HasValue)
            {
                if (ownerId.Value == -1)
                {
                    whereSql.Where("ownerId != 0");
                }
                else
                {
                    whereSql.Where("ownerId = @0", ownerId.Value);
                }
            }
            //审核
            whereSql = AuditSqls(whereSql);
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                sql.InnerJoin("tn_ItemsInCategories")
                .On("ThreadId = tn_ItemsInCategories.ItemId");
                whereSql.Where("tn_ItemsInCategories.CategoryId=@0", categoryId.Value);
            }

            switch (BarDate)
            {
                case SortBy_BarDateThread.ThreeDay:
                    whereSql.Where("tn_Threads.DateCreated>@0", DateTime.Now.AddDays(-3));
                    break;
                case SortBy_BarDateThread.SevenDay:
                    whereSql.Where("tn_Threads.DateCreated>@0", DateTime.Now.AddDays(-7));
                    break;
                case SortBy_BarDateThread.AMonth:

                    whereSql.Where("tn_Threads.DateCreated>@0", DateTime.Now.AddMonths(-1));
                    break;

            }
            //置顶排序
            if (orderBySticky.HasValue && orderBySticky.Value)
                orderSql.OrderBy("IsSticky desc");
            CountService countService = new CountService(TenantTypeIds.Instance().Thread());

            switch (sortBy)
            {
                case SortBy_BarThread.DateCreated_Desc:
                    orderSql.OrderBy("ThreadId desc");
                    break;
                case SortBy_BarThread.LastModified_Desc:
                    orderSql.OrderBy("LastModified desc");
                    break;
                case SortBy_BarThread.HitTimes:
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", CountTypes.Instance().HitTimes()))
                              .On("ThreadId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarThread.StageHitTimes:
                    StageCountTypeManager stageCountTypeManager = StageCountTypeManager.Instance(TenantTypeIds.Instance().Thread());
                    int stageCountDays = stageCountTypeManager.GetMaxDayCount(CountTypes.Instance().HitTimes());
                    string stageCountType = stageCountTypeManager.GetStageCountType(CountTypes.Instance().HitTimes(), stageCountDays);
                    sql.LeftJoin(string.Format("(select * from tn_Counts WHERE (tn_Counts.CountType = '{0}')) c", stageCountType))
                    .On("ThreadId = c.ObjectId");
                    orderSql.OrderBy("c.StatisticsCount desc");
                    break;
                case SortBy_BarThread.PostCount:
                    orderSql.OrderBy("PostCount desc");
                    break;
                default:
                    orderSql.OrderBy("ThreadId desc");
                    break;
            }


            sql.Append(whereSql).Append(orderSql);
            return GetPagingEntities(pageSize, pageIndex, sql);
        }

        /// <summary>
        /// 贴子管理时查询贴子分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="query">贴子查询条件</param>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴子分页集合</returns>
        public PagingDataSet<Thread> Gets(string tenantTypeId, ThreadQuery query, int pageSize, int pageIndex)
        {
            var sql = Sql.Builder;
            sql.Select("*")
            .From("tn_Threads");

            if (query.CategoryId.HasValue && query.CategoryId.Value > 0)
            {
                sql.InnerJoin("tn_ItemsInCategories")
                .On("ThreadId = tn_ItemsInCategories.ItemId");
                sql.Where("tn_ItemsInCategories.CategoryId=@0", query.CategoryId.Value);
            }

            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);

            if (query.UserId != null && query.UserId > 0)
                sql.Where("UserId = @0", query.UserId);

            if (query.SectionId != null && query.SectionId > 0)
                sql.Where("SectionId = @0", query.SectionId);

            if (!string.IsNullOrEmpty(query.SubjectKeyword))
                sql.Where("Subject like @0 or Author like @0", "%"+ StringUtility.StripSQLInjection(query.SubjectKeyword) + "%");

            if (query.StartDate != null)
                sql.Where("DateCreated >= @0", query.StartDate);
            if (query.EndDate != null)
                sql.Where("DateCreated < @0", query.EndDate.Value.AddDays(1));

            if (query.AuditStatus.HasValue)
            {
                //审核
                sql.Where("ApprovalStatus=@0", query.AuditStatus);
            }

            if (query.IsSticky.HasValue&& query.IsSticky.Value)
            {
                sql.OrderBy("IsSticky desc");
            }
            
            sql.OrderBy("ThreadId desc");
            return GetPagingEntities(pageSize, pageIndex, sql);
        }

        /// <summary>
        /// 帖子计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        public int GetThreadCount(AuditStatus? approvalStatus, bool is24Hours)
        {
            Sql sql = Sql.Builder;
            sql.Select(" count(tn_Threads.ThreadId )").From("tn_Threads");
            if (approvalStatus.HasValue)
                sql.Where("tn_Threads.ApprovalStatus=@0", (int)approvalStatus.Value);
            if (is24Hours)
                sql.Where("tn_Threads.DateCreated>@0", DateTime.Now.AddHours(-24));
            return CreateDAO().SingleOrDefault<int>(sql);
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
                wheresql.Where("ApprovalStatus>@0 ", PubliclyAuditStatus.Pending);
            else if (setting.AuditStatus == PubliclyAuditStatus.Pending)
                wheresql.Where("ApprovalStatus>@0 ", PubliclyAuditStatus.Fail);
            return wheresql;

        }


        /// <summary>
        /// 获取解析正文缓存Key
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        private string GetCacheKeyOfResolvedBody(long threadId)
        {
            return "BarThreadResolvedBody" + threadId;
        }
        /// <summary>
        /// 获取解析正文缓存Key
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        private string GetCacheKeyOfResolvedBodyForMobile(long threadId)
        {
            return "BarThreadResolvedBodyForMobile" + threadId;
        }

    }
}