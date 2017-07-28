//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 租户类型Repository
    /// </summary>
    public class TenantTypeRepository : Repository<TenantType>, ITenantTypeRepository
    {
        /// <summary>
        /// 依据服务或应用获取租户类型
        /// </summary>
        /// <param name="serviceKey">服务标识</param>
        /// <returns>如未满足条件的TenantType则返回空集合</returns>
        public IEnumerable<TenantType> Gets(string serviceKey)
        {
            string cacheKey = "TenantType::ServiceKey:" + serviceKey;
            List<TenantType> tenantTypes = cacheService.Get<List<TenantType>>(cacheKey);
            if (tenantTypes == null|| tenantTypes.Count()==0)
            {
                Sql sql = Sql.Builder;                
                if (!string.IsNullOrEmpty(serviceKey))
                {
                    sql.Select("tn_TenantTypes.*")
                       .From("tn_TenantTypes")
                       .InnerJoin("tn_TenantTypesInServices TTIS")
                       .On("tn_TenantTypes.TenantTypeId = TTIS.TenantTypeId")
                       .Where("TTIS.ServiceKey = @0", serviceKey);
                }
                else
                {
                    sql.Select("*")
                       .From("tn_TenantTypes");
                }

                tenantTypes = CreateDAO().Fetch<TenantType>(sql);
                
                cacheService.Set(cacheKey, tenantTypes, CachingExpirationType.ObjectCollection);
            }

            return tenantTypes;
        }
    }
}