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
    /// 评价访问数据库仓储接口
    /// </summary>
    public interface IReviewRepository : IRepository<Review>
    {
        /// <summary>
        /// 获取对象所有的父级评价内容
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="reviewObjectId">对象Id</param>
        /// <param name="sortBy">排序</param>
        /// <param name="isParend">是否仅包含父级(后台则需要全部) </param>
        ///<param name="startTtime">开始时间</param>
        ///<param name="endTtime">结束时间</param>
        /// <param name="keyword">关键字/评论人</param>
        /// <param name="userId">用户Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="auditStatus">审核状态（是否屏蔽）</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <returns></returns>
        PagingDataSet<Review> GetReviews(string tenantTypeId, long? reviewObjectId,  bool isParend, DateTime? startTtime, DateTime? endTtime, string keyword, long? userId, long? ownerId, AuditStatus? auditStatus, SortBy_Review sortBy, int pageIndex, int pageSize);

        /// <summary>
        /// 获取评价的子集(用于前台回复显示)
        /// </summary>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
         Review GetChildrenReview(long parentId);
       

    }
}
