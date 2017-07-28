//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户上下文
    /// </summary>
    public static class UserContext
    {
        static readonly string[] UserControllers = { "account", "mall", "pay", "appmall", "" };
        static readonly string[] StoreControllers = { "store", "storeaccount" };
        static readonly string[] AdminControllers = { "controlpanel", "controlpanelaccount", "operation", "trading" };
        /// <summary>
        /// 获取当前用户
        /// </summary>
        public static IUser CurrentUser
        {
            get
            {
                IUserService userService = DIContainer.Resolve<IUserService>();
#if DEBUG
                //return userService.GetUser(123456789);    //houlp 会员
                //if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("appmall"))
                //{
                //    return userService.GetUser(1122860334245);      //houlp 会员
                //}
#endif
                IAuthenticationService authenticationService = DIContainer.ResolvePerHttpRequest<FormsAuthenticationService>();
                //return userService.GetUser(1553066489672); //cccccc  业务员
                var currentUser = authenticationService.GetAuthenticatedUser();
                if (currentUser != null)
                    return currentUser;

                #region cookie禁用情况

                //string token = string.Empty;
                //if (HttpContext.Current != null && HttpContext.Current.Request != null)
                //{

                //    token = HttpContext.Current.Request.Form.Get<string>("CurrentUserIdToken", string.Empty);

                //    if (string.IsNullOrEmpty(token))
                //        token = HttpContext.Current.Request.QueryString.Get<string>("CurrentUserIdToken", string.Empty);
                //}

                //if (!string.IsNullOrEmpty(token))
                //{
                //    token = Tunynet.Utilities.WebUtility.UrlDecode(token);
                //    bool isTimeOut = false;
                //    long userId = Utility.DecryptTokenForUploadfile(token.ToString(), out isTimeOut);
                //    if (userId > 0)
                //    {
                //        currentUser = userService.GetUser(userId);
                //        if (currentUser != null)
                //        {
                //            return currentUser;
                //        }
                //    }
                //}

                #endregion
                return null;
            }
        }
    }
}