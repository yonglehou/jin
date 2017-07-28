//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Autofac;
using Autofac.Integration.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Tunynet;
using Tunynet.Caching;
using Tunynet.Repositories;
using Tunynet.Settings;
using Tunynet.Events;
using Tunynet.FileStore;
using Tunynet.Logging;
using Tunynet.Common;
using System.Xml.Linq;
using Tunynet.Utilities;
using Tunynet.Post;
using Tunynet.Tasks;
using Tunynet.Tasks.Quartz;
using Tunynet.Logging.Log4Net;
using CaptchaMvc.Infrastructure;
using System;
using Tunynet.Spacebuilder;
using Tunynet.Email;
using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(Spacebuilder.Environments.Startup))]
namespace Spacebuilder.Environments
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            //注册自定义用户Id提供器
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new CustomUserIdProvider());
            //映射signalr
            app.MapSignalR();
        }
    }
    /// <summary>
    /// 启动应用程序并预热
    /// </summary>
    public class Starter
    {

        /// <summary>
        /// 启动
        /// </summary>
        public static void Start()
        {

            var containerBuilder = new ContainerBuilder();

            containerBuilder.Register(c => new DefaultRunningEnvironment()).As<IRunningEnvironment>().SingleInstance();
            containerBuilder.Register(c => new DefaultCacheService(new RuntimeMemoryCache(), 0F)).As<ICacheService>().SingleInstance();
            //Kvstore注入
            containerBuilder.Register(c => new SqlKvStore()).As<IKvStore>().SingleInstance();

            //获取web引用的所有Tunynet开头的程序集
            //AssemblyName[] assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(n => n.Name.StartsWith("Tunynet")).ToArray();
            //List<Assembly> assemblyList = assemblyNames.Select(n => Assembly.Load(n)).ToList();
            ////重复 上面 的寻找
            IEnumerable<string> files = Directory.EnumerateFiles(HttpRuntime.BinDirectory, "Tunynet.*.dll");
            files = files.Union(Directory.EnumerateFiles(HttpRuntime.BinDirectory, "Spacebuilder.*.dll"));
            Assembly[] assemblies = files.Select(n => Assembly.Load(AssemblyName.GetAssemblyName(n))).ToArray();

            //批量注入所有的Repository
            containerBuilder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Repositories")).AsSelf().AsImplementedInterfaces().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            //批量注入所有的Service,注意如果需要根据接口和实现进行特殊处理的service需要进行排除或者放到批量注入的下面
            containerBuilder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Service") && !t.Name.Contains("CacheService")).AsSelf().AsImplementedInterfaces().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            //批量注入所有的EventMoudle
            containerBuilder.RegisterAssemblyTypes(assemblies).Where(t => typeof(IEventMoudle).IsAssignableFrom(t)).As<IEventMoudle>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            //批量注入所有的Handler
            containerBuilder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Handler")).AsSelf().AsImplementedInterfaces().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            //批量注入所有的UrlGetter
            containerBuilder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("UrlGetter")).AsSelf().AsImplementedInterfaces().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            containerBuilder.RegisterGeneric(typeof(SettingManager<>)).As(typeof(ISettingsManager<>)).SingleInstance().PropertiesAutowired();
            containerBuilder.RegisterGeneric(typeof(SettingsRepository<>)).As(typeof(ISettingsRepository<>)).SingleInstance().PropertiesAutowired();
            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).SingleInstance().PropertiesAutowired();
       

            //用户身份认证
            containerBuilder.Register(c => new FormsAuthenticationService()).As<IAuthenticationService>().PropertiesAutowired().InstancePerRequest();
            containerBuilder.Register(c => new OperatorInfoGetter()).As<IOperatorInfoGetter>().SingleInstance();

            //权限认证
            containerBuilder.Register(c => new Tunynet.Common.Authorizer()).AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            //注册任务调度器
            containerBuilder.Register(c => new QuartzTaskScheduler()).As<ITaskScheduler>().SingleInstance();
            //注册默认的IStoreProvider
            if (Utility.IsFileDistributedDeploy())
            {
                string fileServerRootUrl = ConfigurationManager.AppSettings["DistributedDeploy:FileServerRootUrl"];
                string fileServerUsername = ConfigurationManager.AppSettings["DistributedDeploy:FileServerUsername"];
                string fileServerPassword = ConfigurationManager.AppSettings["DistributedDeploy:FileServerPassword"];
                //containerBuilder.Register(c => new LocalStoreProvider(fileServerRootPath, fileServerRootUrl)).As<IStoreProvider>().Named<IStoreProvider>("CommonStorageProvider").SingleInstance();
            }
            else
            {
                containerBuilder.Register(c => new LocalStoreProvider(@"~/Uploads")).As<IStoreProvider>().Named<IStoreProvider>("CommonStorageProvider").SingleInstance();
            }
            containerBuilder.Register(c => new AttachmentRepository<Attachment>()).As<IAttachmentRepository<Attachment>>().SingleInstance();
            containerBuilder.Register(c => new CommentBodyProcessor()).As<ICommentBodyProcessor>().SingleInstance();

            ////注册Html信任标签配置  百度编辑器自己已经做过处理,此处可以忽略
            //containerBuilder.Register(c => new SafeTrustedHtml()).As<TrustedHtml>().SingleInstance();


            //注册id生成器
            containerBuilder.Register(c => new DefaultIdGenerator()).As<IdGenerator>().SingleInstance();
            containerBuilder.Register(c => new DefaultUserIdToUserNameDictionary()).As<UserIdToUserNameDictionary>().SingleInstance().PropertiesAutowired();



            //注册贴吧正文解析器
            containerBuilder.Register(c => new BarBodyProcessor()).Named<IBodyProcessor>(TenantTypeIds.Instance().Section()).SingleInstance();

            containerBuilder.RegisterControllers(assemblies).PropertiesAutowired();
            containerBuilder.RegisterModelBinders(assemblies);
            containerBuilder.RegisterModelBinderProvider();
            containerBuilder.RegisterFilterProvider();
            containerBuilder.RegisterModule(new AutofacWebTypesModule());
            containerBuilder.RegisterSource(new ViewRegistrationSource());

            //注册Signalr 通知 
            containerBuilder.Register(c => new SignalRNoticeSender()).As<INoticeSender>().SingleInstance();

            //注册系统日志
            containerBuilder.Register(c => new Log4NetLoggerFactoryAdapter()).As<ILoggerFactoryAdapter>().SingleInstance();
            //注册缓存
            containerBuilder.Register(c => new DefaultCacheService(new RuntimeMemoryCache(), new RuntimeMemoryCache(), 1.0F, true)).As<ICacheService>().SingleInstance();
            //邮件注册
            containerBuilder.Register(c => new EmailSettingsManager()).As<IEmailSettingsManager>().SingleInstance();
            //重新自定义的视图引擎
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TunynetViewEngine());

      
            //注册各应用模块的组件
            ApplicationConfig.InitializeAll(containerBuilder);
            //注册各Api合并模块
            ManageMergerStarter.InitializeAll(containerBuilder);

            IContainer container = containerBuilder.Build();
            //将Autofac容器中的实例注册到mvc自带DI容器中（这样才获取到每请求缓存的实例）
            DependencyResolver.SetResolver(new Autofac.Integration.Mvc.AutofacDependencyResolver(container));
            DIContainer.RegisterContainer(container);


         
            //2016-11-02,libsh:解決mvc3以后自动为非空字段添加验证问题
            ModelValidatorProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Add(new DataAnnotationsModelValidatorProvider());
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
             

      
            ///初始化 automapper
            AutoMapperConfiguration.Initialize();

            //注册附件设置
            string ConfigurePhysicalPath = WebUtility.GetPhysicalFilePath("~/Config/tenant.config");
            if (File.Exists(ConfigurePhysicalPath))
            {

                XElement xElement = XElement.Load(ConfigurePhysicalPath);
                TenantFileSettings.RegisterSettings(xElement.Element("tenantFileSettings"));
            }

            //初始化事件处理程序
            IEnumerable<IEventMoudle> eventMoudles = DIContainer.Resolve<IEnumerable<IEventMoudle>>().ToList().Distinct();
            foreach (var eventMoudle in eventMoudles)
            {
                eventMoudle.RegisterEventHandler();
            }
            //自定义模型绑定 处理敏感字
            ModelBinders.Binders.DefaultBinder = new CustomModelBinder();



            //初始化第三方帐号获取器
            ThirdAccountGetterFactory.InitializeAll();

            //containerBuilder.Register(c => new BarSectionActivityReceiverGetter()).Named<IActivityReceiverGetter>(ActivityOwnerTypes.Instance().BarSection().ToString()).SingleInstance();
            //containerBuilder.Register(c => new BarSearcher("贴吧", "~/App_Data/IndexFiles/Bar", true, 6)).As<ISearcher>().Named<ISearcher>(BarSearcher.CODE).SingleInstance();

            //containerBuilder.Register(c => new BarApplicationStatisticDataGetter()).Named<IApplicationStatisticDataGetter>(this.ApplicationKey).SingleInstance();

            //containerBuilder.Register(c => new BarTenantAuthorizationHandler()).As<ITenantAuthorizationHandler>().SingleInstance();

            ////设置SignalR的解析器
            //GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);

            //启动定时任务
            TaskSchedulerFactory.GetScheduler().Start();
            //注册用户每日计数服务
            CountService countService = new CountService(TenantTypeIds.Instance().User());
            countService.RegisterStageCount(CountTypes.Instance().LoginTimes(), 10000);

            //注册全局过滤器
            GlobalFilters.Filters.Add(new SecurityFilter());
            GlobalFilters.Filters.Add(new PauseSiteCheckAttribute());
            GlobalFilters.Filters.Add(new ExceptionHandlerAttribute());


            //GlobalFilters.Filters.Add(new SecurityFilter());
            //注册资源管理
            ResourceAccessor.Initialize("Web.Resources.Resource", typeof(Web.Resources.Resource).Assembly);

            //解析模板(其实是编译 很耗时)
            NoticeBuilder.Instance();

            #region 验证码初始化
            CaptchaMvc.Interface.ICaptchaManager captchaManager = CaptchaUtils.CaptchaManager;
            captchaManager.StorageProvider = new CookieStorageProvider();
            //var captchaManager = new DefaultCaptchaManager(new CookieStorageProvider());
            //只能验证低于5秒就出验证码
            captchaManager.IntelligencePolicy = new
              MultiIntelligencePolicy(
              new FakeInputIntelligencePolicy(),
              new JavaScriptIntelligencePolicy(),
              new ResponseTimeIntelligencePolicy(TimeSpan.FromSeconds(3)));
            //改变背景颜色
            var imageGenerator = CaptchaUtils.ImageGenerator;
            imageGenerator.Width = 90;
            imageGenerator.Height = 35;
            //imageGenerator.Generate(new )
            //    ICaptcha.BuildInfo.
            #endregion
        }
    }
}

