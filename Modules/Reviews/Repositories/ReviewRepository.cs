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
    /// 评价访问数据库
    /// </summary>
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {

        /// <summary>
        /// 获取对象所有的父级评价内容
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="reviewObjectId">对象Id</param>
        ///<param name="startTtime">开始时间</param>
        ///<param name="endTtime">结束时间</param>
        /// <param name="keyword">关键字/评论人</param>
        /// <param name="sortBy">排序</param>
        /// <param name="isParend">是否仅包含父级(后台则需要全部) </param>
        /// <param name="userId">用户Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="auditStatus">审核状态（是否屏蔽）</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <returns></returns>
        public PagingDataSet<Review> GetReviews(string tenantTypeId, long? reviewObjectId,bool isParend, DateTime? startTtime, DateTime? endTtime,  string keyword, long? userId, long? ownerId, AuditStatus? auditStatus, SortBy_Review sortBy, int pageIndex, int pageSize)
        {
            var sql = Sql.Builder;
            if (!string.IsNullOrEmpty(tenantTypeId))
            {
                sql.Where("TenantTypeId = @0  ", tenantTypeId);
            }
           
            if (reviewObjectId.HasValue)
            {
                sql.Where("ReviewedObjectId = @0",reviewObjectId);

            }
            if (isParend)
            {
                sql.Where("ParentId = 0");
            }
            if (userId.HasValue)
            {
                sql.Where("userId = @0", userId.Value);
            }
            if (ownerId.HasValue)
            {
                sql.Where("ownerId = @0", ownerId.Value);
            }
            if (auditStatus.HasValue)
            {
                sql.Where("ApprovalStatus = @0", auditStatus.Value);
            }
            if (startTtime.HasValue)
            {
                sql.Where("DateCreated >= @0", startTtime.Value);
            }
            if (endTtime.HasValue)
            {
                sql.Where("DateCreated < @0", endTtime.Value.AddDays(1));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql.Where("Body like @0 or Author like @0", "%" + keyword + "%");
            }
            switch (sortBy)
            {
                case SortBy_Review.DateCreated:
                    sql.OrderBy("Id");
                    break;
                case SortBy_Review.DateCreatedDesc:
                    sql.OrderBy("Id desc");
                    break;
            }
           return  GetPagingEntities(pageSize, pageIndex, sql);
        }

        /// <summary>
        /// 获取评价的子集(用于前台回复显示)
        /// </summary>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        public Review GetChildrenReview(long parentId)
        {
            var sql = Sql.Builder;
            sql.Where("ParentId =@0 and ApprovalStatus=@1 ", parentId, AuditStatus.Success);
            return CreateDAO().Fetch<Review>(sql).FirstOrDefault();
        }

    }
}
