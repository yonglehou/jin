//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Attitude;
using Tunynet.CMS;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 资讯扩展
    /// </summary>
    public static class ContentItemExtension
    {
        static CommentService commentService = DIContainer.Resolve<CommentService>();
        static CountService countService = new CountService(TenantTypeIds.Instance().ContentItem());
        static SpecialContentitemService specialContentitemService = DIContainer.Resolve<SpecialContentitemService>();


        /// <summary>
        /// 是否有权限操作
        /// </summary>
        /// <param name="isAdmin">是否后台操作</param>
        /// <returns></returns>
        public static bool IsAuthorizer(this ContentItem operationType, bool isAdmin = false)
        {
            var currentUser = UserContext.CurrentUser;
            if (currentUser == null)
                return false;
            if (!isAdmin)
            {
                if (currentUser.UserId == operationType.UserId)
                    return true;
            }
            var authorizer = new Authorizer();
            var isCategoryManager = authorizer.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), currentUser, operationType.ContentCategoryId);
            return isCategoryManager;
        }

        /// <summary>
        /// 资讯第一张图片
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static string FristFeaturedImage(this ContentItem operationType, string key = "List", string imgurl = "")
        {
            var newsAttachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Article());
            var imgAttachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Image());
            var videoAttachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Video());

            var attachments = new Attachment();
            var directlyUrl = "";
            if (operationType.ContentModel == null)
                return directlyUrl;
            var modelKey = operationType.ContentModel.ModelKey;
            if (modelKey == ContentModelKeys.Instance().Article() || modelKey == ContentModelKeys.Instance().Contribution())
            {
                attachments = newsAttachmentService.Get(operationType.FeaturedImageAttachmentId);
                if (attachments != null)
                    directlyUrl = attachments.GetDirectlyUrl(key);
            }
            else if (modelKey == ContentModelKeys.Instance().Video())
            {
                attachments = videoAttachmentService.Get(operationType.FeaturedImageAttachmentId);
                if (attachments != null)
                    directlyUrl = attachments.GetDirectlyUrl(key);
            }
            else if (modelKey == ContentModelKeys.Instance().Image())
            {
                var attachmentList = imgAttachmentService.GetsByAssociateId(operationType.ContentItemId);
                if (attachmentList.Count() > 0)
                    directlyUrl = attachmentList.OrderBy(n => n.DisplayOrder).First().GetDirectlyUrl(key);
            }

            if (string.IsNullOrEmpty(directlyUrl))
                return imgurl;
            return directlyUrl;
        }
        /// <summary>
        /// 推荐资讯的第一张图片
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static string specialFristFeaturedImage(this SpecialContentItem operationType, string key = "List", string imgurl = "")
        {
            var newsAttachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Article());

            var attachments = new Attachment();
            var directlyUrl = "";
            if (operationType == null)
                return directlyUrl;
            attachments = newsAttachmentService.Get(operationType.FeaturedImageAttachmentId);
            if (attachments != null)
                directlyUrl = attachments.GetDirectlyUrl(key);

            if (string.IsNullOrEmpty(directlyUrl))
                return imgurl;
            return directlyUrl;
        }
        /// <summary>
        /// 获取资讯的视频
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static Attachment GetCMSVideo(this ContentItem operationType)
        {
            var attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Video());
            var attachents = attachmentService.GetsByAssociateId(operationType.ContentItemId);
            if (attachents.Count() > 0)
            {
                return attachents.Where(t => t.MediaType == MediaType.Video).FirstOrDefault();
            }
            return new Attachment();
        }

        /// <summary>
        /// 资讯-组图列表 的前三张图片
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static IEnumerable<Attachment> FeaturedImageList(this ContentItem operationType)
        {
            var attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Image());
            var attachments = attachmentService.GetsByAssociateId(operationType.ContentItemId);
            if (attachments.Count() >= 3)
                attachments = attachments.OrderBy(n => n.DisplayOrder);
            return attachments;
        }

        /// <summary>
		 /// 是否推荐
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static bool isEssential(this ContentItem operationType)
        {
            var tenantTypeId = TenantTypeIds.Instance().ContentItem();
            if (operationType.ContentModel.ModelKey == ContentModelKeys.Instance().Article())
                tenantTypeId = TenantTypeIds.Instance().CMS_Article();
            else if (operationType.ContentModel.ModelKey == ContentModelKeys.Instance().Image())
                tenantTypeId = TenantTypeIds.Instance().CMS_Image();
            else if (operationType.ContentModel.ModelKey == ContentModelKeys.Instance().Video())
                tenantTypeId = TenantTypeIds.Instance().CMS_Video();
            if (!specialContentitemService.IsRecommend(tenantTypeId, operationType.ContentItemId))
                if (!specialContentitemService.IsSpecial(operationType.ContentItemId, tenantTypeId, SpecialContentTypeIds.Instance().Essential()))
                    return false;
            return true;
        }
        /// <summary>
        /// 资讯是否被点赞过
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static bool? IsSupport(this ContentItem operationType)
        {
            var user = UserContext.CurrentUser;
            if (user == null)
                return false;
            var attitudeService = new AttitudeService(TenantTypeIds.Instance().ContentItem());
            return attitudeService.IsSupport(operationType.ContentItemId, user.UserId);
        }

        /// <summary>
        /// 资讯阅读数
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static int ReadCount(this ContentItem operationType)
        {
            return countService.Get(CountTypes.Instance().HitTimes(), operationType.ContentItemId);
        }

        /// <summary>
        /// 资讯是否为新
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static bool isNew(this ContentItem operationType)
        {
            var published = Convert.ToDateTime(operationType.DatePublished.ToString("yyyy-MM-dd"));
            return Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).Subtract(published).Days < 3;
        }

        /// <summary>
        /// 资讯前台发布时间显示
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static string DateTimeDisplay(this ContentItem operationType)
        {
            var values = string.Empty;
            return operationType.DatePublished.ToFriendlyDate();
        }

        /// <summary>
        /// 资讯是否读过
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static bool isRead(this ContentItem operationType, long userId)
        {
            var isReads = countService.GetOfUser(CountTypes.Instance().HitTimes(), operationType.ContentItemId, userId);
            return isReads > 0;
        }
        /// <summary>
        /// 评论数
        /// </summary>
        /// <returns></returns>
        public static long CommentCount(this ContentItem operationType)
        {
            //return commentService.GetObjectComments(TenantTypeIds.Instance().ContentItem(), operationType.ContentItemId, 1, SortBy_Comment.DateCreated, null).TotalRecords;
            return countService.Get(CountTypes.Instance().CommentCount(), operationType.ContentItemId);
        }

     
        /// <summary>
        /// 资讯 的租户类型
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static string CMSTenantTypeId(this ContentItem operationType)
        {
            var modelKey = operationType.ContentModel.ModelKey;
            if (ContentModelKeys.Instance().Image() == modelKey)
                return TenantTypeIds.Instance().CMS_Image();
            else if (ContentModelKeys.Instance().Article() == modelKey || ContentModelKeys.Instance().Contribution() == modelKey)
                return TenantTypeIds.Instance().CMS_Article();
            else if (ContentModelKeys.Instance().Video() == modelKey)
                return TenantTypeIds.Instance().CMS_Video();
            return TenantTypeIds.Instance().ContentItem();
        }
        /// <summary>
        /// 资讯是否被收藏
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static bool IsFavorited(this ContentItem operationType)
        {

            FavoriteService favoriteService = new FavoriteService(TenantTypeIds.Instance().ContentItem());
            if (UserContext.CurrentUser != null)
            {
                return favoriteService.IsFavorited(operationType.ContentItemId, UserContext.CurrentUser.UserId);
            }

            return false;
        }

        //@item.ContentModel.ModelKey


        ///// <summary>
        ///// 获取资讯流转实例
        ///// </summary>
        ///// <param name="operationType"></param>
        ///// <returns></returns>
        //public static FlowInstance GetFlowInstance(this ContentItem operationType)
        //{
        //    var flowInstanceService = new FlowInstanceService();
        //    var flowInstance = flowInstanceService.GetFlowInstance(TenantTypeIds.Instance().ContentItem(), operationType.ContentItemId);
        //    if (flowInstance != null)
        //        return flowInstance;
        //    else
        //        return new FlowInstance();
        //    //var flowService = new FlowService();
        //    //if (flowInstance != null)
        //    //{
        //    //    title = flowService.GetFlowStep("CMSFlow", flowInstance.CurrentStepCode).StepName;
        //    //}
        //}
        ///// <summary>
        ///// 获取资讯流转实例
        ///// </summary>
        ///// <param name="operationType"></param>
        ///// <returns></returns>
        //public static string TitleName(int categoryId)
        //{
        //    if (CMSCategoryConfig.NWGK == categoryId)
        //        return "内务公开";
        //    if (CMSCategoryConfig.ZTZJ == categoryId)
        //        return "专题征集";
        //    if (CMSCategoryConfig.WTJD == categoryId)
        //        return "问题解答";
        //    if (CMSCategoryConfig.ZDJY == categoryId)
        //        return "主动建议";
        //    if (CMSCategoryConfig.TZGG == categoryId)
        //        return "通知公告";
        //    return "";

        //}
    }
}
