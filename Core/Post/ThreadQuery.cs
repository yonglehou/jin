//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using PetaPoco;
using Tunynet;
using Tunynet.Caching;
using Tunynet.Common;

namespace Tunynet.Post
{

    /// <summary>
    /// 封装管理贴子时用于查询贴子的条件
    /// </summary>
    public class ThreadQuery
    {
        /// <summary>
        /// 标题关键字
        /// </summary>
        public string SubjectKeyword { get; set; }

        /// <summary>
        /// 贴吧Id
        /// </summary>
        public long? SectionId { get; set; }

        /// <summary>
        /// 作者用户Id
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 开始日期（用于发布时间条件）
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期（用于发布时间条件）
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public AuditStatus? AuditStatus { get; set; }

        /// <summary>
        ///是否置顶
        /// </summary>
        public bool? IsSticky { get; set; }

        /// <summary>
        /// 类别Id
        /// </summary>
        public long? CategoryId { get; set; }

    }
}
