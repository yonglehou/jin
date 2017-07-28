//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tunynet.Common;
using Tunynet.Common.Configuration;
using Tunynet.Settings;
using Tunynet.Utilities;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 链接管理
    /// </summary>
    public static class SiteUrlsExtension
    {
        private static readonly string ControlPanelAreaName = "ConsoleViews";
        private static readonly string PanelAreaName = "";
        #region 实例
        /// <summary>
        /// 实例 , string tab = null, string tagName = null
        /// </summary>
        //public static string Lizi(this SiteUrls siteUrls)
        //{
        //RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
        //if (!string.IsNullOrEmpty(tab))
        //{
        //    routeValueDictionary.Add("tab", tab);
        //}
        //if (!string.IsNullOrEmpty(tagName))
        //{
        //    routeValueDictionary.Add("tagName", WebUtility.UrlEncode(tagName.TrimEnd('.')));
        //}
        //return CachedUrlHelper.Action("Questions", "ChannelAsk", AskAreaName, routeValueDictionary);
        //}

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳</param>
        /// <returns></returns>
        private static string GetTimeStamp(bool bflag = true)
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string ret = string.Empty;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds).ToString();
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();

            return ret;
        }

        #endregion




        #region 栏目操作
        /// <summary>
        /// 后台栏目首页
        /// </summary>
        public static string ManageContentCategories(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageContentCategories", "ControlPanel", ControlPanelAreaName);
        }
        /// <summary>
        /// 后台创建栏目
        /// </summary>
        public static string _EditContentCategories(this SiteUrls siteUrls, int parentId = 0, int categoryId = 0)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (parentId > 0)
                routeValueDictionary.Add("parentId", parentId);
            if (categoryId > 0)
                routeValueDictionary.Add("categoryId", categoryId);
            routeValueDictionary.Add("t", GetTimeStamp(false));
            return CachedUrlHelper.Action("_EditContentCategories", "ControlPanel", ControlPanelAreaName, routeValueDictionary);
        }
        /// <summary>
        /// 后台栏目顺序
        /// </summary>
        public static string ChangeContentCategories(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ChangeContentCategories", "ControlPanel", ControlPanelAreaName);
        }

        /// <summary>
        /// 后台删除栏目
        /// </summary>
        /// <param name="contentFolderId">栏目ID</param>
        /// <returns></returns>
        public static string DeleteContentCategories(this SiteUrls siteUrls, int categoryId = 0)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (categoryId > 0)
                routeValueDictionary.Add("categoryId", categoryId);
            return CachedUrlHelper.Action("DeleteContentCategories", "ControlPanel", ControlPanelAreaName, routeValueDictionary);
        }

        /// <summary>
        /// 前台栏目列表
        /// </summary>
        /// <param name="contentFolderId">栏目ID</param>
        /// <returns></returns>
        public static string CategoryCMS(this SiteUrls siteUrls, int categoryId = 0)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (categoryId > 0)
                routeValueDictionary.Add("contentCategoryId", categoryId);
            return CachedUrlHelper.Action("CategoryCMS", "CMS", PanelAreaName, routeValueDictionary);
        }
        #endregion

        #region 资讯操作
        /// <summary>
        /// 后台资讯首页
        /// </summary>
        public static string ManageCMS(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageCMS", "ControlPanel", ControlPanelAreaName);
        }


        /// <summary>
        /// 后台资讯分布页
        /// </summary>
        public static string _ListCMS(this SiteUrls siteUrls, string keyword, AuditStatus? auditStatus, int? contentCategoryId, DateTime? startDate, DateTime? endDate, int pageIndex = 1)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(keyword))
                rvd.Add("keyword", keyword);
            if (auditStatus.HasValue)
                rvd.Add("auditStatus", auditStatus);
            if (contentCategoryId.HasValue)
                rvd.Add("contentCategoryId", contentCategoryId);
            if (startDate.HasValue)
                rvd.Add("startDate", startDate.Value.ToString("yyy-MM-dd"));
            if (endDate.HasValue)
                rvd.Add("endDate", endDate.Value.ToString("yyy-MM-dd"));
            rvd.Add("pageIndex", pageIndex);
            rvd.Add("t", GetTimeStamp(false));
            return CachedUrlHelper.Action("_ListCMS", "ControlPanel", ControlPanelAreaName, rvd);
        }



        
        #endregion

        #region 贴吧操作
        /// <summary>
        /// 后台贴子管理
        /// </summary>
        public static string ManageThreads(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageThreads", "ControlPanel", ControlPanelAreaName);
        }

        /// <summary>
        /// 后台贴吧管理
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string ManageSections(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageSections", "ControlPanel", ControlPanelAreaName);
        }


        /// <summary>
        /// 后台贴贴子分布页
        /// </summary>
        public static string _ListThreads(this SiteUrls siteUrls, string keyword, AuditStatus? auditStatus, long? sectionId, DateTime? startDate, DateTime? endDate, int pageSize = 20, int pageIndex = 1)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
          
            rvd.Add("keyword", keyword);
            rvd.Add("auditStatus", auditStatus);
            rvd.Add("sectionId", sectionId);
            if (startDate.HasValue)
            rvd.Add("startDate", startDate.Value.ToString("yyyy-MM-dd"));
            if (endDate.HasValue)
                rvd.Add("endDate", endDate.Value.ToString("yyyy-MM-dd"));

            rvd.Add("pageSize", pageSize);
            rvd.Add("pageIndex", pageIndex);
            rvd.Add("t", GetTimeStamp(false));
            return CachedUrlHelper.Action("_ListThreads", "ControlPanel", ControlPanelAreaName, rvd);
        }

        #endregion


        #region 注册&&登录
        /// <summary>
        /// 登录页面
        /// </summary>
        public static string Login(this SiteUrls siteUrls, bool isEmail)
        {
            return CachedUrlHelper.Action("Login", "Account");

        }
        /// <summary>
        /// 登出页面
        /// </summary>
        public static string SignOut(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("SignOut", "Account");

        }

        /// <summary>
        /// 发送激活邮件
        /// </summary>
        /// <param name="accountEmail">邮箱</param>
        /// <param name="token">登录凭证</param>
        /// <returns></returns>
        public static string _ActivateByEmail(this SiteUrls siteUrls, string accountEmail, long userId)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("accountEmail", accountEmail);
            routeValueDictionary.Add("userId", userId);
            return CachedUrlHelper.Action("_ActivateByEmail", "Account", null, routeValueDictionary);
        }
        /// <summary>
        /// 激活页面
        /// </summary>
        /// <param name="accountEmail">邮箱</param>
        /// <param name="token">登录凭证</param>
        /// <returns></returns>
        public static string ValideMailActive(this SiteUrls siteUrls, string token = null, bool change = false)
        {
            return CachedUrlHelper.Action("ValideMailActive", "Account", null, new RouteValueDictionary { { "token", token }, { "change", change } });
        }

        /// <summary>
        /// 明文密文切换
        /// </summary>
        /// <returns></returns>
        public static string _PassWordPoclaimed(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("_PassWordPoclaimed", "Account");
        }
        /// <summary>
        /// 注册页面
        /// </summary>
        public static string Register(this SiteUrls siteUrls, bool? isEmail)
        {
            var userSetting = DIContainer.Resolve<ISettingsManager<UserSettings>>().Get();
            if (isEmail.HasValue)
            {
                if (isEmail.Value)
                    return CachedUrlHelper.Action("EmailRegister", "Account");
                else
                    return CachedUrlHelper.Action("PhoneRegister", "Account");
            }
            switch (userSetting.RegisterType)
            {
                case RegisterType.MobileOrEmail:
                case RegisterType.Mobile:
                    return CachedUrlHelper.Action("PhoneRegister", "Account");
                case RegisterType.EmailOrMobile:
                case RegisterType.Email:
                    return CachedUrlHelper.Action("EmailRegister", "Account");

            }
            return CachedUrlHelper.Action("PhoneRegister", "Account");
        }

    




       
        

        /// <summary>
        /// 找回密码
        /// </summary>
        public static string ResetPassword(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ResetPassword", "Account");

        }

        #endregion

        #region 用户设置

        /// <summary>
        /// 资料分布页
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string _UserProfile(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("_UserProfile", "UserSpace");

        }

        /// <summary>
        /// 用户设置
        /// </summary>
        public static string UserSetting(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("UserSetting", "UserSpace");
        }
        /// <summary>
        /// 我的等级
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string MyRank(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("MyRank", "UserSpace");
        }
        /// <summary>
        /// 帐号绑定
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string AccountBinding(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("AccountBinding", "UserSpace");
        }

        /// <summary>
        /// 第三方授权绑定
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <param name="accountTypeKey"></param>
        /// <returns></returns>
        public static string ThirdBinding(this SiteUrls siteUrls, string accountTypeKey)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(accountTypeKey))
                routeValueDictionary.Add("accountTypeKey", accountTypeKey);

            return CachedUrlHelper.Action("ThirdBinding", "UserSpace", null, routeValueDictionary);
        }

        /// <summary>
        /// 帐号绑定返回页面
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <param name="accountTypeKey"></param>
        /// <returns></returns>
        public static string ThirdBindingCallBack(this SiteUrls siteUrls, string accountTypeKey)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(accountTypeKey))
                routeValueDictionary.Add("accountTypeKey", accountTypeKey);

            return CachedUrlHelper.Action("ThirdBindingCallBack", "UserSpace", null, routeValueDictionary);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string CancelBinding(this SiteUrls siteUrls, long id)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (id > 0)
                routeValueDictionary.Add("id", id);

            return CachedUrlHelper.Action("CancelBinding", "UserSpace", null, routeValueDictionary);
        }

        /// <summary>
        /// 我的通知
        /// </summary>
        /// <returns></returns>
        public static string MyNotice(this SiteUrls siteUrls)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("MyNotice", "UserSpace", null, routeValueDictionary);
        }

        #endregion

        #region 用户操作
        /// <summary>
        /// 用户选择器
        /// </summary>
        public static string GetAllOguUser(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("GetAllOguUser", "Common");
        }

        /// <summary>
        /// 关注、取关用户
        /// </summary>
        /// <param name="targetUserId">目标用户id</param>
        /// <returns></returns>
        public static string Follow(this SiteUrls siteUrls, long targetUserId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("targetUserId", targetUserId);
            return CachedUrlHelper.Action("_FollowUser", "UserSpace", "", rvd);
        }

        /// <summary>
        /// 邀请链接地址
        /// </summary>
        /// <returns></returns>
        public static string InviteRegister(this SiteUrls siteUrls, string invitationCode)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("invitationCode", invitationCode);
            return SiteUrls.FullUrl(CachedUrlHelper.Action("Invite", "Account", "", rvd));
        }
        #endregion

        #region 设置操作
        /// <summary>
        /// 站点设置
        /// </summary>
        public static string ManageSiteSettings(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageSiteSettings", "ControlPanel");
        }

        #endregion

        #region 第三方登录

        /// <summary>
        /// 第三方登录
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <param name="accountTypeKey"></param>
        /// <returns></returns>
        public static string LoginToThird(this SiteUrls siteUrls, string accountTypeKey)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(accountTypeKey))
                routeValueDictionary.Add("accountTypeKey", accountTypeKey);

            return CachedUrlHelper.Action("LoginToThird", "Account", null, routeValueDictionary);
        }

        /// <summary>
        /// 首次登录网站完善资料页
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string ThirdRegister(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ThirdRegister", "Account");
        }

        /// <summary>
        /// 关联已有帐号
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string AssociateAccount(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("AssociateAccount", "Account");
        }

        /// <summary>
        /// 关联新帐号(邮箱)
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string AssociateEmail(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("AssociateEmail", "Account");
        }

        /// <summary>
        /// 关联新帐号(手机号)
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string AssociatePhone(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("AssociatePhone", "Account");
        }

        #endregion

        #region 用户管理
        /// <summary>
        /// 后台用户管理
        /// </summary>
        public static string ManageUser(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageUsers", "ControlPanel", ControlPanelAreaName);
        }

        /// <summary>
        /// 后台角色管理
        /// </summary>
        /// <returns></returns>
        public static string ManageRoles(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageRoles", "ControlPanel", ControlPanelAreaName);
        }

        /// <summary>
        /// 用户列表分布页
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="role">用户角色名</param>
        /// <param name="state">用户状态</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public static string _ListUsers(this SiteUrls SiteUrls, string keyword, long roleId, string state, DateTime? startDate, DateTime? endDate)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("keyword", keyword);
            rvd.Add("roleId", roleId);
            rvd.Add("state", state);
            if (startDate.HasValue)
                rvd.Add("startDate", startDate.Value.ToString("yyyy-MM-dd"));
            if (endDate.HasValue)
                rvd.Add("endDate", endDate.Value.ToString("yyyy-MM-dd"));
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_ListUsers", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 修改密码分布页
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public static string _ChangePassword(this SiteUrls SiteUrls, long userId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("userId", userId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_ChangePassword", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 封禁用户分布页
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public static string _BanUser(this SiteUrls SiteUrls, long userId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("userId", userId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_BanUser", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 设置用户角色分布页
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public static string _SetRole(this SiteUrls SiteUrls, long userId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("userId", userId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_SetRole", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 编辑用户分布页
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public static string _EditUser(this SiteUrls SiteUrls, long userId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("userId", userId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_EditUser", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 奖惩用户分布页
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public static string _RewardUser(this SiteUrls SiteUrls, long userId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("userId", userId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_RewardUser", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        public static string _EditRole(this SiteUrls SiteUrls, long roleId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("roleId", roleId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_EditRole", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 第三方登录管理
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string ManageThirdLogin(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageThirdLogin", "ControlPanel", ControlPanelAreaName);
        }
        #endregion

        #region 推荐管理

        #region 推荐类别管理

        /// <summary>
        /// 推荐类别列表
        /// </summary>
        /// <returns></returns>
        public static string _ListSpecialContentTypes(this SiteUrls siteUrls)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_ListSpecialContentTypes", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 推荐类别编辑
        /// </summary>
        /// <returns></returns>
        public static string _EditSpecialContentType(this SiteUrls siteUrls, int specialContentTypeId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("typeId", specialContentTypeId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_EditSpecialContentType", "ControlPanel", ControlPanelAreaName, rvd);
        }

        #endregion

        #region 内容推荐管理

        /// <summary>
        /// 内容推荐管理页
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string ManageContentItems(this SiteUrls siteUrls)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            return CachedUrlHelper.Action("ManageSpecialContentItems", "ControlPanel", ControlPanelAreaName, rvd);
        }

        #endregion

        #region 前台内容推荐管理

        /// <summary>
        /// 推荐内容(使用时在调用页面添加Uploader的引用)
        /// @Styles.Render("~/Bundle/Styles/Uploader")
        /// @Scripts.Render("~/Bundle/Scripts/Uploader")
        /// 提交表单回调函数为recommendCallBack
        /// </summary>
        /// <param name="itemId">推荐内容的Id</param>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="itemName">推荐内容标题</param>
        /// <param name="featuredImageAttachmentId">推荐内容标题图Url</param>
        /// <param name="typeId">推荐类型Id（编辑时使用此参数）</param>
        /// <param name="specialContentItemId">推荐内容唯一标识Id</param>
        /// <returns></returns>
        public static string _RecommendContent(this SiteUrls siteUrls, long itemId, string tenantTypeId, string itemName, long featuredImageAttachmentId = 0, int typeId = 0, long specialContentItemId = 0)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("itemId", itemId);
            rvd.Add("tenantTypeId", tenantTypeId);
            //对标题转码（IE8）
            itemName = HttpUtility.UrlEncode(itemName);
            rvd.Add("title", itemName);
            rvd.Add("featuredImageAttachmentId", featuredImageAttachmentId);
            rvd.Add("TypeId", typeId);
            rvd.Add("specialContentItemId", specialContentItemId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_RecommendContent", "Common", "", rvd);
        }

        /// <summary>
        /// 前台内容管理
        /// </summary>
        /// <param name="topNumber">选取前n个</param>
        /// <param name="typeId">类型Id</param>
        /// <returns></returns>
        public static string _ManageSpecialContentItems(this SiteUrls siteUrls, int typeId, int topNumber = 10)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("requestTime", GetTimeStamp(false));
            rvd.Add("typeId", typeId);
            rvd.Add("topNumber", topNumber);
            return CachedUrlHelper.Action("_ManageSpecialContentItems", "Common", "", rvd);
        }

        /// <summary>
        /// 前台内容管理列表
        /// </summary>
        /// <param name="topNumber">选取前n个</param>
        /// <param name="typeId">类型Id</param>
        /// <returns></returns>
        public static string _ListSpecialContentItems(this SiteUrls siteUrls, int typeId, int topNumber = 10)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("requestTime", GetTimeStamp(false));
            rvd.Add("typeId", typeId);
            rvd.Add("topNumber", topNumber);
            return CachedUrlHelper.Action("_ListSpecialContentItems", "Common", "", rvd);
        }

        /// <summary>
        /// 首页幻灯片
        /// </summary>
        /// <returns></returns>
        public static string _HomePageSlider(this SiteUrls siteUrls)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_HomePageSlider", "Common", "", rvd);
        }
        #endregion

        #endregion

        #region 广告管理
        /// <summary>
        /// 后台广告管理
        /// </summary>
        /// <returns></returns>
        public static string ManageAdvertising(this SiteUrls siteUrls, long? positionId=0)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("positionId", positionId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("ManageAdvertising", "ControlPanel", ControlPanelAreaName,rvd);
        }

        /// <summary>
        /// 广告列表
        /// </summary>
        /// <returns></returns>
        public static string _ListAdvertising(this SiteUrls siteUrls, long? positionId=0)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("positionId", positionId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_ListAdvertising", "ControlPanel", ControlPanelAreaName, rvd);
        }

        public static string _EditAdvertising(this SiteUrls siteUrls, long? advertisingId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("advertisingId", advertisingId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_EditAdvertising", "ControlPanel", ControlPanelAreaName, rvd);
        }
        /// <summary>
        /// 后台广告位管理
        /// </summary>
        /// <returns></returns>
        public static string ManageAdvertisingPosition(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageAdvertisingPosition", "ControlPanel", ControlPanelAreaName);
        }

        /// <summary>
        /// 编辑广告位
        /// </summary>
        /// <param name="positionId">广告位Id</param>
        /// <returns></returns>
        public static string _EditPosition(this SiteUrls siteUrls, long? positionId=0)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("positionId", positionId);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_EditPosition", "ControlPanel", ControlPanelAreaName, rvd);
        }

        /// <summary>
        /// 广告位列表
        /// </summary>
        /// <param name="height">建议高度</param>
        /// <param name="siteUrls">建议宽度</param>
        /// <returns></returns>
        public static string _ListPositions(this SiteUrls siteUrls, int height = 0, int width = 0)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("height", height);
            rvd.Add("width", width);
            rvd.Add("requestTime", GetTimeStamp(false));
            return CachedUrlHelper.Action("_ListPositions", "ControlPanel", ControlPanelAreaName, rvd);
        }


        #endregion

        #region 通用评论
        /// <summary>
        /// （通用）评论列表
        /// </summary>
        /// <param name="tenantType">评论的租户类型id</param>
        /// <param name="commentedObjectId">被评论对象id</param>
        /// <param name="sortBy">排序方式</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns>评论列表</returns>
        public static string _CommentList(this SiteUrls siteUrls, string tenantTypeId, long commentedObjectId, SortBy_Comment sortBy = SortBy_Comment.DateCreated, int pageIndex = 1)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("tenantTypeId", tenantTypeId);
            routeValueDictionary.Add("commentedObjectId", commentedObjectId);
            if (sortBy != SortBy_Comment.DateCreated)
                routeValueDictionary.Add("sortBy", sortBy);
            if (pageIndex != 1)
                routeValueDictionary.Add("pageIndex", pageIndex);
            return CachedUrlHelper.Action("_CommentList", "portal", PanelAreaName, routeValueDictionary);
        }

        /// <summary>
        /// 子级评论局部页面
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <returns>子级评论局部页面的链接</returns>
        public static string _ChildComment(this SiteUrls siteUrls, string tenantTypeId, long? parentId = null)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("tenantTypeId", tenantTypeId);
            if (parentId.HasValue)
                routeValueDictionary.Add("parentId", parentId);
            return CachedUrlHelper.Action("_ChildComment", "portal", PanelAreaName, routeValueDictionary);
        }

        ///// <summary>
        ///// （通用）子级评论列表
        ///// </summary>
        ///// <param name="parentId">父级评论列表id</param>
        ///// <param name="pageIndex">当前页码</param>
        ///// <param name="sortBy">排序方式</param>
        ///// <returns>排序方式</returns>
        //public static string _ChildCommentList(this SiteUrls siteUrls,long parentId, int pageIndex = 1, SortBy_Comment sortBy = SortBy_Comment.DateCreatedDesc)
        //{
        //    RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
        //    routeValueDictionary.Add("parentId", parentId);
        //    if (pageIndex > 1)
        //        routeValueDictionary.Add("pageIndex", pageIndex);
        //    if (sortBy != SortBy_Comment.DateCreatedDesc)
        //        routeValueDictionary.Add("sortBy", sortBy);
        //    return CachedUrlHelper.Action("_ChildCommentList", "portal", PanelAreaName, routeValueDictionary);
        //}

        /// <summary>
        /// (通用)删除评论
        /// </summary>
        /// <returns></returns>
        public static string _DeleteComment(this SiteUrls siteUrls, long commentId)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("commentId", commentId);
            return CachedUrlHelper.Action("_DeleteComment", "portal", PanelAreaName, routeValueDictionary);
        }

        /// <summary>
        /// 评论控件
        /// </summary>
        public static string _Comment(this SiteUrls siteUrls, long parentId, string tenantTypeId, long commentedObjectId)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add("parentId", parentId);
            routeValueDictionary.Add("tenantTypeId", tenantTypeId);
            routeValueDictionary.Add("commentedObjectId", commentedObjectId);
            return CachedUrlHelper.Action("_Comment", "portal", PanelAreaName, routeValueDictionary);
        }

        #endregion


        #region 操作日志

        /// <summary>
        /// 操作日志管理
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string ManageOperationLogs(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageOperationLogs", "ControlPanel", ControlPanelAreaName);
        }

        #endregion


        #region 类别管理

        /// <summary>
        /// 类别管理页面
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string ManageCategories(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageCategories", "ControlPanel", ControlPanelAreaName);
        }
        
        #endregion

        #region 评论管理

        public static string ManageComments(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageComments", "ControlPanel", ControlPanelAreaName);
        }

        #endregion

        #region 积分规则
        public static string Managepointrules(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("Managepointrules", "ControlPanel", ControlPanelAreaName);
        }
        #endregion

        #region 积分记录
        public static string Managepointrecords(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("Managepointrecords", "ControlPanel", ControlPanelAreaName);
        }
        #endregion
        #region 等级管理
        public static string ManageUserRanks(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageUserRanks", "ControlPanel", ControlPanelAreaName);
        }
        #endregion
        #region 标签管理
        public static string ManageTags(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageTags", "ControlPanel", ControlPanelAreaName);
        }
        #endregion

        #region 友情链接
        public static string ManageLinks(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageLinks", "ControlPanel", ControlPanelAreaName);
        }
        #endregion

        #region 权限管理
        public static string ManagePermissions(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManagePermissions", "ControlPanel", ControlPanelAreaName);
        }

        #endregion

        #region 审核规则
        public static string ManageAudits(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageAudits", "ControlPanel", ControlPanelAreaName);
        }

        #endregion

        #region 工具

        /// <summary>
        /// 索引管理
        /// </summary>
        /// <param name="siteUrls"></param>
        /// <returns></returns>
        public static string ManageSearchIndexs(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageSearchIndexs", "ControlPanel", ControlPanelAreaName);
        }

        /// <summary>
        /// 立即执行任务
        /// </summary>
        /// <param name="id">任务Id</param>
        /// <returns></returns>
        public static string RunTask(this SiteUrls siteUrls,int? id)
        {
            RouteValueDictionary routeData = new RouteValueDictionary();
            if (id.HasValue)
            {
                routeData.Add("id", id);
            }
            return CachedUrlHelper.Action("RunTask", "ControlPanel", ControlPanelAreaName, routeData);
        }
        /// <summary>
        /// 任务管理
        /// </summary>
        /// <param name="id">任务Id</param>
        /// <returns></returns>
        public static string ManageTasks(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageTasks", "ControlPanel", ControlPanelAreaName);
        }

        #region 重启站点
        /// <summary>
        /// 重启站点
        /// </summary>
        /// <returns></returns>
        public static string UnloadAppDomain(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("_UnloadAppDomain", "ControlPanel", ControlPanelAreaName);
        }
        /// <summary>
        /// 暂停站点
        /// </summary>
        /// <returns></returns>
        public static string PauseSiteSettings(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("PauseSiteSettings", "ControlPanel", ControlPanelAreaName);
        }
        #endregion
        #region 清除缓存
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        public static string ResetCache(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("_ResetCache", "ControlPanel", ControlPanelAreaName);
        }
        #endregion
        /// <summary>
        /// 暂停页面
        /// </summary>
        /// <returns></returns>
        public static string PausePage(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("PausePage", "ControlPanel", ControlPanelAreaName);

        }
        #endregion

        #region 导航管理

        public static string ManageNavigations(this SiteUrls siteUrls)
        {
            return CachedUrlHelper.Action("ManageNavigations", "ControlPanel", ControlPanelAreaName);
        }
        #endregion

    }
}
