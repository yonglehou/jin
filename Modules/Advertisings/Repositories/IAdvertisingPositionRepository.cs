//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    ///广告位数据访问接口
    /// </summary>
    public interface IAdvertisingPositionRepository : IRepository<AdvertisingPosition>
    {
        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <param name="height">高度</param>
        /// <param name="width">宽度</param>
        /// <param name="isEnable">是否启用</param>
        /// <returns></returns>
        IEnumerable<AdvertisingPosition> GetPositionsForAdmin( int? height, int? width, bool? isEnable);

        /// <summary>
        /// 根据广告位Id取所有的广告
        /// </summary>
        /// <param name="positionId">广告位Id</param>
        /// <param name="isEnable">是否启用</param>
        /// <returns></returns>
        IEnumerable<Advertising> GetAdvertisingsByPositionId(long positionId,bool? isEnable);

        /// <summary>
        /// 获取广告位统计数据
        /// </summary>
        /// <returns></returns>
        long GetPositionCount();
    }
}
