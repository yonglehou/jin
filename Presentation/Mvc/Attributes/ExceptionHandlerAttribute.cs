//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Tunynet;

namespace Tunynet.Common
{
    

    /// <summary>
    /// 用于处理异常的过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ExceptionHandlerAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// 发生异常时，跳转至异常信息显示页
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (filterContext.IsChildAction)
                return;

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            if (filterContext.ExceptionHandled)
                return;

            Exception exception = filterContext.Exception;

            ExceptionFacade exceptionFacade = new ExceptionFacade(exception.Message, exception);
            exceptionFacade.Log();

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
            // ignore it.
            if (new HttpException(null, exception).GetHttpCode() != 500)
                return;

            //过滤异步请求
            if (filterContext.HttpContext.Request.Headers.Get("X-Requested-With") != null)
                return;

            if (exception.InnerException != null && exception.InnerException is ExceptionFacade)
                exceptionFacade = exception.InnerException as ExceptionFacade;

            if (exceptionFacade == null)
                return;

            if (!filterContext.HttpContext.IsCustomErrorEnabled)
                return;
            //判断是否后台出错 后台出错跳后台错误页 前台也是
            var areaName = filterContext.Controller.ControllerContext.RouteData.DataTokens["area"];
            var Isbankend = areaName != null? areaName.ToString() == "ConsoleViews":false;
          
            Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                if (Isbankend)
                    buttonLink.Add("返回首页", SiteUrls.Instance().ControlPanelHome());
                else
                    buttonLink.Add("返回首页", SiteUrls.Instance().Home());
          
            Dictionary<string, string> bodyLink = new Dictionary<string, string>();
            bodyLink.Add("Title", "抱歉服务器出现问题！");
           var systemMessageViewModel = new SystemMessageViewModel
            {
                Body = "请稍后重新刷新页面，或点击以下按钮返回上一页或返回首页。",
                ReturnUrl = SiteUrls.Instance().ControlPanelHome(),
                Title = "500",
                StatusMessageType = StatusMessageType.Error,
                ButtonLink = buttonLink,
                BodyLink = bodyLink
            };
                //filterContext.Controller.TempData["SystemMessageViewModel"] = systemMessageViewModel;
                //验证失败后跳转页面
                if (!Isbankend)
                    filterContext.Result = new RedirectResult(SiteUrls.Instance().Error(filterContext.Controller.TempData, systemMessageViewModel));
                else
                    filterContext.Result = new RedirectResult(SiteUrls.Instance().BankEndError(filterContext.Controller.TempData, systemMessageViewModel));

                filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            

        }

    }
}
