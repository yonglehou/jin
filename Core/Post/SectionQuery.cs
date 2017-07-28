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
    /// 封装管理贴吧时用于查询贴吧的条件
    /// </summary>
    public class SectionQuery
    {
        /// <summary>
        /// 贴吧关键字
        /// </summary>
        public string NameKeyword { get; set; }

        /// <summary>
        /// 贴吧类别Id（包含后代子类别）
        /// </summary>
        public long? CategoryId { get; set; }

        /// <summary>
        /// 吧主Id
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsEnabled { get; set; }
        
        /// <summary>
        /// 审核状态
        /// </summary>
        public int AuditStatus { get; set; }
    }
}
