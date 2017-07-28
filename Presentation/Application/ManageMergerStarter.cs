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
using System.Configuration;

namespace Tunynet.Common
{

    /// <summary>
    /// 管理Starter合并启动文件配置
    /// </summary>
    public abstract class ManageMergerStarter
    {
        #region 加载配置文件
        private static readonly object lockObject = new object();
        private static bool isInitialized;
        private static ConcurrentDictionary<string, ManageMergerStarter> starterConfigs = null;

        /// <summary>
        /// 加载所有的starter
        /// </summary>
        /// <param name="containerBuilder">容器构建器</param>
        /// <returns>Key=starterKey</returns>
        public static void InitializeAll(ContainerBuilder containerBuilder)
        {
            if (!isInitialized)
            {
                lock (lockObject)
                {
                    if (!isInitialized)
                    {
                        starterConfigs = LoadConfigs();
                        foreach (var config in starterConfigs.Values)
                        {
                            config.Initialize(containerBuilder);
                        }
                        isInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// 加载所有的MergerStarter
        /// </summary>
        private static ConcurrentDictionary<string, ManageMergerStarter> LoadConfigs()
        {
            var starters = new ConcurrentDictionary<string, ManageMergerStarter>();
            var apiMergerAssemblys = ConfigurationManager.AppSettings["ApiMergerAssembly"];
            if (apiMergerAssemblys!=null)
            {
                string[] assemblys = apiMergerAssemblys.Split('|');
                foreach (var assembly in assemblys)
                {
                    string configType = string.Empty;
                    Type assemblyClassType = Type.GetType(assembly);
                    if (assemblyClassType != null)
                    {
                        ConstructorInvoker assemblyConstructor = assemblyClassType.DelegateForCreateInstance();
                        ManageMergerStarter starterConfig = assemblyConstructor() as ManageMergerStarter;

                        if (starterConfig != null)
                            starters[starterConfig.ApiKey] = starterConfig;
                    }
                }
            }
            return starters;
        }

        #endregion


        /// <summary>
        /// ApiKey
        /// </summary>
        public abstract string ApiKey { get; }

        /// <summary>
        /// Api Starter初始化Di容器的配置 例如:搜索的索引,个人推送等
        /// </summary>
        /// <param name="containerBuilder">Di构造器</param>
        public abstract void Initialize(ContainerBuilder containerBuilder);


        /// <summary>
        /// 初始化Api路由
        /// </summary>
        /// </remarks>
        public abstract void RegisterRoutes();


    }
}
