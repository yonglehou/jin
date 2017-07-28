////------------------------------------------------------------------------------
//// <copyright company="Tunynet">
////     Copyright (c) Tunynet Inc.  All rights reserved.
//// </copyright> 
////------------------------------------------------------------------------------

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;

//namespace Tunynet.Common
//{
//    /// <summary>
//    /// 后台身份验证
//    /// </summary>
//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
//    public class ManageAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
//    {
//        private bool requireSystemAdministrator = false;
//        /// <summary>
//        /// 是否需要系统管理员权限
//        /// </summary>
//        public bool RequireSystemAdministrator
//        {
//            get { return requireSystemAdministrator; }
//            set { requireSystemAdministrator = value; }
//        }

//        private bool checkCookie = true;
//        /// <summary>
//        /// 是否需要检查Cookie
//        /// </summary>
//        public bool CheckCookie
//        {
//            get { return this.checkCookie; }
//            set { this.checkCookie = value; }
//        }

//        #region IAuthorizationFilter 成员
//        public void OnAuthorization(AuthorizationContext filterContext)
//        {
//            if (filterContext == null)
//            {
//                throw new ArgumentNullException("filterContext");
//            }

//            if (!AuthorizeCore(filterContext))
//            {
//                // auth failed, redirect to login page
//                // filterContext.Cancel = true;
//                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
//                {
//                    filterContext.Result = new JsonResult() { Data = new StatusMessageData(StatusMessageType.Hint, "您必须先以管理员身份登录下后台，才能继续操作") };
//                }
//                else
//                {
//                    filterContext.Controller.TempData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Hint, "请以管理员身份登录");
//                    filterContext.Result = new RedirectResult(SiteUrls.Instance().CPLogin());
//                }
//                return;
//            }
//        }

//        #endregion

//        // This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.
//        protected virtual bool AuthorizeCore(AuthorizationContext filterContext)
//        {
//            IUser currentUser = UserContext.CurrentAdmin;
//            if (currentUser == null)
//                return false;

//            if (CheckCookie)
//            {
//                var urlPart = filterContext.HttpContext.Request.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
//                HttpCookie adminCookie = filterContext.HttpContext.Request.Cookies["ln_admincookie_" + urlPart[0] + "_" + currentUser.UserId];

//                if (adminCookie != null)
//                {
//                    bool isLoginMarked = false;
//                    try
//                    {
//                        var valuePart = Utility.DecryptTokenForCookie(adminCookie.Value).Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
//                        bool.TryParse(valuePart[0], out isLoginMarked);

//                        DateTime currentTime = DateTime.Now;

//                        int cookieTotalMinutes = valuePart[1].ConvertToInt32(),
//                            currentTotalMinutes = (int)currentTime.TimeOfDay.TotalMinutes;

//                        if (cookieTotalMinutes < currentTotalMinutes)
//                        {
//                            adminCookie.Expires = DateTime.Now.AddDays(-1);
//                            filterContext.HttpContext.Response.Cookies.Set(adminCookie);

//                            return false;
//                        }
//                        else if (cookieTotalMinutes - currentTotalMinutes < 5)
//                        {
//                            adminCookie.Expires.AddMinutes(1);
//                            filterContext.HttpContext.Response.Cookies.Set(adminCookie);
//                        }

//                    }
//                    catch { }

//                    if (!isLoginMarked)
//                        return false;
//                }
//                else
//                {
//                    return false;
//                }
//            }

//            RoleService roleService = new RoleService();
//            if (RequireSystemAdministrator)
//            {
//                if (roleService.IsUserInRoles(currentUser.UserId, RoleNames.Instance().SuperAdministrator()))
//                    return true;
//                else
//                    return false;
//            }
//            else
//            {
//                var urlPart = HttpContext.Current.Request.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
//                List<string> roles = new List<string> { RoleNames.Instance().SuperAdministrator(), RoleNames.Instance().ContentAdministrator() };
//                var user = new UserService().GetFullUser(UserContext.CurrentAdmin.UserId);

//                if (urlPart[0] == "customer")
//                {
//                    roles.Add(RoleNames.Instance().Customer());
//                }
//                else if (urlPart[0] == "financial")
//                {
//                    roles.Add(RoleNames.Instance().Financial());
//                }
//                else if (urlPart[0] == "purchase")
//                {
//                    roles.Add(RoleNames.Instance().PurchasingManager());
//                    roles.Add(RoleNames.Instance().PurchasingAgent());
//                }
//                else if (urlPart[0] == "supplier")
//                {
//                    roles.Add(RoleNames.Instance().Supplier());
//                }
//                if (user.Roles.Contains("UserAdministrator"))
//                {
//                    roles.Add(RoleNames.Instance().UserAdministrator());
//                }
//                else if (user.Roles.Contains("ProductAdministrator"))
//                {
//                    roles.Add(RoleNames.Instance().ProductAdministrator());
//                }
//                else if (user.Roles.Contains("PointAdministrator"))
//                {
//                    roles.Add(RoleNames.Instance().PointAdministrator());
//                }
//                else if (user.Roles.Contains("TeamAdministrator"))
//                {
//                    roles.Add(RoleNames.Instance().TeamAdministrator());
//                }

//                if (roleService.IsUserInRoles(currentUser.UserId, roles.ToArray()))
//                    return true;
//            }

//            return currentUser.IsAllowEntryControlPannel();
//        }

//        /// <summary>
//        /// 从路由数据获取AreaName
//        /// </summary>
//        /// <param name="routeData"></param>
//        /// <returns></returns>
//        private string GetAreaName(RouteData routeData)
//        {
//            object area;
//            if (routeData.DataTokens.TryGetValue("area", out area))
//            {
//                return area as string;
//            }

//            return GetAreaName(routeData.Route);
//        }

//        /// <summary>
//        /// 从路由数据获取AreaName
//        /// </summary>
//        /// <param name="route"><see cref="RouteBase"/></param>
//        /// <returns>返回路由中的AreaName，如果无AreaName则返回null</returns>
//        private string GetAreaName(RouteBase route)
//        {
//            IRouteWithArea routeWithArea = route as IRouteWithArea;
//            if (routeWithArea != null)
//                return routeWithArea.Area;

//            Route castRoute = route as Route;
//            if (castRoute != null && castRoute.DataTokens != null)
//                return castRoute.DataTokens["area"] as string;

//            return null;
//        }


//    }
//}