//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Xml.Linq;
using Autofac;
using Tunynet.Common;
using System.Web.Routing;
using System.Web.Mvc;

namespace Spacebuilder.Setup
{
    /// <summary>
    /// SetupConfig
    /// </summary>
    ///  /// <summary>
    public class SetupConfig : ApplicationConfig
    {
        private XElement tenantAttachmentSettingsElement;

       
        /// <summary>
        /// 获取SetupConfig实例
        /// </summary>
        /// <summary>
        public static SetupConfig Instance()
        {
            return GetConfig("Setup") as SetupConfig;
        }
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="xElement"></param>
        public SetupConfig(XElement xElement)
        {
            this.tenantAttachmentSettingsElement = xElement.Element("tenantFileSettings");
        }
        /// <summary>
        /// ApplicationKey
        /// </summary>
        public override string ApplicationKey
        {
            get { return "Setup"; }
        }
        /// <summary>
        /// IsEnabled
        /// </summary>
        public override bool IsEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// 应用初始化
        /// </summary>
        /// <param name="containerBuilder">容器构建器</param>
        public override void Initialize(ContainerBuilder containerBuilder)
        {
            RegisterRoutes();
         }
        /// <summary>
        /// 
        /// </summary>
        public override void RegisterRoutes()
        {
            //安装首页
            RouteTable.Routes.MapRoute(
                name: "Setup_Home",
                url: "Setup",
                defaults: new { controller = "Setup", action = "Setup" }
            );
            //环境监测
            RouteTable.Routes.MapRoute(
                name: "Setup_Step1",
                url: "Setup/Step1",
                defaults: new { controller = "Setup", action = "Setup_Step1" }
            );

            //配置信息
            RouteTable.Routes.MapRoute(
               name: "Setup_Step2",
               url: "Setup/Step2",
               defaults: new { controller = "Setup", action = "Setup_Step2" }
           );

            //初始化数据
            RouteTable.Routes.MapRoute(
               name: "Setup_Step3",
               url: "Setup/Step3",
               defaults: new { controller = "Setup", action = "Setup_Step3" }
           );
            //安装完成
            RouteTable.Routes.MapRoute(
               name: "Setup_Step4",
               url: "Setup/Step4",
               defaults: new { controller = "Setup", action = "Setup_Step4" }
           );
            //安装协议
            RouteTable.Routes.MapRoute(
               name: "Setup_SetupProtocol",
               url: "Setup/SetupProtocol",
               defaults: new { controller = "Setup", action = "SetupProtocol" }
           );
        }
    }
}