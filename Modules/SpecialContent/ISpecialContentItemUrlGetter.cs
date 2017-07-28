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
    /// 推荐相关Url获取器
    /// </summary>
    public interface ISpecialContentItemUrlGetter
    {
        /// <summary>
        /// 租户类型Id
        /// </summary>
        string TenantTypeId { get; }

        /// <summary>
        /// 获取特殊对象详细显示url
        /// </summary>
        /// <remarks>如果无特殊对象详细显示页面</remarks>
        /// <param name="commentedObjectId">被加特殊对象Id</param>
        /// <param name="id">特殊对象Id</param>
        /// 
        /// <returns></returns>
        string GetSpecialContentItemDetailUrl(long specialContentItemObjectId, string tenantTypeId = null);


    
    }


}
