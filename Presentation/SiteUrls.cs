//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;
using Tunynet;
using Tunynet.Utilities;
using Tunynet.FileStore;
using Tunynet.Settings;
using Tunynet.Attitude;
using Tunynet.UI;
using System.Web.Mvc;

namespace Tunynet.Common
{
    /// <summary>
    /// 站点Url配置
    /// </summary>
    public class SiteUrls
    {
        //平台的AreaName
        private readonly string CommonAreaName = "Common";
        private readonly string PanelAreaName = "";
        private static readonly string ControlPanelAreaName = "ConsoleViews";

        IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
        UserService userService = DIContainer.Resolve<UserService>();


        #region 站点登录页面

        /// <summary>
        /// 站点登录页面
        /// </summary>
        /// <param name="loginModal">登录模式</param>
        /// <param name="includeReturnUrl">是否包含returnUrl(默认为false)</param>
        /// <param name="returnUrl">回跳地址</param>
        public string Login(string returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                HttpContext httpContext = HttpContext.Current;
                string currentPath = httpContext.Request.Url.PathAndQuery;

                returnUrl = SiteUrls.ExtractQueryParams(currentPath)["ReturnUrl"];

                if (string.IsNullOrEmpty(returnUrl))
                    returnUrl = WebUtility.UrlEncode(HttpContext.Current.Request.RawUrl);
            }
            return CachedUrlHelper.Action("Login", "Account", null, new RouteValueDictionary { { "returnUrl", returnUrl } });
        }
        /// <summary>
        /// 前台首页
        /// </summary>
        public string Home()
        {
            return CachedUrlHelper.Action("Home", "Portal");
        }
        /// <summary>
        /// 后台首页
        /// </summary>
        public string ControlPanelHome()
        {
            return CachedUrlHelper.Action("Home", "ControlPanel");
        }

        /// <summary>
        /// 前台错误页面 404/ 500
        /// </summary>
        /// <returns></returns>
        public string Error(TempDataDictionary tempData = null, SystemMessageViewModel model = null, string returnUrl = null)
        {
            if (tempData != null && model != null)
            {
                tempData["SystemMessageViewModel"] = model;
            }
            return CachedUrlHelper.Action("Error", "Common");
        }

        /// <summary>
        /// 后台错误页面 404/ 500
        /// </summary>
        /// <returns></returns>
        public string BankEndError(TempDataDictionary tempData = null, SystemMessageViewModel model = null, string returnUrl = null)
        {
            if (tempData != null && model != null)
            {
                tempData["SystemMessageViewModel"] = model;
            }
            return CachedUrlHelper.Action("BankEndError", "ControlPanel");
        }

        #endregion

        #region 用户头像

