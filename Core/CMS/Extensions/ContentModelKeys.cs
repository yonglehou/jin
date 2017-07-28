//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tunynet.CMS
{
    /// <summary>
    /// 内容模型标识
    /// </summary>
    public class ContentModelKeys
    {
        #region Instance

        private static volatile ContentModelKeys _instance = null;
        private static readonly object lockObject = new object();

        public static ContentModelKeys Instance()
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new ContentModelKeys();
                    }
                }
            }
            return _instance;
        }

        private ContentModelKeys()
        { }

        #endregion Instance


        /// <summary>
        /// 文章
        /// </summary>
        /// <returns></returns>
        public string Article()
        {
            return "Article";
        }

        /// <summary>
        /// 组图
        /// </summary>
        /// <returns></returns>
        public string Image()
        {
            return "Image";
        }
        /// <summary>
        /// 视频
        /// </summary>
        /// <returns></returns>
        public string Video()
        {
            return "Video";
        }
        /// <summary>
        /// 用户投稿
        /// </summary>
        /// <returns></returns>
        public string Contribution()
        {
            return "Contribution";
        }
    }
}