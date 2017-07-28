//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Tunynet.Common
{
    /// <summary>
    /// 评价排序字段
    /// </summary>
    public enum SortBy_Review
    {
        /// <summary>
        /// 发布日期
        /// </summary>
        DateCreated,

        /// <summary>
        /// 发布日期倒序
        /// </summary>
        DateCreatedDesc

    }
    /// <summary>
    /// 评价排序字段
    /// </summary>
    public enum Review_Type
    {
        /// <summary>
        /// 好评
        /// </summary>
        Positive = 10,

        /// <summary>
        /// 中评
        /// </summary>
        Moderate=20,
        /// <summary>
        /// 差评
        /// </summary>
        Negative=30
    }

    
}
