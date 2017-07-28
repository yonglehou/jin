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
using Tunynet.Settings;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 用户身份验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class UserAuthorizeAttribute : ActionFilterAttribute
    {
        #region IAuthorizationFilter 成员
        //是否匿名过滤
        public bool isAnonymous { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = UserContext.CurrentUser;

            //匿名访问过滤
            if (isAnonymous)
            {
                 SiteSettings siteSettings = DIContainer.Resolve<ISettingsManager<SiteSettings>>().Get();
                if (!siteSettings.EnableAnonymousBrowse&& user==null)
                {

                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                        filterContext.Result = new EmptyResult();
                    else
                        filterContext.Result = new RedirectResult(SiteUrls.Instance().Login(HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl)));
                }
            }
            else
            {
                UserProfileService userProfileService = DIContainer.Resolve<UserProfileService>();
           
                if (user == null)
                {
                    IAuthenticationService authenticationService = DIContainer.ResolvePerHttpRequest<FormsAuthenticationService>();
                    authenticationService.SignOut();
                    filterContext.Result = new RedirectResult(SiteUrls.Instance().Login(HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl)));
                }
                else
                {
                 
                }
            }
           
        }
       

        #endregion

        /// <summary>
        /// 从路由数据获取AreaName
        /// </summary>
        /// <param name="routeData"></param>
        /// <returns></returns>
        private string GetAreaName(RouteData routeData)
        {
            object area;
            if (routeData.DataTokens.TryGetValue("area", out area))
            {
                return area as string;
            }

            return GetAreaName(routeData.Route);
        }

        /// <summary>
        /// 从路由数据获取AreaName
        /// </summary>
        /// <param name="route"><see cref="RouteBase"/></param>
        /// <returns>返回路由中的AreaName，如果无AreaName则返回null</returns>
        private string GetAreaName(RouteBase route)
        {
            IRouteWithArea routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
                return routeWithArea.Area;

            Route castRoute = route as Route;
            if (castRoute != null && castRoute.DataTokens != null)
                return castRoute.DataTokens["area"] as string;

            return null;
        }

    }
}