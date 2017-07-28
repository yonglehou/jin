//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;
using Tunynet.Common;

namespace Tunynet.Common
{
    /// <summary>
    /// 通知数据访问
    /// </summary>
    public class NoticeRepository : Repository<Notice>, INoticeRepository
    {
        private int PageSize = 30;

        /// <summary>
        /// 创建通知
        /// </summary>
        /// <param name="entity">通知实体</param>
        public override void Insert(Notice entity)
        {
            base.Insert(entity);

            if (entity.Id > 0)
            {
                //更新缓存
                string cacheKey = GetCacheKey_UnhandledNoticeCount(entity.ReceiverId);
                int count;
                if (cacheService.TryGetValue<int>(cacheKey, out count))
                {
                    count++;
                    cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
                }
            }
        }

        /// <summary>
        /// 删除单条通知
        /// </summary>
        /// <param name="entityId">通知Id</param>
        public override int DeleteByEntityId(object entityId)
        {
            Notice entity = Get(entityId);
            if (entity == null)
                return 0;
            //更新缓存
            string cacheKey = GetCacheKey_UnhandledNoticeCount(entity.ReceiverId);

            if (entity.Status == NoticeStatus.Unhandled)
            {
                int count;
                if (cacheService.TryGetValue<int>(cacheKey, out count))
                {
                    if (count > 0)
                    {
                        count--;
                        cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
                    }
                }
            }
            int affectCount = base.DeleteByEntityId(entityId);
            return affectCount;
        }

        /// <summary>
        /// 清空接收人的通知记录
        /// </summary>
        /// <param name="userId">接收人Id</param>
        /// <param name="status">通知状态</param>
        public void ClearAll(long userId, NoticeStatus? status = null)
        {
            var sql = Sql.Builder;
            sql.Append("Delete from tn_Notices")
               .Where("ReceiverId=@0", userId);

            if (status.HasValue)
            {
                sql.Where("Status = @0", (int)status);
            }

            CreateDAO().Execute(sql);
            //更新缓存            
            RealTimeCacheHelper.IncreaseAreaVersion("UserId", userId);
        }

        /// <summary>
        /// 删除用户的记录（删除用户时使用）
        /// </summary>
        /// <param name="userId">用户id</param>
        public void CleanByUser(long userId)
        {
            List<Sql> sql_Deletes = new List<Sql>();
            sql_Deletes.Add(Sql.Builder.Append("delete from tn_Notices where LeadingActorUserId = @0", userId));
            CreateDAO().Execute(sql_Deletes);
        }

        /// <summary>
        /// 将通知设置为处理状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="noticestatus">处理状态</param>
        public void SetIsHandled(long id, NoticeStatus noticestatus)
        {
            Notice entity = Get(id);
            if (entity == null)
                return;
            var sql = Sql.Builder;

            sql.Append("UPDATE tn_Notices Set Status=@0", (int)noticestatus)
            .Where("Id=@0", id);
            CreateDAO().Execute(sql);
            //更新缓存
            string cacheKey = GetCacheKey_UnhandledNoticeCount(entity.ReceiverId);
            int count;
            if (cacheService.TryGetValue<int>(cacheKey, out count))
            {
                if (count >= 0)
                {
                    count--;
                    cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
                }
            }
            //更新缓存
            entity.Status = noticestatus;
            base.OnUpdated(entity);
        }

        /// <summary>
        /// 批量将所有未处理通知修改处理状态
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="noticestatus">处理状态</param>
        public void BatchSetIsHandled(long userId, NoticeStatus noticestatus)
        {
            var dao = CreateDAO();
            var sql = Sql.Builder;
            sql.Select("Id").From("tn_Notices")
                .Where("UseLeadingActorUserIdrId=@0", userId)
                .Where("Status=@0", (int)NoticeStatus.Unhandled);
            IEnumerable<object> ids_object = dao.FetchFirstColumn(sql);
            IEnumerable<long> ids = ids_object.Cast<long>();
            sql = Sql.Builder;
            sql.Append("UPDATE tn_Notices Set Status=@0", (int)noticestatus)
            .Where("LeadingActorUserId=@0", userId)
            .Where("Status=@0", (int)NoticeStatus.Unhandled);
            dao.Execute(sql);
            //更新缓存
            foreach (long id in ids)
            {
                Notice notice = Get(id);
                if (notice != null)
                    notice.Status = noticestatus;
                RealTimeCacheHelper.IncreaseEntityCacheVersion(id);
            }

            string cacheKey = GetCacheKey_UnhandledNoticeCount(userId);
            int count;
            if (cacheService.TryGetValue<int>(cacheKey, out count))
            {
                count = 0;
                cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
            }

            RealTimeCacheHelper.IncreaseAreaVersion("UserId", userId);
        }
        /// <summary>
        /// 根据触发通知对象的ID获取通知 (用于评论审核通过时发送通知)
        /// </summary>
        /// <param name="objectId">触发通知对象ID</param>
        /// <returns></returns>
        public Notice GetNoticeByObjectId(long objectId)
        {
           var sql = Sql.Builder.Where("Objectid=@0", objectId);
           return CreateDAO().SingleOrDefault<Notice>(sql);
        }

