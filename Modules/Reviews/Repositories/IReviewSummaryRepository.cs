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

namespace Tunynet.Common
{
    /// <summary>
    /// 评价汇总数据库仓储接口
    /// </summary>
    public interface IReviewSummaryRepository : IRepository<ReviewSummary>
    {
        /// <summary>
        /// 获取对象评价汇总
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="reviewObjectId">对象Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns></returns>
        ReviewSummary GetReviewSummary(string tenantTypeId, long? reviewObjectId, long? ownerId);

        /// <summary>
        /// 获取拥有者的总评分
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        int GetTotalRate(long ownerId);
        /// <summary>
        /// 获取拥有者的总评价人数
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        int GetTotalCount(long ownerId);

    }
}
