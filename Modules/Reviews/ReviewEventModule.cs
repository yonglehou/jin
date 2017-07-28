//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Tunynet.Events;
using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 创建评价事件处理
    /// </summary>
    public class ReviewEventModule : IEventMoudle
    {
        IReviewSummaryRepository reviewSummaryRepository = new ReviewSummaryRepository();
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Review>.Instance().After += new CommonEventHandler<Review, CommonEventArgs>(ReviewPointModuleForManagerOperation_After);
        }
        /// <summary>
        /// 处理评价汇总
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ReviewPointModuleForManagerOperation_After(Review sender, CommonEventArgs eventArgs)
        {
            
            //获取评价汇总实体
          var reviewSummary = reviewSummaryRepository.GetReviewSummary(sender.TenantTypeId, sender.ReviewedObjectId, sender.OwnerId);
            if (reviewSummary==null)
            {
                reviewSummary = new ReviewSummary();
            }
            if (eventArgs.EventOperationType== EventOperationType.Instance().Create())
            {
                reviewSummary.ReviewedObjectId = sender.ReviewedObjectId;
                reviewSummary.OwnerId = sender.OwnerId;
                reviewSummary.TenantTypeId = sender.TenantTypeId;
                reviewSummary.RateSum += sender.RateNumber;
                reviewSummary.RateCount++;
                switch (sender.ReviewRank)
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
                if (reviewSummary.Id>0)
                {
                    reviewSummaryRepository.Update(reviewSummary);
                }
                else
                {
                    reviewSummaryRepository.Insert(reviewSummary);
                }


            }
            else if (eventArgs.EventOperationType== EventOperationType.Instance().Delete())
            {
                AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Review());
                attachmentService.DeletesByAssociateId(sender.Id);

            }
          
          
        }

        
    }
}