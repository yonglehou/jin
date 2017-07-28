//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using Autofac;
using Autofac.Integration.Mvc;
using Spacebuilder.Environments;
using StackExchange.Profiling;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Tunynet;
using Tunynet.Caching;
using Tunynet.Common;
using Tunynet.Settings;
using Tunynet.Repositories;

namespace Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {

            if (!CheckInstallStatus())
            {
                //注册 ASP.NET MVC 应用程序中的所有区域。
                AreaRegistration.RegisterAllAreas();
                //注册 ASP.NET MVC 全局过滤器集合。
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                InitializeSetup();
                RegisterRoutes();
            }
            else
            {
                //注册 ASP.NET MVC 应用程序中的所有区域。
                AreaRegistration.RegisterAllAreas();
                //GlobalConfiguration.Configure(WebApiConfig.Register);
                //注册 ASP.NET MVC 全局过滤器集合。
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                //注册所使用的捆绑的CSS 和 JS文件
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                ////注册公司常用组件服务
                Starter.Start();

                //注册配置MVC应用程序的系统路由路径。
                RouteConfig.RegisterRoutes(RouteTable.Routes);

            }




        }
        bool CheckInstallStatus()
        {
            //System.IO.FileInfo FileInfo = new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Themes\\Default\\Setup\\Setup.cshtml");
            //if (!FileInfo.Exists)
            //    return true;
            int connectionStringCount = 0;
            if (ConfigurationManager.ConnectionStrings["SqlServer"] != null)
            {
                connectionStringCount++;
            }
            if (ConfigurationManager.ConnectionStrings["MySql"] != null)
            {
                connectionStringCount++;
            }
            if (connectionStringCount == 0)
                return false;
            else
            {
                return true;
            }

        }

        /// <summary>
        /// 初始化程序安装步骤
        /// </summary>
        /// <returns></returns>
        void InitializeSetup()
        {
            var containerBuilder = new ContainerBuilder();

            IEnumerable<string> files = Directory.EnumerateFiles(HttpRuntime.BinDirectory, "Tunynet.*.dll");
            files = files.Union(Directory.EnumerateFiles(HttpRuntime.BinDirectory, "Spacebuilder.*.dll"));
            Assembly[] assemblies = files.Select(n => Assembly.Load(AssemblyName.GetAssemblyName(n))).ToArray();

            //批量注入所有的Repository
            containerBuilder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Repositories")).AsSelf().AsImplementedInterfaces().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            //批量注入所有的Service,注意如果需要根据接口和实现进行特殊处理的service需要进行排除或者放到批量注入的下面
            containerBuilder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Service") && !t.Name.Contains("CacheService")).AsSelf().AsImplementedInterfaces().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            string resourceSite = null;
            if (ConfigurationManager.AppSettings["PageResource:Site"] != null)
                resourceSite = ConfigurationManager.AppSettings["PageResource:Site"];
            //注册缓存
            containerBuilder.Register(c => new DefaultCacheService(new RuntimeMemoryCache(), 1.0F)).As<ICacheService>().SingleInstance();
            containerBuilder.RegisterGeneric(typeof(SettingManager<>)).As(typeof(ISettingsManager<>)).SingleInstance().PropertiesAutowired();
            containerBuilder.RegisterGeneric(typeof(SettingsRepository<>)).As(typeof(ISettingsRepository<>)).SingleInstance().PropertiesAutowired();
            IContainer container = containerBuilder.Build();
            ////将Autofac容器中的实例注册到mvc自带DI容器中（这样才获取到每请求缓存的实例）
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            DIContainer.RegisterContainer(container);

        }

        /// <summary>
        /// 启动Sql调试
        /// </summary>
        protected void Application_BeginRequest()
        {
            if (Utility.IsMiniProfilerEnabled())
            {
                if (Request.IsLocal)
                {
                    MiniProfiler.Start();
                }
            }
        }
        /// <summary>
        /// 关闭Sql 调试
        /// </summary>

        protected void Application_EndRequest()
        {
            if (Utility.IsMiniProfilerEnabled())
            {
                MiniProfiler.Stop();
            }
        }
        /// <summary>
        /// 安装路由注册
        /// </summary>
        public void RegisterRoutes()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TunynetViewEngine());
            //安装首页
            RouteTable.Routes.MapRoute(
                name: "Setup",
                url: "",
                defaults: new { controller = "Setup", action = "Setup" }
            );
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
            RouteTable.Routes.MapRoute(
         name: "Default",
         url: "{controller}/{action}",
         defaults: new { controller = "Setup", action = "Setup" }
      );



        }
    }
}
