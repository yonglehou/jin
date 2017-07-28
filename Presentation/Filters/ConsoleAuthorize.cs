//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Web.Mvc;
using System.Web;
using Tunynet.Spacebuilder;
using System.Collections.Generic;

namespace Tunynet.Common
{
    /// <summary>
    /// 后台身份验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ConsoleAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region IAuthorizationFilter 成员

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                IAuthenticationService authenticationService = DIContainer.ResolvePerHttpRequest<FormsAuthenticationService>();
                authenticationService.SignOut();
                filterContext.Result = new RedirectResult(SiteUrls.Instance().Login(HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl)));

            }
            IUser user = UserContext.CurrentUser;
            if (user == null)
                filterContext.Result = new RedirectResult(SiteUrls.Instance().Login(HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl)));
            else if (!AuthorizeCore(user))
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("首页", SiteUrls.Instance()._Perfecthref(SiteUrls.Instance().Home()));
                filterContext.Controller.TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "您可能没有权限查看此页面,<br/><span id='seconds'>5</span>秒后，自动跳转到",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "无权查看",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink
                };

                filterContext.Result = new RedirectResult(SiteUrls.Instance().SystemMessage());

            }
        }

        #endregion

        // This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.
        protected virtual bool AuthorizeCore(IUser user)
        {

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


    }
}