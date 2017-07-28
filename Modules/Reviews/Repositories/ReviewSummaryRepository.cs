//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Repositories;


namespace Tunynet.Common
{
    /// <summary>
    /// 评价汇总数据访问
    /// </summary>
    public class ReviewSummaryRepository : Repository<ReviewSummary>, IReviewSummaryRepository
    {

        /// <summary>
        /// 获取对象评价汇总
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="reviewObjectId">对象Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns></returns>
        public ReviewSummary GetReviewSummary(string tenantTypeId, long? reviewObjectId, long? ownerId)
        {
            var sql = Sql.Builder;
            sql.Where("TenantTypeId = @0 ", tenantTypeId);
            if (reviewObjectId.HasValue)
            {
                sql.Where("ReviewedObjectId = @0", reviewObjectId.Value);
            }
            if (ownerId.HasValue)
            {
                sql.Where("ownerId = @0", ownerId.Value);
            }
            return CreateDAO().SingleOrDefault<ReviewSummary>(sql);
        }

        /// <summary>
        /// 获取拥有者的总评分
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public int GetTotalRate(long ownerId)
        {
            return CreateDAO().ExecuteScalar<int>(Sql.Builder.Append("select IsNull(sum(RateSum),0) from tn_ReviewSummaries where OwnerId = @0", ownerId));
        }

        /// <summary>
        /// 获取拥有者的总评价人数
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public int GetTotalCount(long ownerId)
        {
            return CreateDAO().ExecuteScalar<int>(Sql.Builder.Append("select IsNull(sum(RateCount),0) from tn_ReviewSummaries where OwnerId = @0", ownerId));
        }
    }
}
