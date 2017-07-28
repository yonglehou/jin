//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Tunynet.CMS;
using Tunynet.Common;
using Tunynet.Email;
using Tunynet.Settings;
using Tunynet.FileStore;
using Tunynet.Post;
using Tunynet.Common.Configuration;

namespace Tunynet.Spacebuilder
{
    //[FrontEndAuthorize]
    [UserAuthorize(isAnonymous = true)]

    public partial class UserSpaceController : Controller
    {
        private IUser _currentUser = UserContext.CurrentUser;
        private UserService userService;
        private AccountBindingService accountBindingService;
        private ValidateCodeService validateCodeService;
        private UserProfileService userProfileService;
        private MembershipService membershipService;
        private IAuthenticationService authenticationService;
        private SiteSettings siteSetting;
        private ContentItemService contentItemService;
        private CommentService commentService;
        //贴子
        private SectionService sectionService;
        private ThreadService threadService;
        //分类
        private CategoryService categoryService;
        //用户等级
        private UserRankService userRankService;
        private FollowService followService;
        //积分
        private PointService pointService;
        private UserSettings userSetting;
        //收藏
        private FavoriteService favoriteService;
        //邀请朋友
        private InviteFriendService inviteFriendService;
        //附件
        private AttachmentService newsAttachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Article());
        //标签
        private TagService tagService = new TagService(TenantTypeIds.Instance().ContentItem());
        //栏目
        private ContentCategoryService contentCategoryService;
        //通知
        private NoticeService noticeService;
        private IKvStore kvStore;
        private INoticeSender noticeSender;
        private ContentModelService contentModelService;
        private InviteFriendSettings inviteFriendSetting;
        private Authorizer authorizer;
        public UserSpaceController(UserService userService,
                                   FollowService followService,
                                   AccountBindingService accountBindingService,
                                   SectionService sectionService,
                                   ValidateCodeService validateCodeService,
                                   UserProfileService userProfileService,
                                   MembershipService membershipService,
                                   IAuthenticationService authenticationService,
                                   ISettingsManager<SiteSettings> siteSettings,
                                   ISettingsManager<UserSettings> userSettings,
                                   ContentItemService contentItemService,
                                   CommentService commentService,
                                   ThreadService threadService,
                                   UserRankService userRankService,
                                   PointService pointService,
                                   InviteFriendService inviteFriendService,
                                   FavoriteService favoriteService,
                                   ContentCategoryService contentCategoryService,
                                   CategoryService categoryService,
                                   NoticeService noticeService,
                                   ContentModelService contentModelService,
                                   IKvStore kvStore,
                                   INoticeSender noticeSender,

                                   ISettingsManager<InviteFriendSettings> inviteFriendSettings,
                                   Authorizer authorizer)
        {
            this.userService = userService;
            this.followService = followService;
            this.accountBindingService = accountBindingService;
            this.validateCodeService = validateCodeService;
            this.userProfileService = userProfileService;
            this.membershipService = membershipService;
            this.authenticationService = authenticationService;
            this.siteSetting = siteSettings.Get();
            this.userSetting = userSettings.Get();
            this.commentService = commentService;
            this.contentItemService = contentItemService;
            this.userRankService = userRankService;
            this.threadService = threadService;
            this.pointService = pointService;
            this.favoriteService = favoriteService;
            this.sectionService = sectionService;
            this.inviteFriendService = inviteFriendService;
            this.contentCategoryService = contentCategoryService;
            this.categoryService = categoryService;
            this.noticeService = noticeService;
            this.kvStore = kvStore;
            this.contentModelService = contentModelService;
            this.noticeSender = noticeSender;
            this.inviteFriendSetting = inviteFriendSettings.Get();
            this.authorizer = authorizer;
        }

        #region 用户空间

        /// <summary>
        /// 头像局部视图异步加载  5.0 未使用
        /// </summary>
        /// <param name="spaceKey">空间标示</param>
        public ActionResult _EditAvatarAsyn(string spaceKey)
        {
            User user = userService.GetFullUser(spaceKey);
            if (user == null)
                return new EmptyResult();

            IStoreFile iStoreFile = userService.GetAvatar(user.UserId, AvatarSizeType.Original);

            return View();
        }

        /// <summary>
        /// 上传头像分布页
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        public PartialViewResult _Avatar()
        {
            return PartialView();
        }

        /// <summary>
        ///  他、她的主页
        /// </summary>
        /// <param name="spaceKey">当前用户</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SpaceHomepage(string spaceKey)
        {
            User userHolder = userService.GetFullUser(spaceKey);
            if (userHolder == null || userHolder.Status == UserStatus.Delete)
                return Redirect(SiteUrls.Instance().Error());
            if (_currentUser != null && _currentUser.UserId == userHolder.UserId)
                return RedirectToAction("MyHomepage", new { spaceKey = spaceKey });

            ViewData["userHolder"] = userHolder;

            //用户资料
            var userProfile = userProfileService.Get(userHolder.UserId);

            //文章数量
            var cmsCount = 0;
            //评论个数
            int conmmentCount = 0;
            //贴子数
            int threadCount = 0;
          
            cmsCount = userService.GetUserContentItemCount(userHolder.UserId, ContentModelKeys.Instance().Contribution());
            conmmentCount = userService.GetUserCommentCount(userHolder.UserId, null);
            threadCount = userService.GetUserThreadCount(userHolder.UserId, TenantTypeIds.Instance().Thread());


            if (_currentUser != null)
            {
                //关注信息
                var isMutualFollowed = followService.IsMutualFollowed(_currentUser.UserId, userHolder.UserId);
                ViewData["isMutualFollowed"] = isMutualFollowed;
                //是否为互相关注
                if (!isMutualFollowed)
                {
                    ViewData["isFollowed"] = followService.IsFollowed(_currentUser.UserId, userHolder.UserId);
                }
            }

            ViewData["userProfile"] = userProfile;
            ViewData["cmsCount"] = cmsCount;
            ViewData["conmmentCount"] = conmmentCount;
            ViewData["threadCount"] = threadCount;

            return View();
        }

