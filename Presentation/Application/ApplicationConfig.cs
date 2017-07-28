//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Resources;
using Autofac;
using System.Collections.Concurrent;
using Tunynet.Utilities;
using System.IO;
using Fasterflect;

namespace Tunynet.Common
{

    /// <summary>
    /// 应用的配置文件
    /// </summary>
    [Serializable]
    public abstract class ApplicationConfig
    {
        #region 加载配置文件
        private static readonly object lockObject = new object();
        private static bool isInitialized;
        private static ConcurrentDictionary<string, ApplicationConfig> applicationConfigs = null;

        /// <summary>
        /// 加载所有的application.config
        /// </summary>
        /// <param name="containerBuilder">容器构建器</param>
        /// <returns>Key=ApplicationId</returns>
        public static void InitializeAll(ContainerBuilder containerBuilder)
        {
            if (!isInitialized)
            {
                lock (lockObject)
                {
                    if (!isInitialized)
                    {
                        applicationConfigs = LoadConfigs();
                        foreach (var config in applicationConfigs.Values)
                        {
                            config.Initialize(containerBuilder);
                        }
                        isInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// 获取某一个Application.Config
        /// </summary>
        /// <param name="applicationKey">applicationKey</param>
        /// <returns>返回ApplicationConfig</returns>
        public static ApplicationConfig GetConfig(string applicationKey)
        {
            if (applicationConfigs != null && applicationConfigs.ContainsKey(applicationKey))
                return applicationConfigs[applicationKey];
            return null;
        }

        /// <summary>
        /// 加载所有的Application.config文件
        /// </summary>
        private static ConcurrentDictionary<string, ApplicationConfig> LoadConfigs()
        {
            var configs = new ConcurrentDictionary<string, ApplicationConfig>();
            //获取Applications中所有的Application.Config
            string applicationsDirectory = WebUtility.GetPhysicalFilePath("~/Applications/");
            foreach (var appPath in Directory.GetDirectories(applicationsDirectory))
            {
                string fileName = Path.Combine(appPath, "Application.Config");
                if (!File.Exists(fileName))
                    continue;

                string configType = string.Empty;
                XElement applicationElement = XElement.Load(fileName);

                //读取各个application节点中的属性     
                if (applicationElement != null)
                {
                    configType = applicationElement.Attribute("configType").Value;
                    Type applicationConfigClassType = Type.GetType(configType);
                    if (applicationConfigClassType != null)
                    {
                        ConstructorInvoker applicationConfigConstructor = applicationConfigClassType.DelegateForCreateInstance(typeof(XElement));
                        ApplicationConfig app = applicationConfigConstructor(applicationElement) as ApplicationConfig;
                        if (app != null)
                            configs[app.ApplicationKey] = app;
                    }
                }
            }
            return configs;
        }

        #endregion

        /// <summary>
        /// 检查应用是否启用
        /// </summary>
        /// <returns></returns>
        public abstract bool IsEnabled { get; }
        /// <summary>
        /// ApplicationKey
        /// </summary>
        public abstract string ApplicationKey { get; }

     
        /// <summary>
        /// 应用初始化运行环境（每次站点启动时DI容器构建前调用）
        /// </summary>
        /// <remarks>
        /// 用于注册组件、解析配置文件，不可使用DI容器解析对象因为此时尚未构建
        /// </remarks>
        /// <param name="containerBuilder">DI容器构建器(autofac)</param>
        public abstract void Initialize(ContainerBuilder containerBuilder);

        /// <summary>
        /// 应用初始化路由
        /// </summary>
        /// </remarks>
        public abstract void RegisterRoutes();


    }
}
