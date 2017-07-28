//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tunynet.UI;
using Tunynet.Common;
using Tunynet.Events;
using System.Web;
using System.IO;
using Tunynet.Post;
using System.Configuration;
using Tunynet.Settings;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 通知控制器
    /// </summary>
    public class MessageController : Controller
    {

        private PauseSiteSettings pauseSiteSetting;
        private SiteSettings siteSetting;

        public MessageController(ISettingsManager<PauseSiteSettings> pauseSiteSettingsManager, ISettingsManager<SiteSettings> siteSettings)
        {
            this.pauseSiteSetting = pauseSiteSettingsManager.Get();
            this.siteSetting = siteSettings.Get();
        }
        #region 
        /// <summary>
        /// 暂停站点
        /// </summary>
        /// <returns></returns>
        public ActionResult PausePage()
        {

            ViewData["siteSettings"] = siteSetting;
            if (pauseSiteSetting.PausePageType)
                return View(pauseSiteSetting);
            else
                return Redirect(pauseSiteSetting.PauseLink);

        }
     
        #endregion

    }
}

