//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Configuration;
using System.Web.Mvc;
using Tunynet.Common;


namespace Tunynet.Spacebuilder
{
    public class UrlRoutingRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "ConsoleViews"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //后台
            context.MapRoute(
                name: "ControlPanel",
                url: "ControlPanel/{action}",
                defaults: new { controller = "ControlPanel", action = "Home" }
            );
            //后台
            context.MapRoute(
                name: "ControlPanelAsk",
                url: "ControlPanelAsk/{action}",
                defaults: new { controller = "ControlPanelAsk", action = "Home" }
            );

            //   context.MapRoute(
            //    name: "ManageMall",
            //    url: "ManageMall/{action}",
            //    defaults: new { controller = "ManageMall", action = "ManageProduct" }
            //);

        }
    }
}