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
using Tunynet.Events;
using Tunynet.Repositories;


namespace Tunynet.Common
{
    /// <summary>
    /// 评价业务逻辑
    /// </summary>
    public class ReviewService
    {
        private IReviewRepository reviewRepository ;
        private IReviewSummaryRepository reviewSummaryRepository;

        public ReviewService(IReviewRepository reviewRepository, IReviewSummaryRepository reviewSummaryRepository)
        {
            this.reviewRepository = reviewRepository;
            this.reviewSummaryRepository = reviewSummaryRepository;
        }
        public Review Get(long id)
        {
            return reviewRepository.Get(id);
        }
        /// <summary>
        /// 创建评价
        /// </summary>
        public void Create(Review review)
        {
            reviewRepository.Insert(review);
            bool reviewSummaryNotNull = true;
            //维护评价汇总实体
            var reviewSummary = reviewSummaryRepository.GetReviewSummary(review.TenantTypeId, review.ReviewedObjectId, review.OwnerId);

            if (reviewSummary == null)
            {
                reviewSummaryNotNull = false;
                reviewSummary = new ReviewSummary() { ReviewedObjectId = review.ReviewedObjectId, OwnerId = review.OwnerId, TenantTypeId = review.TenantTypeId };
            }
          
            reviewSummary.RateSum += review.RateNumber;
            reviewSummary.RateCount++;
            switch (review.ReviewRank)
            {
                case Review_Type.Positive:
                    reviewSummary.PositiveReivewCount++;
                    break;
                case Review_Type.Moderate:
                    reviewSummary.ModerateReivewCount++;
                    break;
                case Review_Type.Negative:
                    reviewSummary.NegativeReivewCount++;
                    break;
            }
            if (reviewSummaryNotNull)
                reviewSummaryRepository.Update(reviewSummary);
            else
                reviewSummaryRepository.Insert(reviewSummary);
        }


        public int GetTotalRate(long ownerId)
        {
            return reviewSummaryRepository.GetTotalRate(ownerId);
        }

        public int GetTotalCount(long ownerId)
        {
            return reviewSummaryRepository.GetTotalCount(ownerId);
        }

        /// <summary>
        /// 删除评价
        /// </summary>
        public bool Delete(long id)
        {
            var review = reviewRepository.Get(id);
            bool isDeleted = reviewRepository.Delete(review) > 0;
            if (isDeleted)
            {
                //维护评价汇总实体
                var reviewSummary = reviewSummaryRepository.GetReviewSummary(review.TenantTypeId, review.ReviewedObjectId, review.OwnerId);
                if (reviewSummary != null)
                {
                    reviewSummary.RateSum -= review.RateNumber;
                    reviewSummary.RateCount--;
                    switch (review.ReviewRank)
                    {
                        case Review_Type.Positive:
                            reviewSummary.PositiveReivewCount--;
                            break;
                        case Review_Type.Moderate:
                            reviewSummary.ModerateReivewCount--;
                            break;
                        case Review_Type.Negative:
                            reviewSummary.NegativeReivewCount--;
                            break;
                    }
                    if (reviewSummary.RateCount > 0)
                        reviewSummaryRepository.Update(reviewSummary);
                    else
                        reviewSummaryRepository.DeleteByEntityId(reviewSummary.Id);
                  
                }
                //删除附件
                AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Review());
                attachmentService.DeletesByAssociateId(review.Id);
            }

            return isDeleted;
        }

        /// <summary>
        /// 获取对象所有的父级评价内容（前台）
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="reviewObjectId">对象Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="auditStatus">审核状态（是否屏蔽）</param>
        /// <param name="sortBy">排序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <returns></returns>
        public PagingDataSet<Review> GetReviews(string tenantTypeId, long? reviewObjectId, long? userId, long? ownerId, AuditStatus? auditStatus, int pageIndex, int pageSize)
        {
            return reviewRepository.GetReviews(tenantTypeId, reviewObjectId, true, null, null, null, userId, ownerId, auditStatus, SortBy_Review.DateCreatedDesc, pageIndex, pageSize);
        }
        /// <summary>
        /// 获取对象所有的评价（后台）
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        ///<param name="startTtime">开始时间</param>
        ///<param name="endTtime">结束时间</param>
        /// <param name="keyword">关键字/评论人</param>
        /// <param name="sortBy">排序</param>
        /// <param name="auditStatus">审核状态（是否屏蔽）</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <returns></returns>
        public PagingDataSet<Review> GetReviews(string tenantTypeId, SortBy_Review sortBy, DateTime? startTtime, DateTime? endTtime, string keyword, AuditStatus? auditStatus, int pageIndex, int pageSize)
        {
            return reviewRepository.GetReviews(tenantTypeId, null, false, startTtime, endTtime, keyword, null, null, auditStatus, sortBy, pageIndex, pageSize);
        }
        /// <summary>
        /// 获取对象评价汇总
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="reviewObjectId">对象Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns></returns>
        public ReviewSummary GetReviewSummary(string tenantTypeId, long? reviewObjectId, long? ownerId)
        {
            return reviewSummaryRepository.GetReviewSummary(tenantTypeId, reviewObjectId, ownerId);
        }

        /// <summary>
        /// 获取评价的子集(用于前台回复显示)
        /// </summary>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        public Review GetChildrenReview(long parentId)
        {
            return reviewRepository.GetChildrenReview(parentId);
        }
    }
}
