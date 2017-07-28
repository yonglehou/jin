//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Tunynet.Repositories;

namespace Tunynet.Attitude.Repositories
{ /// <summary>
  /// 顶踩记录的数据访问接口
  /// </summary>
    public interface IAttitudeRecordRepository : IRepository<AttitudeRecord>
    {
        /// <summary>
        /// 获取参与用户的Id集合
        /// </summary>
        /// <param name="objectId">操作对象Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">条数</param>
        IEnumerable<long> GetTopOperatedUserIds(long objectId, string tenantTypeId,int? topNumber);

        /// <summary>
        /// 获取操作对象的Id集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        ///<param name="userId">用户ID</param>
        ///<param name="pageSize">每页的内容数</param>
        ///<param name="pageIndex">页码</param>
        IEnumerable<long> GetObjectIds(string tenantTypeId, long userId, int pageSize, int pageIndex);

        /// <summary>
        /// 获取用户对某项的所有顶踩
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="tenantTypeId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Dictionary<long, bool> GetAllAttitues(string tenantTypeId, long userId);
    }
}