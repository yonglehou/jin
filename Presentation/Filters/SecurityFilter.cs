//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Text;
using System.Web.Mvc;

namespace Tunynet.Common
{
    /// <summary>
    /// 安全校验过滤器
    /// </summary>
    public class SecurityFilter : IAuthorizationFilter, IActionFilter
    {
        /// <summary>
        /// 校验CSRF Token
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.Equals("post", filterContext.HttpContext.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) && filterContext.HttpContext.Request.Form.Count > 0 && !filterContext.HttpContext.Request.Path.StartsWith("/common/") && !filterContext.HttpContext.Request.Path.StartsWith("/Common/"))
            {
                ValidateAntiForgeryTokenAttribute _validator = new ValidateAntiForgeryTokenAttribute();
                try
                {
                    _validator.OnAuthorization(filterContext);
                }
                catch (Exception ex)
                {
                    filterContext.Result = new JsonResult() { Data = new StatusMessageData(StatusMessageType.Error, "输入有误，安全校验失败！") };
                    return;
                }
            }
        }

        /// <summary>
        /// 校验ModelState
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (string.Equals("post", filterContext.HttpContext.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) && !filterContext.HttpContext.Request.Path.StartsWith("/account/resetpassword"))
            //{
            //    if (!filterContext.Controller.ViewData.ModelState.IsValid)
            //    {
            //        var errorMessage = new StringBuilder();
            //        foreach (var val in filterContext.Controller.ViewData.ModelState.Values)
            //        {
            //            foreach (var error in val.Errors)
            //            {
            //                errorMessage.Append(error.ErrorMessage).Append(";");
            //            }
            //        }
            //        filterContext.Result = new JsonResult() { Data = new StatusMessageData(StatusMessageType.Error, "输入有误: " + errorMessage) };
            //        return;
            //    }
            //}
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}