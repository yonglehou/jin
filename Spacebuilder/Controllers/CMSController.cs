//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tunynet.CMS;
using Tunynet.Common;
using Tunynet.Settings;
using Tunynet.UI;
using System.Configuration;
using Tunynet.Post;
using System.Net;
using Tunynet.Caching;
using Tunynet.Logging;
using Tunynet.Attitude;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// CMS 控制器
    /// </summary>
    [UserAuthorize(isAnonymous = true)]
    public partial class CMSController : Controller
    {
        #region Service&User
        IUser user = UserContext.CurrentUser;
        //分类
        private CategoryService categoryService;
        //资讯
        private ContentItemService contentItemService;
        //栏目
        private ContentCategoryService contentCategorieService;
        //浏览计数
        private CountService countService = new CountService(TenantTypeIds.Instance().ContentItem());
        //附件
        private AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().ContentItem());
        //标签
        private TagService tagService = new TagService(TenantTypeIds.Instance().ContentItem());
        //用户资料
        private UserProfileService userProfileService;
        //评论
        private CommentService commentService;
        //点赞
        private AttitudeService attitudeService = new AttitudeService(TenantTypeIds.Instance().ContentItem());
        private ThreadService threadService;
        private UserService userService;
        private FollowService followService;
        private PointService pointService;
        //收藏
        private FavoriteService favoriteService = new FavoriteService(TenantTypeIds.Instance().ContentItem());
        //推荐
        private SpecialContentitemService specialContentitemService;
        private IKvStore kvStore;
        #endregion

        public CMSController(UserService userService,
                                FollowService followService,
                                CategoryService categoryService,
                                ContentItemService contentItemService,
                                ContentCategoryService contentCategorieService,
                                UserProfileService userProfileService,
                                ThreadService threadService,
                                CommentService commentService,
                                PointService pointService,
            SpecialContentitemService specialContentitemService,
            IKvStore kvStore
                                )
        {
            this.userService = userService;
            this.followService = followService;
            this.categoryService = categoryService;
            this.contentItemService = contentItemService;
            this.contentCategorieService = contentCategorieService;
            this.userProfileService = userProfileService;
            this.threadService = threadService;
            this.commentService = commentService;
            this.pointService = pointService;
            this.specialContentitemService = specialContentitemService;
            this.kvStore = kvStore;
        }

        #region 资讯前台显示

        /// <summary>
        /// 热点图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _HotarticleImg()
        {
            //一周热文
            var hotcmsImgList = new List<ContentItem>();
            var contentItems = contentItemService.GetTopContentItemsofModelKey(6, ContentModelKeys.Instance().Image(), DateTime.Now.AddDays(-7), ContentItemSortBy.HitTimes);
            hotcmsImgList.AddRange(contentItems);
            if (hotcmsImgList.Count < 6)
            {
                var hotcmsImgListMonth = contentItemService.GetTopContentItemsofModelKey(6, ContentModelKeys.Instance().Image(), DateTime.Now.AddMonths(-1), ContentItemSortBy.HitTimes);
                if (hotcmsImgListMonth.Count() > 0)
                    hotcmsImgList = hotcmsImgList.Union(hotcmsImgListMonth).ToList();
            }
            return PartialView(hotcmsImgList.Count > 0 ? hotcmsImgList.Take(6) : hotcmsImgList);
        }

        /// <summary>
        /// 一周热文
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _Hotarticle()
        {
            var contentItems = contentItemService.GetTopContentItems(6, null, true, DateTime.Now.AddDays(-7), ContentItemSortBy.HitTimes);
            if (!contentItems.Any())
                contentItems = contentItemService.GetTopContentItems(6, null, true, DateTime.Now.AddMonths(-1), ContentItemSortBy.HitTimes);
            return PartialView(contentItems);
        }


        /// <summary>
        /// 资讯详情
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CMSDetail(long contentItemId)
        {
            //获取资讯
            var contentItem = contentItemService.Get(contentItemId);
            if (contentItem == null)
                return Redirect(SiteUrls.Instance().Error());
            #region 资讯详情 跳转
            if (contentItem.ContentModel != null)
                if ("CMSDetail" != contentItem.ContentModel.PageDetail)
                    return RedirectToAction(contentItem.ContentModel.PageDetail, new { contentItemId = contentItemId });
            #endregion

            //所属栏目
            var category = contentCategorieService.Get(contentItem.ContentCategoryId);
            ViewData["category"] = category;
            //内容底部附件列表
            attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Article());
            var attachments = attachmentService.GetsByAssociateId(contentItem.ContentItemId);
            ViewData["attachments"] = attachments;
            //标签
            var tags = tagService.attiGetItemInTagsOfItem(contentItemId);
            ViewData["tags"] = tags;

            //点赞
            var attitude = attitudeService.Get(contentItemId);
            ViewData["attitude"] = attitude;
            //更新计数
            countService.ChangeCount(CountTypes.Instance().HitTimes(), contentItemId, 0, 1, true);
            //评论数
            ViewData["commentCount"] = commentService.GetObjectComments(TenantTypeIds.Instance().ContentItem(), contentItemId, 1, SortBy_Comment.DateCreated, null).TotalRecords;
            return View(contentItem);
        }

        /// <summary>
        /// 前台资讯首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ContentItemHome()
        {
            var specialContentiItem = specialContentitemService.GetTops(1, SpecialContentTypeIds.Instance().SpecialCMS(), TenantTypeIds.Instance().CMS_Article(), true);
            var contentItem = specialContentiItem.Any() ? contentItemService.Get(specialContentiItem.First().ItemId) : new ContentItem();
            ViewData["specialContentiItem"] = specialContentiItem.Any() ? specialContentiItem.FirstOrDefault() : new SpecialContentItem();
            return View(contentItem);
        }

        /// <summary>
        /// 前台资讯首页列表
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListContentItem(int pageSize = 10, int pageIndex = 1)
        {
            var contentItems = contentItemService.GetContentItems(null, true, null, pageSize, pageIndex, true, ContentItemSortBy.DatePublished_Desc);
            return PartialView(contentItems);
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Support(long contentItemId)
        {
            var isSupport = attitudeService.IsSupport(contentItemId, user.UserId);
            var isAttitude = attitudeService.Support(contentItemId, user.UserId);
            if (!isAttitude)
            {
                return Json(new { type = 0, msg = "操作失败" });
            }
            if (isSupport == true)
                return Json(new { type = 1, msg = "取消点赞成功" });
            else
                return Json(new { type = 1, msg = "点赞成功" });
        }

        /// <summary>
        /// 作者最近的文章
        /// </summary>
        /// <param name="userId">作者ID</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _RecentCMS(long userId)
        {
            var contentItems = contentItemService.GetContentItems(null, true, userId, 6, 1, true, ContentItemSortBy.DatePublished_Desc);
            return PartialView(contentItems);
        }

        /// <summary>
        /// 标签内资讯列表
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TagCMS(string tagName)
        {
            var tag = tagService.Get(tagName);
            if (tag == null)
                return Redirect(SiteUrls.Instance().Error());
            //获取热门标签
            ViewData["hotTags"] = tagService.GetTopTags(10, null, SortBy_Tag.ItemCountDesc);
            return View(tag);
        }

        /// <summary>
        /// 标签内资讯列表分页
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListTagCMS(string tagName, int pageSize = 10, int pageIndex = 1)
        {
            var tagContentItems = tagService.GetItemIds(tagName, pageSize, pageIndex);
            var itemIds = tagContentItems.Select(n => n.ItemId);
            PagingDataSet<ContentItem> contentItems = new PagingDataSet<ContentItem>(contentItemService.Gets(itemIds))
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecords = tagContentItems.TotalRecords
            };

            return PartialView(contentItems);
        }

        /// <summary>
        /// 栏目内资讯列表
        /// </summary>
        /// <param name="contentCategoryId">栏目Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CategoryCMS(int contentCategoryId)
        {
            var category = contentCategorieService.Get(contentCategoryId);
            if (category == null)
            {
                return Redirect(SiteUrls.Instance().Error());
            }
            //区分栏目级别
            ContentCategory parentCategory;
            if (category.Depth == 0)
                parentCategory = category;
            else
                parentCategory = contentCategorieService.Get(category.ParentId);

            ViewData["contentCategoryId"] = contentCategoryId;
            //栏目
            ViewData["parentCategory"] = parentCategory;

            return View();
        }

        /// <summary>
        /// 栏目内资讯列表分页
        /// </summary>
        /// <param name="categoryId">栏目ID</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListCategoryCMS(int categoryId, int pageSize = 10, int pageIndex = 1)
        {
            var contentList = contentItemService.GetContentItems(categoryId, true, null, pageSize, pageIndex, true);
            ViewData["categoryId"] = categoryId;

            return PartialView(contentList);
        }

        /// <summary>
        /// 获取子栏目信息
        /// </summary>
        /// <param name="contentCategoryId">当前栏目ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetChildCategories(int contentCategoryId)
        {
            var category = contentCategorieService.Get(contentCategoryId);
            if (category == null)
            {
                return Redirect(SiteUrls.Instance().Error());
            }
            return Json(category.Children.Select(t => new
            {
                id = t.CategoryId,
                name = t.CategoryName
            }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 视频新闻
        /// <summary>
        /// 视频资讯详情
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CMSVideoDetail(long contentItemId)
        {
            ContentItem contentItem = contentItemService.Get(contentItemId);
            if (contentItem == null)
            {
                return Redirect(SiteUrls.Instance().Error());
            }

            attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Video());

            #region 资讯详情 跳转
            if (contentItem.ContentModel != null)
                if ("CMSVideoDetail" != contentItem.ContentModel.PageDetail)
                    return RedirectToAction(contentItem.ContentModel.PageDetail, new { contentItemId = contentItemId });
            #endregion

            //如果是上传附件则获取附件
            if (!contentItem.AdditionalProperties.ContainsKey("VideoUrl") || string.IsNullOrEmpty(contentItem.AdditionalProperties["VideoUrl"].ToString()))
            {
                if (attachmentService.GetsByAssociateId(contentItemId).Where(n => n.MediaType == MediaType.Video).Any())
                    ViewData["attachment"] = attachmentService.GetsByAssociateId(contentItemId).Where(n => n.MediaType == MediaType.Video).First();
            }

            
            //更新计数
            countService.ChangeCount(CountTypes.Instance().HitTimes(), contentItemId, contentItem.UserId, 1, true);

            return View(contentItem);
        }

        /// <summary>
        /// 视频资讯列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CMSVideo()
        {
            //获取推荐的视频
            var specialVideoCMS = specialContentitemService.GetTops(1, SpecialContentTypeIds.Instance().CMS_Video(), TenantTypeIds.Instance().CMS_Video(), true);
            //如果没有推荐则显示最新的视频新闻
            ViewData["specialVideoCMS"] = specialVideoCMS.Any() ? contentItemService.Get(specialVideoCMS.First().ItemId) : contentItemService.GetTopContentItemsofModelKey(1, ContentModelKeys.Instance().Video(), null, ContentItemSortBy.DatePublished_Desc).FirstOrDefault();

            ViewData["specialContentiItem"] = specialVideoCMS.Any() ? specialVideoCMS.FirstOrDefault() : new SpecialContentItem();
            //热门视频
            ViewData["hotVideoCMS"] = contentItemService.GetTopContentItemsofModelKey(4, ContentModelKeys.Instance().Video(), null, ContentItemSortBy.HitTimes);

            return View();
        }

        /// <summary>
        /// 视频资讯列表分页
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListCMSVideo(int pageIndex = 1)
        {
            var videoCMSList = contentItemService.GetContentItemsofModelKey(ContentModelKeys.Instance().Video(),8, pageIndex, ContentItemSortBy.DatePublished_Desc);
            return PartialView(videoCMSList);
        }

        /// <summary>
        /// 热点视频
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _HotarticleVideo()
        {
            return PartialView(contentItemService.GetTopContentItemsofModelKey(6, ContentModelKeys.Instance().Video(), null, ContentItemSortBy.HitTimes));
        }

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <param name="isFavorite">是否收藏</param>
        /// <returns></returns>
        public JsonResult Favorite(long contentItemId, bool isFavorite)
        {
            if (user == null)
                return Json(new { state = "用户未登录", msg = "操作失败", isFavorite });
            if (isFavorite)
            {
                if (favoriteService.Favorite(contentItemId, user.UserId))
                    return Json(new { state = "已收藏", msg = "收藏成功", isFavorite });
            }
            else
            {
                if (favoriteService.CancelFavorite(contentItemId, user.UserId))
                    return Json(new { state = "收藏", msg = "取消收藏成功", isFavorite });
            }
            return Json(new { state = "操作失败", msg = "操作失败", isFavorite });
        }

        #endregion

        #region 组图新闻
        /// <summary>
        /// 组图详情
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <returns></returns>
        public ActionResult CMSImgDetail(int contentItemId)
        {
            ContentItem contentItem = contentItemService.Get(contentItemId);
            if (contentItem == null)
            {
                return Redirect(SiteUrls.Instance().Error());
            }

            #region 资讯详情 跳转
            if (contentItem.ContentModel != null)
                if ("CMSImgDetail" != contentItem.ContentModel.PageDetail)
                    return RedirectToAction(contentItem.ContentModel.PageDetail, new { contentItemId = contentItemId });
            #endregion

            var attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Image());
            var attachments = attachmentService.GetsByAssociateId(contentItemId).OrderBy(n => n.DisplayOrder).ToList();

            ViewData["attachmentList"] = attachments;
            //更新计数
            countService.ChangeCount(CountTypes.Instance().HitTimes(), contentItemId, contentItem.UserId, 1, true);
            return View(contentItem);
        }

        /// <summary>
        /// 组图列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CMSImg()
        {
            //获取推荐的组图
            ViewData["cmsImgList"] = specialContentitemService.GetTops(10,SpecialContentTypeIds.Instance().CMS_Image(), TenantTypeIds.Instance().CMS_Image(), true);
            return View();
        }

        /// <summary>
        /// 组图列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PartialViewResult _ListCMSImg(int pageIndex = 1)
        {
            var cmsImgList = contentItemService.GetContentItemsofModelKey(ContentModelKeys.Instance().Image(), 9, pageIndex, ContentItemSortBy.DatePublished_Desc);
            return PartialView(cmsImgList);
        }
        #endregion

    }
}
