//<TunynetCopyright>
//--------------------------------------------------------------
//<copyright>拓宇网络科技有限公司 2005-2012</copyright>
//<version>V0.5</verion>
//<createdate>2016-03-15</createdate>
//<author>libsh</author>
//<email>libsh@tunynet.com</email>
//<log date="2016-03-15" version="0.5">创建</log>
//--------------------------------------------------------------

using System;
using System.Web.Mvc;
using Tunynet.Common;

namespace Tunynet.Common
{
    /// <summary>
    /// 用于处理是否允许用户匿名访问的过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AnonymousVisitAttribute : ActionFilterAttribute
    {
        #region IAuthorizationFilter 成员



        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.IsChildAction)
                return;

            IUser currentUser = UserContext.CurrentUser;

            if (currentUser != null && currentUser.IsBanned)
            {
                IAuthenticationService authenticationService = DIContainer.ResolvePerHttpRequest<IAuthenticationService>();
                authenticationService.SignOut();

                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new EmptyResult();
                else
                    filterContext.Result = new RedirectResult(CachedUrlHelper.Action("Login", "Account"));

            }

            if (currentUser == null)
            {

                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new EmptyResult();
                else
                    filterContext.Result = new RedirectResult(CachedUrlHelper.Action("Login", "Account"));
                return;
            }
        }

        #endregion

    }
}
