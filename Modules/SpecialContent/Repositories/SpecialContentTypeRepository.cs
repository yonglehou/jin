//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    public  class SpecialContentTypeRepository : Repository<SpecialContentType>, ISpecialContentTypeRepository
    {

        /// <summary>
        /// 从数据库删除实体(by EntityId)
        /// </summary>
        /// <param name="entityId">主键</param>
        /// <returns>影响的记录数</returns>
        public override int DeleteByEntityId(object entityId)
        {            
            Sql sql = Sql.Builder.Append("delete  tn_SpecialContentItems  where  tn_SpecialContentItems.TypeId = @0", entityId);
            CreateDAO().Execute(sql);
            int impact = base.DeleteByEntityId(entityId);
            return impact;
        }

        /// <summary>
        /// 从数据库根据租户类型获取推荐类型
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public IEnumerable<SpecialContentType> GetTypesByTenantType(string tenantTypeId)
        {
            Sql sql = Sql.Builder;
            sql.Where("TenantTypeId = @0",tenantTypeId);
            return CreateDAO().Fetch<SpecialContentType>(sql);
        }
    }
}