        /// <summary>
        /// 我的主页
        /// </summary>
        /// <param name="spaceKey">当前用户</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyHomepage(string spaceKey)
        {
            User userHolder = userService.GetFullUser(spaceKey);
            if (userHolder == null || userHolder.Status == UserStatus.Delete)
            {
                return Redirect(SiteUrls.Instance().Error());
            }
            ViewData["userHolder"] = userHolder;

            if (_currentUser == null || _currentUser.UserId != userHolder.UserId)
                return RedirectToAction("SpaceHomepage", new { spaceKey });
            //用户资料
            var userProfile = userProfileService.Get(userHolder.UserId);
            //文章数量
            int cmsCount = 0;
            //评论计数
            int conmmentCount = 0;
            //贴子计数
            int threadCount = 0;
            //收藏计数
            int favoriteCount = 0;

            cmsCount = userService.GetUserContentItemCount(userHolder.UserId, ContentModelKeys.Instance().Contribution(), true);
            conmmentCount = userService.GetUserCommentCount(userHolder.UserId, null, true);
            threadCount = userService.GetUserThreadCount(userHolder.UserId, TenantTypeIds.Instance().Thread(), true);

            kvStore.TryGet<int>(KvKeys.Instance().UserFavoriteCount(userHolder.UserId), out favoriteCount);

            ViewData["cmsCount"] = cmsCount;
            ViewData["favoriteCount"] = favoriteCount;
            ViewData["userProfile"] = userProfile;
            ViewData["conmmentCount"] = conmmentCount;
            ViewData["threadCount"] = threadCount;

            return View();
        }


        /// <summary>
        /// 我的主页（分页）
        /// </summary>
        /// <param name="spaceKey">当前用户</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _MyHomepage(string spaceKey)
        {
            User userHolder = userService.GetFullUser(spaceKey);
            if (userHolder == null || userHolder.Status == UserStatus.Delete)
                return Redirect(SiteUrls.Instance().Error());
            PagingDataSet<ContentItem> contentItems = new PagingDataSet<ContentItem>(new List<ContentItem>());

            if (_currentUser == userHolder)
                contentItems = contentItemService.GetContentItems(null, true, userHolder.UserId, 10, 1, true, ContentItemSortBy.DatePublished_Desc, true, contentModelService.GetContentModelsByContentModeKey(ContentModelKeys.Instance().Contribution()).ModelId);
            else
                contentItems = contentItemService.GetContentItems(null, true, userHolder.UserId, 10, 1, true, ContentItemSortBy.DatePublished_Desc, false, contentModelService.GetContentModelsByContentModeKey(ContentModelKeys.Instance().Contribution()).ModelId);

            var follow = followService.GetFollowerUserIds(userHolder.UserId, Follow_SortBy.DateCreated_Desc, 1);
            if (follow != null)
                ViewData["follow"] = userService.GetFullUsers(follow.Take(6));

            ViewData["follows"] = followService.GetTopFollows(userHolder.UserId, 6, null);
            ViewData["contentItems"] = contentItems;
            ViewData["threads"] = threadService.GetUserThreads(TenantTypeIds.Instance().Thread(), userHolder.UserId, true, false, 10, 1);
            ViewData["userHolder"] = userHolder;
            //用户资料
            ViewData["userProfile"] = userProfileService.Get(userHolder.UserId);

            return PartialView();
        }

        /// <summary>
        /// 我、他（她）的文章
        /// </summary>
        /// <param name="spaceKey">当前用户</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _MyCMS(string spaceKey)
        {
            var userHolder = userService.GetUser(spaceKey);
            if (userHolder == null || userHolder.Status == UserStatus.Delete)
                return Redirect(SiteUrls.Instance().Error());
            ViewData["userHolder"] = userHolder;
            ViewData["categoryCount"] = GetCategories().Count();
            return PartialView();
        }

        /// <summary>
        /// 我、他（她）的文章分页列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _ListMyCMS(long userId, int pageSize = 6, int pageIndex = 1)
        {
            PagingDataSet<ContentItem> contentItems = new PagingDataSet<ContentItem>(new List<ContentItem>());
            var userHolder = userService.GetFullUser(userId);
            if (userHolder == null || userHolder.Status == UserStatus.Delete)
            {
                return Redirect(SiteUrls.Instance().Error());
            }
            if (_currentUser != null && userId == _currentUser.UserId)
                contentItems = contentItemService.GetContentItems(null, true, userId, pageSize, pageIndex, true, ContentItemSortBy.DatePublished_Desc, true, contentModelService.GetContentModelsByContentModeKey(ContentModelKeys.Instance().Contribution()).ModelId);
            else
                contentItems = contentItemService.GetContentItems(null, true, userId, pageSize, pageIndex, true, ContentItemSortBy.DatePublished_Desc, false, contentModelService.GetContentModelsByContentModeKey(ContentModelKeys.Instance().Contribution()).ModelId);

            ViewData["userId"] = userId;
            return PartialView(contentItems);
        }

