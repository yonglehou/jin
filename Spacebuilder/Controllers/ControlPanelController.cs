//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Tunynet.CMS;
using Tunynet.Common;
using Tunynet.Settings;
using Tunynet.UI;
using System.Configuration;
using Tunynet.Post;
using Tunynet.Caching;
using Tunynet.Logging;
using Tunynet.Common.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Tunynet.Tasks;
using DevTrends.MvcDonutCaching;
using System.Net;
using System.Xml;
using Tunynet.Utilities;
using System.Text.RegularExpressions;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 管理员 控制器
    /// </summary>
    [ConsoleAuthorize]
    public partial class ControlPanelController : Controller
    {
        #region Service&User
        private FavoriteService favoriteService = new FavoriteService(TenantTypeIds.Instance().Bar());
        UserService userService;
        MembershipService membershipService;
        RoleService roleService;
        private TagService tagService = new TagService(TenantTypeIds.Instance().ContentItem());
        private ISettingsManager<FileSettings> attachmentSettingsManager;
        private ThreadService threadService;
        private SectionService sectionService;
        private IUser user = UserContext.CurrentUser;
        private Authorizer authorizer;
        private TenantTypeService tenantTypeService;
        //资讯
        private CategoryService categoryService;
        private ContentItemService contentItemService;
        private ContentCategoryService categorieService;
        private SpecialContentitemService specialContentitemService;
        private SpecialContentTypeService specialContentTypeService;
        private CategoryManagerService categoryManagerService;
        private AccountBindingService accountBindingService;
        private ICacheService cacheService;
        private OperationLogService operationLogService;
        //积分
        private PointService pointService;
        //用户等级
        private UserRankService userRankService;
        //评论
        private CommentService commentService;
        //权限验证
        private AuthorizationService authorizationService;
        //权限管理
        private PermissionService permissionService;
        //关注
        private FollowService followService;
        //邀请
        private InviteFriendService inviteFriendService;
        //审核规则
        private AuditService auditService;
        //站点设置
        private ISettingsManager<SiteSettings> siteSettings;
        //用户设置
        private ISettingsManager<UserSettings> userSettings;
        //标识图全局设置类
        private ISettingsManager<ImageSettings> imageSettings;
        //邀请好友设置
        private ISettingsManager<InviteFriendSettings> inviteFriendSettings;
        //导航
        private NavigationService navigationService;
        //栏目
        private ContentCategoryService contentCategorieService;
        //广告
        private AdvertisingService advertisingService;
        //链接
        private LinkService linkService;
        //通知
        private NoticeService noticeService;
        private INoticeSender noticeSender;
        private ContentModelService contentModelService;
        private AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().ContentItem());
        //任务
        private TaskService taskService;
        //KvStore
        private IKvStore kvStore;
      

        private ISettingsManager<PauseSiteSettings> pauseSiteSettingsManager;
        #endregion

        public ControlPanelController(CategoryService categoryService,
                                    ContentItemService contentItemService,
                                    SpecialContentitemService specialContentitemService,
                                    SpecialContentTypeService specialContentTypeService,
                                    CategoryManagerService categoryManagerService,
                                    ContentCategoryService categorieService,
                                    ISettingsManager<FileSettings> attachmentSettingsManager,
                                    ThreadService threadService,
                                    UserService userService,
                                    MembershipService membershipService,
                                    RoleService roleService,
                                    ContentModelService contentModelService,
                                    SectionService sectionService,
                                    AccountBindingService accountBindingService,
                                    ICacheService cacheService,
                                    OperationLogService operationLogService,
                                    PointService pointService,
                                    UserRankService userRankService,
                                    TenantTypeService tenantTypeService,
                                    CommentService commentService,
                                    AuthorizationService authorizationService,
                                    PermissionService permissionService,
                                    FollowService followService,
                                    InviteFriendService inviteFriendService,
                                    Authorizer authorizer,
                                    AuditService auditService,
                                    ISettingsManager<SiteSettings> siteSettings,
                                    ISettingsManager<UserSettings> userSettings,
                                    ISettingsManager<ImageSettings> imageSettings,
                                    ISettingsManager<InviteFriendSettings> inviteFriendSettings,
                                    NavigationService navigationService,
                                    ContentCategoryService contentCategorieService,
                                    AdvertisingService advertisingService,
                                    LinkService linkService,
                                    NoticeService noticeService,
                                    INoticeSender noticeSender,
                                    TaskService taskService,
                                    IKvStore kvStore,
                                    ISettingsManager<PauseSiteSettings> pauseSiteSettingsManager)
        {
            this.categoryService = categoryService;
            this.specialContentitemService = specialContentitemService;
            this.specialContentTypeService = specialContentTypeService;
            this.contentItemService = contentItemService;
            this.categoryManagerService = categoryManagerService;
            this.categorieService = categorieService;
            this.attachmentSettingsManager = attachmentSettingsManager;
            this.threadService = threadService;
            this.sectionService = sectionService;
            this.accountBindingService = accountBindingService;
            this.cacheService = cacheService;
            this.operationLogService = operationLogService;
            this.pointService = pointService;
            this.userRankService = userRankService;
            this.userService = userService;
            this.membershipService = membershipService;
            this.roleService = roleService;
            this.tenantTypeService = tenantTypeService;
            this.commentService = commentService;
            this.authorizationService = authorizationService;
            this.permissionService = permissionService;
            this.followService = followService;
            this.inviteFriendService = inviteFriendService;
            this.authorizer = authorizer;
            this.auditService = auditService;
            this.siteSettings = siteSettings;
            this.userSettings = userSettings;
            this.imageSettings = imageSettings;
            this.inviteFriendSettings = inviteFriendSettings;
            this.navigationService = navigationService;
            this.contentCategorieService = contentCategorieService;
            this.advertisingService = advertisingService;
            this.linkService = linkService;
            this.noticeService = noticeService;
            this.noticeSender = noticeSender;
            this.contentModelService = contentModelService;
            this.taskService = taskService;
            this.kvStore = kvStore;
            this.pauseSiteSettingsManager = pauseSiteSettingsManager;
        }

        #region 站点后台

        /// <summary>
        /// 站点后台首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Home()
        {
            // //授权信息
            Product.TunynetLicenseManager.Instance().Validate();
            ////版本信息
            //string friendlyVersion = string.Empty;
            //var meta = pageResourceManager.GetRegisteredMetas().FirstOrDefault(n => n.Name == "generator");
            //if (meta != null)
            //    friendlyVersion = meta.Content;
            //ViewData["spacebuilderVersion"] = string.Format("{0}({1})", friendlyVersion, GetSpacebuilderVersion());

            //获取系统信息
            SystemInfo systemInfo = new SystemInfo();
            ViewData["systemInfo"] = systemInfo;
            ViewData["tunyNetLicenses"] = GetLicenses();
            ViewData["spacebuilderVersion"] = GetSpacebuilderVersion();

            return View();
        }

        /// <summary>
        /// 获取数据统计Json数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDataStatistics()
        {
            #region 待办事项
            //初始化各项需审核内容数量
            int cmsNeedAuditNum = 0, postNeedAuditNum = 0, commentNeedAuditNum = 0;

            //获取资讯需审核内容计数
            cmsNeedAuditNum = contentItemService.GetContentItemCount(AuditStatus.Pending) + contentItemService.GetContentItemCount(AuditStatus.Again);
            //获取贴子需审核内容计数
            postNeedAuditNum = threadService.GetThreadCount(AuditStatus.Pending) + threadService.GetThreadCount(AuditStatus.Again);
            //获取评论需审核内容计数
            commentNeedAuditNum = commentService.GetCommentCount(AuditStatus.Pending) + commentService.GetCommentCount(AuditStatus.Again);
           

            //json待办事项
            var jsonNeedAuditNum = new
            {
                cmsNeedAuditNum = cmsNeedAuditNum,
                postNeedAuditNum = postNeedAuditNum,
                commentNeedAuditNum = commentNeedAuditNum
            };
            #endregion

            #region 数据统计

            //资讯总计数和每日计数
            int cmsCountAll = 0, cmsCountPerDay = 0;
            cmsCountAll = contentItemService.GetContentItemCount();
            cmsCountPerDay = contentItemService.GetContentItemCount(is24Hours: true);
            //组装贴子Json数据
            var jsonCMSCount = new
            {
                cmsCount = new
                {
                    cmsCountAll = cmsCountAll,
                    cmsCountPerDay = cmsCountPerDay
                }
            };

            //贴子总计数和每日计数
            int threadCountAll = 0, threadCountPerDay = 0;
            threadCountAll = threadService.GetThreadCount();
            threadCountPerDay = threadService.GetThreadCount(is24Hours: true);
            //组装贴子Json数据
            var jsonThreadCount = new
            {
                threadCount = new
                {
                    threadCountAll = threadCountAll,
                    threadCountPerDay = threadCountPerDay
                }
            };

            //评论总计数和每日计数
            int commentCountAll = 0, commentCountPerDay = 0;
            commentCountAll = commentService.GetCommentCount();
            commentCountPerDay = commentService.GetCommentCount(is24Hours: true);
            //组装贴子Json数据
            var jsonCommentCount = new
            {
                commentCount = new
                {
                    commentCountAll = commentCountAll,
                    commentCountPerDay = commentCountPerDay
                }
            };

            //json数据统计
            var jsonDataSatistics = new
            {
                jsonCMSCount,
                jsonThreadCount,
                jsonCommentCount
            };

            #endregion
            //组装整体json数据
            var jsonControlPanelHomeData = new
            {
                jsonNeedAuditNum,
                jsonDataSatistics
            };

            return Json(jsonControlPanelHomeData, JsonRequestBehavior.AllowGet);
        }


        #region 授权版本信息
        /// <summary>
        /// 获取授权信息
        /// </summary>
        /// <returns></returns>
        private List<Product.TunynetLicense> GetLicenses()
        {
            Dictionary<string, Product.TunynetLicense> licenseCollection = Product.TunynetLicenseManager.Instance().LicenseCollection;
            List<Product.TunynetLicense> tunyNetLicenses = null;
            if (licenseCollection.Any(n => n.Value.Product.Level != "Free"))
                tunyNetLicenses = licenseCollection.Values.Where(n => n.Product.Level != "Free").ToList();
            else
                tunyNetLicenses = new List<Product.TunynetLicense>(licenseCollection.Values);
            return tunyNetLicenses;
        }

        /// <summary>
        /// 获取SPB的详细版本信息
        /// </summary>
        /// <returns></returns>
        private string GetSpacebuilderVersion()
        {
            //通过设置程序集Properties下AssemblyInfo.cs 中的内容改变版本信息
            Type t = typeof(ControlPanelController);
            Version spaceBuilderVersion = t.Assembly.GetName().Version;
            return spaceBuilderVersion.ToString();
        }

        /// <summary>
        /// 获取SpaceBuilder最新版本信息
        /// </summary>
        public JsonResult GetMostRecentVersion()
        {
            License.LicenseService licenseService = DIContainer.Resolve<License.LicenseService>();

            var siteSetting = siteSettings.Get();

            var mostRecentVersion = licenseService.GetMostRecentVersion(siteSetting.SiteKey, siteSetting.SiteName, System.Web.HttpContext.Current);
            return Json(mostRecentVersion, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region 资讯

        #region 资讯管理

        #region 资讯创建
        /// <summary>
        /// 是否有权限管理资讯
        /// </summary>
        private bool IsCMSManager(int? contentCategoryId)
        {
            var isCategoryManager = authorizer.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), user, contentCategoryId);
            if (!isCategoryManager)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您没有权限访问该页面！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "无权查看",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
            }
            return isCategoryManager;
        }
        /// <summary>
        /// 是否有权限管理资讯
        /// </summary>
        private bool IsCMSCategoryManager()
        {
            var isCategoryManager = authorizationService.Check(user, PermissionItemKeys.Instance().CMS());
            if (!isCategoryManager)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您没有权限访问该页面！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "无权查看",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
            }
            return isCategoryManager;
        }
        /// <summary>
        ///创建/更新资讯
        /// </summary>
        /// <param name="contentItemId">资讯内容项ID</param>
        /// <param name="contentCategoryId">栏目ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCMS(long? contentItemId, int? contentCategoryId)
        {
            if (!IsCMSManager(contentCategoryId))
            {
                return RedirectToAction("BankEndError");
            }
            var contentItem = ContentItem.New();
            var contentItemEditModel = new ContentItemEditModel();
            if (contentItemId.HasValue)
            {
                contentItem = contentItemService.Get(contentItemId.Value);
                if (contentItem == null)
                    return RedirectToAction("BankEndError");
            }
            //根据Id取出栏目
            if (contentCategoryId.HasValue && contentCategoryId.Value > 0)
            {
                //跳转
                if (contentItem.ContentModel != null)
                    if ("EditCMS" != contentItem.ContentModel.PageEdit)
                        return RedirectToAction(contentItem.ContentModel.PageEdit, new { contentItemId, contentCategoryId });
                var category = categorieService.Get(contentCategoryId.Value);
                if (!(category.ContentModelKeys.Contains(ContentModelKeys.Instance().Article()) || category.ContentModelKeys.Contains(ContentModelKeys.Instance().Contribution())))
                    return RedirectToAction(category.ContentTypes.First().PageEdit, new { contentItemId, contentCategoryId });
                ViewData["category"] = category;
            }

            //获取标签
            if (contentItemId.HasValue && contentItemId.Value > 0)
            {
                var tagsOfItem = tagService.attiGetItemInTagsOfItem(contentItemId.Value);
                ViewData["tagsOfItem"] = tagsOfItem;
            }
            contentItem.MapTo(contentItemEditModel);
            if (!contentItemId.HasValue)
                contentItemEditModel.ContentModelId = contentModelService.GetContentModelsByContentModeKey(ContentModelKeys.Instance().Article()).ModelId;
            return View(contentItemEditModel);
        }
        /// <summary>
        ///创建/更新资讯
        /// </summary>
        /// <param name="contentItemId">资讯</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCMS(ContentItemEditModel contentItemEditModel)
        {
            if (!IsCMSManager(contentItemEditModel.CategoryId))
            {
                return Json(new StatusMessageData(StatusMessageType.Success, "没有删除权限"));
            }

            Tag tag = Tag.New();
            var category = categorieService.Get(contentItemEditModel.CategoryId);
            var contentItem = ContentItem.New();
            contentItem = contentItemEditModel.AsContentItem(Request);
            var contentItemModel = new ContentItem();

            var contentItemTenantTypeId = TenantTypeIds.Instance().CMS_Article();
            //附件操作
            if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Video())
            {
                contentItemTenantTypeId = TenantTypeIds.Instance().CMS_Video();
            }
            else if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Image())
            {
                contentItemTenantTypeId = TenantTypeIds.Instance().CMS_Image();
            }

            if (contentItem.ContentItemId > 0)
            {
                contentItemModel = contentItemService.Get(contentItemEditModel.ContentItemId);
                #region 文章类操作 -标签
                if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Article() || contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Contribution())
                {
                    if (contentItemEditModel.tagvalue == null)
                        contentItemEditModel.tagvalue = new List<string>();
                    tagService.ClearTagsFromItem(contentItemEditModel.ContentItemId);
                    tagService.AddTagsToItem(contentItemEditModel.tagvalue.ToArray(), contentItemEditModel.ContentItemId);
                }
                #endregion

                contentItem.DateCreated = contentItemModel.DateCreated;
                contentItem.DatePublished = contentItem.DatePublished;
                contentItem.LastModified = DateTime.Now;
                contentItem.Author = contentItemModel.Author;
                contentItemService.Update(contentItem, contentItemTenantTypeId, authorizer.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), UserContext.CurrentUser, contentItemEditModel.CategoryId));

            }
            //创建操作
            else
            {
                contentItem.ApprovalStatus = AuditStatus.Fail;
                contentItem.LastModified = DateTime.Now;
                contentItemService.Create(contentItem, contentItemTenantTypeId, authorizer.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), UserContext.CurrentUser, contentItemEditModel.CategoryId));

                if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Article() || contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Contribution())
                {
                    if (contentItemEditModel.tagvalue == null)
                    {
                        contentItemEditModel.tagvalue = new List<string>();
                    }
                    if (contentItemEditModel.tagvalue.Count() > 0)
                    {
                        foreach (var item in contentItemEditModel.tagvalue)
                        {
                            tag.TenantTypeId = TenantTypeIds.Instance().ContentItem();
                            tag.TagName = item;
                            tagService.Create(tag);
                        }
                    }
                    tagService.AddTagsToItem(contentItemEditModel.tagvalue.ToArray(), contentItem.ContentItemId);
                }
            }
            return RedirectToAction("ManageCMS", new { contentCategoryId = contentItemEditModel.CategoryId });
        }

        #endregion 


        /// <summary>
        /// 资讯首页
        /// </summary>
        /// <param name="contentCategoryId">栏目ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageCMS(int? contentCategoryId)
        {
            if (!IsCMSManager(contentCategoryId))
            {
                return RedirectToAction("BankEndError");
            }

            //栏目
            var categories = categorieService.GetIndentedAllCategories();
            ViewData["categories"] = categories;

            //内容模型
            if (contentCategoryId.HasValue)
            {
                var categorieInfo = categorieService.Get(contentCategoryId.Value);
                ViewData["contentCategoryParentId"] = categorieInfo == null ? 0 : categorieInfo.ParentId;
                ViewData["contentTypes"] = categorieInfo == null ? null : categorieInfo.ContentTypes;
            }

            //审核状态
            List<SelectListItem> auditStatus = new List<SelectListItem>();
            auditStatus.Add(new SelectListItem() { Text = "全部", Value = string.Empty });
            foreach (AuditStatus item in Enum.GetValues(typeof(AuditStatus)))
            {
                auditStatus.Add(new SelectListItem() { Text = item.GetDisplayName(), Value = item.ToString() });
            }
            ViewData["auditStatus"] = auditStatus;
            return View();
        }

        /// <summary>
        /// 资讯列表
        /// </summary>
        /// <param name="contentCategoryId">栏目ID</param>
        /// <param name="keyword">关键字</param>
        /// <param name="auditStatus">审批状态</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListCMS(int? contentCategoryId, string keyword = null, AuditStatus? auditStatus = null, DateTime? startDate = null, DateTime? endDate = null, int pageSize = 15, int pageIndex = 1)
        {
            if (!IsCMSManager(contentCategoryId))
            {
                return PartialView();
            }

            var query = new ContentItemQuery();
            query.CategoryId = contentCategoryId;
            query.SubjectKeyword = keyword;
            query.AuditStatus = auditStatus;
            query.MinDate = startDate;
            query.MaxDate = endDate;
            ViewData["query"] = query;
            var contentItems = contentItemService.GetContentItemForAdmin(keyword, contentCategoryId, true, null, null, auditStatus, startDate, endDate, true, pageSize, pageIndex);
            return PartialView(contentItems);
        }


        /// <summary>
        ///单个删除资讯
        /// </summary>
        /// <param name="contentItemId">内容项Id</param>
        /// <returns></returns>
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

        /// <summary>
        ///批量删除资讯
        /// </summary>
        /// <param name="cmsIds">资讯ID集合</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeleteCMSList(List<long> cmsIds)
        {
            var successCount = 0;
            foreach (var cmsId in cmsIds)
            {
                var cms = contentItemService.Get(cmsId);
                if (cms != null)
                    if (cms.IsAuthorizer())
                    {
                        contentItemService.Delete(cms);
                        successCount++;
                    }
            }
            var content = string.Format("批量删除成功。{0}个成功，{1}个失败。", successCount, cmsIds.Count - successCount);
            return Json(new StatusMessageData(StatusMessageType.Success, content));
        }

        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <param name="isSticky">是否置顶</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StickyCMS(long contentItemId, bool isSticky)
        {
            var contentItem = contentItemService.Get(contentItemId);
            if (contentItem != null && contentItem.IsAuthorizer(true))
            {
                contentItemService.ContentItemIsSticky(contentItemId, isSticky);
                return Json(new StatusMessageData(StatusMessageType.Success, "操作成功"));
            }
            return Json(new StatusMessageData(StatusMessageType.Error, "操作失败"));
        }

        /// <summary>
        /// 批量置顶
        /// </summary>
        /// <param name="cmsIds">资讯ID集合</param>
        /// <param name="isSticky">是否置顶</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StickyCMSs(List<long> cmsIds, bool isSticky)
        {
            var successCount = 0;

            foreach (var cmsId in cmsIds)
            {
                var contentItem = contentItemService.Get(cmsId);
                if (contentItem != null)
                {
                    if (contentItem.IsAuthorizer())
                    {
                        contentItemService.ContentItemIsSticky(cmsId, isSticky);
                        successCount++;
                    }
                }
            }
            var content = string.Format("操作成功。{0}个成功，{1}个失败。", successCount, cmsIds.Count - successCount);
            return Json(new StatusMessageData(StatusMessageType.Success, content));
        }

        /// <summary>
        /// 批量资讯通过/不通过审核
        /// </summary>
        /// <param name="cmsIds"></param>
        /// <param name="isApproved"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AuditStatusCMS(List<long> cmsIds, bool isApproved)
        {
            var successCount = 0;
            foreach (var cmsId in cmsIds)
            {
                var contentItem = contentItemService.Get(cmsId);
                if (contentItem != null)
                {
                    if (contentItem.IsAuthorizer(true))
                    {
                        contentItemService.UpdateAuditStatus(cmsId, isApproved);
                        successCount++;
                    }
                }
            }
            var content = string.Format("操作成功。{0}个成功，{1}个失败。", successCount, cmsIds.Count - successCount);
            return Json(new StatusMessageData(StatusMessageType.Success, content));
        }

        #region 组图资讯

        /// <summary>
        ///创建/更新资讯
        /// </summary>
        /// <param name="contentItemId">资讯内容项ID</param>
        /// <param name="contentCategoryId">栏目ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCMSImg(long? contentItemId, int? contentCategoryId)
        {
            if (!IsCMSManager(contentCategoryId))
            {
                return RedirectToAction("BankEndError");
            }
            var contentItem = ContentItem.New();
            var contentitemEditModel = new ContentItemEditModel();
            var contentCategoryPortal = new ContentCategoryPortal();
            var contentModelKeys = ContentModelKeys.Instance().Image();

            if (contentItemId.HasValue)
                contentItem = contentItemService.Get(contentItemId.Value);

            //根据id取出栏目
            if (contentCategoryId.HasValue && contentCategoryId.Value > 0)
            {
                //跳转
                if (contentItem.ContentModel != null && "EditCMSImg" != contentItem.ContentModel.PageEdit)
                    return RedirectToAction(contentItem.ContentModel.PageEdit, new { contentItemId = contentItemId, contentCategoryId = contentCategoryId });

                var categorie = categorieService.Get(contentCategoryId.Value);
                if (!categorie.ContentModelKeys.Contains(contentModelKeys))
                    return RedirectToAction(categorie.ContentTypes.First().PageEdit, new { contentItemId = contentItemId, contentCategoryId = contentCategoryId });

                ViewData["contentCategory"] = categorie;
            }
            contentItem.MapTo(contentitemEditModel);
            contentitemEditModel.ContentModelId = contentModelService.GetContentModelsByContentModeKey(ContentModelKeys.Instance().Image()).ModelId;
            return View(contentitemEditModel);
        }

        /// <summary>
        /// 组图分布页
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _EditCMSImg(long contentItemId = 0)
        {
            if (!IsCMSManager(null))
            {
                return PartialView();
            }

            var attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Image());
            if (contentItemId > 0)
            {
                var attachments = attachmentService.GetsByAssociateId(contentItemId);
                ViewData["attachmentList"] = attachments;
            }
            else
                ViewData["attachmentList"] = attachmentService.GetTemporaryAttachments(user.UserId, TenantTypeIds.Instance().CMS_Image());
            ViewData["contentItemId"] = contentItemId;
            return PartialView();
        }

        /// <summary>
        /// 编辑图片附件描述
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="contentItemId">资讯ID</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _EditAttachmentDiscription(long attachmentId = 0, long contentItemId = 0)
        {
            var attachmentEditModel = new AttachmentEditModel();
            var attachment = attachmentService.Get(attachmentId);
            ViewData["contentItemId"] = contentItemId;
            attachment.MapTo(attachmentEditModel);

            return PartialView(attachmentEditModel);
        }

        /// <summary>
        /// 编辑图片附件描述
        /// </summary>
        /// <param name="attachmentEditModel"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditAttachmentDiscription(AttachmentEditModel attachmentEditModel)
        {
            var attachment = attachmentService.Get(attachmentEditModel.AttachmentId);
            if (attachment != null)
            {
                if (authorizer.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), user, attachment.AssociateId))
                {
                    attachment.Discription = attachmentEditModel.Discription;
                    attachmentService.Update(attachment);
                    return Json(new StatusMessageData(StatusMessageType.Success, "编辑成功"));
                }
            }
            return Json(new StatusMessageData(StatusMessageType.Error, "编辑失败"));
        }

        /// <summary>
        /// 更改附件显示顺序(上下移动)
        /// </summary>
        /// <param name="fromAttachmentId">需要交换的附件ID</param>
        /// <param name="toAttachmentId">被交换的附件ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangeAttachmentOrder(long fromAttachmentId, long toAttachmentId)
        {
            if (!IsCMSManager(null))
            {
                return Json(new StatusMessageData(StatusMessageType.Error, "调整失败"));
            }
            //todo: wanglei 判断是否统一内容项

            var fromAttachment = attachmentService.Get(fromAttachmentId);
            var toAttachment = attachmentService.Get(toAttachmentId);

            if (fromAttachment == null || toAttachment == null)
                return Json(new StatusMessageData(StatusMessageType.Error, "调整失败"));

            if (fromAttachment.AssociateId != toAttachment.AssociateId)
                return Json(new StatusMessageData(StatusMessageType.Error, "调整失败"));

            if (authorizer.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), user, fromAttachment.AssociateId))
            {
                //更改栏目显示顺序
                var temp = fromAttachment.DisplayOrder;

                fromAttachment.DisplayOrder = toAttachment.DisplayOrder;
                attachmentService.Update(fromAttachment);

                toAttachment.DisplayOrder = temp;
                attachmentService.Update(toAttachment);
                return Json(new StatusMessageData(StatusMessageType.Success, "调整成功"));
            }

            return Json(new StatusMessageData(StatusMessageType.Error, "调整失败"));
        }

        /// <summary>
        /// 删除组图图片
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeleteAttachmentCMSImg(long attachmentId)
        {
            if (!IsCMSManager(null))
            {
                return Json(new StatusMessageData(StatusMessageType.Error, "删除失败"));
            }
            var attachment = attachmentService.Get(attachmentId);
            if (attachment == null)
                return Json(new StatusMessageData(StatusMessageType.Error, "删除失败"));
            if (authorizer.IsCategoryManager(TenantTypeIds.Instance().ContentItem(), user, attachment.AssociateId))
            {
                attachmentService.Delete(attachmentId);
                return Json(new StatusMessageData(StatusMessageType.Success, "删除附件成功"));
            }

            return Json(new StatusMessageData(StatusMessageType.Error, "删除失败"));
        }
        #endregion



        #region 视频资讯

        /// <summary>
        /// 创建/更新资讯 视频
        /// </summary>
        /// <param name="contentItemId">资讯ID</param>
        /// <param name="contentCategoryId">栏目ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCMSVideo(long? contentItemId, int? contentCategoryId)
        {
            if (!IsCMSManager(contentCategoryId))
            {
                return RedirectToAction("BankEndError");
            }
            var contentItem = ContentItem.New();
            var contentitemEditModel = new ContentItemEditModel();
            var contentModelKeys = ContentModelKeys.Instance().Video();

            //根据id取出栏目
            if (contentCategoryId.HasValue && contentCategoryId.Value > 0)
            {
                //跳转
                if (contentItem.ContentModel != null)
                    if ("EditCMSVideo" != contentItem.ContentModel.PageEdit)
                        return RedirectToAction(contentItem.ContentModel.PageEdit, new { contentItemId, contentCategoryId });
                var categorie = categorieService.Get(contentCategoryId.Value);
                if (!categorie.ContentModelKeys.Contains(contentModelKeys))
                    return RedirectToAction(categorie.ContentTypes.First().PageEdit, new { contentItemId, contentCategoryId });
                ViewData["contentCategory"] = categorie;
            }

            //如果是编辑，则取出这个contentitem
            if (contentItemId.HasValue)
            {
                contentItem = contentItemService.Get(contentItemId.Value);
                if (contentItem != null)
                {
                    //如果录入的不是地址则获取附件
                    if (contentItem.AdditionalProperties.ContainsKey("VideoUrl") && !string.IsNullOrEmpty(contentItem.AdditionalProperties["VideoUrl"].ToString()))
                        contentitemEditModel.VideoUrl = contentItem.AdditionalProperties["VideoUrl"].ToString();
                    else
                        //获取附件
                        ViewData["videoAttachment"] = contentItem.GetCMSVideo();
                }
            }

            contentItem.MapTo(contentitemEditModel);
            contentitemEditModel.ContentModelId = contentModelService.GetContentModelsByContentModeKey(ContentModelKeys.Instance().Video()).ModelId;

            List<SelectListItem> selectListItem = new List<SelectListItem>();
            selectListItem.Add(new SelectListItem { Text = "上传视频", Value = "0" });
            selectListItem.Add(new SelectListItem { Text = "录入地址", Value = "1" });
            SelectList selectList = new SelectList(selectListItem, "Value", "Text", string.IsNullOrEmpty(contentitemEditModel.VideoUrls) ? "0" : "1");
            ViewData["selectList"] = selectList;
            return View(contentitemEditModel);
        }
        #endregion
        #endregion

        #region 栏目管理

        /// <summary>
        ///栏目管理列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageContentCategories()
        {
            if (!IsCMSCategoryManager())
            {
                return RedirectToAction("BankEndError");
            }

            var contentCategories = categorieService.GetIndentedAllCategories();
            return View(contentCategories);
        }

        /// <summary>
        ///创建栏目（包括创建子栏目）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditContentCategories(int categoryId = 0, int parentId = 0)
        {
            if (!IsCMSCategoryManager())
            {
                return RedirectToAction("BankEndError");
            }

            var contentCategoryModel = new ContentCategoryModel();
            var contentCategory = new ContentCategory();
            var contentCategoryPortal = new ContentCategoryPortal();

            //栏目ID大于0时编辑，否则添加
            if (categoryId > 0)
            {
                contentCategory = categorieService.Get(categoryId);
                contentCategory.MapTo(contentCategoryPortal);

                if (contentCategory == null)
                    return HttpNotFound();

                contentCategoryPortal.MapTo(contentCategoryModel);
                contentCategoryModel.ContentModelKeys =
                    new List<string>(contentCategoryPortal.ContentModelKeys.Split(','));
                bool isReference = false;
                var contentCategoryAdmin = categoryManagerService.GetCategoryManagerIds(TenantTypeIds.Instance().ContentItem(), categoryId, out isReference);
                contentCategoryModel.ContentCategoryAdmin = contentCategoryAdmin.Count() > 0 ? contentCategoryAdmin.Select(l => l.ToString()).ToList() : new List<string>();
                contentCategoryModel.IsInherit = isReference;
            }
            else
            {
                if (parentId > 0)
                    contentCategoryModel.ParentId = parentId;
            }
            return View(contentCategoryModel);
        }

        /// <summary>
        ///创建栏目
        /// </summary>
        /// <param name="contentCategoryModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _EditContentCategories(ContentCategoryModel contentCategoryModel)
        {
            if (!IsCMSCategoryManager())
            {
                return RedirectToAction("BankEndError");
            }

            ContentCategoryPortal contentCategoryPortal = new ContentCategoryPortal();
            //编辑
            if (contentCategoryModel.CategoryId > 0)
            {
                contentCategoryPortal = contentCategoryModel.AsContentCategory();
                if (contentCategoryModel.ParentId.HasValue && contentCategoryModel.ParentId.Value > 0)
                {
                    if (contentCategoryModel.IsInherit)
                    {
                        categoryManagerService.UpdateManagerIds(TenantTypeIds.Instance().ContentItem(), Convert.ToInt64(contentCategoryModel.CategoryId), null, contentCategoryModel.ParentId.Value);
                        contentCategoryPortal.ContentCategoryAdmin = string.Empty;
                    }
                    else
                    {
                        if (contentCategoryModel.ContentCategoryAdmin != null && contentCategoryModel.ContentCategoryAdmin.Count > 0)
                        {
                            var contentCategoryAdmins = new List<long>();
                            foreach (var item in contentCategoryModel.ContentCategoryAdmin)
                            {
                                contentCategoryAdmins.Add(Convert.ToInt64(item));
                            }
                            categoryManagerService.UpdateManagerIds(TenantTypeIds.Instance().ContentItem(), Convert.ToInt64(contentCategoryModel.CategoryId), contentCategoryAdmins);
                            contentCategoryPortal.ContentCategoryAdmin = string.Join(",", contentCategoryModel.ContentCategoryAdmin);
                        }
                    }
                }
                else
                {
                    if (contentCategoryModel.ContentCategoryAdmin != null && contentCategoryModel.ContentCategoryAdmin.Count > 0)
                    {
                        var contentCategoryAdmins = new List<long>();
                        foreach (var item in contentCategoryModel.ContentCategoryAdmin)
                        {
                            contentCategoryAdmins.Add(Convert.ToInt64(item));
                        }
                        categoryManagerService.UpdateManagerIds(TenantTypeIds.Instance().ContentItem(), Convert.ToInt64(contentCategoryModel.CategoryId), contentCategoryAdmins);
                        contentCategoryPortal.ContentCategoryAdmin = string.Join(",", contentCategoryModel.ContentCategoryAdmin);
                    }
                }
                categorieService.Update(contentCategoryPortal);
            }
            //添加
            else
            {
                contentCategoryPortal = contentCategoryModel.AsContentCategory();
                var contentCategoryAdmins = new List<long>();
                if (contentCategoryModel.ContentCategoryAdmin != null)
                    foreach (var item in contentCategoryModel.ContentCategoryAdmin)
                    {
                        contentCategoryAdmins.Add(Convert.ToInt64(item));
                    }
                categorieService.Create(contentCategoryPortal);
                if (contentCategoryPortal.IsInherit)
                    categoryManagerService.UpdateManagerIds(TenantTypeIds.Instance().ContentItem(), Convert.ToInt64(contentCategoryPortal.CategoryId), null, contentCategoryPortal.ParentId);
                else
                    categoryManagerService.UpdateManagerIds(TenantTypeIds.Instance().ContentItem(), Convert.ToInt64(contentCategoryPortal.CategoryId),
                    contentCategoryAdmins.Select(l => Convert.ToInt64(l)).ToList());
            }
            return RedirectToAction("ManageContentCategories");
        }

        /// <summary>
        ///删除栏目
        /// </summary>
        /// <param name="CategoryId">栏目ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteContentCategories(int categoryid)
        {
            if (!IsCMSManager(null))
            {
                return Json(new StatusMessageData(StatusMessageType.Success, "没有删除权限"));
            }

            var contentCategories = categorieService.Get(categoryid);
            if (contentCategories == null)
                return Json(new StatusMessageData(StatusMessageType.Hint, "找不到栏目"));

            if (authorizationService.Check(user, PermissionItemKeys.Instance().CMS()))
                categorieService.Delete(contentCategories);
            else
                return Json(new StatusMessageData(StatusMessageType.Error, "没有删除权限"));
            return Json(new StatusMessageData(StatusMessageType.Success, "删除成功"));
        }

        /// <summary>
        ///更改栏目显示顺序(上下移动)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangeContentCategories(int fromContentCategoryId, int toContentCategoryId)
        {
            if (!IsCMSManager(null))
            {
                return Json(new StatusMessageData(StatusMessageType.Success, "没有删除权限"));
            }

            if (authorizationService.Check(user, PermissionItemKeys.Instance().CMS()))
                categorieService.ChangeContentCategory(fromContentCategoryId, toContentCategoryId);
            else
                return Json(new StatusMessageData(StatusMessageType.Error, "没有删除权限"));
            return Json(new StatusMessageData(StatusMessageType.Success, "交换成功"));
        }

        #endregion

        #endregion

        #region 贴吧

        #region 贴吧管理

        /// <summary>
        /// 贴吧管理页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageSections()
        {
            if (!IsPostManager())
            {
                SystemMessageViewModel systemMessageViewModel = SystemMessageViewModel.NoCompetence();
                TempData["SystemMessageViewModel"] = systemMessageViewModel;
                return Redirect(SiteUrls.Instance().SystemMessage());
            }

            return View();
        }

        /// <summary>
        /// 贴吧列表
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _ListSections(string keyword, int pageSize = 20, int pageIndex = 1)
        {
            //权限 
            if (!IsPostManager())
            {
                SystemMessageViewModel systemMessageViewModel = SystemMessageViewModel.NoCompetence();
                TempData["SystemMessageViewModel"] = systemMessageViewModel;
                return Redirect(SiteUrls.Instance().SystemMessage());
            }

            var barSectionList = sectionService.Gets(TenantTypeIds.Instance().Bar(), new SectionQuery { NameKeyword = keyword }, pageSize, pageIndex);


            return PartialView(barSectionList);
        }

        /// <summary>
        /// 创建、编辑贴吧
        /// </summary>
        /// <param name="sectionId">贴吧ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditBarSection(long? sectionId)
        {
            if (!IsPostManager())
                return Json(new { state = 0, msg = "无权操作" });

            ViewData["Categories"] = new JavaScriptSerializer().Serialize(this.GetCategories());
            SectionEditModel sectionEditModel = new SectionEditModel();
            if (sectionId.HasValue)
            {
                var section = sectionService.Get(sectionId.Value);

                section.MapTo(sectionEditModel);

                if (section.ThreadCategorySettings != ThreadCategoryStatus.Disabled)
                {
                    sectionEditModel.EnabledThreadCategory = true;

                }

                if (section.Category.ParentId == 0)
                {
                    ViewData["rootCategoryId"] = section.Category.CategoryId;
                    ViewData["childrenCategoryId"] = section.Category.CategoryId;
                }
                else
                {
                    ViewData["rootCategoryId"] = section.Category.ParentId;
                    ViewData["childrenCategoryId"] = section.Category.CategoryId;
                }

                ViewData["managers"] = section.SectionManagers.Select(n => n.UserId);

                return PartialView(sectionEditModel);
            }
            return PartialView(sectionEditModel);
        }


        /// <summary>
        /// 创建、编辑贴吧
        /// </summary>
        /// <param name="sectionEditModel">编辑viewmodel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _EditBarSection(SectionEditModel sectionEditModel)
        {
            if (!IsPostManager())
                return Json(new { state = 0, msg = "无权操作" });

            if (sectionEditModel.SectionId == 0)
            {
                //创建

                var section = Section.New();
                section.TenantTypeId = TenantTypeIds.Instance().Bar();
                section.Name = sectionEditModel.Name;
                section.Description = sectionEditModel.Description;
                section.IsEnabled = sectionEditModel.IsEnabled;
                section.UserId = user.UserId;
                section.FeaturedImageAttachmentId = sectionEditModel.FeaturedImageAttachmentId;
                section.ThreadCategorySettings = sectionEditModel.EnabledThreadCategory ? ThreadCategoryStatus.NotForceEnabled : ThreadCategoryStatus.Disabled;

                List<long> manageIds = new List<long>();

                if (sectionEditModel.SectionManagers != null && sectionEditModel.SectionManagers.Any())
                {
                    manageIds = sectionEditModel.SectionManagers.Select(n => long.Parse(n)).ToList();
                    //管理员关注贴吧
                    foreach (var item in manageIds)
                    {
                        favoriteService.Favorite(section.SectionId, item);
                    }
                }
                sectionService.Create(section, manageIds);

                //添加贴吧分类
                categoryService.AddCategoriesToItem(new List<long> { sectionEditModel.CategoryId }, section.SectionId);


                //吧主关注贴吧
                favoriteService.Favorite(section.SectionId, user.UserId);
                //添加贴子分类
                if (sectionEditModel.EnabledThreadCategory)
                {
                    if (!string.IsNullOrEmpty(sectionEditModel.ThreadCategoryNames))
                    {
                        string[] threadCategoriesList = sectionEditModel.ThreadCategoryNames.Split(';');
                        foreach (var categoryname in threadCategoriesList)
                        {
                            var newcategory = Category.New();
                            newcategory.TenantTypeId = TenantTypeIds.Instance().Thread();
                            newcategory.OwnerId = section.SectionId;
                            newcategory.CategoryName = categoryname;
                            categoryService.Create(newcategory);
                        }
                    }
                }
                return Json(new { state = 1, msg = "创建成功" });
            }
            else
            {
                //编辑
                var section = sectionService.Get(sectionEditModel.SectionId);
                //更新前类别和启用状态
                var oldIsEnabled = section.IsEnabled;
                var oldCategory = section.Category;

                section.UserId = long.Parse(sectionEditModel.SectionOwner.SingleOrDefault());
                section.Name = sectionEditModel.Name;
                section.IsEnabled = sectionEditModel.IsEnabled;
                section.Description = sectionEditModel.Description;
                section.FeaturedImageAttachmentId = sectionEditModel.FeaturedImageAttachmentId;


                if (sectionEditModel.EnabledThreadCategory)
                {
                    section.ThreadCategorySettings = ThreadCategoryStatus.NotForceEnabled;
                }
                else
                {
                    section.ThreadCategorySettings = ThreadCategoryStatus.Disabled;
                }

                List<long> manageIds = new List<long>();
                if (sectionEditModel.SectionManagers != null && sectionEditModel.SectionManagers.Any())
                {
                    manageIds = sectionEditModel.SectionManagers.Select(n => long.Parse(n)).ToList();
                }
                //吧主关注贴吧
                favoriteService.Favorite(section.SectionId, section.UserId);
                //管理员关注贴吧
                foreach (var item in manageIds)
                {
                    favoriteService.Favorite(section.SectionId, item);
                }

                //更新
                sectionService.Update(section, manageIds);
                //删除贴吧类别
                categoryService.DeleteItemInCategory(section.Category.CategoryId, section.SectionId);
                //添加贴吧类别
                categoryService.AddCategoriesToItem(new List<long> { sectionEditModel.CategoryId }, section.SectionId);
                //启用状态变化时更新对应类别计数
                if (oldIsEnabled != sectionEditModel.IsEnabled)
                {
                    if (oldIsEnabled)
                    {
                        oldCategory.ItemCount--;
                        categoryService.Update(oldCategory);
                    }
                    if (sectionEditModel.IsEnabled)
                    {
                        var newCategory = categoryService.Get(sectionEditModel.CategoryId);
                        newCategory.ItemCount++;
                        categoryService.Update(newCategory);
                    }
                }


                //贴子类别
                if (!string.IsNullOrEmpty(sectionEditModel.ThreadCategoryNames) && !string.IsNullOrEmpty(sectionEditModel.ThreadCategoryIds))
                {
                    var threadCategoryNamesList = sectionEditModel.ThreadCategoryNames.Split(';');
                    var threadCategoryIdsList = sectionEditModel.ThreadCategoryIds.Split(';');

                    for (int i = 0; i < threadCategoryIdsList.Count(); i++)
                    {
                        var threadCategoryId = long.Parse(threadCategoryIdsList[i]);
                        if (threadCategoryId == 0)
                        {
                            //添加新贴子分类
                            Category newcategory = Category.New();
                            newcategory.CategoryName = threadCategoryNamesList[i];
                            newcategory.OwnerId = section.SectionId;
                            newcategory.TenantTypeId = TenantTypeIds.Instance().Thread();

                            categoryService.Create(newcategory);
                        }
                        else
                        {
                            //更新前贴吧贴子分类
                            var oldThreadCategory = categoryService.Get(threadCategoryId);

                            //与更新前不一致则更新
                            if (oldThreadCategory.CategoryName != threadCategoryNamesList[i])
                            {
                                oldThreadCategory.CategoryName = threadCategoryNamesList[i];
                                categoryService.Update(oldThreadCategory);
                            }
                        }
                    }
                }

                return Json(new { state = 1, msg = "操作成功" });
            }

        }


        /// <summary>
        /// 删除贴吧
        /// </summary>
        /// <param name="sectionId">贴吧ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSection(long sectionId)
        {
            if (!IsPostManager())
                return Json(new { state = 0, msg = "无权操作" });

            sectionService.Delete(sectionId);

            return Json(new { state = 1, msg = "删除成功" });
        }

        /// <summary>
        /// 推荐贴吧
        /// </summary>
        /// <param name="sectionId">贴吧ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSpecialSection(long sectionId, bool isApproved)
        {
            if (!IsPostManager())
                return Json(new { state = 0, msg = "无权操作" });

            if (isApproved)
            {
                bool result = specialContentitemService.Create(sectionId, TenantTypeIds.Instance().Bar(), SpecialContentTypeIds.Instance().Special(), user.UserId, sectionService.Get(sectionId).Name);

                if (result)
                    return Json(new { state = 1, msg = "推荐成功" });

                return Json(new { state = 0, msg = "推荐失败" });
            }
            else
            {
                specialContentitemService.UnStick(sectionId, TenantTypeIds.Instance().Bar(), SpecialContentTypeIds.Instance().Special());

                return Json(new { state = 1, msg = "取消推荐成功" });
            }

        }

        /// <summary>
        /// 删除贴子分类
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteThreadCategorys(long sectionId, string categoryIds)
        {
            if (!IsPostManager())
                return Json(new { state = 0, msg = "无权操作" });

            foreach (var id in categoryIds.Split(';'))
            {
                categoryService.Delete(long.Parse(id));
            }

            return Json(new { state = 1 });
        }

        /// <summary>
        /// 获取分类级联
        /// </summary>
        /// <returns></returns>
        private List<object> GetCategories()
        {
            var rootCategories = categoryService.GetRootCategoriesOfOwner(TenantTypeIds.Instance().Bar()); ;
            List<object> results = new List<object>();
            results.Add(new
            {
                v = 0,
                n = "所有分类"
            });
            foreach (var item in rootCategories)
            {
                List<object> childrenResults = new List<object>();

                childrenResults.Add(new
                {
                    v = item.CategoryId,
                    n = "所有分类"
                });

                foreach (var childItem in item.Children)
                {
                    childrenResults.Add(new
                    {
                        v = childItem.CategoryId,
                        n = childItem.CategoryName
                    });
                }

                results.Add(new
                {
                    v = item.CategoryId,
                    n = item.CategoryName,
                    s = childrenResults
                });

            }
            return results;
        }

        #endregion

        #region 贴子管理

        /// <summary>
        //贴子管理列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageThreads()
        {
            //如果没有权限 直接跳转
            if (!IsPostManager())
                return RedirectToAction("BankEndError");


            List<SelectListItem> auditStatus = new List<SelectListItem>();
            auditStatus.Add(new SelectListItem() { Text = "全部", Value = string.Empty });
            foreach (AuditStatus item in Enum.GetValues(typeof(AuditStatus)))
            {
                auditStatus.Add(new SelectListItem() { Text = item.GetDisplayName(), Value = item.ToString() });
            }
            ViewData["auditStatus"] = auditStatus;
            var sections = sectionService.Gets(TenantTypeIds.Instance().Bar(), new SectionQuery(), 1000, 1);
            List<SelectListItem> sectionitems = new List<SelectListItem>();
            sectionitems.Add(new SelectListItem() { Text = "全部", Value = string.Empty });
            foreach (var item in sections)
            {
                sectionitems.Add(new SelectListItem() { Text = item.Name, Value = item.SectionId.ToString() });
            }
            ViewData["sectionitems"] = sectionitems;
            return View();
        }

        /// <summary>
        /// 贴子列表页面
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="auditStatus">审核状态</param>
        /// <param name="sectionId">贴吧ID</param>
        /// <param name="startDate">开始时间 </param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageSize">个数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PartialViewResult _ListThreads(string keyword, AuditStatus? auditStatus, long? sectionId, DateTime? startDate, DateTime? endDate, int pageSize = 20, int pageIndex = 1)
        {
            ThreadQuery query = new ThreadQuery();
            query.AuditStatus = auditStatus;
            query.SectionId = sectionId;
            query.StartDate = startDate;
            query.EndDate = endDate;
            query.SubjectKeyword = keyword;
            query.IsSticky = true;
            var threads = threadService.Gets(TenantTypeIds.Instance().Thread(), query, pageSize, pageIndex);

            ViewData["query"] = query;

            return PartialView(threads);
        }

        /// <summary>
        //编辑贴子
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditThread(long threadId)
        {
            //如果没有权限 直接跳转
            if (!IsPostManager())
                return RedirectToAction("BankEndError");
            ThreadEditModel threadEditModel = new ThreadEditModel();
            List<SelectListItem> selectListItems = new List<System.Web.Mvc.SelectListItem>();

            var thread = threadService.Get(threadId);
            if (thread != null)
            {

                var categories = categoryService.GetAll().Where(n => n.TenantTypeId == TenantTypeIds.Instance().Thread());
                var threadCategories = categoryService.GetCategoriesOfItem(thread.ThreadId, null, TenantTypeIds.Instance().Section()).FirstOrDefault();

                thread.MapTo(threadEditModel);
                threadEditModel.Body = thread.GetBody();
                if (threadCategories == null)
                {
                    return View(threadEditModel);
                }
                foreach (var item in categories)
                {
                    var selectListItem = new SelectListItem();
                    selectListItem.Text = item.CategoryName;
                    selectListItem.Value = item.CategoryId.ToString();
                    if (item.CategoryId == threadCategories.CategoryId)
                    {
                        selectListItem.Selected = true;
                        threadEditModel.CategoryId = threadCategories.CategoryId;
                    }

                    selectListItems.Add(selectListItem);
                }
                ViewData["selectListItems"] = selectListItems;

            }



            return View(threadEditModel);
        }
        /// <summary>
        //编辑贴子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditThread(ThreadEditModel threadEditModel)
        {
            //如果没有权限 直接跳转
            if (!IsPostManager())
                return RedirectToAction("BankEndError");
            var thread = threadService.Get(threadEditModel.ThreadId);
            if (thread != null)
            {
                thread.Subject = threadEditModel.Subject;
                thread.Body = threadEditModel.Body;
                threadService.Update(thread, user.UserId, authorizer.IsPostManager(UserContext.CurrentUser));
                if (threadEditModel.CategoryId > 0)
                {
                    categoryService.ClearCategoriesFromItem(threadEditModel.ThreadId, null, TenantTypeIds.Instance().Thread());
                    categoryService.AddCategoriesToItem(new List<long>() { threadEditModel.CategoryId }, threadEditModel.ThreadId);
                }
            }
            return RedirectToAction("ManageThreads");
        }
        /// <summary>
        //删除贴子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteThread(long threadId)
        {
            //如果没有权限 直接跳转
            if (!IsPostManager())
                return Json(new { state = 0, message = "无权操作" });
            var thread = threadService.Get(threadId);
            if (thread != null)
            {
                threadService.Delete(threadId);
                return Json(new { state = 1, message = "删除成功!" });
            }
            return Json(new { state = 0, message = "删除失败" });

        }
        /// <summary>
        //设置加精/取消精华
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Essential(long threadId, bool isApproved)
        {
            //如果没有权限 直接跳转
            if (!IsPostManager())
                return Json(new { state = 0, message = "无权操作" });
            if (isApproved)
                specialContentitemService.Create(threadId, TenantTypeIds.Instance().Thread(), SpecialContentTypeIds.Instance().Essential(), user.UserId, threadService.Get(threadId).Subject);
            else
                specialContentitemService.UnStick(threadId, TenantTypeIds.Instance().Thread(), SpecialContentTypeIds.Instance().Essential());
            return Json(new { state = 1, message = "操作成功" });
        }
        /// <summary>
        //批量删除贴子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteThreads(string threadIds)
        {
            //如果没有权限 直接跳转
            if (!IsPostManager())
            {
                return Json(new { state = 0, message = "无权操作" });
            }
            var threadId = threadIds.Split(new char[] { ',' });

            long id = 0;
            foreach (var item in threadId)
            {
                if (long.TryParse(item, out id))
                    threadService.Delete(id);
            }
            return Json(new { state = 1, message = "删除成功" });
        }
        /// <summary>
        //批量贴子通过/不通过审核
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AuditStatusThreads(string threadIds, bool isApproved)
        {
            // 如果没有权限  则提示 
            if (!IsPostManager())
                return Json(new { state = 0, message = "无权操作" });
            var threadId = threadIds.Split(new char[] { ',' });
            long id = 0;
            foreach (var item in threadId)
            {
                if (long.TryParse(item, out id))
                    threadService.UpdateAuditStatus(id, isApproved);

            }

            return Json(new { state = 1, message = "操作成功" });
        }
        /// <summary>
        //批量贴子加精/取消加精
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateEssential(string threadIds, bool isApproved)
        {
            // 如果没有权限  则提示 
            if (!IsPostManager())
                return Json(new { state = 0, message = "无权操作" });
            var threadId = threadIds.Split(new char[] { ',' });
            long id = 0;
            foreach (var item in threadId)
            {
                if (long.TryParse(item, out id) && isApproved)
                    specialContentitemService.Create(id, TenantTypeIds.Instance().Thread(), SpecialContentTypeIds.Instance().Essential(), user.UserId, threadService.Get(long.Parse(item)).Subject);
                else
                    specialContentitemService.UnStick(id, TenantTypeIds.Instance().Thread(), SpecialContentTypeIds.Instance().Essential());
            }

            return Json(new { state = 1, message = "操作成功" });
        }
        /// <summary>
        //批量贴子置顶/取消置顶
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateSticky(string threadIds, bool isApproved)
        {
            // 如果没有权限  则提示 
            if (!IsPostManager())
                return Json(new { state = 0, message = "无权操作" });
            var threadId = threadIds.Split(new char[] { ',' });
            long id = 0;
            foreach (var item in threadId)
            {
                if (long.TryParse(item, out id))
                {
                    var result = threadService.SetSticky(id, isApproved);
                    if (!result)
                        return Json(new { state = 0, message = $"{id}不存在，操作失败" });
                }

            }

            return Json(new { state = 1, message = "操作成功" });
        }

        /// <summary>
        /// 是否有权限管理贴子
        /// </summary>
        private bool IsPostManager()
        {
            var IsPostManager = authorizer.IsPostManager(UserContext.CurrentUser);
            if (!IsPostManager)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您没有权限访问该页面！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "无权查看",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
            }
            return IsPostManager;
        }

        /// <summary>
        /// 后台400/500错误提示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult BankEndError()
        {
            if (TempData["SystemMessageViewModel"] == null)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您访问的页面不存在！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().ControlPanelHome(),
                    Title = "404",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
            }
            SystemMessageViewModel systemMessageViewModel = TempData["SystemMessageViewModel"] as SystemMessageViewModel;
            ViewData["title"] = systemMessageViewModel.Title;

            return View(systemMessageViewModel);
        }

        #endregion

        #endregion

        #region 公共内容

        #region 推荐内容管理

        /// <summary>
        /// 推荐内容管理页面
        /// </summary>
        /// <param name="belong">推荐内容所属（租户Id）</param>
        /// <param name="typeId">推荐内容类型</param>
        /// <returns></returns>
        public ActionResult ManageSpecialContentItems(string belong, int? typeId)
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
            //推荐内容所属选择列表
            List<SelectListItem> contentTenantList = new List<SelectListItem>();
            contentTenantList.Add(new SelectListItem { Value = "", Text = "全部" });
            foreach (var tenant in tenantTypeService.Gets(MultiTenantServiceKeys.Instance().Recommend()))
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = tenant.Name;
                selectListItem.Value = tenant.TenantTypeId;
                if (tenant.TenantTypeId == belong)
                {
                    selectListItem.Selected = true;
                }
                contentTenantList.Add(selectListItem);
            }

            //推荐内容类型选择列表
            List<SelectListItem> contentTypeList = new List<SelectListItem>();
            contentTypeList.Add(new SelectListItem { Value = "0", Text = "全部" });
            foreach (var specialContentType in specialContentTypeService.GetAll())
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = specialContentType.Name;
                selectListItem.Value = specialContentType.TypeId.ToString();
                if (specialContentType.TypeId == typeId)
                {
                    selectListItem.Selected = true;
                }
                contentTypeList.Add(selectListItem);
            }
            ViewData["specialContentTypeList"] = contentTypeList;
            ViewData["contentTenantList"] = contentTenantList;
            //if (typeId == null)
            //    typeId = null;
            //if (belong == string.Empty || belong == null)
            //    belong = contentTenantList.First().Value;
            ViewData["typeId"] = typeId;
            ViewData["belong"] = belong;
            return View();
        }

        /// <summary>
        /// 推荐内容列表
        /// </summary>
        /// <param name="belong">推荐内容所属租户Id</param>
        /// <param name="typeId">类型Id</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">分页索引</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListSpecialContentItems(string belong, int typeId, int pageSize = 20, int pageIndex = 1)
        {
            PagingDataSet<SpecialContentItem> specialContentItemList = specialContentitemService.Gets(typeId, belong, pageSize, pageIndex);
            if (specialContentItemList.Count != 0)
            {
                if (pageSize * pageIndex >= specialContentItemList.TotalRecords)
                {
                    ViewData["lastContentItemId"] = specialContentItemList.Last().Id;
                }
                if (pageIndex == 1)
                {
                    ViewData["firstContentItemId"] = specialContentItemList.First().Id;
                }
            }
            ViewData["typeId"] = typeId;
            ViewData["belong"] = belong;
            return PartialView(specialContentItemList);
        }

        /// <summary>
        /// 删除推荐内容
        /// </summary>
        /// <param name="id">要删除的推荐内容Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeleteSpecialContentItem(long specialContentItemId)
        {
            if (!IsGlobalContentManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            SpecialContentItem specialContentItem = specialContentitemService.Get(specialContentItemId);
            specialContentitemService.UnStick(specialContentItem.ItemId, specialContentItem.TenantTypeId, specialContentItem.TypeId);
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 交换推荐内容顺序
        /// </summary>
        /// <param name="firstId">第一个推荐内容的Id</param>
        /// <param name="secondId">第二个推荐内容的Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _ChangeSpecialContentOrder(long firstId, long secondId)
        {
            if (!IsGlobalContentManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            specialContentitemService.ChangeOrder(firstId, secondId);
            return Json(new { state = 1 });
        }

        #endregion

        #region 推荐内容类别管理

        /// <summary>
        /// 推荐内容类别管理页
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageSpecialContentTypes()
        {
            if (!IsGlobalContentManager())
            {
                return RedirectToAction("BankEndError");
            }
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "全部", Value = "All" });
            selectList.Add(new SelectListItem { Text = "通用", Value = string.Empty });
            foreach (var tenantType in tenantTypeService.Gets(MultiTenantServiceKeys.Instance().Recommend()))
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = tenantType.Name;
                selectListItem.Value = tenantType.TenantTypeId;
                selectList.Add(selectListItem);
            }
            ViewData["selectList"] = selectList;
            return View();
        }

        /// <summary>
        /// 推荐内容类型列表展示
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _ListSpecialContentTypes(string tenantType)
        {
            if (tenantType != "All")
            {
                return PartialView(specialContentTypeService.GetTypesByTenantType(tenantType).OrderBy(s => s.TypeId));
            }
            else
            {
                return PartialView(specialContentTypeService.GetAll().OrderBy(s => s.TypeId));
            }

        }

        /// <summary>
        /// 编辑/添加推荐内容类型
        /// </summary>
        /// <param name="typeId">推荐内容类型Id</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _EditSpecialContentType(int typeId)
        {
            SpecialContentTypeEditModel specialContentTypeEditModel = new SpecialContentTypeEditModel();
            string selectedTenantType = string.Empty;
            //判断是否为创建
            if (typeId != 0)
            {
                SpecialContentType specialContentType = specialContentTypeService.Get(typeId);
                specialContentType.MapTo(specialContentTypeEditModel);
                specialContentTypeEditModel.isNew = false;
                selectedTenantType = specialContentType.TenantTypeId;
            }
            else
            {
                specialContentTypeEditModel.isNew = true;
            }
            //所属类型选择列表
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "通用", Value = string.Empty });
            foreach (var tenantType in tenantTypeService.Gets(MultiTenantServiceKeys.Instance().Recommend()))
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = tenantType.Name;
                selectListItem.Value = tenantType.TenantTypeId;
                if (tenantType.TenantTypeId == selectedTenantType)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            ViewData["selectList"] = selectList;
            return PartialView(specialContentTypeEditModel);
        }

        /// <summary>
        /// 编辑、添加推荐类别(Post)
        /// </summary>
        /// <param name="specialContentTypeEditModel">推荐类型模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditSpecialContentType(SpecialContentTypeEditModel specialContentTypeEditModel)
        {
            SpecialContentType specialContentType = new SpecialContentType();
            if (!IsGlobalContentManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            if (specialContentTypeEditModel.TypeId != null)
            {
                if (!specialContentTypeEditModel.RequireFeaturedImage)
                    specialContentTypeEditModel.FeaturedImageDescrption = string.Empty;
                if (specialContentTypeEditModel.TenantTypeId == null)
                    specialContentTypeEditModel.TenantTypeId = string.Empty;
                if (specialContentTypeEditModel.FeaturedImageDescrption == null)
                    specialContentTypeEditModel.FeaturedImageDescrption = string.Empty;
                //是否为新建
                if (specialContentTypeEditModel.isNew)
                {
                    specialContentTypeEditModel.MapTo(specialContentType);
                    bool result = specialContentTypeService.Create(specialContentType);
                    if (result)
                    {
                        return Json(new { state = 1, successmsg = "创建类别成功" });
                    }
                    else
                    {
                        return Json(new { state = 0, errormsg = "创建类别失败" });
                    }
                }
                else
                {
                    var specialcontenttype = specialContentTypeService.Get((int)specialContentTypeEditModel.TypeId);
                    if (specialcontenttype != null)
                    {
                        specialContentTypeEditModel.MapTo(specialcontenttype);
                        specialContentTypeService.Update(specialcontenttype);
                        return Json(new { state = 1, successmsg = "修改类别成功" });
                    }
                    else
                    {
                        return Json(new { state = 0, errormsg = "修改类别失败" });
                    }
                }
            }
            else
            {
                return Json(new { state = 0, errormsg = "类别Id不允许为空" });
            }
        }

        /// <summary>
        /// 删除推荐内容类型
        /// </summary>
        /// <param name="typeId">推荐内容类型Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeleteSpecialContentType(int typeId)
        {
            if (!IsGlobalContentManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            if (specialContentTypeService.Get(typeId).IsBuiltIn)
            {
                return Json(new { state = 0 });
            }
            else
            {
                if (typeId != 0)
                {
                    if (specialContentTypeService.Delete(typeId) != 0)
                        return Json(new { state = 1 });
                    else
                        return Json(new { state = 0 });
                }
                else
                {
                    return Json(new { state = 0 });
                }
            }

        }

        #region 验证

        /// <summary>
        /// 验证推荐内容类型是否重复（用于添加内容推荐类型时）
        /// </summary>
        /// <param name="typeId">添加时填写的类型Id</param>
        /// <returns></returns>
        public JsonResult CheckUniqueType(int typeId)
        {
            var specialContentType = specialContentTypeService.Get(typeId);
            if (specialContentType != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #endregion

        #region 评论管理

        /// <summary>
        /// 评论管理主页
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageComments()
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
            //审核下拉列表项
            List<SelectListItem> auditStatus = new List<SelectListItem>();
            auditStatus.Add(new SelectListItem() { Text = "全部", Value = string.Empty });
            foreach (AuditStatus item in Enum.GetValues(typeof(AuditStatus)))
            {
                auditStatus.Add(new SelectListItem() { Text = item.GetDisplayName(), Value = item.ToString() });
            }

            //所属下拉列表项
            List<SelectListItem> tenantTypes = new List<SelectListItem>();
            var commentTenantTypes = tenantTypeService.Gets(MultiTenantServiceKeys.Instance().Comment());
            tenantTypes.Add(new SelectListItem { Text = "全部", Value = "" });
            foreach (var item in commentTenantTypes)
            {
                tenantTypes.Add(new SelectListItem { Text = item.Name, Value = item.TenantTypeId });
            }

            ViewData["auditStatus"] = auditStatus;
            ViewData["tenantTypes"] = tenantTypes;

            return View();


        }

        /// <summary>
        /// 评论管理列表
        /// </summary>
        /// <param name="publicAuditStatus">显示的审核状态</param>
        /// <param name="keyword">关键词</param>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型</param>
        /// <param name="minDate">起始日期</param>
        /// <param name="maxDate">结束日期</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns>分页评论</returns>
        public ActionResult _ListComments(PubliclyAuditStatus? publicAuditStatus, string keyword = null, long userId = 0, string tenantTypeId = null, DateTime? minDate = null, DateTime? maxDate = null, int pageSize = 20, int pageIndex = 1)
        {
            if (IsGlobalContentManager())
            {
                var url = ConfigurationManager.AppSettings["Search"];

                PagingDataSet<Comment> comments = new PagingDataSet<Comment>(new List<Comment>());

                if (string.IsNullOrWhiteSpace(url))
                {
                    return PartialView(comments);
                }

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    comments = commentService.GetComments(tenantTypeId, publicAuditStatus, userId, minDate, maxDate, pageSize, pageIndex);
                }
                else
                {
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                       new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        HttpResponseMessage response = httpClient.GetAsync(string.Format("{0}/Api/SearchApi/CommentSearch?publicAuditStatus={1}&keyword={2}&tenantTypeId={3}&minDate={4}&maxDate={5}&pageSize={6}&pageIndex={7}", url, publicAuditStatus, keyword, tenantTypeId, minDate, maxDate, pageSize, pageIndex)).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var results = JsonConvert.DeserializeObject<SearchResultIdModel>(response.Content.ReadAsStringAsync().Result);

                            comments = new PagingDataSet<Comment>(commentService.Gets(results.Data))
                            {
                                PageSize = results.Page.PageSize,
                                PageIndex = results.Page.PageIndex,
                                TotalRecords = results.Page.TotalRecords
                            };
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                //前台页面通过查字典将租户Id转换为租户名称
                Dictionary<string, string> tenantTypes = new Dictionary<string, string>();
                var commentTenantTypes = tenantTypeService.Gets(MultiTenantServiceKeys.Instance().Comment());
                foreach (var item in commentTenantTypes)
                {
                    tenantTypes.Add(item.TenantTypeId, item.Name);
                }

                ViewData["tenantTypes"] = tenantTypes;

                return PartialView(comments);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }


        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="commentIds">用";"拼接的评论Id</param>
        /// <param name="isApproved">是否通过审核</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult UpdateComments(string commentIds, bool isApproved)
        {
            if (IsGlobalContentManager())
            {
                if (!string.IsNullOrEmpty(commentIds))
                {
                    int updateNum = 0;
                    foreach (var commentId in commentIds.Split(';'))
                    {
                        commentService.UpdateAuditStatus(long.Parse(commentId), isApproved);
                        updateNum++;
                      
                    }

                    return Json(new StatusMessageData(StatusMessageType.Success, "成功更改" + updateNum + "条评论的审核状态"));
                }

                return Json(new StatusMessageData(StatusMessageType.Error, "更改失败"));
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Error, "无权操作"));
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="commentIds">用";"拼接的评论Id</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult DeleteComments(string commentIds)
        {
            if (IsGlobalContentManager())
            {
                if (!string.IsNullOrEmpty(commentIds))
                {
                    int deletedNum = 0;
                    foreach (var item in commentIds.Split(';'))
                    {
                        if (commentService.Delete(long.Parse(item)))
                            deletedNum++;
                    }

                    return Json(new StatusMessageData(StatusMessageType.Success, "成功删除" + deletedNum + "条评论"));
                }

                return Json(new StatusMessageData(StatusMessageType.Error, "批量删除失败"));
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Error, "无权操作"));
            }
        }


        #endregion

        #region 类别管理

        /// <summary>
        /// 类别管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageCategories(string tenantTypeId)
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
            //所属下拉列表
            List<SelectListItem> tenantTypes = new List<SelectListItem>();
            var categoryTenantTypes = tenantTypeService.Gets(MultiTenantServiceKeys.Instance().Category());
            foreach (var item in categoryTenantTypes)
            {
                if (tenantTypeId != null && item.TenantTypeId == tenantTypeId)
                    tenantTypes.Add(new SelectListItem { Text = item.Name, Value = item.TenantTypeId, Selected = true });
                else
                    tenantTypes.Add(new SelectListItem { Text = item.Name, Value = item.TenantTypeId });
            }

            ViewData["tenantTypeIds"] = tenantTypes;

            return View();

        }

        /// <summary>
        /// 类别列表
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _ListCategories(string tenantTypeId = null)
        {
            if (IsGlobalContentManager())
            {
                IEnumerable<Category> categories;
                if (string.IsNullOrEmpty(tenantTypeId))
                {
                    categories = categoryService.GetOwnerCategories(0, "");
                }
                else
                {
                    categories = categoryService.GetOwnerCategories(0, tenantTypeId);
                }

                return PartialView(categories);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }

        /// <summary>
        /// 添加/编辑类别GET
        /// </summary>
        /// <param name="categoryId">类别Id</param>
        /// <param name="parentId">父类别Id</param>
        /// <param name="tenantTypeId">租户Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditCategory(long categoryId = 0, long parentId = 0, string tenantTypeId = null)
        {
            if (IsGlobalContentManager())
            {
                Category category = Category.New();
                CategoryEditModel categoryEditModel = new CategoryEditModel();
                if (!string.IsNullOrEmpty(tenantTypeId))
                {
                    //categoryId>0为编辑,=0添加类别
                    if (categoryId > 0)
                    {
                        category = categoryService.Get(categoryId);
                        if (category != null)
                        {
                            category.MapTo(categoryEditModel);
                        }
                    }
                    else
                    {
                        //parentId=0添加类别,>0添加子类别
                        if (parentId > 0)
                        {
                            var parentCategory = categoryService.Get(parentId);
                            if (parentCategory != null)
                            {
                                categoryEditModel.ParentId = parentId;
                            }
                        }
                    }
                }

                categoryEditModel.Description = Formatter.FormatMultiLinePlainTextForEdit(categoryEditModel.Description, true);
                categoryEditModel.TenantTypeId = tenantTypeId;
                return View(categoryEditModel);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }

        /// <summary>
        /// 添加/编辑类别POST
        /// </summary>
        /// <param name="categoryEditModel">类别编辑视图模型</param>
        /// <returns>Json 状态</returns>
        [HttpPost]
        public JsonResult _EditCategory(CategoryEditModel categoryEditModel)
        {
            if (IsGlobalContentManager())
            {
                if (!(string.IsNullOrEmpty(categoryEditModel.TenantTypeId) || string.IsNullOrWhiteSpace(categoryEditModel.CategoryName)))
                {
                    Category category = categoryService.Get(categoryEditModel.CategoryId) ?? Category.New();

                    //为类别对象属性赋值
                    category.CategoryName = categoryEditModel.CategoryName;

                    if (!string.IsNullOrWhiteSpace(categoryEditModel.Description))
                    {
                        category.Description = categoryEditModel.Description;
                    }

                    //CategoryId=0添加,>0编辑
                    if (categoryEditModel.CategoryId > 0)
                    {
                        category.LastModified = DateTime.Now;

                        categoryService.Update(category);

                        return Json(new StatusMessageData(StatusMessageType.Success, "编辑成功"));
                    }
                    else
                    {
                        category.TenantTypeId = categoryEditModel.TenantTypeId;
                        category.ParentId = categoryEditModel.ParentId;

                        //parentId=0添加类别,>0添加子类别
                        if (categoryEditModel.ParentId > 0)
                        {
                            var parentCategory = categoryService.Get(categoryEditModel.ParentId);
                            category.Depth = parentCategory.Depth + 1;
                        }

                        categoryService.Create(category);
                        return Json(new StatusMessageData(StatusMessageType.Success, "添加成功"));
                    }
                }
                else
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "添加失败"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }
        }


        /// <summary>
        /// 删除类别/删除父类别会同时删除子类别
        /// </summary>
        /// <param name="categoryId">类别Id</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult DeleteCategory(long categoryId)
        {
            if (IsGlobalContentManager())
            {
                var isDeleted = categoryService.Delete(categoryId);

                if (isDeleted)
                {
                    return Json(new StatusMessageData(StatusMessageType.Success, "删除成功"));
                }
                else
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "删除失败"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }
        }


        /// <summary>
        /// 上移下移
        /// </summary>
        /// <param name="fromCategoryId">初始位置</param>
        /// <param name="toCategoryId">目标位置</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult ChangeCategoryOrder(long fromCategoryId, long toCategoryId)
        {
            if (IsGlobalContentManager())
            {
                var fromCategory = categoryService.Get(fromCategoryId);
                var toCategory = categoryService.Get(toCategoryId);
                long midCategoryOrder = fromCategory.DisplayOrder;

                fromCategory.DisplayOrder = toCategory.DisplayOrder;
                categoryService.Update(fromCategory);

                toCategory.DisplayOrder = midCategoryOrder;
                categoryService.Update(toCategory);

                return Json(new StatusMessageData(StatusMessageType.Success, "移动成功"));
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }
        }


        #endregion

        #region 标签管理

        /// <summary>
        /// 标签管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageTags()
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
             List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var tenantType in tenantTypeService.Gets(MultiTenantServiceKeys.Instance().Tag()))
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = tenantType.Name;
                selectListItem.Value = tenantType.TenantTypeId;
                if (tenantType.TenantTypeId == TenantTypeIds.Instance().ContentItem())
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return View(selectList);
        }

        /// <summary>
        /// 标签列表
        /// </summary>
        /// <param name="keyWord">标签关键字</param>
        /// <param name="tenantTypeId">标签所属租户</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListTags(string keyWord, string tenantTypeId, int pageSize = 20, int pageIndex = 1)
        {
            TagQuery tagQuery = new TagQuery();
            tagQuery.Keyword = keyWord;
            tagQuery.TenantTypeId = tenantTypeId;
            var tags = tagService.GetTags(tagQuery, pageIndex, pageSize);
            return PartialView(tags);
        }

        /// <summary>
        /// 创建、编辑标签
        /// </summary>
        /// <param name="tenantTypeId">标签所属租户</param>
        /// <param name="tagId">标签ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditTag(string tenantTypeId, long tagId = 0)
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
            Tag tag = Tag.New();
            TagEditModel tagEditModel = new TagEditModel();
            tag.TenantTypeId = tenantTypeId;
            if (string.IsNullOrEmpty(tenantTypeId))
                tag = tagService.Get(tagId);
            tag.MapTo(tagEditModel);
            return PartialView(tagEditModel);
        }

        /// <summary>
        ///  创建、编辑标签
        /// </summary>
        /// <param name="tagEditModel">标签编辑实体</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _EditTag(TagEditModel tagEditModel)
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
            bool type = false;
            Tag tag = Tag.New();
            if (string.IsNullOrEmpty(tagEditModel.Description))
                tagEditModel.Description = "";
            tagEditModel.MapTo(tag);
            if (tagEditModel.TagId > 0)
            {
                var oldTag = tagService.Get(tagEditModel.TagId);
                var tagSel = tagService.Get(tagEditModel.TagName);
                if (tagSel != null && tagSel != oldTag && tagSel.TenantTypeId == tagEditModel.TenantTypeId)
                    return Json(new { type = "exist" });
                if (oldTag != null)
                {
                    if (oldTag != null && oldTag.ImageAttachmentId != tag.ImageAttachmentId)
                        new AttachmentService(TenantTypeIds.Instance().Tag()).Delete(oldTag.ImageAttachmentId);
                    tagService.Update(tag);
                    type = true;
                }
            }
            else
            {
                var tagSel = tagService.Get(tagEditModel.TagName);
                if (tagSel != null && tagSel.TenantTypeId == tagEditModel.TenantTypeId)
                    return Json(new { type = "exist" });
                type = tagService.Create(tag);
            }
            return Json(new { type = type });
        }
        /// <summary>
        /// 删除标签
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteTag(long tagId)
        {
            if (!IsGlobalContentManager())
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            var tag = tagService.Get(tagId);
            if (tag != null)
            {
                tagService.Delete(tagId);
                return Json(new StatusMessageData(StatusMessageType.Success, "删除成功"));
            }
            else
                return Json(new StatusMessageData(StatusMessageType.Error, "删除失败"));
        }
        #endregion

        #region 链接管理
        /// <summary>
        /// 链接管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageLinks()
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
            var categories = categoryService.GetRootCategoriesOfOwner(TenantTypeIds.Instance().Link());
            List<SelectListItem> linkTypes = new List<SelectListItem>();
            foreach (var item in categories)
            {
                linkTypes.Add(new SelectListItem { Text = item.CategoryName, Value = item.CategoryId.ToString() });
            }
            return View(linkTypes);
        }

        /// <summary>
        /// 链接列表
        /// </summary>
        /// <param name="categoryId">链接类别ID</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListLinks(long? categoryId)
        {
            var ListLink = linkService.GetsOfSiteForAdmin(categoryId).OrderBy(n => n.DisplayOrder).ToList();
            ViewData["categoryId"] = categoryId;
            return PartialView(ListLink);
        }

        /// <summary>
        /// 添加/编辑链接
        /// </summary>
        /// <param name="linkId">链接ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditLink(long linkId = 0)
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
            LinkEditModel linkEditModel = new LinkEditModel();
            linkEditModel.IsEnabled = true;
            var categoryId = 0L;
            var typeLinks = categoryService.GetRootCategoriesOfOwner(TenantTypeIds.Instance().Link());
            var linkTypes = new List<SelectListItem>();
            if (linkId > 0)
            {
                var link = linkService.Get(linkId);
                if (link != null)
                {
                    categoryId = link.Categories.Any() ? link.Categories.First().CategoryId : 0L;
                    link.MapTo(linkEditModel);
                    linkEditModel.CategoryId = categoryId;
                }
            }
            linkTypes.Add(new SelectListItem { Text = "请选择", Value = "" });
            foreach (var item in typeLinks)
            {
                linkTypes.Add(new SelectListItem { Text = item.CategoryName, Value = item.CategoryId.ToString() });
            }
            SelectList linkTypeSels = new SelectList(linkTypes, "Value", "Text", categoryId);

            ViewData["linkTypeSels"] = linkTypeSels;
            return PartialView(linkEditModel);
        }

        /// <summary>
        /// 添加/编辑链接
        /// </summary>
        /// <param name="linkEditModel">链接编辑实体</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _EditLink(LinkEditModel linkEditModel)
        {
            if (!IsGlobalContentManager())
                return RedirectToAction("BankEndError");
            bool type = false;
            LinkEntity linkEntity = new LinkEntity();
            if (linkEditModel.LinkId > 0)
            {
                linkEntity = linkService.Get(linkEditModel.LinkId);
                if (linkEntity != null)
                {
                    if (linkEntity.ImageAttachmentId != linkEditModel.ImageAttachmentId)
                        new AttachmentService(TenantTypeIds.Instance().Link()).Delete(linkEntity.ImageAttachmentId);
                    linkEditModel.MapTo(linkEntity);
                    if (string.IsNullOrEmpty(linkEntity.Description))
                        linkEntity.Description = "";
                    linkService.Update(linkEntity);
                    categoryService.MoveItemsToCategory(new List<long> { linkEntity.LinkId }, linkEditModel.CategoryId, 0, TenantTypeIds.Instance().Link());
                    type = true;
                }
            }
            else
            {
                linkEditModel.MapTo(linkEntity);
                if (string.IsNullOrEmpty(linkEntity.Description))
                    linkEntity.Description = "";
                linkEntity.DateCreated = DateTime.Now;
                type = linkService.Create(linkEntity);
                categoryService.AddCategoriesToItem(new List<long> { linkEditModel.CategoryId }, linkEntity.LinkId);
            }
            return Json(new { type = type });
        }

        /// <summary>
        /// 删除链接
        /// </summary>
        /// <param name="linkId">链接ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteLink(long linkId)
        {
            if (!IsGlobalContentManager())
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            var link = linkService.Get(linkId);
            if (link != null)
            {
                linkService.Delete(link);
                return Json(new StatusMessageData(StatusMessageType.Success, "删除成功"));
            }
            else
                return Json(new StatusMessageData(StatusMessageType.Error, "删除失败"));
        }

        /// <summary>
        /// 更改链接顺序
        /// </summary>
        /// <param name="fromLinkId">当前链接ID</param>
        /// <param name="toLinkId">指定位置链接ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangeLinkOrder(long fromLinkId, long toLinkId)
        {
            if (!IsGlobalContentManager())
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            linkService.ChangeLinkOrder(fromLinkId, toLinkId);
            return Json(new StatusMessageData(StatusMessageType.Success, ""));
        }
        #endregion

        #region 广告管理

        /// <summary>
        /// 广告管理页
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageAdvertising(long? positionId = 0)
        {
            if (!IsGlobalContentManager())
            {
                return RedirectToAction("BankEndError");
            }
            if (positionId != 0)
            {
                ViewData["PositionId"] = positionId;
            }
            List<SelectListItem> positionSelectList = new List<SelectListItem>();
            positionSelectList.Add(new SelectListItem { Text = "广告投放位置", Value = "" });
            foreach (var item in advertisingService.GetPositionsForAdmin())
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = item.Description;
                selectListItem.Value = item.PositionId.ToString();
                if (item.PositionId == positionId)
                {
                    selectListItem.Selected = true;
                }
                positionSelectList.Add(selectListItem);
            }
            ViewData["positionSelectList"] = positionSelectList;
            return View();
        }

        /// <summary>
        /// 广告列表
        /// </summary>
        /// <param name="positionId">广告位Id</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListAdvertising(long? positionId = 0)
        {

            IEnumerable<Advertising> advertisingList = new List<Advertising>();
            ViewData["PositionId"] = positionId;
            advertisingList = advertisingService.GetAdvertisingsForAdmin(null, positionId, null, null, null, null);
            return PartialView(advertisingList);
        }

        /// <summary>
        /// 编辑、添加广告（新开页面）
        /// </summary>
        /// <param name="advertisingId">广告Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditAdvertising(long? advertisingId)
        {
            if (!IsGlobalContentManager())
            {
                return RedirectToAction("BankEndError");
            }
            List<SelectListItem> sizeSelectList = new List<SelectListItem>();
            //建议尺寸下拉列表
            sizeSelectList.Add(new SelectListItem { Text = "尺寸", Value = "" });
            foreach (var size in advertisingService.GetAllPositionSize())
            {
                string[] widthHeight = size.Split('*');
                if (widthHeight[0] == "0")
                {
                    widthHeight[0] = "不限";
                }
                if (widthHeight[1] == "0")
                {
                    widthHeight[1] = "不限";
                }
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = widthHeight[0] + "*" + widthHeight[1];
                selectListItem.Value = size;
                sizeSelectList.Add(selectListItem);
            }
            ViewData["sizeSelectList"] = sizeSelectList;
            //全部广告位
            List<AdvertisingPosition> allPositions = (List<AdvertisingPosition>)advertisingService.GetPositionsForAdmin();
            //全部广告位编辑模型
            List<AdvertisingPositionEditModel> allEditPostions = new List<AdvertisingPositionEditModel>();
            //选中的广告位
            List<AdvertisingPosition> selectedPositions = new List<AdvertisingPosition>();
            AdvertisingEditModel advertisingEdit = new AdvertisingEditModel();
            Advertising advertising = Advertising.New();
            //编辑
            if (advertisingId != null)
            {
                advertising = advertisingService.GetAdvertising((long)advertisingId);
                selectedPositions = (List<AdvertisingPosition>)advertisingService.GetPositionsByAdvertisingId((long)advertisingId);
            }
            foreach (var item in allPositions)
            {
                AdvertisingPositionEditModel advertisingPositionEditModel = new AdvertisingPositionEditModel();
                //映射模型
                item.MapTo(advertisingPositionEditModel);
                if (selectedPositions.Where(p => p.PositionId == item.PositionId).Count() > 0)
                    advertisingPositionEditModel.IsChecked = true;
                else
                    advertisingPositionEditModel.IsChecked = false;
                //添加到编辑模型
                allEditPostions.Add(advertisingPositionEditModel);
            }
            advertising.MapTo(advertisingEdit);
            advertisingEdit.positionList = allEditPostions;
            return View(advertisingEdit);
        }

        [HttpPost]
        public JsonResult _EditAdvertising(AdvertisingEditModel advertisingEditModel)
        {
            if (!IsGlobalContentManager())
            {
                return Json(new { state = 0, errormsg = "无权操作！" });
            }
            //取出选中位置
            IEnumerable<long> positionIds = advertisingEditModel.positionList.Where(p => p.IsChecked == true).Select(m => m.PositionId);
            if (advertisingEditModel.AdvertisingType == AdvertisingType.Image)
            {
                advertisingEditModel.Body = "";
            }
            else
            {
                advertisingEditModel.ImageAttachmentId = 0;
                advertisingEditModel.LinkUrl = "";
            }
            Advertising advertising = new Advertising();
            //创建
            if (advertisingEditModel.AdvertisingId == 0)
            {
                advertisingEditModel.DateCreated = DateTime.Now;
                advertisingEditModel.MapTo(advertising);
                if (advertisingService.CreateAdvertising(advertising, positionIds))
                {

                    return Json(new { state = 1, successmsg = "创建广告成功！" });
                }
                else
                {
                    return Json(new { state = 0, errormsg = "创建广告失败！" });
                }
            }
            //修改
            else
            {
                advertisingEditModel.MapTo(advertising);
                advertisingService.UpdateAdvertising(advertising, positionIds);

                return Json(new { state = 1, successmsg = "编辑广告成功！" });
            }
        }

        /// <summary>
        /// 删除广告
        /// </summary>
        /// <param name="advertisingId">广告Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeleteAdvertising(long advertisingId)
        {
            if (!IsGlobalContentManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            if (advertisingService.DeleteAdvertising(advertisingId))
            {
                return Json(new { state = 1, successmsg = "删除成功!" });
            }
            return Json(new { state = 0, errormsg = "删除失败!" });

        }
        #endregion

        #region 广告位管理

        /// <summary>
        /// 广告位管理页
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageAdvertisingPosition()
        {
            if (!IsGlobalContentManager())
            {
                return RedirectToAction("BankEndError");
            }
            List<SelectListItem> sizeSelectList = new List<SelectListItem>();
            //建议尺寸下拉列表
            sizeSelectList.Add(new SelectListItem { Text = "建议尺寸", Value = "" });
            foreach (var size in advertisingService.GetAllPositionSize())
            {
                SelectListItem selectListItem = new SelectListItem();
                string[] widthHeight = size.Split('*');
                if (widthHeight[0] == "0")
                {
                    widthHeight[0] = "不限";
                }
                if (widthHeight[1] == "0")
                {
                    widthHeight[1] = "不限";
                }
                selectListItem.Text = widthHeight[0] + "*" + widthHeight[1];
                selectListItem.Value = size;
                sizeSelectList.Add(selectListItem);
            }
            ViewData["sizeSelectList"] = sizeSelectList;

            return View();
        }

        /// <summary>
        /// 广告位列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListPositions(int? height, int? width)
        {
            ViewData["height"] = height;
            ViewData["width"] = width;
            IEnumerable<AdvertisingPosition> positionList;
            positionList = advertisingService.GetPositionsForAdmin(height, width);
            return PartialView(positionList);
        }

        /// <summary>
        /// 编辑、添加广告位
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _EditPosition(long? positionId = 0)
        {
            AdvertisingPosition advertisingPosition;
            bool isNew = false;
            //编辑
            if (positionId > 0)
            {
                advertisingPosition = advertisingService.GetPosition(positionId.Value);
            }
            //添加
            else
            {
                advertisingPosition = AdvertisingPosition.New();
                isNew = true;
            }
            AdvertisingPositionEditModel advertisingPositionEdit = new AdvertisingPositionEditModel();
            advertisingPosition.MapTo(advertisingPositionEdit);
            advertisingPositionEdit.IsNew = isNew;
            return PartialView(advertisingPositionEdit);
        }

        /// <summary>
        /// 修改提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditPosition(AdvertisingPositionEditModel advertisingPositionEdit)
        {
            if (!IsGlobalContentManager())
            {
                return Json(new { state = 0, msg = "无权操作!" });
            }

            var regex = new Regex("^[1-9][0-9]{1,7}$");
            var m = regex.Match(advertisingPositionEdit.PositionId.ToString());
            if (!m.Success)
            {
                return Json(new { state = 0, msg = "编码格式不正确!" });
            }
            AdvertisingPosition advertisingPosition = new AdvertisingPosition();
            advertisingPositionEdit.MapTo<AdvertisingPositionEditModel, AdvertisingPosition>(advertisingPosition);
            //修改
            if (!advertisingPositionEdit.IsNew)
            {
                advertisingService.UpdatePosition(advertisingPosition);

                return Json(new { state = 1, msg = "修改广告位成功!" });
            }
            //创建
            else
            {
                //判断positionId是否唯一
                if (!CheckUniquePositionId(advertisingPositionEdit.PositionId))
                {
                    return Json(new { state = 0, msg = "广告位编码已存在!" });
                }
                if (advertisingService.CreatePosition(advertisingPosition))
                {

                    return Json(new { state = 1, msg = "创建广告位成功!" });
                }
                else
                {
                    return Json(new { state = 0, msg = "创建广告位失败!" });
                }
            }
        }

        /// <summary>
        /// 删除广告位
        /// </summary>
        /// <param name="positionId">广告位Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeletePosition(long positionId)
        {
            if (!IsGlobalContentManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            AdvertisingPosition position = advertisingService.GetPosition(positionId);
            advertisingService.DeletePosition(positionId);

            return Json(new { state = 1, successmsg = "删除广告位成功!" });
        }

        /// <summary>
        /// 验证广告位Id是否重复
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public bool CheckUniquePositionId(long positionId)
        {
            var position = advertisingService.GetPosition(positionId);
            if (position != null)
            {
                return false;
            }
            return true;
        }
        #endregion

        #endregion

        #region 用户

        #region 用户管理

        /// <summary>
        /// 用户管理页
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageUsers()
        {
            if (!IsUserManager())
            {
                return RedirectToAction("BankEndError");
            }
            List<SelectListItem> roleSelectList = new List<SelectListItem>();
            foreach (var item in roleService.GetRoles())
            {
                SelectListItem selectItem = new SelectListItem();
                selectItem.Value = item.RoleId.ToString();
                selectItem.Text = item.RoleName;
                roleSelectList.Add(selectItem);
            }
            List<SelectListItem> stateList = new List<SelectListItem>();
            stateList.Add(new SelectListItem() { Value = ((int)SelectUserStatus.IsBaned).ToString(), Text = SelectUserStatus.IsBaned.GetDisplayName() });
            stateList.Add(new SelectListItem() { Value = ((int)SelectUserStatus.IsModerated).ToString(), Text = SelectUserStatus.IsModerated.GetDisplayName() });
            ViewData["stateList"] = stateList;
            ViewData["roleSelectList"] = roleSelectList;
            return View();
        }

        /// <summary>
        /// 显示用户列表
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="role">用户角色名</param>
        /// <param name="state">用户状态:Baned,Activated,Moderated</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">页面索引</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListUsers(string keyword, long roleId, int state, DateTime? startDate, DateTime? endDate, int pageSize = 20, int pageIndex = 1)
        {
            UserQuery userQuery = new UserQuery();
            userQuery.Keyword = keyword;
            userQuery.RoleId = roleId;
            userQuery.RegisterTimeLowerLimit = startDate;
            userQuery.RegisterTimeUpperLimit = endDate;
            switch (state)
            {
                case (int)SelectUserStatus.IsBaned:
                    userQuery.IsBanned = true;
                    break;
                case (int)SelectUserStatus.IsModerated:
                    userQuery.IsModerated = true;
                    break;
                default:
                    break;
            }
            ViewData["state"] = state;
            ViewData["query"] = userQuery;
            PagingDataSet<User> userList = userService.GetUsers(userQuery, pageSize, pageIndex);
            return PartialView(userList);
        }

        /// <summary>
        /// 编辑、添加用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _EditUser(long userId)
        {
            UserManageEditModel userEdit = new UserManageEditModel();
            if (userId > 0)
            {
                User user = (User)userService.GetUser(userId);
                user.MapTo<User, UserManageEditModel>(userEdit);
                if (user.IsForceModerated)
                {
                    userEdit.ModerateState = (int)ModerateState.ForceModerated;
                }
                else if (user.IsModerated)
                {
                    userEdit.ModerateState = (int)ModerateState.Moderated;
                }
                else
                {
                    userEdit.ModerateState = (int)ModerateState.NoModerated;
                }
            }
            else
            {
                userId = 0;
            }
            return PartialView(userEdit);
        }

        /// <summary>
        /// 编辑、添加用户Post
        /// </summary>
        /// <param name="userEdit">编辑用户模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditUser(UserManageEditModel userEdit)
        {
            if (!IsUserManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            //验证重复性
            if (userEdit.AccountMobile != null)
            {
                User checkMobileUnique = (User)userService.GetUserByMobile(userEdit.AccountMobile);
                if (checkMobileUnique != null && checkMobileUnique.UserId != userEdit.UserId)
                {
                    return Json(new { state = 0, errormsg = "手机号已存在!" });
                }
            }
            if (userEdit.AccountEmail != null)
            {
                User checkEmailUnique = (User)userService.GetUserByEmail(userEdit.AccountEmail);
                if (checkEmailUnique != null && checkEmailUnique.UserId != userEdit.UserId)
                {
                    return Json(new { state = 0, errormsg = "邮箱已存在!" });
                }
            }
            if (userEdit.UserName != null)
            {
                User checkUserName = (User)userService.GetUser(userEdit.UserName);
                if (checkUserName != null && checkUserName.UserId != userEdit.UserId)
                {
                    return Json(new { state = 0, errormsg = "昵称已存在!" });
                }
            }
            //编辑用户
            if (userEdit.UserId > 0)
            {
                User user = (User)userService.GetUser(userEdit.UserId);
                switch (userEdit.ModerateState)
                {
                    case (int)ModerateState.Moderated:
                        user.IsModerated = true;
                        break;
                    case (int)ModerateState.NoModerated:
                        user.IsModerated = false;
                        user.IsForceModerated = false;
                        break;
                    case (int)ModerateState.ForceModerated:
                        user.IsForceModerated = true;
                        break;
                    default:
                        break;
                }
                if (user.AccountEmail == null && userEdit.AccountEmail != null)
                {
                    user.IsEmailVerified = true;
                }
                if (user.AccountMobile == null && userEdit.AccountMobile != null)
                {
                    user.IsMobileVerified = true;
                }
                user.AccountEmail = userEdit.AccountEmail;
                user.AccountMobile = userEdit.AccountMobile;
                //移除字典中的ID与名字的关联
                UserIdToUserNameDictionary.RemoveUserId(user.UserId);
                UserIdToUserNameDictionary.RemoveUserName(user.UserName);
                user.UserName = userEdit.UserName;
                membershipService.UpdateUser(user);
             

                return Json(new { state = 1, successmsg = "修改用户信息成功!" });
            }
            //添加用户
            else
            {
                UserCreateStatus result;
                IUser newuser = userEdit.GetNewUser();
                membershipService.CreateUser(newuser, userEdit.Password, out result);
                if (result != UserCreateStatus.Created)
                {
                    return Json(new { state = 0, errormsg = "创建用户失败!" });
                }
                return Json(new { state = 1, successmsg = "创建用户成功!" });
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ChangePassword(long userId)
        {
            User user = (User)userService.GetUser(userId);
            UserManageEditModel userManageEditModel = new UserManageEditModel();
            user.MapTo(userManageEditModel);
            ViewData["user"] = user;
            return PartialView(userManageEditModel);
        }

        /// <summary>
        /// 修改密码post
        /// </summary>
        /// <param name="userEdit">用户编辑模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _ChangePassword(UserManageEditModel userEdit)
        {
            if (!IsUserManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            User user = (User)userService.GetUser(userEdit.UserId);
            bool result = membershipService.ResetPassword(user.UserName, userEdit.Password);
            if (result)
            {
                return Json(new { state = 1 });
            }
            else
            {
                return Json(new { state = 0 });
            }
        }

        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _SetRole(long userId)
        {
            User user = (User)userService.GetUser(userId);
            ViewData["UserTureName"] = user.TrueName == string.Empty ? user.UserName : user.TrueName;
            List<long> userRoles = roleService.GetRoleIdsOfUser(userId) as List<long>;
            List<UserInRoleEditModel> userRoleEditList = new List<UserInRoleEditModel>();
            foreach (var item in roleService.GetRoles())
            {
                UserInRoleEditModel userInRoleEdit = new UserInRoleEditModel();
                userInRoleEdit.UserRole = item;
                if (!item.ConnectToUser)
                    continue;
                if (userRoles.Contains(item.RoleId))
                    userInRoleEdit.IsInRole = true;
                else
                    userInRoleEdit.IsInRole = false;
                userRoleEditList.Add(userInRoleEdit);
            }
            return PartialView(userRoleEditList);
        }

        /// <summary>
        /// 设置用户角色post
        /// </summary>
        /// <param name="userRoles">用户角色列表</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _SetRole(List<UserInRoleEditModel> userRoles)
        {
            if (!IsUserManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            long userId;
            long.TryParse(Request["UserId"], out userId);
            List<long> roleIds = new List<long>();
            foreach (var item in userRoles)
                if (item.IsInRole)
                    roleIds.Add(item.UserRole.RoleId);
            roleService.RemoveUserRoles(userId);
            roleService.AddUserToRoles(userId, roleIds);
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 奖惩用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _RewardUser(long userId)
        {
            User user = (User)userService.GetUser(userId);
            ViewData["UserTureName"] = user.TrueName == string.Empty ? user.UserName : user.TrueName;
            RewardEditModel rewardEditModel = new RewardEditModel();
            rewardEditModel.UserId = userId;
            ViewData["experience"] = pointService.GetPointCategory("ExperiencePoints");
            ViewData["trade"] = pointService.GetPointCategory("TradePoints");
            return PartialView(rewardEditModel);
        }

        /// <summary>
        /// 奖惩用户post
        /// </summary>
        /// <param name="rewardEditModel">奖惩用户编辑模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _RewardUser(RewardEditModel rewardEditModel)
        {
            if (!IsUserManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            //判断登录
            var currentUser = UserContext.CurrentUser;
            if (currentUser == null)
            {
                return Json(new { state = 0 });
            }
            User user = (User)userService.GetUser(rewardEditModel.UserId);
            if (user == null)
            {
                return Json(new { state = 0 });
            }
            if (rewardEditModel.ExperiencePoints == 0 && rewardEditModel.TradePoints == 0)
            {
                return Json(new { state = 1 });
            }
            pointService.Reward(user.UserId, currentUser.UserId, (int)rewardEditModel.ExperiencePoints, 0, (int)rewardEditModel.TradePoints, rewardEditModel.Reason);
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 封禁用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _BanUser(long userId)
        {
            BanUserEditModel banUserEditModel = new BanUserEditModel();
            banUserEditModel.UserId = userId;
            return PartialView(banUserEditModel);
        }

        /// <summary>
        /// 封禁用户post
        /// </summary>
        /// <param name="banUserEditModel">封禁用户模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _BanUser(BanUserEditModel banUserEditModel)
        {
            if (!IsUserManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            banUserEditModel.BanDeadline = banUserEditModel.BanDeadline;
            userService.BanUser(banUserEditModel.UserId, banUserEditModel.BanDeadline, banUserEditModel.BanReason);
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 解封用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _UnbanUser(long userId)
        {
            userService.UnbanUser(userId);
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="Ids">用户ID列表</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeleteUser(List<long> Ids)
        {
            if (!IsUserManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            string ErrorIdMessage = "";
            foreach (var item in Ids)
            {
                UserDeleteStatus state = membershipService.DeleteUser(item, UserContext.CurrentUser.UserName);
                if (state != UserDeleteStatus.Deleted)
                {
                    ErrorIdMessage += item + " ";
                }
            }
            if (ErrorIdMessage != string.Empty)
            {
                return Json(new { state = 0, failedMsg = ErrorIdMessage });
            }
            else
            {
                return Json(new { state = 1 });
            }
        }

        #endregion

        #region 角色管理

        public ActionResult ManageRoles()
        {
            if (!IsUserManager())
            {
                return RedirectToAction("BankEndError");
            }
            return View();
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _ListRoles()
        {
            return PartialView(roleService.GetRoles());
        }

        /// <summary>
        /// 添加、编辑角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _EditRole(long roleId)
        {
            RoleEditModel editRole = new RoleEditModel();
            Role role = roleService.Get(roleId);
            if (role != null)
            {
                editRole = role.MapTo<Role, RoleEditModel>(editRole);
            }
            return PartialView(editRole);
        }

        /// <summary>
        /// 添加、编辑角色（post）
        /// </summary>
        /// <param name="roleEdit">编辑角色模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditRole(RoleEditModel roleEdit)
        {
            if (!IsUserManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            if (roleEdit.RoleImageAttachmentId == null || roleEdit.RoleImageAttachmentId == string.Empty)
            {
                roleEdit.RoleImageAttachmentId = "0";
            }
            Role role = roleService.Get(roleEdit.RoleId);
            if (role == null)
            {
                //新建角色判断角色ID重复性
                if (roleService.Get(roleEdit.RoleId) != null)
                {
                    return Json(new { state = 0, errormsg = "角色ID已存在!" });
                }
                role = new Role();
                roleEdit.MapTo(role);
                bool result = roleService.Create(role);
                if (result)
                {

                    return Json(new { state = 1, successmsg = "用户角色创建成功!" });
                }
                else
                {
                    return Json(new { state = 0, errormsg = "用户角色创建失败!" });
                }
            }
            else
            {
                role = new Role();
                roleEdit.MapTo(role);
                roleService.Update(role);
                return Json(new { state = 1, successmsg = "用户角色信息修改成功!" });
            }
        }

        /// <summary>
        /// 检查角色Id重复性
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <returns></returns>
        public JsonResult CheckUniqueRoleId(long roleId)
        {
            var role = roleService.Get(roleId);
            if (role != null || userSettings.Get().NoCreatedRoleIds.Contains(roleId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId">用户角色名称</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeleteRole(long roleId)
        {
            if (!IsUserManager())
            {
                return Json(new { state = 0, errormsg = "无权操作!" });
            }
            Role role = roleService.Get(roleId);
            if (role == null || role.IsBuiltIn)
            {
                return Json(new { state = 0 });
            }
            else
            {
                roleService.Delete(roleId);
                return Json(new { state = 1 });
            }
        }

        #endregion

        #region 等级管理

        /// <summary>
        /// 管理用户等级
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageUserRanks()
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");
            return View();
        }
        /// <summary>
        /// 用户等级列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListUserRanks()
        {
            ViewData["experience"] = pointService.GetPointCategory("ExperiencePoints");
            List<UserRank> listUserRanks = new List<UserRank>();
            listUserRanks.AddRange(userRankService.GetAll().Select(n => n.Value));
            return PartialView(listUserRanks);
        }

        /// <summary>
        ///  创建、编辑用户等级
        /// </summary>
        /// <param name="userRank">用户等级</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditUserRank(int userRank = 0)
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");
            ViewData["experience"] = pointService.GetPointCategory("ExperiencePoints");
            UserRank _userRank = new UserRank();
            UserRankEditModel userRankEditModel = new UserRankEditModel();
            if (userRank > 0)
                _userRank = userRankService.Get(userRank);
            _userRank.MapTo(userRankEditModel);
            userRankEditModel.OldRank = userRank > 0 ? userRank : 0;
            return PartialView(userRankEditModel);
        }

        /// <summary>
        ///  创建、编辑用户等级
        /// </summary>
        /// <param name="userRankEditModel">用户等级编辑实体</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _EditUserRank(UserRankEditModel userRankEditModel)
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");
            bool type = false;
            UserRank userRank = new UserRank();
            userRankEditModel.MapTo(userRank);
            if (userRankEditModel.OldRank > 0)
            {
                userRankService.Update(userRank);
                type = true;
            }
            else
            {
                var _userRank = userRankService.Get(userRankEditModel.Rank);
                if (_userRank != null)
                    return Json(new StatusMessageData(StatusMessageType.Error, "用户等级已存在"));
                type = userRankService.Create(userRank);
            }
            return Json(new { type = type });
        }

        /// <summary>
        /// 删除用户等级
        /// </summary>
        /// <param name="userRank">用户等级</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteUserRank(int userRank)
        {
            if (!IsUserManager())
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            var _userRank = userRankService.Get(userRank);
            if (_userRank != null)
            {
                userRankService.Delete(userRank);
                return Json(new StatusMessageData(StatusMessageType.Success, "删除成功"));
            }
            else
                return Json(new StatusMessageData(StatusMessageType.Error, "删除失败"));
        }

        /// <summary>
        /// 重新计算所有用户等级
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResetAllUserRank()
        {
            if (!IsUserManager())
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            userRankService.ResetAllUser();
            return Json(new StatusMessageData(StatusMessageType.Success, "重置用户等级成功"));
        }
        #endregion

        #region 权限管理
        /// <summary>
        /// 权限管理主页
        /// </summary>
        /// <returns></returns>
        public ActionResult ManagePermissions()
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");
            return View();
        }
        /// <summary>
        /// 权限管理列表
        /// </summary>
        /// <returns></returns>
        public ActionResult _ListPermissions()
        {
            if (IsUserManager())
            {
                //获取所有的用户角色的权限对应
                var permissionsDictionary = permissionService.GetAllPermission();

                //取出所有的角色Id和角色Name组成字典
                var roles = roleService.GetRoles();
                Dictionary<long, string> rolesDictionary = new Dictionary<long, string>();
                foreach (var item in roles)
                {
                    rolesDictionary.Add(item.RoleId, item.RoleName);
                }

                //页面下方的表格中列出所有的权限信息
                var allPermissions = permissionService.GetPermissionItems();

                ViewData["rolesDictionary"] = rolesDictionary;
                ViewData["allPermissions"] = allPermissions;

                return PartialView(permissionsDictionary);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }

        /// <summary>
        /// (添加/编辑)(角色/用户)授权 GET
        /// </summary>
        /// <param name="ownerType">被授权对象类型</param>
        /// <param name="ownerId">被授权对象Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditPermission(OwnerType ownerType, long ownerId = 0)
        {
            if (IsUserManager())
            {
                IEnumerable<Permission> ownerPermission = null;

                //ownerId>0为编辑授权
                if (ownerId > 0)
                {
                    ownerPermission = permissionService.GetPermissionsInUserRole(ownerId, ownerType);
                }

                //根据ownerType选择为角色/用户添加授权
                if (ownerType == OwnerType.Role)
                {
                    List<Permission> rolesHavePermissions = new List<Permission>();

                    //取出所有的角色Id和角色Name组成字典
                    var roles = roleService.GetRoles();
                    Dictionary<long, string> rolesDictionary = new Dictionary<long, string>();

                    foreach (var item in roles)
                    {
                        rolesDictionary.Add(item.RoleId, item.RoleName);
                        var rolesPermissions = permissionService.GetPermissionsInUserRole(item.RoleId, OwnerType.Role);
                        rolesHavePermissions.AddRange(rolesPermissions);
                    }

                    //已经拥有权限的角色
                    ViewData["rolesHavePermissions"] = rolesHavePermissions;
                    //角色Id和角色Name组成的字典
                    ViewData["rolesDictionary"] = rolesDictionary;
                }

                //模态窗中列出所有的权限信息供选择
                ViewData["allPermissions"] = permissionService.GetPermissionItems();

                ViewData["ownerType"] = ownerType;

                return View(ownerPermission);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }


        /// <summary>
        /// (添加/编辑/删除)(角色/用户)授权 POST
        /// </summary>
        /// <param name="permissionItemKeysString">被授权字符串</param>
        /// <param name="ownerIds">被授权对象Id</param>
        /// <param name="ownerType">被授权对象类型</param>
        /// <returns>Json操作状态</returns>
        [HttpPost]
        public JsonResult _EditPermission(string permissionItemKeysString, string ownerIds, OwnerType ownerType)
        {
            if (IsUserManager())
            {
                if (!string.IsNullOrEmpty(ownerIds))
                {
                    IEnumerable<string> permissionItemKeys = new List<string>();
                    if (!string.IsNullOrEmpty(permissionItemKeysString))
                    {
                        //添加/编辑授权
                        //将权限项字符串转换为IEnumerable<string>        
                        List<string> permissionItemKeysList = new List<string>();
                        foreach (var item in permissionItemKeysString.Split(';'))
                        {
                            permissionItemKeysList.Add(item);
                        }
                        permissionItemKeys = permissionItemKeysList;

                        //将ownerIds分解成ownerId
                        foreach (var ownerId in ownerIds.Split(';'))
                        {
                            permissionService.UpdatePermissionsInUserRole(permissionItemKeys, long.Parse(ownerId), ownerType);
                        }

                        return Json(new StatusMessageData(StatusMessageType.Success, "操作成功"));
                    }
                    else
                    {
                        //删除授权
                        var ownerId = ownerIds;
                        permissionService.UpdatePermissionsInUserRole(permissionItemKeys, long.Parse(ownerId), ownerType);

                        var ownerPermission = permissionService.GetPermissionsInUserRole(long.Parse(ownerId), ownerType);

                        if (ownerPermission != null && ownerPermission.Any())
                        {
                            //有些授权无法删除
                            return Json(new StatusMessageData(StatusMessageType.Success, "部分授权被清除,但该" + (ownerType == OwnerType.Role ? "角色" : "用户") + "包含某些无法删除的内置授权"));
                        }
                        else
                        {
                            //用户/角色授权被清空
                            return Json(new StatusMessageData(StatusMessageType.Success, "该" + (ownerType == OwnerType.Role ? "角色" : "用户") + "所有授权已被清除"));
                        }
                    }
                }
                else
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "操作失败"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }

        }

        #endregion

        #region 审核规则

        /// <summary>
        /// 审核规则主页
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageAudits()
        {
            if (authorizationService.IsSuperAdministrator(user))
            {
                return View();
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }

        /// <summary>
        /// 显示/编辑审核规则 (GET)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditAuditRules()
        {
            if (authorizationService.IsSuperAdministrator(user))
            {
                ViewData["publiclyAuditStatus"] = siteSettings.Get().AuditStatus;
                ViewData["enableAudit"] = userSettings.Get().EnableAudit;

                return PartialView();
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }

        /// <summary>
        /// 编辑审核规则 (POST)
        /// </summary>
        /// <param name="enableAudit">是否启用人工审核</param>
        /// <param name="publiclyAuditStatus">哪些审核状态对外显示</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditAuditRules(bool enableAudit, PubliclyAuditStatus publiclyAuditStatus)
        {
            if (authorizationService.IsSuperAdministrator(user))
            {
                //获取和设置人工审核
                var userSetting = userSettings.Get();
                userSetting.EnableAudit = enableAudit;
                userSettings.Save(userSetting);

                //获取和设置哪些审核状态对外显示
                var siteSetting = siteSettings.Get();
                siteSetting.AuditStatus = publiclyAuditStatus;
                siteSettings.Save(siteSetting);

                return Json(new StatusMessageData(StatusMessageType.Success, "操作成功"));
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }
        }


        /// <summary>
        /// 显示/编辑所有用户的审核规则 (GET)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditRolesAuditRules()
        {
            if (authorizationService.IsSuperAdministrator(user))
            {
                //获取所有的审核项
                var auditItems = auditService.GetAll();

                //获取所有用户审核规则
                var auditDictionray = auditService.GetAllAuditItemsInUserRole();

                //获取所有的角色存入ViewData
                var allRolesList = new List<Role>(roleService.GetRoles());
                allRolesList.Add(new Role() { RoleId = 123, RoleName = "受管制用户" });

                ViewData["allRoles"] = allRolesList;
                ViewData["auditItems"] = auditItems;

                return PartialView(auditDictionray);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }

        /// <summary>
        /// 编辑所有用户的审核规则 (POST)
        /// </summary>
        /// <param name="jsonText">Json字符串</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditRolesAuditRules(string jsonText)
        {
            var jsonTextReal = jsonText.Replace('“', '"');

            if (authorizationService.IsSuperAdministrator(user))
            {
                //获取所有的审核项
                var auditItems = auditService.GetAll();

                var auditItemInRolesEditModelList = JsonConvert.DeserializeObject<List<AuditItemInRolesEditModel>>(jsonTextReal);

                if (auditItemInRolesEditModelList != null && auditItemInRolesEditModelList.Any())
                {
                    //待更新的角色审核规则集合
                    IEnumerable<AuditItemInUserRole> auditItemInUserRoles = null;
                    List<AuditItemInUserRole> auditItemInUserRolesList = new List<AuditItemInUserRole>();

                    foreach (var item in auditItemInRolesEditModelList)
                    {

                        //将当前角色的审核严格程度添加到List中
                        AuditItemInUserRole auditItemInUserRole = AuditItemInUserRole.New();
                        auditItemInUserRole.RoleId = item.RoleId;
                        auditItemInUserRole.ItemKey = item.ItemKey;
                        auditItemInUserRole.StrictDegree = item.StrictDegree;
                        auditItemInUserRolesList.Add(auditItemInUserRole);

                    }
                    //给待更新的角色审核规则集合赋值
                    auditItemInUserRoles = auditItemInUserRolesList;

                    //更新角色审核规则
                    auditService.UpdateAuditItemInUserRole(auditItemInUserRoles);

                    return Json(new StatusMessageData(StatusMessageType.Success, "更改角色审核规则成功"));
                }
                else
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "更改角色审核规则失败"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }
        }
        #endregion

        #region 积分规则
        /// <summary>
        /// 积分规则管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManagePointRules()
        {
            if (!IsSiteManager())
                return RedirectToAction("BankEndError");
            return View();
        }
        /// <summary>
        /// 积分规则列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListPointRules()
        {
            var pointsRuleList = pointService.GetPointItems();
            ViewData["experience"] = pointService.GetPointCategory("ExperiencePoints").CategoryName;
            ViewData["trade"] = pointService.GetPointCategory("TradePoints").CategoryName;
            return PartialView(pointsRuleList);
        }

        /// <summary>
        /// 编辑积分规则
        /// </summary>
        /// <param name="itemKey">积分规则名称</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditPointRule(string itemKey)
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");
            var pointRule = new PointItemEditModel();
            ViewData["Experience"] = pointService.GetPointCategory("ExperiencePoints").CategoryName;
            ViewData["Trade"] = pointService.GetPointCategory("TradePoints").CategoryName;
            pointService.GetPointItem(itemKey).MapTo(pointRule);
            return PartialView(pointRule);
        }
        /// <summary>
        /// 编辑积分规则
        /// </summary>
        /// <param name="pointItemEditModel">积分规则编辑实体</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _EditPointRule(PointItemEditModel pointItemEditModel)
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");
            var pointItem = PointItem.New();
            pointItemEditModel.MapTo(pointItem);
            pointService.UpdatePointItem(pointItem);
            return Json(new StatusMessageData(StatusMessageType.Success, "保存成功"));
        }


        #endregion

        #region 第三方登录

        /// <summary>
        /// 第三方登录设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageThirdLogin()
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");
            var accountTypes = accountBindingService.GetAccountTypes();

            return View(accountTypes);
        }

        /// <summary>
        /// 创建&编辑第三方帐号
        /// </summary>
        /// <param name="accountTypeKey">帐号类型</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _EditThirdAccount(string accountTypeKey)
        {
            var accountType = accountBindingService.GetAccountType(accountTypeKey);
            AccountTypeEditModel accountTypeEditModel = new AccountTypeEditModel();

            if (accountType != null)
            {
                accountType.MapTo(accountTypeEditModel);
            }
            else
            {
                accountTypeEditModel.AccountTypeKey = accountTypeKey;
                accountTypeEditModel.IsEnabled = true;
            }

            return PartialView(accountTypeEditModel);
        }

        /// <summary>
        /// 创建&编辑第三方帐号
        /// </summary>
        /// <param name="accountTypeEditModel"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditThirdAccount(AccountTypeEditModel accountTypeEditModel)
        {
            var accountType = accountBindingService.GetAccountType(accountTypeEditModel.AccountTypeKey);
            if (accountType != null)
            {
                accountType.AppKey = accountTypeEditModel.AppKey;
                accountType.AppSecret = accountTypeEditModel.AppSecret;
                accountType.IsEnabled = accountTypeEditModel.IsEnabled;

                accountBindingService.UpdateAccountType(accountType);

                return Json(new { state = 1, msg = "编辑成功" });
            }
            else
            {
                AccountType newAccountType = new AccountType()
                {
                    AccountTypeKey = accountTypeEditModel.AccountTypeKey,
                    ThirdAccountGetterClassType = string.Format("Tunynet.Spacebuilder.{0}AccountGetter,Tunynet.AccountBindings", accountTypeEditModel.AccountTypeKey),
                    AppKey = accountTypeEditModel.AppKey,
                    AppSecret = accountTypeEditModel.AppSecret,
                    IsEnabled = accountTypeEditModel.IsEnabled
                };

                accountBindingService.CreateAccountType(newAccountType);

                return Json(new { state = 1, msg = "创建成功" });
            }
        }

        #endregion

        #region 积分记录
        /// <summary>
        /// 积分记录管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManagePointRecords()
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");
            return View();
        }
        /// <summary>
        /// 积分记录列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListPointRecords(long? userId, DateTime? startDate, DateTime? endDate, int pageSize = 20, int pageIndex = 1)
        {
            ViewData["experience"] = pointService.GetPointCategory("ExperiencePoints").CategoryName;
            ViewData["trade"] = pointService.GetPointCategory("TradePoints").CategoryName;
            var pointsRecordList = pointService.GetPointRecords(userId, null, null, startDate, endDate, pageSize, pageIndex);
            return PartialView(pointsRecordList);
        }
        #endregion

        #region 操作日志管理
        /// <summary>
        /// 操作日志管理界面
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageOperationLogs()
        {
            if (!IsUserManager())
                return RedirectToAction("BankEndError");

            //所属下拉列表框
            List<SelectListItem> tenantTypes = new List<SelectListItem>();
            var logTenantTypes = tenantTypeService.Gets(MultiTenantServiceKeys.Instance().OperationLog());

            tenantTypes.Add(new SelectListItem { Text = "全部", Value = "" });
            foreach (var item in logTenantTypes)
            {
                tenantTypes.Add(new SelectListItem { Text = item.Name, Value = item.TenantTypeId });
            }
            ViewData["tenantTypes"] = tenantTypes;

            return View();

        }

        /// <summary>
        /// 根据条件检索操作日志
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="operationUserRole">操作人角色</param>
        /// <param name="operationUserIds">操作人Id</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="minDate">起始日期</param>
        /// <param name="maxDate">结束日期</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">当前页面页码</param>
        /// <returns>分页数据操作日志</returns>
        [HttpGet]
        public ActionResult _ListOperationLogs(string tenantTypeId, string operationUserRole = "", string operationUserIds = "", string operationType = "",
         DateTime? minDate = null, DateTime? maxDate = null, int pageSize = 20, int pageIndex = 1)
        {


            if (authorizationService.Check(user, PermissionItemKeys.Instance().User()))
            {
                List<long> userIds = new List<long>();

                if (!string.IsNullOrEmpty(operationUserIds))
                {
                    foreach (var item in operationUserIds.Split(';'))
                    {
                        userIds.Add(long.Parse(item));
                    }
                }

                if (maxDate.HasValue)
                {
                    maxDate = maxDate.Value.AddDays(1);
                }
                var operationLogs = operationLogService.GetLogs(tenantTypeId, operationUserRole, userIds, operationType, minDate, maxDate, pageSize, pageIndex);

                //前台页面通过查字典将租户Id转换为租户名称
                Dictionary<string, string> tenantTypes = new Dictionary<string, string>();
                var logTenantTypes = tenantTypeService.Gets(MultiTenantServiceKeys.Instance().OperationLog());
                foreach (var item in logTenantTypes)
                {
                    tenantTypes.Add(item.TenantTypeId, item.Name);
                }
                ViewData["tenantTypes"] = tenantTypes;

                return PartialView(operationLogs);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }

        /// <summary>
        /// 删除一段时间内的操作日志
        /// </summary>
        /// <param name="minDate">起始日期</param>
        /// <param name="maxDate">结束日期</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult DeleteOperationLog(DateTime? minDate = null, DateTime? maxDate = null)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().User()))
            {
                if (maxDate.HasValue)
                {
                    maxDate = maxDate.Value.AddDays(1);
                }
                if (minDate == null || maxDate == null || minDate > maxDate)
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "清除历史数据失败"));
                }
                else
                {
                    var cleanNum = operationLogService.Clean(minDate, maxDate);
                    return Json(new StatusMessageData(StatusMessageType.Success, "成功清除了 \"" + cleanNum + "\" 条操作日志"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }
        }

        #endregion

        #endregion

        #region 设置

        #region 站点设置

        /// <summary>
        /// 站点设置
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageSiteSettings()
        {
            if (!IsSiteManager())
            {
                return RedirectToAction("BankEndError");
            }
            return View();
        }

        /// <summary>
        /// 编辑站点设置 (GET)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditSiteSetting()
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().SiteManage()))
            {
                SiteSettingEditModel siteSettingEditModel = new SiteSettingEditModel();

                //获取需要使用到的设置项
                //站点设置
                var siteSetting = siteSettings.Get();
                //用户设置
                var userSetting = userSettings.Get();
                //水印设置
                var watermarkSetting = imageSettings.Get().WatermarkSettings;
                //邀请好友设置
                var inviteFriendSetting = inviteFriendSettings.Get();

                //将获取到的数据填充到ViewModel中
                siteSettingEditModel = siteSetting.MapTo(siteSettingEditModel);
                siteSettingEditModel = userSetting.MapTo(siteSettingEditModel);
                siteSettingEditModel = watermarkSetting.MapTo(siteSettingEditModel);
                siteSettingEditModel = inviteFriendSetting.MapTo(siteSettingEditModel);

                //设置ViewModel中用户注册方式选项
                if (userSetting.RegisterType == RegisterType.Email)
                {
                    siteSettingEditModel.isEmail = true;
                    siteSettingEditModel.isMobile = false;
                }
                else if (userSetting.RegisterType == RegisterType.Mobile)
                {
                    siteSettingEditModel.isEmail = false;
                    siteSettingEditModel.isMobile = true;
                }
                else
                {
                    siteSettingEditModel.isEmail = true;
                    siteSettingEditModel.isMobile = true;
                }

                //邮箱手机优先下拉列表
                IEnumerable<SelectListItem> emailOrMobileDropdownList = new List<SelectListItem>(){
                new SelectListItem {Text=RegisterType.Email.GetDisplayName(),Value=RegisterType.EmailOrMobile.ToString()},
                new SelectListItem {Text=RegisterType.Mobile.GetDisplayName(),Value=RegisterType.MobileOrEmail.ToString()}
            };
                //水印类型下拉列表
                IEnumerable<SelectListItem> watermarkDropdownList = new List<SelectListItem>() {
                new SelectListItem {Text=WatermarkType.None.GetDisplayName(),Value=WatermarkType.None.ToString()},
                new SelectListItem {Text=WatermarkType.Text.GetDisplayName(),Value=WatermarkType.Text.ToString()},
                new SelectListItem {Text=WatermarkType.Image.GetDisplayName(),Value=WatermarkType.Image.ToString()}
            };

                ViewData["emailOrMobileDropdownList"] = emailOrMobileDropdownList;
                ViewData["watermarkDropdownList"] = watermarkDropdownList;

                return PartialView(siteSettingEditModel);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }

        }

        /// <summary>
        /// 编辑站点设置 (POST)
        /// </summary>
        /// <param name="siteSettingEditModel">站点设置ViewModel</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _EditSiteSetting(SiteSettingEditModel siteSettingEditModel)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().SiteManage()))
            {
                if (siteSettingEditModel.isEmail == false && siteSettingEditModel.isMobile == false)
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "编辑失败"));
                }
                else
                {
                    //获取设置
                    var siteSetting = siteSettings.Get();
                    var userSetting = userSettings.Get();
                    var imageSetting = imageSettings.Get();
                    var watermarkSetting = imageSetting.WatermarkSettings;
                    var inviteFriendSetting = inviteFriendSettings.Get();

                    //处理并将ViewModel中的数据分别存入相应的设置
                    if (siteSettingEditModel.isEmail == false || siteSettingEditModel.isMobile == false)
                    {
                        siteSettingEditModel.RegisterType = siteSettingEditModel.isEmail ? RegisterType.Email : RegisterType.Mobile;
                    }

                    siteSetting = siteSettingEditModel.MapTo(siteSetting);
                    userSetting = siteSettingEditModel.MapTo(userSetting);
                    watermarkSetting = siteSettingEditModel.MapTo(watermarkSetting);
                    inviteFriendSetting = siteSettingEditModel.MapTo(inviteFriendSetting);

                    //保存设置
                    siteSettings.Save(siteSetting);
                    userSettings.Save(userSetting);
                    imageSettings.Save(imageSetting);
                    inviteFriendSettings.Save(inviteFriendSetting);

                    //更改Formatter中的siteName值,让站点更改的名称生效
                    Formatter.ChangeSiteName(siteSetting.SiteName);

                    return Json(new StatusMessageData(StatusMessageType.Success, "编辑站点设置成功"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }

        }
        #endregion

        #region 导航管理

        /// <summary>
        /// 导航管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageNavigations()
        {
            if (!IsSiteManager())
            {
                return RedirectToAction("BankEndError");
            }
            var navigations = navigationService.GetIndentedAllNavigation();
            return View(navigations);
        }

        /// <summary>
        /// 添加/编辑 导航/子导航 (GET)
        /// </summary>
        /// <param name="navigationId">导航Id</param>
        /// <param name="parentNavigationId">父导航Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditNavigation(int navigationId = 0, int parentNavigationId = 0)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().SiteManage()))
            {
                Navigation navigation = new Navigation();
                NavigationEditModel navigationEditModel = NavigationEditModel.New();
                if (navigationId > 0)
                {
                    //navigationId>0 编辑导航/子导航
                    navigation = navigationService.Get(navigationId);
                    if (navigation != null)
                    {
                        navigationEditModel = navigation.MapTo<Navigation, NavigationEditModel>(navigationEditModel);
                        //是否来自栏目
                        if (navigation.NavigationType == NavigationTypes.AddCategory)
                        {
                            navigationEditModel.isFromContent = true;
                            navigationEditModel.CategoryId = navigation.CategoryId;
                        }
                        else
                        {
                            navigationEditModel.isFromContent = false;
                        }
                        //是否使用路由名称
                        if (string.IsNullOrEmpty(navigation.NavigationUrl))
                        {
                            navigationEditModel.IsRouteName = true;
                            navigationEditModel.UrlFromEditModel = navigation.UrlRouteName;
                        }
                        else
                        {
                            navigationEditModel.IsRouteName = false;
                            navigationEditModel.UrlFromEditModel = navigation.NavigationUrl;
                        }
                        //是否在新窗口中打开链接
                        if (navigation.NavigationTarget == "_blank")
                        {
                            navigationEditModel.IsBlank = true;
                        }
                        else
                        {
                            navigationEditModel.IsBlank = false;
                        }
                    }
                }
                else
                {
                    //navigationId>0 添加导航/子导航
                    if (parentNavigationId > 0)
                    {
                        var parentNavgation = navigationService.Get(parentNavigationId);
                        if (parentNavgation != null)
                        {
                            navigationEditModel.ParentNavigationId = parentNavigationId;
                        }
                    }
                }

                //取出所有的资讯栏目供SelectList使用
                var contentCategories = categorieService.GetIndentedAllCategories();
                //选出启用的栏目
                contentCategories = contentCategories.Where(n => n.IsEnabled == true);

                ViewData["contentCategories"] = contentCategories;

                return View(navigationEditModel);
            }
            else
            {
                return RedirectToAction("BankEndError");
            }
        }

        /// <summary>
        /// 添加/编辑 导航/子导航 (POST)
        /// </summary>
        /// <param name="navigationEditModle">导航编辑ViewModel</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult _EditNavigation(NavigationEditModel navigationEditModle)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().SiteManage()))
            {
                if (!(string.IsNullOrWhiteSpace(navigationEditModle.NavigationText) || string.IsNullOrWhiteSpace(navigationEditModle.UrlFromEditModel)))
                {
                    Navigation navigation = navigationService.Get(navigationEditModle.NavigationId) ?? new Navigation();

                    //为导航对象属性赋值
                    navigation.NavigationText = navigationEditModle.NavigationText;
                    if (navigationEditModle.isFromContent == true)
                    {
                        //来自于资讯栏目时,存入资讯栏目Id和资讯栏目URL
                        navigation.NavigationUrl = SiteUrls.Instance().CategoryCMS(navigationEditModle.CategoryId);
                        navigation.UrlRouteName = string.Empty;
                        navigation.NavigationType = NavigationTypes.AddCategory;
                    }
                    else
                    {
                        //存入模态框中输入的URL或URLRouteName
                        if (navigationEditModle.IsRouteName == true)
                        {
                            if (navigationEditModle.UrlFromEditModel.Length > 64)
                            {
                                return Json(new StatusMessageData(StatusMessageType.Hint, "操作失败"));
                            }
                            else
                            {
                                navigation.UrlRouteName = navigationEditModle.UrlFromEditModel ?? string.Empty;
                                navigation.NavigationUrl = string.Empty;
                            }
                        }
                        else
                        {
                            navigation.UrlRouteName = string.Empty;
                            navigation.NavigationUrl = navigationEditModle.UrlFromEditModel ?? string.Empty;
                        }

                        navigation.NavigationType = NavigationTypes.Application;
                    }

                    navigation.CategoryId = navigationEditModle.CategoryId;

                    //是否在新窗口中打开链接
                    if (navigationEditModle.IsBlank == true)
                    {
                        navigation.NavigationTarget = "_blank";
                    }
                    else
                    {
                        navigation.NavigationTarget = "_self";
                    }
                    //是否启用
                    navigation.IsEnabled = navigationEditModle.IsEnabled;
                    if (navigationEditModle.NavigationId > 0)
                    {
                        //编辑导航和子导航
                        //将ViewModel中的值存入Model并更新到数据库中
                        navigationService.Update(navigation);
                        return Json(new StatusMessageData(StatusMessageType.Success, "成功编辑该导航"));

                    }
                    else
                    {
                        //添加导航和子导航
                        navigation.ParentNavigationId = navigationEditModle.ParentNavigationId;
                        navigationService.Create(navigation);
                        if (navigationEditModle.ParentNavigationId > 0)
                        {
                            return Json(new StatusMessageData(StatusMessageType.Success, "成功添加子导航"));
                        }
                        else
                        {
                            return Json(new StatusMessageData(StatusMessageType.Success, "成功添加导航"));
                        }
                    }
                }
                else
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "操作失败"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }
        }

        /// <summary>
        /// 删除导航/子导航
        /// </summary>
        /// <param name="navigationId">导航Id</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult DeleteNavigation(int navigationId)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().SiteManage()))
            {
                if (navigationId > 0)
                {
                    navigationService.Delete(navigationId);
                    return Json(new StatusMessageData(StatusMessageType.Success, "成功删除该导航"));
                }
                else
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "删除该导航失败"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }

        }

        /// <summary>
        /// 上移下移导航
        /// </summary>
        /// <param name="fromNavigationId">起始位置导航Id</param>
        /// <param name="toNavigationId">目标位置导航Id</param>
        /// <returns>Json状态</returns>
        [HttpPost]
        public JsonResult ChangeNavigationOrder(int fromNavigationId, int toNavigationId)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().SiteManage()))
            {
                if (fromNavigationId > 0 && toNavigationId > 0)
                {
                    navigationService.ChangeNavigationDisplayOrder(fromNavigationId, toNavigationId);
                    return Json(new StatusMessageData(StatusMessageType.Success, "成功移动导航位置"));
                }
                else
                {
                    return Json(new StatusMessageData(StatusMessageType.Hint, "移动失败"));
                }
            }
            else
            {
                return Json(new StatusMessageData(StatusMessageType.Hint, "无权操作"));
            }

        }

        #endregion

        #endregion

        #region 工具



        #region 自运行任务

        /// <summary>
        /// 任务管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageTasks()
        {
            if (!IsSiteManager())
            {
                return RedirectToAction("BankEndError");
            }
            IEnumerable<TaskDetail> tasks = taskService.GetAll();
            return View(tasks);
        }

        /// <summary>
        /// 编辑任务
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditTask(int Id)
        {
            if (!IsSiteManager())
            {
                return RedirectToAction("BankEndError");
            }
            TaskDetail taskDetail = taskService.Get(Id);

            if (taskDetail == null)
                return HttpNotFound();

            TaskDetailEditModel editModel = taskDetail.AsEditModel();
            InitRules(editModel);
            return View(editModel);
        }
        /// <summary>
        /// 管理任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditTask(TaskDetailEditModel model)
        {
            if (!IsSiteManager())
            {
                return RedirectToAction("BankEndError");
            }
            InitRules(model);

            //if (!ModelState.IsValid)
            //    return View(model);

            try
            {
                taskService.Update(model.AsTaskDetail());
            }
            catch (Exception e)
            {
                TempData["StatusMessageData"] = "更新失败!";
                return RedirectToAction("EditTask", new { Id = model.Id });
            }

            TempData["StatusMessageData"] = "更新成功！";
            return this.RedirectToAction("ManageTasks");
        }

        /// <summary>
        /// 直接运行任务
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RunTask(int id)
        {
            if (!IsSiteManager())
            {
                return Json(new { status = false, message = "无权操作!" });
            }
            TaskDetail td = taskService.Get(id);
            if (td == null)
            {
                return Json(new { status = false, message = "执行失败!" });

            }

            if (td.RunAtServer == RunAtServer.Search)
            {
                try
                {
                    var url = ConfigurationManager.AppSettings["Search"];
                    var secret = ConfigurationManager.AppSettings["ApiAccessSecret"];

                    if (string.IsNullOrWhiteSpace(url))
                    {
                        return Json(new { status = false, message = "执行失败!" });
                    }
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("BasicAuth {0}", secret));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = httpClient.GetAsync(string.Format("{0}/Api/SearchApi/RunAskTask?id={1}", url, id)).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "执行成功!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "执行失败!" });
                    }
                }
                catch (Exception e)
                {
                    return Json(new { status = false, message = "执行失败!" });
                }
            }
            else
            {
                TaskSchedulerFactory.GetScheduler().Run(id);
            }
            return Json(new { success = true, message = "执行成功!" });
        }

        /// <summary>
        /// 初始化任务规则
        /// </summary>
        private void InitRules(TaskDetailEditModel editModel)
        {
            List<SelectListItem> seconds = new List<SelectListItem>();
            List<SelectListItem> minutes = new List<SelectListItem>();
            List<SelectListItem> hours = new List<SelectListItem>();
            List<SelectListItem> mouth = new List<SelectListItem>();
            List<SelectListItem> day = new List<SelectListItem>();
            List<SelectListItem> dayOfMouth = new List<SelectListItem>();

            for (int i = 0; i < 60; i++)
            {
                seconds.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = editModel.Seconds == i.ToString() });
                minutes.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = editModel.Minutes == i.ToString() });
                if (i > 0 && i <= 23)
                    hours.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = editModel.Hours == i.ToString() });

                if (i > 0 && i <= 12)
                    mouth.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = editModel.Mouth == i.ToString() });

                if (i > 0 && i <= 31)
                {
                    day.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = editModel.Day == i.ToString() });
                    dayOfMouth.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = editModel.DayOfMouth == i.ToString() });
                }
            }

            ViewData["Seconds"] = seconds;
            ViewData["Minutes"] = minutes;
            ViewData["Hours"] = hours;
            ViewData["Mouth"] = mouth;
            ViewData["Day"] = day;
            ViewData["DayOfMouth"] = dayOfMouth;

            ViewData["Frequency"] = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "每天", Value  = ((int)TaskFrequency.EveryDay).ToString(), Selected = TaskFrequency.EveryDay == editModel.Frequency },
                new SelectListItem(){ Text = "每周", Value  = ((int)TaskFrequency.Weekly).ToString(), Selected = TaskFrequency.Weekly == editModel.Frequency },
                new SelectListItem(){ Text = "每月", Value  = ((int)TaskFrequency.PerMonth).ToString(), Selected = TaskFrequency.PerMonth == editModel.Frequency  }
            };
            ViewData["Number"] = new List<SelectListItem>() {
                new SelectListItem(){ Text = "第一周", Value  = "1",Selected = editModel.Number == "1" },
                new SelectListItem(){ Text = "第二周", Value  = "2",Selected = editModel.Number == "2" },
                new SelectListItem(){ Text = "第三周", Value  = "3",Selected = editModel.Number == "3" },
                new SelectListItem(){ Text = "第四周", Value  = "4",Selected = editModel.Number == "4" }

            };

            ViewData["DayOfWeek"] = new Dictionary<string, string>() { { "周一", "2" }, { "周二", "3" }, { "周三", "4" }, { "周四", "5" }, { "周五", "6" }, { "周六", "7" }, { "周日", "1" } };
            ViewData["WeeklyOfMouth"] = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "周一", Value  = "2", Selected = editModel.WeeklyOfMouth == "2" },
                new SelectListItem(){ Text = "周二", Value  = "3", Selected = editModel.WeeklyOfMouth == "3" },
                new SelectListItem(){ Text = "周三", Value  = "4", Selected = editModel.WeeklyOfMouth == "4" },
                new SelectListItem(){ Text = "周四", Value  = "5", Selected = editModel.WeeklyOfMouth == "5" },
                new SelectListItem(){ Text = "周五", Value  = "6", Selected = editModel.WeeklyOfMouth == "6" },
                new SelectListItem(){ Text = "周六", Value  = "7", Selected = editModel.WeeklyOfMouth == "7" },
                new SelectListItem(){ Text = "周日", Value  = "1", Selected = editModel.WeeklyOfMouth == "1" }
            };
        }


        /// <summary>
        /// 重启站点
        /// </summary>
        public ActionResult _UnloadAppDomain()
        {
            if (!IsSiteManager())
            {
                return Json(0);
            }
            System.Web.HttpRuntime.UnloadAppDomain();
            return Json(1);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public ActionResult _ResetCache()
        {

            if (!IsSiteManager())
            {
                return Json(0);
            }
            ICacheService cacheService = DIContainer.Resolve<ICacheService>();
            cacheService.Clear();
            OutputCacheManager outputCacheManager = new OutputCacheManager();
            outputCacheManager.RemoveItems();
            return Json(1);
        }

        /// <summary>
        /// 暂停站点设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PauseSiteSettings()
        {
            if (!IsSiteManager())
            {
                return RedirectToAction("BankEndError");
            }

            PauseSiteSettings pauseSiteSettings = pauseSiteSettingsManager.Get();
            //pauseSiteSettings.PauseLink = "http://" + pauseSiteSettings.PauseLink;
            PauseSiteSettingsEditModel pauseSiteSettingsEditModel = new PauseSiteSettingsEditModel(pauseSiteSettings);
            return View(pauseSiteSettingsEditModel);
        }

        /// <summary>
        /// 保存暂停站点的设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SavePauseSiteSettings(PauseSiteSettingsEditModel pauseSiteSettingsEditModel)
        {
            if (!IsSiteManager())
            {
                return Json(new { state = 0, msg = "无权操作" });
            }
            PauseSiteSettings pauseSiteSettings = pauseSiteSettingsEditModel.AsPauseSiteSettings();
            pauseSiteSettingsManager.Save(pauseSiteSettings);
            return Json(new { state = 1, msg = "设置成功" });
        }

        #endregion




        #region 索引



        /// <summary>
        /// 重建索引
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RebuildIndex(string code)
        {
            if (!IsSiteManager())
            {
                return Json(new { state = 0 });
            }
            var url = ConfigurationManager.AppSettings["Search"];
            var secret = ConfigurationManager.AppSettings["ApiAccessSecret"];

            if (string.IsNullOrWhiteSpace(url))
            {
                return Json(new { state = 0 });
            }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("BasicAuth {0}", secret));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(string.Format("{0}/Api/SearchApi/ReBuildIndex?code={1}", url, code)).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { state = 1 });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Json(new { state = 0 });
        }



        /// <summary>
        /// 索引管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageSearchIndexs()
        {
            if (!IsSiteManager())
                return RedirectToAction("BankEndError");
            var url = ConfigurationManager.AppSettings["Search"];

            if (string.IsNullOrWhiteSpace(url))
            {
                return View(new Searchers());
            }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(url + "/Api/SearchApi/GetSearchers").Result;

                if (response.IsSuccessStatusCode)
                {
                    var results = JsonConvert.DeserializeObject<Searchers>(response.Content.ReadAsStringAsync().Result);

                    return View(results);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View(new Searchers());
        }


        #endregion 索引


        #endregion 工具

        #region 调用方法

        #region 临时附件
        /// <summary>
        /// 正式附件分布页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _Attachments(long contentItemId, string tenantTypeId = "")
        {
            if (string.IsNullOrEmpty(tenantTypeId))
                tenantTypeId = TenantTypeIds.Instance().ContentItem();
            AttachmentService attachmentService = new AttachmentService(tenantTypeId);
            if (contentItemId > 0)
                ViewData["attachmentCItemList"] = attachmentService.GetsByAssociateId(contentItemId);

            return PartialView();
        }

        /// <summary>
        /// 临时附件分布页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _TemporaryAttachments(string tenantTypeId = "")
        {
            if (string.IsNullOrEmpty(tenantTypeId))
                tenantTypeId = TenantTypeIds.Instance().ContentItem();
            ViewData["attachmentList"] = attachmentService.GetTemporaryAttachments(user.UserId, tenantTypeId);
            return PartialView();
        }

        /// <summary>
        /// 删除临时附件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _DeleteAttachments(long attachmentId)
        {
            attachmentService.Delete(attachmentId);
            var statusMessage = new StatusMessageData(StatusMessageType.Success, "删除附件成功");
            return Json(statusMessage);
        }

        #endregion

        /// <summary>
        /// 是否具有用户管理的权限
        /// </summary>
        /// <returns></returns>
        private bool IsUserManager()
        {
            var IsPostManager = authorizer.IsUserManager(UserContext.CurrentUser);
            if (!IsPostManager)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您没有权限访问该页面！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "无权查看",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
                return false;
            }
            return true;
        }

        /// <summary>
        /// 是否具有公共内容管理的权限
        /// </summary>
        /// <returns></returns>
        private bool IsGlobalContentManager()
        {
            var IsPostManager = authorizer.IsGlobalContentManager(UserContext.CurrentUser);
            if (!IsPostManager)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您没有权限访问该页面！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "无权查看",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
                return false;
            }
            return true;
        }
        /// <summary>
        /// 是否具有公共内容管理的权限
        /// </summary>
        /// <returns></returns>
        private bool IsSiteManager()
        {
            var IsPostManager = authorizer.IsSiteManager(UserContext.CurrentUser);
            if (!IsPostManager)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您没有权限访问该页面！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "无权查看",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
                return false;
            }
            return true;
        }
        /// <summary>
        /// 是否具有工具管理权限
        /// </summary>
        /// <returns></returns>
        private bool IsSuperAdministrator()
        {
            var IsPostManager = authorizer.IsSuperAdministrator(UserContext.CurrentUser);
            if (!IsPostManager)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您没有权限访问该页面！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "无权查看",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
                return false;
            }
            return true;
        }

        #endregion

    }
}
