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
using Tunynet.Repositories;

namespace Tunynet.Common
{
    public interface ISpecialContentTypeRepository : IRepository<SpecialContentType>
    {
        /// <summary>
        /// 从数据库删除实体(by EntityId)
        /// </summary>
        /// <param name="entityId">主键</param>
        /// <returns>影响的记录数</returns>
        new int DeleteByEntityId(object entityId);

        /// <summary>
        /// 从数据库根据租户类型获取推荐类型
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        IEnumerable<SpecialContentType> GetTypesByTenantType(string tenantTypeId);
    }
}
