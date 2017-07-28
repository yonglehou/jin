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
    /// 特殊内容URL获取器工厂
    /// </summary>
    public static class SpecialContentItemUrlGetterFactory
    {
        /// <summary>
        /// 依据tenantTypeId获取ISpecialContentItemUrlGetter
        /// </summary>
        /// <returns></returns>
        public static ISpecialContentItemUrlGetter Get(string tenantTypeId)
        {
            return DIContainer.Resolve<IEnumerable<ISpecialContentItemUrlGetter>>().Where(g => g.TenantTypeId.Equals(tenantTypeId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
    }
}
