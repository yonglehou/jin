//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Tunynet.Common;

namespace Tunynet.Post
{
    /// <summary>
    /// 计数类型扩展类
    /// </summary>
    public static class CountTypesExtension
    {
        /// <summary>
        /// 主题贴数
        /// </summary>
        public static string ThreadCount(this CountTypes countTypes)
        {
            return "ThreadCount";
        }

        /// <summary>
        /// 主题贴和回贴总数
        /// </summary>
        public static string ThreadAndPostCount(this CountTypes countTypes)
        {
            return "ThreadAndPostCount";
        }
        /// <summary>
        /// 被关注数
        /// </summary>
        public static string FollowedCount(this CountTypes countTypes)
        {
            return  "FollowedCount";
        }
    }

}
