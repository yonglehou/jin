//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using Tunynet.Common;

namespace Tunynet.CMS
{
    /// <summary>
    /// ContentItem查询条件封装
    /// </summary>
    public class ContentItemQuery
    {
        /// <summary>
        /// CategoryId
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// 是否包含CategoryId的后代
        /// </summary>
        public bool? IncludeCategoryDescendants { get; set; }

        /// <summary>
        /// ContentModelId
        /// </summary>
        public int? ContentModelId { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 最小时间
        /// </summary>
        public DateTime? MinDate { get; set; }

        /// <summary>
        /// 最大时间
        /// </summary>
        public DateTime? MaxDate { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public AuditStatus? AuditStatus { get; set; }

        /// <summary>
        /// 标题关键词
        /// </summary>
        public string SubjectKeyword { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public ContentItemSortBy SortBy { get; set; }

        /// <summary>
        /// 置顶排序
        /// </summary>
        public bool OrderBySticky { get; set; }

        /// <summary>
        /// 是否后台查询
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否为当前用户
        /// </summary>
        public bool IsContextUser { get; set; }
    }
}
