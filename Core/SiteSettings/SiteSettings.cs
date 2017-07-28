//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Utilities;
using System.Web;
using Tunynet.Caching;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Tunynet.Common;

namespace Tunynet.Settings
{
    /// <summary>
    /// 站点设置
    /// </summary>
    /// <remarks>安装站点时，必须设置MainSiteRootUrl</remarks>
    [CacheSetting(true)]
    [Serializable]
    public class SiteSettings : IEntity
    {
        string beiAnScript = string.Empty;
        /// <summary>
        /// 备案信息
        /// </summary>
        [AllowHtml]
        public string BeiAnScript
        {
            get { return beiAnScript; }
            set { beiAnScript = value; }
        }

        string statScript = string.Empty;
        /// <summary>
        /// 页脚统计脚本
        /// </summary>
        [AllowHtml]
        public string StatScript
        {
            get { return statScript; }
            set { statScript = value; }
        }

        string links = string.Empty;
        /// <summary>
        /// 页脚链接
        /// </summary>
        [AllowHtml]
        public string Links
        {
            get { return links; }
            set { links = value; }
        }


        private Guid siteKey = Guid.NewGuid();
        /// <summary>
        /// 站点唯一标识
        /// </summary>
        public Guid SiteKey
        {
            get { return siteKey; }
            set { siteKey = value; }
        }

        string defaultSiteName = "近乎";
        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName
        {
            get { return defaultSiteName; }
            set { defaultSiteName = value; }
        }

        string defaultSiteDescription = "基于asp.net mvc 最强大SNS社区软件";
        /// <summary>
        /// 站点描述
        /// </summary>
        public string SiteDescription
        {
            get { return defaultSiteDescription; }
            set { defaultSiteDescription = value; }
        }

        string defaultCopyright = "©2005-2017 Tunynet Inc.";
        /// <summary>
        /// 版权声明
        /// </summary>
        [AllowHtml]
        public string Copyright
        {
            get { return defaultCopyright; }
            set { defaultCopyright = value; }
        }

        string searchMetaDescription = "“近乎”是一款业内领先的SNS社区软件。借助预置的微博、群组、日志、相册、贴吧、问答等应用模块，可以帮助客户快速搭建以用户为中心、用户乐于贡献内容、互动无处不在、易于运营的社区网站。它采用了业内领先的技术体系架构、隐私保护功能、用户评价体系、优异的缓存技术、全文检索技术。可以承载千万级的数据，具备优异的扩展性并提供丰富的API，方便用户进行定制开发或者二次开发。";
        /// <summary>
        /// 页面头信息的description
        /// </summary>
        public string SearchMetaDescription
        {
            get { return searchMetaDescription; }
            set { searchMetaDescription = value; }
        }

        string searchMetaKeyWords = "近乎,SNS社区软件处不在、易于运营的社区网站。";
        /// <summary>
        /// 页面头信息的KeyWord
        /// </summary>
        public string SearchMetaKeyWords
        {
            get { return searchMetaKeyWords; }
            set { searchMetaKeyWords = value; }
        }

        private string defaultLanguage = "zh-cn";
        /// <summary>
        /// 系统默认语言
        /// </summary>
        public string DefaultLanguage
        {
            get { return defaultLanguage; }
            set { defaultLanguage = value; }
        }

        //主站点Url
        private string mainSiteRootUrl = @"http://localhost";
        /// <summary>
        /// 主站URL
        /// </summary>
        /// <remarks>
        /// 安装程序（或者首次启动时）需要自动保存该地址
        /// </remarks>
        public string MainSiteRootUrl
        {
            get { return mainSiteRootUrl; }
            set { mainSiteRootUrl = value; }
        }


        private PubliclyAuditStatus auditStatus = PubliclyAuditStatus.Success;
        /// <summary>
        /// 用于显示的审核状态 默认通过审核
        /// </summary>
        /// <remarks>
        /// 包括所有的：评论、资讯 贴子
        /// </remarks>
        public PubliclyAuditStatus AuditStatus
        {
            get { return auditStatus; }
            set { auditStatus = value; }
        }

        private bool enableAnonymousBrowse = true;
        /// <summary>
        /// 是否允许匿名用户访问站点
        /// </summary>
        public bool EnableAnonymousBrowse
        {
            get { return enableAnonymousBrowse; }
            set { enableAnonymousBrowse = value; }
        }

        private SiteStyleType siteStyle = SiteStyleType.Default;
        /// <summary>
        /// 站点风格设置
        /// </summary>
        public SiteStyleType SiteStyle
        {
            get { return siteStyle; }
            set { siteStyle = value; }
        }





        #region IEntity 成员

        object IEntity.EntityId { get { return typeof(SiteSettings).FullName; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

    }
    


}