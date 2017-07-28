//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Tunynet.Repositories;
using PetaPoco;
using System.Text;
using Tunynet.Caching;
using System;

namespace Tunynet.Attitude.Repositories
{
    /// <summary>
    /// 顶踩记录的数据访问
    /// </summary>
    public class AttitudeRecordRepository : Repository<AttitudeRecord>, IAttitudeRecordRepository
    {

        /// <summary>
        /// 获取参与用户的Id集合
        /// </summary>
        /// <param name="objectId">操作对象Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">获取条数</param>
        public IEnumerable<long> GetTopOperatedUserIds(long objectId, string tenantTypeId, int? topNumber)
        {
            StringBuilder cacheKey = new StringBuilder(RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "ObjectId", objectId));
            cacheKey.AppendFormat("TenantTypeId-{0}", tenantTypeId);
            IEnumerable<long> topOperatedUserIds;
            cacheService.TryGetValue<IEnumerable<long>>(cacheKey.ToString(), out topOperatedUserIds);
            if (topOperatedUserIds == null)
            {
                var sql = PetaPoco.Sql.Builder;
                sql.Select("UserId")
                   .From("tn_AttitudeRecords")
                   .Where("ObjectId =@0", objectId)
                   .Where("TenantTypeId=@0", tenantTypeId)
                   .OrderBy("Id desc");
                topOperatedUserIds = CreateDAO().FetchTop<long>(1000, sql).Cast<long>();
                cacheService.Set(cacheKey.ToString(), topOperatedUserIds, CachingExpirationType.ObjectCollection);
            }
            if (topNumber.HasValue)
            {
                return topOperatedUserIds.Take(topNumber.Value);
            }
            return topOperatedUserIds;
        }

        /// <summary>
        /// 获取用户在某一租户下顶过的内容
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        ///<param name="userId">用户Id</param>
        ///<param name="pageSize">每页的内容数</param>
        ///<param name="pageIndex">页码</param>
        public IEnumerable<long> GetObjectIds(string tenantTypeId, long userId, int pageSize, int pageIndex)
        {
            var sql = Sql.Builder;
            sql.Select("ObjectId")
               .From("tn_AttitudeRecords")
               .Where("TenantTypeId = @0", tenantTypeId)
               .Where("UserId=@0", userId);

            sql.OrderBy("Id DESC");

            IEnumerable<object> followedObjectIds = null;
            long totalRecords;

            followedObjectIds = CreateDAO().FetchPagingPrimaryKeys(pageSize, pageIndex,"ObjectId", sql, out totalRecords);
            if (followedObjectIds != null)
            {
                PagingDataSet<long> pds = new PagingDataSet<long>(followedObjectIds.Cast<long>());
                pds.PageSize = pageSize;
                pds.PageIndex = pageIndex;
                pds.TotalRecords = totalRecords;
                return pds;
            }
          
            return new List<long>();
        }

        /// <summary>
        /// 获取用户对某项的所有顶踩
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="tenantTypeId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dictionary<long, bool> GetAllAttitues(string tenantTypeId, long userId)
        {
            var sql = Sql.Builder;
            sql.Select("*")
               .From("tn_AttitudeRecords")
               .Where("TenantTypeId = @0", tenantTypeId)
               .Where("UserId=@0", userId);

            sql.OrderBy("Id DESC");

            string cacheKey = GetCacheKey_AllAttitues(tenantTypeId, userId);
            Dictionary<long, bool> dict = null;
            cacheService.TryGetValue<Dictionary<long, bool>>(cacheKey, out dict);
            if (dict == null)
            {
                dict = new Dictionary<long, bool>();
                var data = CreateDAO().Fetch<AttitudeRecord>(sql);

            foreach (var item in data)
            {
                dict[item.ObjectId] = true;
            }

                cacheService.Set(cacheKey, dict, CachingExpirationType.ObjectCollection);
            }
            return dict;
        }

        /// <summary>
        /// 获取用户对某项的所有顶踩
        /// </summary>
        /// <param name="tenantTypeId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetCacheKey_AllAttitues(string tenantTypeId, long userId)
        {
            return string.Format("AllAttitues::TenantTypeId-{0}:UserId-{1}", tenantTypeId, userId);
        }

        
    }
}