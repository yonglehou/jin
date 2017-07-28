//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;
using System;

namespace Tunynet.Common
{
    /// <summary>
    ///广告位数据访问仓储
    /// </summary>
    public class AdvertisingPositionRepository : Repository<AdvertisingPosition>, IAdvertisingPositionRepository
    {
        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <param name="height">高度</param>
        /// <param name="width">宽度</param>
        /// <param name="isEnable">是否启用</param>
        /// <returns></returns>
        public IEnumerable<AdvertisingPosition> GetPositionsForAdmin( int? height, int? width, bool? isEnable)
        {
            Sql sql = Sql.Builder;
            sql.Select("*")
                .From("tn_AdvertisingPositions");
            if (height.HasValue&& width.HasValue)
            {
                sql.Where("Height=@0 and Width=@1", height, width);
            }
            if (isEnable.HasValue)
            {
                sql.Where("IsEnable=@0", isEnable);
            }
            sql.OrderBy("Width desc");
            return CreateDAO().Fetch<AdvertisingPosition>(sql);
        }

        /// <summary>
        /// 获取广告位统计数据
        /// </summary>
        /// <returns></returns>
        public long GetPositionCount()
        {
            string cacheKey = "GetPositionCount";
            long positionCount;
            if (cacheService.TryGetValue<long>(cacheKey,out positionCount))
            {
                return positionCount;
            }
            Sql sql = Sql.Builder;
            sql.Select("count(*)")
                .From("tn_AdvertisingPositions");
            positionCount = CreateDAO().FirstOrDefault<long>(sql);

            cacheService.Set(cacheKey, positionCount, CachingExpirationType.UsualSingleObject);

            return positionCount;
        }

        /// <summary>
        /// 根据广告位Id取所有的广告
        /// </summary>
        /// <param name="positionId">广告位Id</param>
        /// <param name="isEnable">是否启用</param>
        /// <returns></returns>
        public IEnumerable<Advertising> GetAdvertisingsByPositionId(long positionId, bool? isEnable)
        {
                Sql sql = Sql.Builder
                            .Select("tn_Advertisings.*")
                            .From("tn_Advertisings")
                            .InnerJoin("tn_AdvertisingsInPositions").On("tn_AdvertisingsInPositions.AdvertisingId=tn_Advertisings.AdvertisingId")
                            .Where("tn_AdvertisingsInPositions.PositionId=@0", positionId);
                if (isEnable.HasValue)
                {
                    sql.Where("tn_Advertisings.IsEnable=@0", isEnable.Value);
                }
                var advertisings = CreateDAO().Fetch<Advertising>(sql);
            return advertisings;
        }
    }
}
