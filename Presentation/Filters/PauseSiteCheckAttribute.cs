//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Tunynet.Common;
using Tunynet;
using System.Web;
using Tunynet.Settings;
using Tunynet.Spacebuilder;

namespace Tunynet.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class PauseSiteCheckAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.IsChildAction)
                return;

            PauseSiteSettings pauseSiteSettings = DIContainer.Resolve<ISettingsManager<PauseSiteSettings>>().Get();

            HttpContext context = HttpContext.Current;
            if (!pauseSiteSettings.IsEnable)
            {
                var routeDataDictionary = context.Request.RequestContext.RouteData.Values;
                if (!routeDataDictionary.ContainsKey("Controller"))
                    return;
                string controllerName = routeDataDictionary["Controller"].ToString();
                if (!controllerName.ToLower().Contains("controlpanel")&& !controllerName.ToLower().Contains("message"))
                {
                    if (filterContext.ActionDescriptor.ActionName.Equals("PausePage", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return;
                    }
                    else
                    {
                        Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                        filterContext.Controller.TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                        {
                            Body = pauseSiteSettings.PauseAnnouncement,
                            ReturnUrl = SiteUrls.Instance().Home(),
                            Title = "暂停站点",
                            StatusMessageType = StatusMessageType.Error,
                            ButtonLink = buttonLink
                        };
                        filterContext.Result = new RedirectResult(SiteUrls.Instance().PausePage());
                    }
                    return;
                }
            }

        }
    }
}
