//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tunynet.Common;

namespace Tunynet.CMS
{
    /// <summary>
    /// 资讯全文检索条件
    /// </summary>
    public class CmsFullTextQuery
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 筛选
        /// </summary>
        public CmsSearchRange Range { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public PubliclyAuditStatus? PubliclyAuditStatus { get; set; }

        /// <summary>
        /// 栏目Id
        /// </summary>
        public int ContentCategoryId { get; set; }

        private int pageIndex = 1;
        /// <summary>
        /// 当前显示页面页码
        /// </summary>
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
    /// 搜索范围
    /// </summary>
    public enum CmsSearchRange
    {
        /// <summary>
        /// 全部
        /// </summary>
        ALL = 0,
        /// <summary>
        /// 标题
        /// </summary>
        TITLE = 1,
        /// <summary>
        /// 内容
        /// </summary>
        BODY = 2,
        /// <summary>
        /// 作者
        /// </summary>
        AUTHOR = 3
    }
}
