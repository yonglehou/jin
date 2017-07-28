//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tunynet.Common
{
    /// <summary>
    /// 推荐类型 Id集合
    /// </summary>
    public class SpecialContentTypeIds
    {
        #region Instance

        private static volatile SpecialContentTypeIds _instance = null;
        private static readonly object lockObject = new object();

        public static SpecialContentTypeIds Instance()
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new SpecialContentTypeIds();
                    }
                }
            }
            return _instance;
        }

        private SpecialContentTypeIds()
        { }

        #endregion Instance

        /// <summary>
        /// 置顶
        /// </summary>
        public int Stick()
        {
            return 3;
        }

        /// <summary>
        /// 推荐贴吧
        /// </summary>
        public int Special()
        {
            return 121;
        }

        /// <summary>
        /// 首页幻灯推荐
        /// </summary>
        public int Slide()
        {
            return 101;
        }

        /// <summary>
        /// 资讯头条
        /// </summary>
        public int SpecialCMS()
        {
            return 111;
        }

        /// <summary>
        /// 官方贴子
        /// </summary>
        public int OfficialThread()
        {
            return 122;
        }

        /// <summary>
        /// 视频推荐
        /// </summary>
        public int CMS_Video()
        {
            return 113;
        }
        /// <summary>
        /// 组图幻灯片推荐
        /// </summary>
        public int CMS_Image()
        {
            return 112;
        }

        /// <summary>
        /// 精华
        /// </summary>
        public int Essential()
        {
            return 11;
        }
        //***************************************内置推荐类别  请勿乱加********************************
      
    }
}
