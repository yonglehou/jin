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

namespace Tunynet.Common
{
    /// <summary>
    /// 自定义的视图引擎，支持前台多个Theme和独立的后台（Console）
    /// </summary>
    public class TunynetViewEngine : RazorViewEngine
    {

        public TunynetViewEngine()
            : this(null)
        {
        }

        public TunynetViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {

            string theme = System.Configuration.ConfigurationManager.AppSettings["theme"];
            if (string.IsNullOrEmpty(theme))
                theme = "Default";

            ViewLocationFormats = new string[]
            {
                "~/Themes/" + theme + "/{1}/{0}.cshtml",
                "~/Themes/" + theme + "/Shared/{0}.cshtml"
            };

            MasterLocationFormats = new string[]
            {
                "~/Themes/" + theme + "/{1}/{0}.cshtml",
                "~/Themes/" + theme + "/Shared/{0}.cshtml"
            };

            PartialViewLocationFormats = new string[]
            {
                "~/Themes/" + theme + "/{1}/{0}.cshtml",
                "~/Themes/" + theme + "/Shared/{0}.cshtml"
            };

            AreaViewLocationFormats = new string[]
            {
                "~/{2}/{1}/{0}.cshtml",
                "~/{2}/Shared/{0}.cshtml"
            };

            AreaMasterLocationFormats = new string[]
            {
                "~/{2}/{1}/{0}.cshtml",
                "~/{2}/Shared/{0}.cshtml"
            };

            AreaPartialViewLocationFormats = new string[]
            {
                "~/{2}/{1}/{0}.cshtml",
                "~/{2}/Shared/{0}.cshtml"
            };

            FileExtensions = new string[]
            {
                "cshtml"
            };
        }

    }
}
