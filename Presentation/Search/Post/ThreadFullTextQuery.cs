//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using Tunynet.Common;

namespace Tunynet.Post
{
    /// <summary>
    /// 贴子全文检索条件
    /// </summary>
    public class ThreadFullTextQuery
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 租户类型id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public PubliclyAuditStatus? PubliclyAuditStatus { get; set; }

        /// <summary>
        /// 搜索范围：标题 全文 作者 
        /// </summary>
        public ThreadSearchRange Range { get; set; }

        private string sectionId = "-1";

        /// <summary>
        /// 帖吧ID
        /// </summary>
        public string SectionId { get { return sectionId; } set { sectionId = value; } }

        /// <summary>
        /// 当前显示页面页码
        /// </summary>
        private int pageIndex = 1;
        public int PageIndex
        {
            get
            {
                if (pageIndex < 1)
                    return 1;
                else
                    return pageIndex;
            }
            set { pageIndex = value; }
        }

        /// <summary>
        /// 每页显示记录数
        /// </summary>
        public int PageSize = 10;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? MinDate { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime? MaxDate { get; set; }

        private bool isDefaultOrder = true;

        /// <summary>
        /// 是否按照默认排序 
        /// </summary>
        public bool IsDefaultOrder { get { return isDefaultOrder; } set { isDefaultOrder = value; } }
    }


    /// <summary>
    /// 帖吧搜索范围
    /// </summary>
    public enum ThreadSearchRange
    {
        /// <summary>
        /// 全部
        /// </summary>
        ALL = 0,

        /// <summary>
        /// 标题
        /// </summary>
        SUBJECT = 1,

        /// <summary>
        /// 全文
        /// </summary>
        BODY = 2,

        /// <summary>
        /// 作者
        /// </summary>
        AUTHOR = 3
    }
}