        /// <summary>
        /// 写文章
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <param name="contentCategoryId">栏目ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditCMS(long? contentItemId, int? contentCategoryId)
        {
            if (_currentUser == null)
            {
                return Redirect(SiteUrls.Instance().Error());
            }
            var contentItem = ContentItem.New();
            var contentItemEditModel = new ContentItemEditModel();

            if (contentItemId.HasValue && contentItemId > 0)
            {
                contentItem = contentItemService.Get(contentItemId.Value);
                if (contentItem == null)
                    return Redirect(SiteUrls.Instance().Error());
                contentItem.MapTo(contentItemEditModel);
                contentItemEditModel.AudiStatus = (int)contentItem.ApprovalStatus;
                //栏目
                if (contentCategoryId.HasValue && contentCategoryId.Value > 0)
                {
                    //获取栏目
                    var category = contentCategoryService.Get(contentCategoryId.Value);
                    if (category == null)
                        return Redirect(SiteUrls.Instance().Error());
                    ViewData["category"] = category;
                }
                //获取标签
                if (contentItemId.HasValue && contentItemId.Value > 0)
                {
                    var tagsOfItem = tagService.attiGetItemInTagsOfItem(contentItemId.Value);
                    ViewData["tagsOfItem"] = tagsOfItem;
                }
            }
            ViewData["categorylist"] = GetCategories();
            contentItemEditModel.ContentModelId = contentModelService.GetContentModelsByContentModeKey(ContentModelKeys.Instance().Contribution()).ModelId;

            return PartialView(contentItemEditModel);
        }

        #region 资讯扩展
        public List<SelectListItem> GetCategories()
        {
            List<SelectListItem> categorylist = new List<SelectListItem>();
            var categories = contentCategoryService.GetIndentedAllCategories().Where(n => n.ContentModelKeys.Contains(ContentModelKeys.Instance().Contribution())).Where(c => c.IsEnabled);
            for (int i = 0; i < categories.Count(); i++)
            {
                var folder = categories.ElementAt(i);
                var selecttext = string.Format("{0}", folder.CategoryName);
                if (folder.Depth == 1)
                {
                    selecttext = string.Format("{0}{1}", "-", folder.CategoryName);
                }
                if (folder.Depth > 1)
                {
                    selecttext = string.Format("{0}{1}", "──", folder.CategoryName);
                }

                categorylist.Add(new SelectListItem { Text = selecttext, Value = folder.CategoryId.ToString() });
            }
            return categorylist;
        }
        #endregion

