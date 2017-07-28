//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.CMS;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 栏目扩展
    /// </summary>
    public static class ContentCategoryExtension
    {
        /// <summary>
        ///栏目总内容数量
        /// </summary>
        /// <returns></returns>
        public static long ContentCategoryCount(this ContentCategory operationType)
        {
            return DIContainer.Resolve<ContentItemService>().GetContentItems(operationType.CategoryId, true).TotalRecords;
        }

        /// <summary>
        ///栏目每日内容数量
        /// </summary>
        /// <returns></returns>
        public static long ContentCategoryDayCount(this ContentCategory operationType)
        {
            DateTime dateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            return DIContainer.Resolve<ContentItemService>().GetContentItemForAdmin(string.Empty, operationType.CategoryId, true, null, null, null, dateTime, dateTime.AddDays(1), false, 1, 1).TotalRecords;
        }

        /// <summary>
        ///栏目本月内容数量
        /// </summary>
        /// <returns></returns>
        public static long ContentCategoryMonthCount(this ContentCategory operationType)
        {
            DateTime dateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM"));
            return DIContainer.Resolve<ContentItemService>().GetContentItemForAdmin(string.Empty, operationType.CategoryId, true, null, null, null, dateTime, dateTime.AddMonths(1), false, 1, 1).TotalRecords;
        }

    }
}