        /// <summary>
        /// 获取用户连接的头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="avatarSizeType"></param>
        /// <param name="enableClientCaching"></param>
        /// <returns></returns>
        public string _UserAvatarLink(AvatarSizeType avatarSizeType = AvatarSizeType.Small, bool enableClientCaching = true)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            if (avatarSizeType != AvatarSizeType.Small)
                dic.Add("avatarSizeType", avatarSizeType);
            if (enableClientCaching)
                dic.Add("enableClientCaching", enableClientCaching);
            return CachedUrlHelper.Action("_UserAvatarLink", "Channel", CommonAreaName, dic);
        }

        #endregion


        #region 通用评论

        public string _CommentDemo(long commentedObjectId, long ownerId, string tenantTypeId, SortBy_Comment sortBy = SortBy_Comment.DateCreatedDesc, string subject = null, string originalAuthor = null)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            dic.Add("commentedObjectId", commentedObjectId);
            dic.Add("ownerId", ownerId);
            dic.Add("tenantTypeId", tenantTypeId);
            dic.Add("sortBy", sortBy);
            if (!string.IsNullOrEmpty(subject))
                dic.Add("subject", subject);
            if (!string.IsNullOrEmpty(originalAuthor))
                dic.Add("originalAuthor", originalAuthor);
            return CachedUrlHelper.Action("_Comment", "Channel", CommonAreaName, dic);
        }

        /// <summary>
        /// 获取一条
        /// </summary>
        /// <param name="id">评论的id</param>
        /// <returns></returns>
        public string _OneComment(long? id = null)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (id.HasValue)
                routeValueDictionary.Add("id", id);
            return CachedUrlHelper.Action("_OneComment", "Channel", CommonAreaName, routeValueDictionary);
        }

        ///// <summary>
        ///// （通用）评论列表
        ///// </summary>
        ///// <param name="tenantType">评论的租户类型id</param>
        ///// <param name="commentedObjectId">被评论对象id</param>
        ///// <param name="sortBy">排序方式</param>
        ///// <param name="pageIndex">当前页码</param>
        ///// <returns>评论列表</returns>
        //public string _CommentList(string tenantType, long commentedObjectId, SortBy_Comment sortBy = SortBy_Comment.DateCreated, int pageIndex = 1, bool showBefor = true, bool showAfter = false)
        //{
        //    RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
        //    routeValueDictionary.Add("tenantType", tenantType);
        //    routeValueDictionary.Add("commentedObjectId", commentedObjectId);
        //    if (sortBy != SortBy_Comment.DateCreated)
        //        routeValueDictionary.Add("sortBy", sortBy);
        //    if (pageIndex != 1)
        //        routeValueDictionary.Add("pageIndex", pageIndex);
        //    if (!showBefor)
        //        routeValueDictionary.Add("showBefor", showBefor);
        //    if (showAfter)
        //        routeValueDictionary.Add("showAfter", showAfter);
        //    return CachedUrlHelper.Action("_CommentList", "Channel", CommonAreaName, routeValueDictionary);
        //}

        ///// <summary>
        ///// 子级评论局部页面（第一次的时候使用）
        ///// </summary>
        ///// <param name="parentId">父级id</param>
        ///// <returns>子级评论局部页面的链接</returns>
        //public string _ChildComment(long? parentId = null, bool enableComment = true)
        //{
        //    RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
        //    if (parentId.HasValue)
        //        routeValueDictionary.Add("parentId", parentId);

        //    if (!enableComment)
        //        routeValueDictionary.Add("enableComment", enableComment);

        //    return CachedUrlHelper.Action("_ChildComment", "Channel", CommonAreaName, routeValueDictionary);
        //}

        ///// <summary>
        ///// （通用）子级评论列表
        ///// </summary>
        ///// <param name="parentId">父级评论列表id</param>
        ///// <param name="pageIndex">当前页码</param>
        ///// <param name="sortBy">排序方式</param>
        ///// <returns>排序方式</returns>
        //public string _ChildCommentList(long parentId, int pageIndex = 1, SortBy_Comment sortBy = SortBy_Comment.DateCreatedDesc, bool showBefor = true, bool showAfter = false)
        //{
        //    RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
        //    routeValueDictionary.Add("parentId", parentId);
        //    if (pageIndex > 1)
        //        routeValueDictionary.Add("pageIndex", pageIndex);
        //    if (sortBy != SortBy_Comment.DateCreatedDesc)
        //        routeValueDictionary.Add("sortBy", sortBy);
        //    if (!showBefor)
        //        routeValueDictionary.Add("showBefor", showBefor);
        //    if (showAfter)
        //        routeValueDictionary.Add("showAfter", showAfter);
        //    return CachedUrlHelper.Action("_ChildCommentList", "Channel", CommonAreaName, routeValueDictionary);
        //}

        ///// <summary>
        ///// (通用)删除评论
        ///// </summary>
        ///// <returns></returns>
        //public string _DeleteComment(long commentId)
        //{
        //    RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
        //    routeValueDictionary.Add("commentId", commentId);
        //    return CachedUrlHelper.Action("_DeleteComment", "Channel", CommonAreaName, routeValueDictionary);
        //}

        ///// <summary>
        ///// 评论控件
        ///// </summary>
        //public string _Comment(long commentedObjectId, long ownerId, string tenantTypeId)
        //{
        //    return CachedUrlHelper.Action("_Comment", "Channel", CommonAreaName, new RouteValueDictionary { { "commentedObjectId", commentedObjectId }, { "ownerId", ownerId }, { "tenantTypeId", tenantTypeId } });
        //}

        #endregion

        #region User 用户相关 

        /// <summary>
        /// 用户封面图Url
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="avatarSizeType">头像尺寸</param>
        /// <param name="enableClientCaching">是否启用客户端缓存</param>
        public string UserCoverUrl(IUser user, bool enableClientCaching = true, AvatarSizeType avatarSizeType = AvatarSizeType.Small)
        {
            return userService.GetCoverDirectlyUrl(user, enableClientCaching, avatarSizeType);
        }

        /// <summary>
        /// 用户头像Url
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="avatarSizeType">头像尺寸</param>
        /// <param name="enableClientCaching">是否启用客户端缓存</param>
        public string UserAvatarUrl(IUser user, AvatarSizeType avatarSizeType, bool enableClientCaching = true)
        {
            return userService.GetAvatarDirectlyUrl(user, avatarSizeType, enableClientCaching);
        }
        /// <summary>
        /// 根据用户ID获取头像Url
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="avatarSizeType">头像尺寸</param>
        /// <param name="enableClientCaching">是否启用客户端缓存</param>
        public string UserIDOfAvatarUrl(long userId, AvatarSizeType avatarSizeType, bool enableClientCaching = true)
        {
            var userInfo = userService.GetFullUser(userId);
            if (userInfo == null)
                return string.Empty;
            return userService.GetAvatarDirectlyUrl(userInfo, avatarSizeType, enableClientCaching);
        }

        #region 用户卡片
        /// <summary>
        /// 用户卡片
        /// </summary>
        public string _UserCard(long userId)
        {
            return CachedUrlHelper.Action("_UserCard", "Common", PanelAreaName, new RouteValueDictionary { { "userId", userId }, { "t", DateTime.Now.Ticks } });
        }

        #endregion

        /// <summary>
        /// 我的首页
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public string MyHome(long userId)
        {
            string spaceKey = UserIdToUserNameDictionary.GetUserName(userId);
            if (string.IsNullOrEmpty(spaceKey))
                return string.Empty;
            return MyHome(spaceKey);
        }

        /// <summary>
        /// 我的首页
        /// </summary>
        /// <param name="spaceKey">用户空间标识</param>
        /// <returns></returns>
        public string MyHome(string spaceKey)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(spaceKey))
                routeValueDictionary.Add("spaceKey", Tunynet.Utilities.WebUtility.UrlEncode(spaceKey));
            return CachedUrlHelper.Action("MyHomepage", "UserSpace", PanelAreaName, routeValueDictionary);
        }

        /// <summary>
        /// 我的主页
        /// </summary>
        /// <param name="spaceKey">用户空间标识</param>
        /// <returns></returns>
        public string SpaceHome(string spaceKey, int? applicationId = null)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            dic.Add("spaceKey", spaceKey);
            if (applicationId.HasValue)
                dic.Add("applicationId", applicationId);
            return CachedUrlHelper.Action("SpaceHomepage", "UserSpace", PanelAreaName, dic);
        }

        /// <summary>
        /// 我的主页
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public string SpaceHome(long userId, int? applicationId = null)
        {
            return SpaceHome(UserIdToUserNameDictionary.GetUserName(userId), applicationId);
        }
        #endregion


        #region Instance

        private static volatile SiteUrls _instance = null;
        private static readonly object lockObject = new object();

        /// <summary>
        /// 创建主页实体
        /// </summary>
        /// <returns></returns>
        public static SiteUrls Instance()
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new SiteUrls();
                    }
                }
            }
            return _instance;
        }

        private SiteUrls()
        { }

        #endregion Instance

        #region 公共控件

        /// <summary>
        /// 设置标题图
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id</param>
        ///<param name="htmlFieldName">隐藏域全名称</param>
        /// <param name="isMultiSelect">是否多选</param>
        /// <param name="attachmentIds">附件Id</param>
        /// <param name="maxSelect">最大选择数量</param>
        /// <summary>
        /// 统一的文件上传
        /// </summary>
        public string UploadFile(string CurrentUserIdToken)
        {
            return CachedUrlHelper.Action("UploadFile", "Channel", CommonAreaName, new RouteValueDictionary { { "CurrentUserIdToken", CurrentUserIdToken } });
        }

        /// <summary>
        /// 同意的文件上传
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="timeliness">时间限制</param>
        /// <returns></returns>
        public string UploadFile(long userId, double timeliness = 0.1)
        {
            return UploadFile(Utility.EncryptTokenForUploadfile(timeliness, userId));
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="attachmentId">附件Id</param>
        /// <returns></returns>
        public string _DeleteAttachment(string tenantTypeId, long attachmentId)
        {
            return CachedUrlHelper.Action("_DeleteAttachment", "Channel", CommonAreaName, new RouteValueDictionary() { { "tenantTypeId", tenantTypeId }, { "attachmentId", attachmentId } });
        }

        /// <summary>
        /// 文件库文件
        /// </summary>
        public string _EditAttachmentLibraries()
        {
            return CachedUrlHelper.Action("_EditAttachmentLibraries", "Channel", CommonAreaName);
        }

        /// <summary>
        /// 相册图片
        /// </summary>
        public string _EditPhoto()
        {
            return CachedUrlHelper.Action("_EditPhoto", "Channel", CommonAreaName);
        }

        /// <summary>
        /// 网络图片
        /// </summary>
        public string _EditNetImage()
        {
            return CachedUrlHelper.Action("_EditNetImage", "Channel", CommonAreaName);
        }

        /// <summary>
        /// 网络文件
        /// </summary>
        public string _EditNetAttachment()
        {
            return CachedUrlHelper.Action("_EditNetAttachment", "Channel", CommonAreaName);
        }

        /// <summary>
        /// 上传附件列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id</param>
        public string _ListAttachments(string tenantTypeId, long associateId = 0)
        {
            return CachedUrlHelper.Action("_ListAttachments", "Channel", CommonAreaName, new RouteValueDictionary() { { "tenantTypeId", tenantTypeId }, { "associateId", associateId }, { "t", new Random().Next(1, 100).ToString() } });
        }

        /// <summary>
        /// 上传图片列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id</param>
        public string _ListImages(string tenantTypeId, long associateId = 0)
        {
            return CachedUrlHelper.Action("_ListImages", "Channel", CommonAreaName, new RouteValueDictionary() { { "tenantTypeId", tenantTypeId }, { "associateId", associateId }, { "t", new Random().Next(1, 100).ToString() } });
        }

        /// <summary>
        /// 图片上传管理
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id</param>
        public string _ImageManage(string tenantTypeId, long associateId = 0)
        {
            return CachedUrlHelper.Action("_ImageManage", "Channel", CommonAreaName, new RouteValueDictionary() { { "tenantTypeId", tenantTypeId }, { "associateId", associateId }, { "t", new Random().Next(1, 100).ToString() } });
        }

        /// <summary>
        /// Html编辑器中的@用户
        /// </summary>
        /// <param name="textareaId"></param>
        /// <param name="seletorId"></param>
        /// <returns></returns>
        public string _AtUsers()
        {
            return CachedUrlHelper.Action("_AtUsers", "Channel", CommonAreaName);
        }
        /// <summary>
        /// 附件上传管理
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id</param>
        /// <returns></returns>
        public string _AttachmentManage(string tenantTypeId, long associateId = 0)
        {
            return CachedUrlHelper.Action("_AttachmentManage", "Channel", CommonAreaName, new RouteValueDictionary() { { "tenantTypeId", tenantTypeId }, { "associateId", associateId }, { "t", new Random().Next(1, 100).ToString() } });
        }

        /// <summary>
        /// 附件上传
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id</param>
        /// <returns></returns>
        public string _EditAttachment(string tenantTypeId, long associateId = 0)
        {
            return CachedUrlHelper.Action("_EditAttachment", "Channel", CommonAreaName, new RouteValueDictionary() { { "tenantTypeId", tenantTypeId }, { "associateId", associateId }, { "t", new Random().Next(1, 100).ToString() } });
        }

        /// <summary>
        /// 保存附件售价
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public string _SavePrice(string tenantTypeId)
        {
            return CachedUrlHelper.Action("_SavePrice", "Channel", CommonAreaName, new RouteValueDictionary() { { "tenantTypeId", tenantTypeId } });
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="associateId">附件关联Id</param>
        public string _EditImage(string tenantTypeId, long associateId = 0)
        {
            return CachedUrlHelper.Action("_EditImage", "Channel", CommonAreaName, new RouteValueDictionary() { { "tenantTypeId", tenantTypeId }, { "associateId", associateId }, { "t", new Random().Next(1, 100).ToString() } });
        }

        /// <summary>
        /// 获取我关注的人
        /// </summary>
        /// <returns></returns>
        public string GetMyFollowedUsers()
        {
            return CachedUrlHelper.Action("GetMyFollowedUsers", "Channel", CommonAreaName);
        }

        /// <summary>
        /// 自定义隐私设置
        /// </summary>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        public string PrivacySpecifyObjectSelector(string itemKey)
        {
            return CachedUrlHelper.Action("PrivacySpecifyObjectSelector", "Channel", CommonAreaName, new RouteValueDictionary { { "itemKey", itemKey } });
        }

        /// <summary>
        /// 多媒体解析
        /// </summary>
        /// <param name="mediaType">媒体类型</param>
        /// <returns></returns>
        public string ParseMedia(MediaType mediaType)
        {
            return CachedUrlHelper.Action("ParseMedia", "Channel", CommonAreaName, new RouteValueDictionary { { "mediaType", mediaType } });
        }

        /// <summary>
        /// 获取表情
        /// </summary>
        /// <param name="directoryName">表情目录名</param>
        /// <returns></returns>
        public string GetEmotions(string directoryName = "")
        {
            RouteValueDictionary routeValue = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(directoryName))
            {
                routeValue.Add("directoryName", directoryName);
            }

            return CachedUrlHelper.Action("GetEmotions", "Channel", CommonAreaName, routeValue);
        }

        /// <summary>
        /// @用户提醒
        /// </summary>
        /// <returns></returns>
        public string _AtRemindUser()
        {
            return CachedUrlHelper.Action("_AtRemindUser", "Channel", CommonAreaName);
        }
        /// <summary>
        /// 获取子地区
        /// </summary>
        public string GetChildAreas()
        {
            return CachedUrlHelper.Action("GetChildAreas", "Channel", CommonAreaName);
        }

        /// <summary>
        /// 验证码地址
        /// </summary>
        /// <returns></returns>
        public string CaptchaImage()
        {
            return CachedUrlHelper.RouteUrl("Captcha");
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <returns></returns>
        public string GetAskTags(int topNumber = 20, string tenantTypeId = "")
        {
            return CachedUrlHelper.Action("JsonTags", "Common", "", new RouteValueDictionary { { " topNumber", topNumber }, { "tenantTypeId", tenantTypeId } });
        }

        #endregion 公共控件

        #region 表情选择器

        /// <summary>
        /// 获取表情选择器Url
        /// </summary>
        public string _EmotionSelector()
        {
            return CachedUrlHelper.Action("_EmotionSelector", "Channel", CommonAreaName);
        }

        /// <summary>
        /// 显示表情包内的表情列表
        /// </summary>
        /// <param name="directoryName">表情包目录名</param>
        /// <returns></returns>
        public string _ListEmotions(string directoryName)
        {
            return CachedUrlHelper.Action("_ListEmotions", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "directoryName", directoryName } });
        }

        #endregion

        #region Help Methods

        /// <summary>
        /// 获取完整的Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string FullUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                return url;

            string fullUrl = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
                fullUrl = WebUtility.HostPath(HttpContext.Current.Request.Url) + WebUtility.ResolveUrl(url);
            else
            {
                ISettingsManager<SiteSettings> siteSettingsManager = DIContainer.Resolve<ISettingsManager<SiteSettings>>();
                SiteSettings siteSettings = siteSettingsManager.Get();
                if (!string.IsNullOrEmpty(siteSettings.MainSiteRootUrl))
                {
                    return siteSettings.MainSiteRootUrl + WebUtility.ResolveUrl(url);
                }
            }
            if (!string.IsNullOrEmpty(fullUrl))
                return fullUrl;
            else
                return url;
        }

        /// <summary>
        /// 获取url中的查询字符串参数
        /// </summary>
        public static NameValueCollection ExtractQueryParams(string url)
        {
            int startIndex = url.IndexOf("?");
            NameValueCollection values = new NameValueCollection();

            if (startIndex <= 0)
                return values;

            string[] nameValues = url.Substring(startIndex + 1).Split('&');

            foreach (string s in nameValues)
            {
                string[] pair = s.Split('=');

                string name = pair[0];
                string value = string.Empty;

                if (pair.Length > 1)
                    value = pair[1];

                values.Add(name, value);
            }

            return values;
        }

        /// <summary>
        /// 登录模式
        /// </summary>
        public enum LoginModal
        {
            login = 0,
            _login = 1,
            _LoginInModal = 2
        }

        #endregion Help Methods

        #region 附件

        /// <summary>
        /// 购买附件的
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <param name="attachementId">附件的id</param>
        /// <returns>购买附件的链接</returns>
        public string _BuyAttachement(string tenantTypeId, long attachementId)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            dic.Add("tenantTypeId", tenantTypeId);
            dic.Add("attachementId", attachementId);
            return CachedUrlHelper.Action("_BuyAttachement", "Channel", CommonAreaName, dic);
        }

        /// <summary>
        /// 购买附件的post请求
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <param name="attachementId">附件的id</param>
        /// <returns>购买附件的postid链接</returns>
        public string BuyAttachementPost(long attachementId)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("attachementId", attachementId);
            return CachedUrlHelper.Action("BuyAttachementPost", "Channel", CommonAreaName, routeValueDictionary);
        }

        /// <summary>
        /// 附件的购买记录
        /// </summary>
        /// <param name="attachementId">附件的id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public string _BuyAttachementRecord(long attachementId, int pageIndex = 1)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            dic.Add("attachementId", attachementId);
            if (pageIndex > 1)
                dic.Add("pageIndex", pageIndex);
            return CachedUrlHelper.Action("_BuyAttachementRecord", "Channel", CommonAreaName, dic);
        }

        #endregion

        #region 视频/音乐

        /// <summary>
        /// 添加音乐
        /// </summary>
        /// <param name="textAreaId">TextAreaId</param>
        /// <returns></returns>
        public string _AddMusic(string textAreaId = null)
        {
            RouteValueDictionary routeValue = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(textAreaId))
            {
                routeValue = new RouteValueDictionary() { { "textAreaId", textAreaId } };
            }
            return CachedUrlHelper.Action("_AddMusic", "Channel", "Common", routeValue);
        }

        /// <summary>
        /// 添加音乐
        /// </summary>
        /// <param name="textAreaId">TextAreaId</param>
        /// <returns></returns>
        public string _AddVideo(string textAreaId = null)
        {
            RouteValueDictionary routeValue = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(textAreaId))
            {
                routeValue = new RouteValueDictionary() { { "textAreaId", textAreaId } };
            }
            return CachedUrlHelper.Action("_AddVideo", "Channel", "Common", routeValue);
        }

        /// <summary>
        /// 音乐详细页
        /// </summary>
        /// <param name="alias">别名</param>
        /// <returns></returns>
        public string _MusicDetail(string alias)
        {
            return CachedUrlHelper.Action("_MusicDetail", "Channel", "Common", new RouteValueDictionary() { { "alias", alias } });
        }

        /// <summary>
        /// 视频详细页
        /// </summary>
        /// <param name="alias">别名</param>
        /// <returns></returns>
        public string _VideoDetail(string alias)
        {
            return CachedUrlHelper.Action("_VideoDetail", "Channel", "Common", new RouteValueDictionary() { { "alias", alias } });
        }

        #endregion

        #region 栏目管理


        /// <summary>
        /// 资讯详情
        /// </summary>
        public string CMSDetail(long contentItemId = 0)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (contentItemId > 0)
                routeValueDictionary.Add("contentItemId", contentItemId);
            return CachedUrlHelper.Action("CMSDetail", "CMS", null, routeValueDictionary);
        }
        public string GetChildContentFolders(string contentTypeKey = null, int exceptFolderId = 0, bool? onlyModerated = null)
        {
            return CachedUrlHelper.Action("GetChildContentFolders", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "contentTypeKey", contentTypeKey }, { "exceptFolderId", exceptFolderId }, { "onlyModerated", onlyModerated } });
        }

        /// <summary>
        /// 添加编辑类别页
        /// </summary>
        public string _EditContentCategoriesLink(int contentFolderId = 0)
        {
            RouteValueDictionary dic = new RouteValueDictionary();

            if (contentFolderId != 0)
            {
                dic.Add("contentFolderId", contentFolderId);
            }

            return CachedUrlHelper.Action("_EditContentCategoriesLink", "ControlPanel", CommonAreaName, dic);
        }


        /// <summary>
        /// 合并移动
        /// </summary>
        public string _MoveContentFolder(int fromCategoryId = 0, string option = "move")
        {
            return CachedUrlHelper.Action("_MoveContentFolder", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "fromCategoryId", fromCategoryId }, { "option", option } });
        }

        /// <summary>
        /// 合并移动资讯栏目
        /// </summary>
        public string _MoveContentFolder(int fromCategoryId = 0, int CategoryId = 0, string option = "move")
        {
            return CachedUrlHelper.Action("_MoveContentFolder", "ControlPanelCms", CommonAreaName, new RouteValueDictionary { { "fromCategoryId", fromCategoryId }, { "CategoryId", CategoryId }, { "option", option } });
        }


        /// <summary>
        /// 组图资讯详情
        /// </summary>
        public string CMSImgDetail(long contentItemId = 0)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (contentItemId > 0)
                routeValueDictionary.Add("contentItemId", contentItemId);
            return CachedUrlHelper.Action("CMSImgDetail", "CMS", PanelAreaName, routeValueDictionary);
        }
        /// <summary>
        /// 视频资讯详情
        /// </summary>
        public string CMSVideoDetail(long contentItemId = 0)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (contentItemId > 0)
                routeValueDictionary.Add("contentItemId", contentItemId);
            return CachedUrlHelper.Action("CMSVideoDetail", "CMS", PanelAreaName, routeValueDictionary);
        }

        #endregion 导航管理

        #region 友情链接

        /// <summary>
        /// 管理导航
        /// </summary>
        /// <returns></returns>
        public string _ChangeLinkDisplayOrder()
        {
            return CachedUrlHelper.Action("_ChangeLinkDisplayOrder", "ControlPanel", CommonAreaName);
        }

        ///// <summary>
        ///// 创建更新链接
        ///// </summary>
        ///// <param name="linkId"></param>
        ///// <param name="tenantTypeId"></param>
        ///// <returns></returns>
        //public string _EditLink(long? linkId, string tenantTypeId)
        //{
        //    RouteValueDictionary dic = new RouteValueDictionary();
        //    if (linkId != null)
        //    {
        //        dic.Add("linkId", linkId);
        //    }
        //    dic.Add("tenantTypeId", tenantTypeId);
        //    return CachedUrlHelper.Action("_EditLink", "ControlPanel", CommonAreaName);
        //}

        /// <summary>
        /// 创建更新
        /// </summary>
        /// <returns></returns>
        public string _EditLink(long? linkId = null)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            if (linkId != null)
            {
                dic.Add("linkId", linkId);
            }
            return CachedUrlHelper.Action("_EditLink", "ControlPanel", CommonAreaName, dic);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string _DeleteLink(long linkId)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            dic.Add("linkId", linkId);
            return CachedUrlHelper.Action("_DeleteLink", "ControlPanel", CommonAreaName, dic);
        }

        /// <summary>
        /// 批量删除友情链接
        /// </summary>
        /// <returns></returns>
        public string _BatchDeleteLink()
        {
            return CachedUrlHelper.Action("_BatchDeleteLink", "ControlPanel", CommonAreaName);
        }

        #endregion

        //#region 导航管理
        ///// <summary>
        ///// 管理导航
        ///// </summary>
        ///// <param name="presentAreaKey"></param>
        ///// <returns></returns>
        //public string ManageNavigations(string presentAreaKey = null)
        //{
        //    RouteValueDictionary dic = new RouteValueDictionary();
        //    if (presentAreaKey != null)
        //    {
        //        dic.Add("presentAreaKey", presentAreaKey);
        //    }
        //    return CachedUrlHelper.Action("ManageNavigations", "ControlPanel", CommonAreaName, dic);
        //}
        ///// <summary>

        ///// <summary>
        ///// 添加导航
        ///// </summary>
        //public string _CreateNavigation(int parentNavigationId, string presentAreaKey = PresentAreaKeysOfBuiltIn.Channel)
        //{
        //    return CachedUrlHelper.Action("_CreateNavigation", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "parentNavigationId", parentNavigationId }, { "presentAreaKey", presentAreaKey } });
        //}

        ///// <summary>
        ///// 编辑导航
        ///// </summary>
        ///// <param name="navigationId"></param>
        ///// <returns></returns>
        //public string _EditNavigation(int? navigationId)
        //{
        //    RouteValueDictionary dic = new RouteValueDictionary();
        //    if (navigationId != null)
        //    {
        //        dic.Add("navigationId", navigationId);
        //    }
        //    return CachedUrlHelper.Action("_EditNavigation", "ControlPanel", CommonAreaName, dic);
        //}

        ///// <summary>
        ///// 更改显示顺序
        ///// </summary>
        //public string ChangeNavigationOrder()
        //{
        //    return CachedUrlHelper.Action("ChangeNavigationOrder", "ControlPanel", CommonAreaName);
        //}
        ///// <summary>
        ///// 设置导航状态
        ///// </summary>
        //public string setNavigationStatus()
        //{
        //    return CachedUrlHelper.Action("setNavigationStatus", "ControlPanel", CommonAreaName);
        //}
        ///// <summary>
        ///// 删除导航
        ///// </summary>
        //public string _DeleteNavigation(int navigationId)
        //{
        //    return CachedUrlHelper.Action("_DeleteNavigation", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "navigationId", navigationId } });
        //}

        ///// <summary>
        ///// 批量删除导航
        ///// </summary>
        //public string _BatchRemoveNavigation()
        //{
        //    return CachedUrlHelper.Action("_BatchRemoveNavigation", "ControlPanel", CommonAreaName);
        //}

        ///// <summary>
        ///// 查询子导航
        ///// </summary>
        ///// <param name="navigationId"></param>
        ///// <returns></returns>
        //public string ManageChildNavigations(int navigationId)
        //{
        //    RouteValueDictionary dic = new RouteValueDictionary();
        //    dic.Add("navigationId", navigationId);
        //    return CachedUrlHelper.Action("ManageChildNavigations", "ControlPanel", CommonAreaName, dic);
        //}

        //#endregion

        #region 用户相关
        /// <summary>
        /// 跳转至忘记密码的页面
        /// </summary>
        /// <returns></returns>
        public string FindPassword(string accountEmail = null, bool isPartial = false)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(accountEmail))
                routeValueDictionary.Add("accountEmail", accountEmail);
            if (isPartial)
                routeValueDictionary.Add("isPartial", isPartial);
            return CachedUrlHelper.Action("FindPassword", "Account", CommonAreaName, routeValueDictionary);
        }
        ///// <summary>
        ///// 跳转至忘记密码的页面
        ///// </summary>
        ///// <returns></returns>
        //public string FindPassword()
        //{
        //    return CachedUrlHelper.Action("FindPassword", "Account", CommonAreaName);
        //}

        #endregion

        #region 列表及列表项管理
        /// <summary>
        /// 列表管理链接
        /// </summary>
        /// <returns>返回列表管理页</returns>
        public string ListManage()
        {
            return CachedUrlHelper.Action("ListManage", "ControlPanel", CommonAreaName);
        }
        /// <summary>
        /// 添加、编辑列表链接
        /// </summary>
        /// <returns>返回添加、编辑列表页</returns>
        public string EditList()
        {
            return CachedUrlHelper.Action("EditList", "ControlPanel", CommonAreaName);
        }

        /// <summary>
        /// 添加、编辑列表链接
        /// </summary>
        /// <returns>返回添加、编辑列表页</returns>
        public string EditList(string code)
        {
            return CachedUrlHelper.Action("EditList", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "code", code } });
        }

        /// <summary>
        /// 删除列表链接
        /// </summary>
        /// <returns>返回添加、编辑列表页</returns>
        public string DeleteList(string code)
        {
            return CachedUrlHelper.Action("DeleteList", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "code", code } });
        }

        /// <summary>
        /// 验证列表编码
        /// </summary>
        /// <returns>验证结果</returns>
        public string ValidateListCode()
        {
            return CachedUrlHelper.Action("ValidateListCode", "ControlPanel", CommonAreaName);
        }

        /// <summary>
        /// 验证列表编码
        /// </summary>
        /// <returns>验证结果</returns>
        public string ValidateListItemCode()
        {
            return CachedUrlHelper.Action("ValidateListItemCode", "ControlPanel", CommonAreaName);
        }


        /// <summary>
        /// 列表管理链接
        /// </summary>
        /// <returns>返回列表管理页</returns>
        public string ListItemManage(string parentCode)
        {
            return CachedUrlHelper.Action("ListItemManage", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "parentCode", parentCode } });
        }

        /// <summary>
        /// 列表管理链接
        /// </summary>
        /// <returns>返回列表管理页</returns>
        public string ListItemManage(string name, string listCode, int? isMultilevel, int? allowAddOrDelete)
        {
            return CachedUrlHelper.Action("ListItemManage", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "name", name }, { "listCode", listCode }, { "isMultilevel", isMultilevel }, { "allowAddOrDelete", allowAddOrDelete } });
        }

        /// <summary>
        ///添加、编辑列表项链接
        /// </summary>
        /// <returns>返回添加、编辑列表项页</returns>
        public string EditListItem()
        {
            return CachedUrlHelper.Action("EditListItem", "ControlPanel", CommonAreaName);
        }
        /// <summary>
        ///添加、编辑列表项链接
        /// </summary>
        /// <returns>返回添加、编辑列表项页</returns>
        public string EditListItem(string itemName, string listCode, int? isMultilevel, int? allowAddOrDelete, long id = 0)
        {
            return CachedUrlHelper.Action("EditListItem", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "itemName", itemName }, { "listCode", listCode }, { "isMultilevel", isMultilevel }, { "allowAddOrDelete", allowAddOrDelete }, { "id", id } });
        }

        /// <summary>
        ///添加、编辑列表项链接
        /// </summary>
        /// <param name="listCode">列表编码</param>
        /// 
        /// <returns>返回添加、编辑列表项页</returns>
        public string EditListItem(string itemName, string parentCode, string listCode, int? isMultilevel, int? allowAddOrDelete)
        {
            return CachedUrlHelper.Action("EditListItem", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "itemName", itemName }, { "parentCode", parentCode }, { "listCode", listCode }, { "isMultilevel", isMultilevel }, { "allowAddOrDelete", allowAddOrDelete } });
        }

        /// <summary>
        /// 删除列表项链接
        /// </summary>
        /// <returns>返回删除列表项页</returns>
        public string DeleteListItems(long id)
        {
            return CachedUrlHelper.Action("DeleteListItems", "ControlPanel", CommonAreaName, new RouteValueDictionary { { "id", id } });
        }
        /// <summary>
        /// 列表项上移下移
        /// </summary>
        /// <returns></returns>
        public string ChangeListItemOrder()
        {
            return CachedUrlHelper.Action("ChangeListItemOrder", "ControlPanel", CommonAreaName);

        }
        #endregion

        #region 前台贴吧/贴子
        /// <summary>
        /// 贴子详情页
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public string ThreadDetail(long threadId)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            dic.Add("threadId", threadId);
            return CachedUrlHelper.Action("ThreadDetail", "Post", null, dic);
        }

        /// <summary>
        /// 贴吧详情页
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public string SectionDetail(long sectionId)
        {
            RouteValueDictionary dic = new RouteValueDictionary();
            dic.Add("sectionId", sectionId);

            return CachedUrlHelper.Action("BarSectionDetail", "Post", null, dic);
        }
        #endregion

        /// <summary>
        /// 完善资料页中转页面
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public string _Perfecthref(string url)
        {
            return CachedUrlHelper.Action("_Perfecthref", "Account", null, new RouteValueDictionary { { "url", url } });

        }
        /// <summary>
        /// 系统消息提示页面（登录时未激活、被封禁状态的提示页面）
        /// </summary>
        /// <returns></returns>
        public string SystemMessage(TempDataDictionary tempData = null, SystemMessageViewModel model = null, string returnUrl = null)
        {
            if (tempData != null && model != null)
            {
                tempData["SystemMessageViewModel"] = model;
            }
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(returnUrl))
                routeValueDictionary.Add("returnUrl", HttpUtility.UrlEncode(returnUrl));
            return CachedUrlHelper.Action("SystemMessage", "Account", null, routeValueDictionary);
        }
        /// <summary>
        /// 暂停页面
        /// </summary>
        /// <returns></returns>
        public string PausePage()
        {
            return CachedUrlHelper.Action("PausePage", "Message", null);

        }
        /// <summary>
        /// 完善资料页
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public string PerfectInformation()
        {
            return CachedUrlHelper.Action("PerfectInformation", "Account");

        }
    }

    }
