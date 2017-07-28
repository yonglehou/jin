//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Web.Mvc;
using Tunynet.UI;
using Tunynet.Common;
using Tunynet.Post;
using System.Configuration;
using Tunynet.CMS;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Tunynet.Settings;
using System.Collections.Generic;
using Tunynet.Utilities;
using Tunynet.Caching;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 前台
    /// </summary>

    public class PortalController : Controller
    {
  
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
        //评论
        private CommentService commentService;
        //点赞
        private ThreadService threadService;
        private UserService userService;
        private FollowService followService;
        private UserRankService userRankService;
        private PointService pointService;
        private NoticeService noticeService;
        private NavigationService navigationService;
        private SpecialContentitemService specialContentitemService;
        private SectionService sectionService;
        //站点设置
        private ISettingsManager<SiteSettings> siteSettings;
        //链接
        private LinkService linkService;
        private Authorizer authorizer;
        private INoticeSender noticeSender;
        private UserProfileService userProfileService;

        public PortalController(UserService userService,
                                FollowService followService,
                                CategoryService categoryService,
                                ContentItemService contentItemService,
                                ContentCategoryService contentCategorieService,
                                ThreadService threadService,
                                CommentService commentService,
                                UserRankService userRankService,
                                PointService pointService,
                                NoticeService noticeService,
                                NavigationService navigationService,
                                ISettingsManager<SiteSettings> siteSettings,
                                LinkService linkService,
                                Authorizer authorizer,
                                SpecialContentitemService specialContentitemService,
                                SectionService sectionService,
                                INoticeSender noticeSender,
                                UserProfileService userProfileService
                                )
        {
            this.userService = userService;
            this.followService = followService;
            this.categoryService = categoryService;
            this.contentItemService = contentItemService;
            this.contentCategorieService = contentCategorieService;
            this.threadService = threadService;
            this.commentService = commentService;
            this.userRankService = userRankService;
            this.pointService = pointService;
            this.noticeService = noticeService;
            this.navigationService = navigationService;
            this.siteSettings = siteSettings;
            this.linkService = linkService;
            this.authorizer = authorizer;
            this.noticeSender = noticeSender;
            this.specialContentitemService = specialContentitemService;
            this.sectionService = sectionService;
            this.userProfileService = userProfileService;
        }
        #region 头部分布页

        /// <summary>
        /// 头部分布页
        /// </summary>
        /// <returns></returns>
        public ActionResult _Header()
        {
            var navigationList = navigationService.GetAll();
            if (navigationList == null)
                return new EmptyResult();
            var navigations = navigationList.Where(n => n.ParentNavigationId == 0).Where(n => n.IsEnabled);
            if (navigations == null)
                return new EmptyResult();
            var currentExecutionFilePath = Request.CurrentExecutionFilePath;
            if (!string.IsNullOrEmpty(currentExecutionFilePath))
            {
                if (navigationList.Count() > 0)
                {
                    navigationList = navigationList.Where(n => n.GetUrl().Trim().ToLower() == currentExecutionFilePath.Trim().ToLower());
                    if (navigationList.Count() > 0)
                    {
                        ViewData["activeNavigation"] = navigationService.GetCurrentNavigationPathIds(navigationList.First().NavigationId); ;
                    }
                }
            }

            //显示通知数量
            if (user != null)
            {
                ViewData["noticeCount"] = noticeService.Gets(user.UserId, NoticeStatus.Unhandled).TotalRecords;
            }
            ViewData["AuthorizeCore"] = AuthorizeCore();

            #region 站点风格
            var siteStyle = SiteStyleType.Default;
            var userBackground = SiteStyleType.Default;

            var siteSetting = siteSettings.Get();
            if (siteSetting != null)
                siteStyle = siteSetting.SiteStyle;

            if (user != null)
            {
                UserProfilePortal userProfilePortal = new UserProfilePortal();
                //用户资料
                var userProfile = userProfileService.Get(user.UserId);
                userProfile.MapTo(userProfilePortal);

                ViewData["isUseCustomStyle"] = userProfilePortal.IsUseCustomStyle;
                ViewData["siteStyle"] = userProfilePortal.IsUseCustomStyle ? userProfilePortal.ThemeAppearance : siteStyle.ToString();
            }

            #endregion

            return PartialView(navigations);
        }

        private bool AuthorizeCore()
        {
            IUser user = UserContext.CurrentUser;
            if (user == null)
                return false;

            if (user.IsSuperAdministrator())
                return true;
            var categoryManagerService = DIContainer.Resolve<CategoryManagerService>();

            if (categoryManagerService.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), user.UserId))
                return true;
            var authorizationService = DIContainer.Resolve<IAuthorizationService>();
            if (authorizationService.Check(user, PermissionItemKeys.Instance().CMS()))
                return true;
            if (authorizationService.Check(user, PermissionItemKeys.Instance().Post()))
                return true;
            if (authorizationService.Check(user, PermissionItemKeys.Instance().User()))
                return true;
            if (authorizationService.Check(user, PermissionItemKeys.Instance().SiteManage()))
                return true;
            if (authorizationService.Check(user, PermissionItemKeys.Instance().GlobalContent()))
                return true;
            return false;

        }
        #endregion

        #region 尾部分布页
        /// <summary>
        /// 尾部分布页
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _Footer()
        {
            ViewData["siteSettings"] = siteSettings.Get();

            return PartialView();
        }
        #endregion

        /// <summary>
        /// 站点主页
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(isAnonymous = true)]
        public ActionResult Home()
        {
            //友情链接
            var imgLinks = new List<LinkEntity>();
            var wordLinks = new List<LinkEntity>();
            var links = linkService.GetsOfSite().Where(n => n.IsEnabled == true).OrderBy(n => n.DisplayOrder);
            if (links.Any())
            {
                imgLinks.AddRange(links.Where(n => n.ImageAttachmentId > 0));
                wordLinks.AddRange(links.Where(n => n.ImageAttachmentId == 0));
            }
            ViewData["imgLinks"] = imgLinks;
            ViewData["wordLinks"] = wordLinks;
            //推荐贴子
            var specialContentItemIds = specialContentitemService.GetItemIds(TenantTypeIds.Instance().Thread(), 0).Distinct();
            var specialthreads = threadService.Gets(specialContentItemIds).Take(10);
            ViewData["listSpecialThread"] = specialthreads;
            //推荐贴吧
            var specialBarIds = specialContentitemService.GetItemIds(TenantTypeIds.Instance().Bar(), SpecialContentTypeIds.Instance().Special());
            ViewData["specialSections"] = sectionService.GetBarsections(specialBarIds);

            ViewData["siteSettings"] = siteSettings.Get();
            ViewData["contentItems"] = contentItemService.GetTopContentItems(6, null, true, null, ContentItemSortBy.DatePublished_Desc);
            return View();
        }

        #region 评论
        /// <summary>
        /// 评论列表
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns></returns>
        [UserAuthorize(isAnonymous = true)]
        public ActionResult CommentList(long commentedObjectId = 0, string tenantTypeId = "")
        {
            if (tenantTypeId == TenantTypeIds.Instance().ContentItem())
            {
                var contentItem = contentItemService.Get(commentedObjectId);
                ViewData["subject"] = contentItem == null ? string.Empty : contentItem.Subject;
                ViewData["IsComment"] = contentItem == null ? true : contentItem.IsComment;
                ViewData["totalRecords"] = contentItem.CommentCount();
            }
            else if (tenantTypeId == TenantTypeIds.Instance().Thread())
            {
                var contentItem = threadService.Get(commentedObjectId);
                ViewData["subject"] = contentItem == null ? string.Empty : contentItem.Subject;
                ViewData["totalRecords"] = contentItem.CommentCount;
            }

            ViewData["tenantTypeId"] = tenantTypeId;
            ViewData["commentedObjectId"] = commentedObjectId;
         
            return View();
        }

        /// <summary>
        /// 评论列表分布页
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns></returns>
        public ActionResult _ListComment(long commentedObjectId, string tenantTypeId, SortBy_Comment sortBy_Comment = SortBy_Comment.DateCreated, int pageIndex = 1)
        {
            PagingDataSet<Comment> comments = null;
            if (commentedObjectId > 0)
            {
                comments = commentService.GetRootComments(tenantTypeId, commentedObjectId, pageIndex, sortBy_Comment);
                if (comments.Count == 0 && pageIndex > 1)
                {
                    comments = commentService.GetRootComments(tenantTypeId, commentedObjectId, pageIndex > 1 ? pageIndex - 1 : 1, sortBy_Comment);
                }
            }
            var contentItem = contentItemService.Get(commentedObjectId);
            ViewData["commentService"] = commentService;
            ViewData["tenantTypeId"] = tenantTypeId;
            ViewData["commentedObjectId"] = commentedObjectId;
            ViewData["contentItem"] = contentItem;
            
            ViewData["count"] = countService.Get(CountTypes.Instance().CommentCount(), commentedObjectId);
            return PartialView(comments);
        }
        /// <summary>
        /// 评论列表分布页 盖楼效果
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns></returns>
        public ActionResult _ChildComment(long parentId, string tenantTypeId, long commentedObjectId, SortBy_Comment sortBy_Comment = SortBy_Comment.DateCreatedDesc, int pageIndex = 1)
        {
            PagingDataSet<Comment> comments = null;
            if (parentId > 0)
                comments = commentService.GetChildren(parentId, pageIndex, 20, sortBy_Comment);
            ViewData["tenantTypeId"] = tenantTypeId;
            ViewData["parentId"] = parentId;
            ViewData["commentedObjectId"] = commentedObjectId;
            return PartialView(comments);
        }
        /// <summary>
        /// 评论控件
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns></returns>
        public ActionResult _Comment(long parentId, string tenantTypeId, long commentedObjectId, SortBy_Comment sortBy_Comment = SortBy_Comment.DateCreatedDesc, int pageIndex = 1)
        {
            CommentEditModel commentEditModel = new CommentEditModel();
            ViewData["tenantTypeId"] = tenantTypeId;
            ViewData["parentId"] = parentId;
            ViewData["commentedObjectId"] = commentedObjectId;
            return PartialView(commentEditModel);
        }
        /// <summary>
        /// 创建评论
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(isAnonymous = true)]
        [HttpPost]
        public ActionResult CreateComment(CommentEditModel commentEditModel)
        {
            #region 资讯是否支持评论
            if (commentEditModel.TenantTypeId == TenantTypeIds.Instance().ContentItem())
            {
                var contentItem = contentItemService.Get(commentEditModel.CommentedObjectId);
                if (contentItem == null)
                    return Json(new StatusMessageData(StatusMessageType.Hint, "评论失败"));

                if (contentItem.IsComment)
                    return Json(new StatusMessageData(StatusMessageType.Hint, "此文章禁止评论"));
            }

            #endregion
            if (string.IsNullOrEmpty(commentEditModel.Body))
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "评论失败"));
            }

            var parentComment = commentService.Get(commentEditModel.ParentId);
            if (parentComment != null)
            {
                commentEditModel.ParentIds = string.Format("{0}{1},", parentComment.ParentIds, commentEditModel.ParentId);
                commentEditModel.TenantTypeId = parentComment.TenantTypeId;
            }
            else
                commentEditModel.ParentIds = "0,";

            commentEditModel.UserId = user.UserId;
            commentEditModel.DateCreated = DateTime.Now;
            commentEditModel.Author = user.DisplayName;
            commentEditModel.IP = Utilities.WebUtility.GetIP();
            commentEditModel.IsPrivate = commentEditModel.IsPrivate;
            Comment comment = Comment.New();
            commentEditModel.MapTo<CommentEditModel, Comment>(comment);

            //判断 内容的所属
            long ownerId = 0;
            if (commentEditModel.TenantTypeId == TenantTypeIds.Instance().ContentItem())
            {
                var contentItem = contentItemService.Get(commentEditModel.CommentedObjectId);
                if (contentItem != null)
                    ownerId = contentItem.UserId;
            }
            else if (commentEditModel.TenantTypeId == TenantTypeIds.Instance().Thread())
            {
                var thread = threadService.Get(commentEditModel.CommentedObjectId);
                if (thread != null)
                    ownerId = thread.UserId;
            }

            comment.OwnerId = parentComment == null ? ownerId : parentComment.UserId;

            commentService.Create(comment);
           
            ////更新最后 回复时间
            //if (commentEditModel.TenantTypeId == TenantTypeIds.Instance().Thread())
            //{

            //    var contentItem = contentItemService.Get(commentEditModel.CommentedObjectId);
            //    if (contentItem != null)
            //    {
            //        contentItem.LastModified = DateTime.Now;
            //        contentItemService.Update(contentItem);
            //    }
            //}

            var statusMessage = new StatusMessageData(StatusMessageType.Success, "评论成功");
            return Json(statusMessage);
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId">评论Id</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult _DeleteComment(long commentId)
        {
            var comment = commentService.Get(commentId);
            if (comment != null)
            {
                if (authorizer.Comment_Delete(comment, UserContext.CurrentUser))
                    if (commentService.Delete(commentId))
                        return Json(new StatusMessageData(StatusMessageType.Success, "删除成功"));
            }

            return Json(new StatusMessageData(StatusMessageType.Error, "删除错误"));
        }
        #endregion

        #region 全文检索 by fanggm

        /// <summary>
        /// 全文检索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UserAuthorize(isAnonymous = true)]
        public ActionResult Search(string keyword = "", string searchType = "All")
        {
            ViewData["keyword"] = keyword;
            ViewData["searchType"] = searchType;

            return View();
        }

        /// <summary>
        /// 快捷搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public PartialViewResult _SearchQuick(string keyword = "")
        {
            var url = ConfigurationManager.AppSettings["Search"];

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(keyword))
            {
                return PartialView(new AllResultModel());
            }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(string.Format("{0}/Api/SearchApi/SearchAll?keyword={1}", url, keyword)).Result;

                ViewData["keyword"] = keyword;

                if (response.IsSuccessStatusCode)
                {
                    var results = JsonConvert.DeserializeObject<AllResultModel>(response.Content.ReadAsStringAsync().Result);

                    return PartialView(results);
                }
            }
            catch (Exception)
            {
                return PartialView(new AllResultModel());
            }

            return PartialView(new AllResultModel());
        }

        /// <summary>
        /// 搜索全部
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        [HttpGet]
        [UserAuthorize(isAnonymous = true)]
        public PartialViewResult _SearchAllResult(string keyword = "")
        {
            var url = ConfigurationManager.AppSettings["Search"];

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(keyword))
            {
                return PartialView(new AllResultModel());
            }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(string.Format("{0}/Api/SearchApi/SearchAll?keyword={1}", url, keyword)).Result;

                ViewData["keyword"] = keyword;

                if (response.IsSuccessStatusCode)
                {
                    var results = JsonConvert.DeserializeObject<AllResultModel>(response.Content.ReadAsStringAsync().Result);

                    return PartialView(results);
                }
            }
            catch (Exception)
            {
                return PartialView(new AllResultModel());
            }

            return PartialView(new AllResultModel());
        }

        /// <summary>
        /// 搜索结果 资讯 or 贴子
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="searchType">搜索类型(cms or thread)</param>
        /// <param name="isDefaultOrder">是否默认排序(默认相关度)</param>
        /// <param name="time">时间筛选</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        [UserAuthorize(isAnonymous = true)]
        public PartialViewResult _SearchResult(string keyword, string searchType, bool isDefaultOrder = true, string time = "year", int pageIndex = 1)
        {
            string url = "";
            switch (searchType.ToLower())
            {
                case "cms":
                    url = ConfigurationManager.AppSettings["Search"] + "/Api/SearchApi/CmsSearch";
                    break;
                case "thread":
                    url = ConfigurationManager.AppSettings["Search"] + "/Api/SearchApi/ThreadSearch";
                    break;
                case "ask":
                    url = ConfigurationManager.AppSettings["Search"] + "/Api/SearchApi/QuickAskSearch";
                    break;
                default:
                    break;
            }

            DateTime minDate = DateTime.Now.AddYears(-1);

            switch (time.ToLower())
            {
                case "month":
                    minDate = DateTime.Now.AddMonths(-1);
                    break;
                case "week":
                    minDate = DateTime.Now.AddDays(-7);
                    break;
                case "all":
                    minDate = DateTime.MinValue;
                    break;
                case "year":
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(keyword))
            {
                return PartialView(new SearchResultModel());
            }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(string.Format("{0}?keyword={1}&minDate={2}&isDefaultOrder={3}&pageIndex={4}", url, keyword, minDate, isDefaultOrder, pageIndex)).Result;

                ViewData["keyword"] = keyword;
                ViewData["searchType"] = searchType;
                ViewData["time"] = time;
                ViewData["isDefaultOrder"] = isDefaultOrder;

                if (response.IsSuccessStatusCode)
                {
                    var results = JsonConvert.DeserializeObject<SearchResultModel>(response.Content.ReadAsStringAsync().Result);
                    var page = results.Page;

                    ViewData["pageIndex"] = pageIndex;
                    ViewData["pageSize"] = 10;
                    ViewData["totalRecords"] = page.TotalRecords;

                    return PartialView(results);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return PartialView(new SearchResultModel());
        }

        #endregion
    }
}

