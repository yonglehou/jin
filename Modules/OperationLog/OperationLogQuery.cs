//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Tunynet.Logging
{
    /// <summary>
    /// OperationLog查询对象
    /// </summary>
    public class OperationLogQuery
    {

        /// <summary>
        /// 操作人名称（可以模糊搜索）
        /// </summary>
        public string Operator;

        /// <summary>
        /// 关键字
        /// </summary>
        /// <remarks>搜索操作对象</remarks>
        public string Keyword;

        /// <summary>
        ///操作者UserId
        /// </summary>
        public List<long> OperationUserId { get; set; }

        /// <summary>
        ///租户Id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime;

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime? EndDateTime;

        /// <summary>
        /// 操作者角色名称
        /// </summary>
        public string OperationUserRole;

    }
}
