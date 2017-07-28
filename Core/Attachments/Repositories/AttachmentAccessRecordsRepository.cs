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

namespace Tunynet.Common
{
    /// <summary>
    /// AttachmentAccessRecords仓储
    /// </summary>
    public class AttachmentAccessRecordsRepository<T> : Repository<AttachmentAccessRecords>, IAttachmentAccessRecordsRepository
    {
        private int pageSize = 20;

        #region Create/Update
        /// <summary>
        /// 创建新的下载记录
        /// </summary>
        /// <param name="record">下载记录实体</param>
        /// <returns>下载记录Id</returns>
        public new void Insert(AttachmentAccessRecords record)
        {
            base.Insert(record);
            if (record.Id > 0)
            {
               
                string cacheKey = GetCacheKey_RecordIds_AttachmentIds(record.UserId);
                Dictionary<long, long> ids_AttachmentIds = cacheService.GetFromFirstLevel<Dictionary<long, long>>(cacheKey);
                if (ids_AttachmentIds != null && !ids_AttachmentIds.Values.Contains(record.AttachmentId))
                {
                    ids_AttachmentIds[record.Id] = record.AttachmentId;
                    cacheService.Set(cacheKey, ids_AttachmentIds, CachingExpirationType.UsualObjectCollection);
                }
            }
        }

        /// <summary>
        /// 更新最后下载时间
        /// </summary>
        /// <param name="userId">下载用户UserId</param>
        /// <param name="attachmentId">附件Id</param>
        public bool UpdateLastDownloadDate(long userId, long attachmentId)
        {
            var sql = Sql.Builder;
            sql.Append("Update tn_AttachmentAccessRecords set LastDownloadDate = @0", DateTime.Now)
               .Where("UserId = @0 and AttachmentId = @1", userId, attachmentId);

            int count = CreateDAO().Execute(sql);

            Dictionary<long, long> ids_AttachmentIds = GetIds_AttachmentIdsByUser(userId);
            if (ids_AttachmentIds != null && ids_AttachmentIds.Values.Contains(attachmentId))
            {
                //更新实体缓存
                RealTimeCacheHelper.IncreaseEntityCacheVersion(ids_AttachmentIds.FirstOrDefault(n => n.Value == attachmentId).Key);
            }

            return count > 0;
        }

        #endregion

        #region Get/Gets

        /// <summary>
        /// 根据获取用户附件下载记录及附件的Id集合
        /// </summary>
        /// <param name="userId">下载用户UserId</param>
        public Dictionary<long, long> GetIds_AttachmentIdsByUser(long userId)
        {
            string cacheKey = GetCacheKey_RecordIds_AttachmentIds(userId);

            Dictionary<long, long> ids_attachmentIds = cacheService.GetFromFirstLevel<Dictionary<long, long>>(cacheKey);

            if (ids_attachmentIds == null || ids_attachmentIds.Count == 0)
            {
                var sql = Sql.Builder;
                sql.Select("Id,AttachmentId")
                   .From("tn_AttachmentAccessRecords")
                   .Where("UserId = @0", userId);

                IEnumerable<dynamic> reuslts = CreateDAO().Fetch<dynamic>(sql);

                if (reuslts != null)
                {
                    ids_attachmentIds = reuslts.ToDictionary<dynamic, long, long>(v => v.Id, v => v.AttachmentId);
                }

                //更新缓存
                cacheService.Set(cacheKey, ids_attachmentIds, CachingExpirationType.RelativelyStable);
            }

            return ids_attachmentIds;
        }

        /// <summary>
        /// 获取附件的前topNumber条下载记录
        /// </summary>
        /// <param name="attachmentId">附件Id</param>
        /// <param name="topNumber">返回的记录数</param>
        public IEnumerable<AttachmentAccessRecords> GetTopsByAttachmentId(long attachmentId, int topNumber)
        {
            var sql = Sql.Builder;
            sql.Where("AttachmentId = @0", attachmentId);
            return GetTopEntities(topNumber, sql);
        }

        /// <summary>
        /// 获取附件的下载记录分页显示
        /// </summary>
        /// <param name="attachmentId">附件Id</param>
        /// <param name="pageIndex">页码</param>
        public PagingDataSet<AttachmentAccessRecords> GetsByAttachmentId(long attachmentId, int pageIndex)
        {
            var sql = Sql.Builder;
            sql.Where("AttachmentId = @0", attachmentId)
                .OrderBy("LastDownloadDate desc");
            return GetPagingEntities(pageSize, pageIndex, sql);            
        }

     
        /// <summary>
        /// 获取用户的下载记录分页显示
        /// </summary>
        /// <param name="userId">下载用户UserId</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="needToBuy">是否需要购买</param>
        public PagingDataSet<AttachmentAccessRecords> GetsByUserId(long userId, int pageIndex, bool needToBuy = true)
        {
            //组装获取实体的sql语句
            var sql = Sql.Builder;
            sql.Where("UserId = @0", userId);
            if (needToBuy)
                sql.Where("Price > 0");
            else
                sql.Where("Price = 0");
            return GetPagingEntities(pageSize, pageIndex, sql);
        }

        /// <summary>
        /// 获取拥有者附件的下载记录分页显示
        /// </summary>
        /// <param name="userId">附件拥有者Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="needToBuy">是否需要购买</param>
        public PagingDataSet<AttachmentAccessRecords> GetsByOwnerId(long userId, int pageIndex, bool needToBuy = true)
        {
            //组装获取实体的sql语句
            var sql = Sql.Builder;
            sql.Where("UserId = @0", userId);
            if (needToBuy)
                sql.Where("Price > 0");
            else
                sql.Where("Price = 0");
            return GetPagingEntities(pageSize, pageIndex, sql);
        }

        #endregion

        #region GetCacheKey

        /// <summary>
        /// 获取下载记录与附件Id集合的CacheKey
        /// </summary>
        /// <param name="userId">下载用户UserId</param>
        /// <returns></returns>
        private string GetCacheKey_RecordIds_AttachmentIds(long userId)
        {
            return "RecordIds_AttachmentIds::UserId:" + userId;
        }

        #endregion

    }
}