        /// <summary>
        /// 写文章
        /// </summary>
        /// <param name="contentItemEditModel">资讯</param>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public ActionResult _EditCMS(ContentItemEditModel contentItemEditModel)
        {
            if (_currentUser == null)
            {
                return Redirect(SiteUrls.Instance().Error());
            }
            Tag tag = Tag.New();
            var contentItem = ContentItem.New();
            contentItem = contentItemEditModel.AsContentItem(Request);
            var contentItemModel = new ContentItem();
            //编辑
            if (contentItemEditModel.ContentItemId > 0)
            {
                contentItemModel = contentItemService.Get(contentItemEditModel.ContentItemId);
                if (contentItemModel == null)
                    return Redirect(SiteUrls.Instance().Error());
                //编辑前是草稿
                if (contentItem.ApprovalStatus == 0)
                    contentItem.IsDraft = true;
                else
                    contentItem.IsDraft = false;
                //编辑后不是草稿
                if (contentItemEditModel.IsDraft == false && contentItem.ApprovalStatus == 0)
                    contentItem.ApprovalStatus = AuditStatus.Pending;
                contentItem.DatePublished = contentItemModel.DatePublished;
                contentItemService.Update(contentItem, TenantTypeIds.Instance().CMS_Article(), authorizer.IsCategoryManager(TenantTypeIds.Instance().CMS_Article(), UserContext.CurrentUser, contentItemEditModel.CategoryId));
                //标签
                if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Contribution())
                {
                    tagService.ClearTagsFromItem(contentItemEditModel.ContentItemId);

                    if (contentItemEditModel.tagvalue != null)
                    {
                        tagService.AddTagsToItem(contentItemEditModel.tagvalue.ToArray(), contentItemEditModel.ContentItemId);
                    }
                }


            }
            //创建用户投稿
            else
            {
                contentItem.ApprovalStatus = AuditStatus.Fail;
                //草稿
                if (contentItemEditModel.IsDraft == true)
                    contentItem.ApprovalStatus = 0;

                contentItemService.Create(contentItem, TenantTypeIds.Instance().CMS_Article(), authorizer.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), UserContext.CurrentUser, contentItemEditModel.CategoryId));
                if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Contribution())
                {
                    if (contentItemEditModel.tagvalue != null)
                    {
                        foreach (var item in contentItemEditModel.tagvalue)
                        {
                            tag.TenantTypeId = TenantTypeIds.Instance().ContentItem();
                            tag.TagName = item;
                            tagService.Create(tag);
                        }
                        tagService.AddTagsToItem(contentItemEditModel.tagvalue.ToArray(), contentItem.ContentItemId);
                    }

                }
            }
            //return Json(new StatusMessageData(StatusMessageType.Success, "保存成功"));
            return Redirect(SiteUrls.Instance().MyHome(_currentUser.UserId) + "#cms");
        }

        /// <summary>
        ///单个删除资讯
        /// </summary>
        /// <param name="contentItemId">内容项Id</param>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _DeleteCMS(long contentItemId)
        {
            var contentItem = contentItemService.Get(contentItemId);
            if (contentItem.IsAuthorizer())
            {
                contentItemService.Delete(contentItem);
                return Json(new StatusMessageData(StatusMessageType.Success, "删除成功"));
            }
            else
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));

        }
        ///<summary>
        ///草稿列表
        /// </summary>
        public ActionResult _ListDraft(long userId)
        {
            var userHolder = userService.GetUser(userId);
            if (userHolder == null || userHolder.Status == UserStatus.Delete)
                return Redirect(SiteUrls.Instance().Error());

            var contentItems = contentItemService.GetContentItems(null, null, userId, 20, 1, true, ContentItemSortBy.DatePublished_Desc, true, null, 0);
            return PartialView(contentItems);
        }

        #region 贴子
        /// <summary>
        /// 我的贴子
        /// </summary>
        /// <param name="spaceKey"></param>
        /// <returns></returns>
        public PartialViewResult _MyPost(string spaceKey)
        {
            var user = userService.GetFullUser(spaceKey);

            var userProfile = userProfileService.Get(user.UserId);

            var sectionIds = favoriteService.GetPagingPartObjectIds(user.UserId, TenantTypeIds.Instance().Bar(), 1, null).Select(n => n.ObjectId).Distinct();

            var sectionList = sectionService.GetBarsections(sectionIds);

            ViewData["user"] = user;
            ViewData["userProfile"] = userProfile;
            ViewData["mysections"] = sectionList;

            return PartialView();
        }

        /// <summary>
        /// 我的贴子 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListMyPost(long userId, int pageSize = 10, int pageIndex = 1)
        {

            bool ignoreAudit = false;
            if (_currentUser != null && _currentUser.UserId == userId)
                ignoreAudit = true;
            var threads = threadService.GetUserThreads(TenantTypeIds.Instance().Thread(), userId, ignoreAudit, false, pageSize, pageIndex);

            ViewData["user"] = userService.GetUser(userId);

            return PartialView(threads);
        }
        #endregion



        #region 我的评论/Ta的评论

        /// <summary>
        /// 我的评论/Ta的评论
        /// </summary>
        /// <param name="spaceKey">用户空间传来的用户名</param>
        /// <param name="isReceived">isReceived=true 为收到的评论,=false 为发出的评论为</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _UserSpaceComments(string spaceKey)
        {
            User spaceUser = userService.GetFullUser(spaceKey);
            ViewData["spaceUser"] = spaceUser;

            return PartialView();
        }

        /// <summary>
        /// 我的评论/Ta的评论 列表
        /// </summary>
        /// <param name="spaceKey">用户空间传来的用户名</param>
        /// <param name="isReceived">isReceived=true 为收到的评论,=false 为发出的评论为</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListComments(string spaceKey, bool isReceived = true, int pageIndex = 1)
        {
            PagingDataSet<Comment> userSpaceComments;

            User spaceUser = userService.GetFullUser(spaceKey);

            var spaceUserProfile = userProfileService.Get(spaceUser.UserId);

            //空间用户==当前用户，说明当前是我的空间
            if (_currentUser != null && spaceUser.UserId == _currentUser.UserId)
            {
                if (isReceived == true)
                {
                    //获取用户收到的评论
                    userSpaceComments = commentService.GetOwnerComments(_currentUser.UserId, null, null, null, pageIndex);
                }
                else
                {
                    //获取用户发布的评论
                    userSpaceComments = commentService.GetComments(null, null, _currentUser.UserId, null, null, 20, pageIndex);
                }
            }
            else
            {
                //获取Ta发布的评论
                userSpaceComments = commentService.GetUserComments(spaceUser.UserId, null, null, null, pageIndex);
            }

            Dictionary<string, string> tenantTypes = new Dictionary<string, string>
            {
                {TenantTypeIds.Instance().ContentItem(),"文章"},
                {TenantTypeIds.Instance().Thread() ,"贴子"}
            };

            ViewData["gender"] = spaceUserProfile.Gender == GenderType.Male ? "他" : "她";
            ViewData["spaceUser"] = spaceUser;
            ViewData["tenantTypes"] = tenantTypes;
            ViewData["isReceived"] = isReceived;

            return PartialView(userSpaceComments);
        }

        #endregion

        #region 我的收藏

        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _MyFavorites()
        {
            return PartialView();
        }

        /// <summary>
        /// 我的收藏列表
        /// </summary>
        /// <param name="isContentItem">isContentItem=true 为我收藏的文章,=false 为我收藏的贴子</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListMyFavorites(bool isContentItem = true, int pageIndex = 1, int pageSize = 20)
        {
            string tenantTypeId = (isContentItem == true ? TenantTypeIds.Instance().ContentItem() : TenantTypeIds.Instance().Thread());

            var myFavorites = favoriteService.GetPagingPartObjectIds(_currentUser.UserId, tenantTypeId, pageIndex, pageSize);
            var myFavoriteIds = myFavorites.Select(n => n.ObjectId).Distinct();

            if (myFavoriteIds.Any())
            {
                if (tenantTypeId == TenantTypeIds.Instance().ContentItem())
                {
                    //根据收藏Id获取收藏的资讯
                    ViewData["contentItems"] = contentItemService.Gets(myFavoriteIds);
                }
                else
                {
                    //根据收藏Id获取收藏的贴子
                    ViewData["threads"] = threadService.Gets(myFavoriteIds);
                }
            }

            ViewData["isContentItem"] = isContentItem;

            return PartialView(myFavorites);
        }



        #endregion

        #region 用户关注
        /// <summary>
        /// 关注、粉丝、邀请管理
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="follow">粉丝/关注列表标识 关注列表为"focus" 粉丝为"fans"默认为粉丝列表</param>
        /// <returns></returns>
        public PartialViewResult _ManageMyFollow(long userId, string follow)
        {
            ViewData["follow"] = follow;
            ViewData["userProfile"] = userProfileService.Get(userId);

            return PartialView();
        }

        /// <summary>
        /// 关注/取消关注用户
        /// </summary>
        /// <param name="targetUserId">需要关注/取消关注的用户Id</param>
        /// <returns></returns>
        public JsonResult _FollowUser(long targetUserId)
        {
            if (targetUserId == _currentUser.UserId)
            {
                return Json(new { state = 0, errormsg = "关注用户失败" });
            }

            if (followService.IsFollowed(_currentUser.UserId, targetUserId))
            {
                followService.CancelFollow(_currentUser.UserId, targetUserId);

                return Json(new { state = 1, successmsg = "取消关注成功", cancelfollow = true });
            }
            else
            {
                if (followService.Follow(_currentUser.UserId, targetUserId))
                {
                    //通知被关注用户
                    Notice notice = Notice.New();
                    notice.NoticeTypeKey = NoticeTypeKeys.Instance().FollowUser();
                    notice.Body = "";
                    notice.LeadingActor = _currentUser.DisplayName;
                    notice.LeadingActorUserId = _currentUser.UserId;
                    notice.ReceiverId = targetUserId;
                    notice.LeadingActorUrl = SiteUrls.Instance().SpaceHome(_currentUser.UserId);
                    noticeService.Create(notice);
                    noticeSender.Send(notice);

                    if (followService.IsMutualFollowed(_currentUser.UserId, targetUserId))
                    {
                        return Json(new { state = 1, successmsg = "关注用户成功", isMutualFollowed = true });
                    }
                    else
                    {
                        return Json(new { state = 1, successmsg = "关注用户成功" });
                    }
                }
                else
                {
                    return Json(new { state = 0, errormsg = "关注用户失败" });
                }
            }
        }

        /// <summary>
        /// 为用户设置备注名
        /// </summary>
        /// <param name="targetUserId">目标用户Id</param>
        /// <returns></returns>
        [UserAuthorize]
        public JsonResult _SetNoteName(long targetUserId, string noteName)
        {
            if (followService.IsFollowed(_currentUser.UserId, targetUserId))
            {
                FollowEntity follow = followService.Get(_currentUser.UserId, targetUserId);
                follow.NoteName = noteName;
                followService.Update(follow);

                return Json(new { state = 1 });
            }
            else
            {
                return Json(new { state = 0 });
            }
        }

        /// <summary>
        /// 关注列表分布页
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="groupId">用户分组Id</param>
        /// <param name="sortBy">排序条件</param>
        /// <param name="pageIndex">分页号</param>
        /// <returns></returns>
        public PartialViewResult _MyFocus(long userId, long? groupId, int pageIndex = 1, int pageSize = 10)
        {
            //关注的用户Id列表
            PagingDataSet<long> followedUserIds = followService.GetFollowedUserIds(userId, groupId, Follow_SortBy.DateCreated_Desc, pageIndex, pageSize);

            //用户列表
            IEnumerable<User> fullUserList = userService.GetFullUsers(followedUserIds);
            //关注用户信息列表
            IEnumerable<UserProfile> followedUserProfile = userProfileService.GetUserProfiles(followedUserIds);

            ViewData["fullUserList"] = fullUserList;
            ViewData["myUserId"] = userId;
            ViewData["followedUserProfile"] = followedUserProfile;

            return PartialView(followedUserIds);
        }

        /// <summary>
        /// 粉丝列表分布页
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">分页号</param>
        /// <returns></returns>
        public PartialViewResult _MyFans(long userId, int pageIndex = 1, int pageSize = 10)
        {
            //粉丝ID列表
            PagingDataSet<long> followerIds = followService.GetFollowerUserIds(userId, Follow_SortBy.DateCreated_Desc, pageIndex, pageSize);
            //用户粉丝列表
            IEnumerable<User> followerList = userService.GetFullUsers(followerIds);
            //粉丝信息列表
            IEnumerable<UserProfile> followerProfile = userProfileService.GetUserProfiles(followerIds);
            //获取当前用户
            if (UserContext.CurrentUser != null)
            {
                ViewData["currentUser"] = userService.GetFullUser(UserContext.CurrentUser.UserId);
            }
            ViewData["myUserId"] = userId;
            ViewData["followerProfile"] = followerProfile;
            ViewData["followerList"] = followerList;
            return PartialView(followerIds);
        }

        #endregion

        #region 邀请用户

        /// <summary>
        /// 邀请朋友分布页
        /// </summary>
        /// <returns></returns>
        [UserAuthorize]
        public PartialViewResult _InviteFriend()
        {
            string inviteCode = inviteFriendService.GetInvitationCode(UserContext.CurrentUser.UserId);

            InvitationCode invitationCode = inviteFriendService.GetInvitationCodeEntity(inviteCode);
            ViewData["invitationCode"] = inviteCode;
            return PartialView();
        }

        /// <summary>
        /// 我邀请的朋友列表
        /// </summary>
        [UserAuthorize]
        public PartialViewResult _MyInvitedFriendsList(int pageSize = 5, int pageIndex = 1)
        {
            long totalRecords;
            //邀请朋友Id列表
            IEnumerable<long> friendsIds = inviteFriendService.GetMyInviteFriendRecords(UserContext.CurrentUser.UserId, pageSize, pageIndex, out totalRecords);
            //邀请朋友列表
            IEnumerable<User> friendsList = userService.GetFullUsers(friendsIds);
            //邀请朋友用户资料列表
            IEnumerable<UserProfile> friendsProfile = userProfileService.GetUserProfiles(friendsIds);
            ViewData["friendsList"] = friendsList;
            ViewData["friendsProfile"] = friendsProfile;
            return PartialView(new PagingDataSet<long>(friendsIds)
            {
                TotalRecords = totalRecords,
                PageSize = pageSize,
                PageIndex = pageIndex
            });

        }


        #endregion

        #endregion

        #region 用户资料设置
       
        /// <summary>
        /// 用户资料设置
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public ActionResult UserSetting()
        {

           

            UserProfileEditModel userProfileEditModel = new UserProfileEditModel();

            var userProfile = userProfileService.Get(_currentUser.UserId);

            userProfile.MapTo(userProfileEditModel);

            userProfileEditModel.TrueName = _currentUser.TrueName;
            userProfileEditModel.UserName = _currentUser.UserName;
            userProfileEditModel.HasAvatar = _currentUser.HasAvatar;

            return View(userProfileEditModel);
        }



        /// <summary>
        /// 用户资料设置
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public ActionResult UserSetting(UserProfileEditModel userProfileEditModel)
        {
            var fullUser = userService.GetFullUser(_currentUser.UserId);
            if (fullUser != null)
            {
                var userProfile = userProfileService.Get(_currentUser.UserId);

                if (userProfile == null)
                {
                    UserProfile newUserProfile = UserProfile.New(_currentUser.UserId);

                    newUserProfile.Gender = userProfileEditModel.Gender;
                    newUserProfile.NowAreaCode = userProfileEditModel.NowAreaCode;
                    newUserProfile.Introduction = userProfileEditModel.Introduction;

                    userProfileService.Create(newUserProfile);
                }
                else
                {
                    userProfile.Gender = userProfileEditModel.Gender;
                    userProfile.NowAreaCode = userProfileEditModel.NowAreaCode;
                    userProfile.Introduction = userProfileEditModel.Introduction;
                    userProfileService.Update(userProfile);
                }

                fullUser.TrueName = userProfileEditModel.TrueName;

                membershipService.UpdateUser(fullUser);

                return Json(1);
            }

            return Json(0);
        }

        /// <summary>
        /// 用户资料分布页
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public PartialViewResult _UserProfile()
        {
            ViewData["user"] = _currentUser;
            ViewData["RegisterType"] = userSetting.RegisterType;

            return PartialView();
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public PartialViewResult _ChangePassword()
        {
            ProfileEditModel profileEditModel = new ProfileEditModel();

            return PartialView(profileEditModel);
        }
        /// <summary>
        /// 更改密码
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _ChangePassword(ProfileEditModel profileEditModel)
        {
            var username = string.Empty;
            if (_currentUser.IsMobileVerified)
                username = _currentUser.AccountMobile;
            if (_currentUser.IsEmailVerified)
                username = _currentUser.AccountEmail;
            var result = membershipService.ChangePassword(username, profileEditModel.PassWord, profileEditModel.NewPassword);
            if (result)
                return Json(1);
            return Json(0);
        }

        /// <summary>
        /// 更改昵称
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public PartialViewResult _ChangeUserName()
        {
            ProfileEditModel profileEditModel = new ProfileEditModel();

            return PartialView(profileEditModel);
        }
        /// <summary>
        /// 更改昵称
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _ChangeUserName(ProfileEditModel profileEditModel)
        {
            var user = (User)UserContext.CurrentUser;

            var oldUserName = user.UserName;
            user.UserName = profileEditModel.UserName;
            membershipService.UpdateUser(user);
            //移除字典中的ID与名字的关联
            UserIdToUserNameDictionary.RemoveUserId(user.UserId);
            UserIdToUserNameDictionary.RemoveUserName(oldUserName);
            return Json(1);
        }

        /// <summary>
        /// 绑定手机
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public PartialViewResult _ChangeMobile()
        {
            RegisterEditModel registerEditModel = new RegisterEditModel();

            return PartialView(registerEditModel);
        }


        /// <summary>
        /// 绑定手机
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _ChangeMobile(RegisterEditModel registerEditModel)
        {
            //手机注册
            var result = validateCodeService.Check(registerEditModel.AccountMobile, registerEditModel.VerfyCode);
            if (result != ValidateCodeStatus.Passed)
            {
                var errorMessage = validateCodeService.GetCodeError(result);
                return Json(new { state = 0, msg = errorMessage });
            }

            var user = userService.GetFullUser(_currentUser.UserId);
            user.AccountMobile = registerEditModel.AccountMobile;
            user.IsMobileVerified = true;
            membershipService.UpdateUser(user);

            return Json(new { state = 1, msg = "绑定成功" });
        }

        /// <summary>
        /// 绑定邮箱
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public PartialViewResult _ChangeEmail()
        {
            RegisterEditModel registerEditModel = new RegisterEditModel();

            return PartialView(registerEditModel);
        }

        /// <summary>
        /// 绑定邮箱
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _ChangeEmail(RegisterEditModel registerEditModel)
        {
            var iuser = userService.GetUserByEmail(registerEditModel.AccountEmail);
            if (iuser != null && iuser.Status == UserStatus.IsActivated)
            {
                return Json(new { state = 0, msg = "发送失败，您发送的邮箱已经是注册用户" });
            }

            MailMessage model = EmailBuilder.Instance().RegisterValidateEmail(_currentUser, true);
            var result = validateCodeService.EmailSend(_currentUser, "绑定邮箱验证", model, true);

            var usre = userService.GetFullUser(_currentUser.UserId);
            usre.UserGuid = registerEditModel.AccountEmail;
            membershipService.UpdateUser(usre);
            Dictionary<string, string> buttonLink = new Dictionary<string, string>();
            buttonLink.Add("用户设置页面", SiteUrls.Instance()._Perfecthref(SiteUrls.Instance().UserSetting()));
            TempData["SystemMessageViewModel"] = new SystemMessageViewModel() { Title = "帐号激活成功！", Body = $"你以后可以使用{_currentUser.AccountEmail}登录。<br/><span id='seconds'>5</span>秒后，自动跳转到", ButtonLink = buttonLink, StatusMessageType = StatusMessageType.Success };

            if (result)
                return Json(new { state = 1, msg = "发送邮件成功" });
            else
                return Json(new { state = 0, msg = "发送邮件失败" });
        }

        /// <summary>
        /// 发送激活邮件
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [UserAuthorize()]
        private bool ActivateByEmail(IUser user)
        {
            MailMessage model = EmailBuilder.Instance().RegisterValidateEmail(user);
            var result = validateCodeService.EmailSend(user, "注册帐号邮箱验证", model);

            return result;
        }

        #endregion 用户资料设置

        #region 第三方绑定


        /// <summary>
        /// 帐号绑定
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public ActionResult AccountBinding()
        {
            var bindings = accountBindingService.GetAccountBindings(_currentUser.UserId);

            ViewData["accountTypes"] = accountBindingService.GetAccountTypes(true);

            return View(bindings);
        }

        /// <summary>
        /// 第三方授权绑定
        /// </summary>
        /// <param name="accountTypeKey"></param>
        /// <returns></returns>
        [UserAuthorize()]
        public ActionResult ThirdBinding(string accountTypeKey)
        {
            ThirdAccountGetter thirdAccountGetter = ThirdAccountGetterFactory.GetThirdAccountGetter(accountTypeKey);

            if (accountTypeKey == AccountTypeKeys.Instance().WeChat())
            {
                AccountType accountType = accountBindingService.GetAccountType(accountTypeKey);

                ViewData["accountType"] = accountType;
                return View(thirdAccountGetter);
            }

            return Redirect(thirdAccountGetter.GetAuthorizationUrl(SiteUrls.FullUrl(SiteUrls.Instance().ThirdBindingCallBack(accountTypeKey))));
        }

        /// <summary>
        /// 第三方授权绑定返回页面
        /// </summary>
        /// <param name="accountTypeKey"></param>
        /// <returns></returns>
        [UserAuthorize()]
        public ActionResult ThirdBindingCallBack(string accountTypeKey)
        {
            ThirdAccountGetter thirdAccountGetter = ThirdAccountGetterFactory.GetThirdAccountGetter(accountTypeKey);
            int expires_in = 0;
            string accessToken = thirdAccountGetter.GetAccessToken(Request, out expires_in);
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Error, "授权失败,请稍后再试！");

                return Redirect(SiteUrls.Instance().AccountBinding());
            }

            //当前第三方帐号上用户标识
            var thirdCurrentUser = thirdAccountGetter.GetThirdUser(accessToken, thirdAccountGetter.OpenId);
            if (thirdCurrentUser != null)
            {
                //是否已绑定过其他帐号
                long userId = accountBindingService.GetUserId(accountTypeKey, thirdCurrentUser.Identification);

                User systemUser = userService.GetFullUser(userId);

                if (systemUser != null)
                {
                    if (_currentUser.UserId != systemUser.UserId)
                    {
                        TempData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Hint, "此帐号已在网站中绑定过，不可再绑定其他网站帐号");
                    }
                    else
                    {
                        accountBindingService.UpdateAccessToken(systemUser.UserId, thirdCurrentUser.AccountTypeKey, thirdCurrentUser.Identification, thirdCurrentUser.AccessToken, expires_in);

                        TempData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Success, "更新授权成功");
                    }
                }
                else
                {
                    AccountBinding account = new AccountBinding()
                    {
                        AccountTypeKey = accountTypeKey,
                        Identification = thirdCurrentUser.Identification,
                        UserId = _currentUser.UserId,
                        AccessToken = accessToken
                    };

                    if (expires_in > 0)
                        account.ExpiredDate = DateTime.Now.AddSeconds(expires_in);
                    accountBindingService.CreateAccountBinding(account);

                    TempData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Success, "绑定成功");
                }
            }

            return Redirect(SiteUrls.Instance().AccountBinding());
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public ActionResult CancelBinding(string accountTypeKey)
        {
            accountBindingService.DeleteAccountBinding(_currentUser.UserId, accountTypeKey);

            return Json(new { state = 1, msg = "解除绑定成功" });
        }

        #endregion 第三方绑定

        #region 会员等级

        /// <summary>
        /// 我的等级
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        public ActionResult MyRank()
        {
            return View();
        }

        /// <summary>
        /// 我的等级
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public PartialViewResult _MyRank()
        {
            List<UserRank> listUserRank = new List<UserRank>();
            var listUserRanks = userRankService.GetAll();
            listUserRank.AddRange(listUserRanks.Select(n => n.Value));
            ViewData["listUserRank"] = listUserRank;

            ViewData["experiencePoints"] = userRankService.Get(_currentUser.Rank + 1).PointLower - _currentUser.ExperiencePoints;
            ViewData["pointItems"] = pointService.GetPointItemsOfIncome();
            ViewData["experience"] = pointService.GetPointCategory("ExperiencePoints");
            ViewData["trade"] = pointService.GetPointCategory("TradePoints");

            return PartialView();
        }

        /// <summary>
        /// 积分记录
        /// </summary>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public PartialViewResult _MyPointRecords(int pageSize = 15, int pageIndex = 1)
        {
            var pointRecord = pointService.GetPointRecords(_currentUser.UserId, null, null, null, null, pageSize, pageIndex);

            ViewData["Experience"] = pointService.GetPointCategory("ExperiencePoints").CategoryName;
            ViewData["Trade"] = pointService.GetPointCategory("TradePoints").CategoryName;

            return PartialView(pointRecord);
        }
        #endregion

        #region 用户通知

        /// <summary>
        /// 用户通知页
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        public ActionResult MyNotice()
        {
            if (UserContext.CurrentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<SelectListItem> noticeStatus = new List<SelectListItem>();
            noticeStatus.Add(new SelectListItem { Text = "未处理的通知", Value = NoticeStatus.Unhandled.ToString() });
            noticeStatus.Add(new SelectListItem { Text = "全部通知", Value = "" });
            ViewData["statusSelect"] = noticeStatus;
            ViewData["title"] = "我的通知";
            return View();
        }

        /// <summary>
        /// 用户通知列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpGet]
        public PartialViewResult _MyNotice(int? pageIndex, NoticeStatus? status)
        {
            var currentUser = UserContext.CurrentUser;
            ViewData["status"] = status;
            PagingDataSet<Notice> noticeList = new PagingDataSet<Notice>(new List<Notice>());
            if (currentUser != null)
            {
                noticeList = noticeService.Gets(currentUser.UserId, status, pageIndex);
                return PartialView(noticeList);
            }
            else
            {
                return PartialView(noticeList);
            }
        }

        /// <summary>
        /// 删除通知
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _DeleteNotice(List<long> noticeIds)
        {
            if (UserContext.CurrentUser == null)
            {
                return Json(new { state = 1 });
            }
            foreach (var id in noticeIds)
            {
                noticeService.Delete(id);
            }
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 标记为我知道了
        /// </summary>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _KnowNotice(List<long> noticeIds)
        {
            if (UserContext.CurrentUser == null)
            {
                return Json(new { state = 1 });
            }
            foreach (var id in noticeIds)
            {
                noticeService.SetIsHandled(id, NoticeStatus.Readed);
            }
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 接受
        /// </summary>
        /// <param name="noticeId">通知Id</param>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _AcceptNotice(long noticeId)
        {
            if (UserContext.CurrentUser == null)
            {
                return Json(new { state = 1 });
            }
            noticeService.SetIsHandled(noticeId, NoticeStatus.Accepted);
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="noticeId">通知Id</param>
        /// <returns></returns>
        [UserAuthorize()]
        [HttpPost]
        public JsonResult _RefuseNotice(long noticeId)
        {
            if (UserContext.CurrentUser == null)
            {
                return Json(new { state = 1 });
            }
            noticeService.SetIsHandled(noticeId, NoticeStatus.Refused);
            return Json(new { state = 1 });
        }
        #endregion

        #region 用户资料侧边栏
        /// <summary>
        /// 用户资料侧边栏
        /// </summary>
        [HttpGet]
        public PartialViewResult _UserInformation(long userId)
        {
            var userHolder = userService.GetUser(userId);
            var cmsCount = 0;
            //评论数量
            int commentCount = 0;

            cmsCount = userService.GetUserContentItemCount(userHolder.UserId, ContentModelKeys.Instance().Contribution());
            commentCount = userService.GetUserCommentCount(userHolder.UserId, null);
            //用户资料
            var userProfile = userProfileService.Get(userId);

            ViewData["commentCount"] = commentCount;
            ViewData["userProfile"] = userProfile;
            ViewData["userHolder"] = userHolder;
            ViewData["cmsCount"] = cmsCount;

            return PartialView();
        }
        #endregion


    }
}