        /// <summary>
        /// 获取某人的未处理通知数
        /// </summary>
        public int GetUnhandledCount(long userId, NoticeStatus noticestatus)
        {
            string cacheKey = GetCacheKey_UnhandledNoticeCount(userId);

            int count;
            if (!cacheService.TryGetValue<int>(cacheKey, out count))
            {
                var sql = Sql.Builder;
                sql.Select("Count(*)")
                .From("tn_Notices")
                .Where("LeadingActorUserId=@0 and Status=@1", userId, (int)NoticeStatus.Unhandled);
                count = CreateDAO().FirstOrDefault<int>(sql);
                cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
            }
            return count;
        }

        /// <summary>
        ///获取用户通知的分页集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">通知状态</param>
        /// <param name="typeId">通知类型Id</param>
        /// <param name="applicationId">应用Id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>通知分页集合</returns>
        public PagingDataSet<Notice> Gets(long userId, NoticeStatus? status, int pageIndex)
        {
            var sql = Sql.Builder;
            sql.Where("ReceiverId = @0", userId);

            if (status.HasValue)
                sql.Where("Status = @0", (int)status);
            sql.OrderBy("Id  DESC");
            return GetPagingEntities(PageSize, pageIndex, sql);
        }
        /// <summary>
        /// 获取某人的未处理通知数(不包括评论)
        /// </summary>
        public int GetUnhandledCountNoComment(long userId)
        {
            string cacheKey = GetCacheKey_UnhandledNoticeCount(userId);

            int count;
            if (!cacheService.TryGetValue<int>(cacheKey, out count))
            {
                var sql = Sql.Builder;
                sql.Select("Count(*)")
                .From("tn_Notices")
                .Where("LeadingActorUserId=@0 and Status=@1", userId, (int)NoticeStatus.Unhandled);
                sql.Where("NoticeTypeKey != @0 ", NoticeTypeKeys.Instance().NewComment());
                count = CreateDAO().FirstOrDefault<int>(sql);
                cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
            }
            return count;
        }
        /// <summary>
        /// 获取用户最近几条未处理的通知
        /// </summary>
        /// <param name="topNumber"></param>
        /// <param name="userId">通知接收人Id</param>
        public IEnumerable<Notice> GetTops(long userId, int topNumber)
        {
           
            var sql = Sql.Builder;
            sql.Where("ReceiverId=@0 and Status=@1", userId, (int)NoticeStatus.Unhandled);
            sql.OrderBy("Id  DESC");
            return GetTopEntities(topNumber, sql);
           
        }

        /// <summary>
        /// 根据条件获取所有通知
        /// </summary>
        /// <param name="noticeTypeKey">通知key</param>
        /// <param name="Interval">上次间隔 （秒）</param>
        /// <param name="Status">状态</param>
        /// <returns></returns>
        public IEnumerable<Notice> GetNotices(string noticeTypeKey, int? Interval = null, int? Status = null
            )
        {
            Sql sql = Sql.Builder;
            sql.Where("NoticeTypeKey =@0", noticeTypeKey);
            if (Interval.HasValue)
            {
                sql.Where("LastSendDate <@0", DateTime.Now.AddSeconds(-Interval.Value));
            }
            if (Status.HasValue)
            {
                sql.Where("Status =@0", Status.Value);
            }
            return base.Gets(sql);
        }

        /// <summary>
        /// 获取用户通知的分页集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">通知状态</param>
        /// <param name="typeId">通知类型Id</param>
        /// <param name="applicationId">应用Id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>通知分页集合</returns>
        public PagingDataSet<Notice> Gets(long userId, NoticeStatus? status, int? typeId, int? applicationId, int pageIndex)
        {
            var sql = Sql.Builder;
            sql.Where(" LeadingActorUserId = @0", userId);

            if (status.HasValue)
                sql.Where("Status = @0", (int)status);

            if (typeId.HasValue && typeId.Value > 0)
                sql.Where("TypeId=@0", typeId.Value);

            sql.OrderBy("Id  DESC");
            return GetPagingEntities(PageSize, pageIndex, sql);
           
        }

        /// <summary>
        /// 获取用户通知的分页集合(不包括评论)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">通知状态</param>
        /// <param name="typeId">通知类型Id</param>
        /// <param name="applicationId">应用Id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>通知分页集合</returns>
        public PagingDataSet<Notice> GetsForNoComment(long userId, NoticeStatus? status, int? typeId, int? applicationId, int pageIndex)
        {
            var sql = Sql.Builder;
            sql.Where(" UserId = @0", userId);

            if (status.HasValue)
                sql.Where("Status = @0", (int)status);

            if (typeId.HasValue && typeId.Value > 0)
                sql.Where("TypeId=@0", typeId.Value);

            sql.Where(" TemplateName != @0 ", NoticeTypeKeys.Instance().NewComment());
            sql.OrderBy("Id  DESC");
            return GetPagingEntities(PageSize, pageIndex, sql);
        }
       

        #region Help Methods

        /// <summary>
        /// 获取用户未处理通知数的CacheKey
        /// </summary>
        /// <param name="userId">用户Id</param>
        private string GetCacheKey_UnhandledNoticeCount(long userId)
        {
            return string.Format("UnhandledNoticeCount::UserId-{0}", userId);
        }

        #endregion

    }
}
