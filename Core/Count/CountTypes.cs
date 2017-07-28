//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Tunynet.Common
{
    /// <summary>
    /// 计数类型
    /// </summary>
    public class CountTypes
    {
        #region Instance
        private static CountTypes _instance = new CountTypes();
        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static CountTypes Instance()
        {
            return _instance;
        }

        private CountTypes()
        { }

        #endregion

        /// <summary>
        /// 浏览计数
        /// </summary>
        /// <returns></returns>
        public string HitTimes()
        {
            return "HitTimes";
        }
        /// <summary>
        /// 登录计数
        /// </summary>
        /// <returns></returns>
        public string LoginTimes()
        {
            return "LoginTimes";
        }

        /// <summary>
        /// 下载计数
        /// </summary>
        /// <returns></returns>
        public string DownloadCount()
        {
            return "DownloadCount";
        }

        /// <summary>
        /// 评论数
        /// </summary>
        /// <returns></returns>
        public string CommentCount()
        {
            return "CommentCount";
        }

        /// <summary>
        /// 威望数
        /// </summary>
        /// <returns></returns>
        public string ReputationPointsCounts()
        {
            return "ReputationPointsCounts";
        }

        /// <summary>
        /// 站点所有内容计数
        /// </summary>
        /// <returns></returns>
        public string AllItems()
        {
            return "AllItems";
        }

    }

}
