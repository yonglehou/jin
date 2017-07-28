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
    /// 通知处理状态
    /// </summary>
    public enum NoticeStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        Unhandled = 0,
        /// <summary>
        /// 知道了
        /// </summary>
        Readed = 1,
        /// <summary>
        /// 接受
        /// </summary>
        Accepted = 2,
        /// <summary>
        /// 拒绝
        /// </summary>
        Refused = 3,
        /// <summary>
        /// 未审核(前台显示只显示0的  当审核未通过的默认是4  用于评论审核通过 个人通知中心才会显示)
        /// </summary>
        NotAudit = 4

    }
}