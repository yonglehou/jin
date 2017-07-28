//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tunynet.Repositories;


namespace Tunynet.Logging.Repositories
{
    /// <summary>
    /// OperationLog仓储接口
    /// </summary>
    public class OperationLogRepository : Repository<OperationLog>, IOperationLogRepository
    {
        /// <summary>
        /// 删除指定时间段内的日志列表
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        public int Clean(DateTime? startDate, DateTime? endDate)
        {
            var sql = PetaPoco.Sql.Builder;
            sql.Append("delete from tn_OperationLogs");

            if (startDate.HasValue)
                sql.Where("DateCreated >= @0", startDate.Value);
            if (endDate.HasValue)
                sql.Where("DateCreated <= @0", endDate.Value);

            int result = CreateDAO().Execute(sql);

            return result;
        }

        /// <summary>
        /// 根据DiscussQuestionQuery查询获取可分页的数据集合
        /// </summary>
        /// <param name="query">OperationLog查询对象</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">当前页码(从1开始)</param>
        public PagingDataSet<OperationLog> GetLogs(OperationLogQuery query, int pageSize, int pageIndex)
        {
            var sql = PetaPoco.Sql.Builder;

            if (!string.IsNullOrEmpty(query.TenantTypeId))
                sql.Where("TenantTypeId = @0", query.TenantTypeId);
            if (!string.IsNullOrEmpty(query.Keyword))
                sql.Where("OperationObjectName like @0 or Description like @0", '%' + query.Keyword + '%');
            if (!string.IsNullOrEmpty(query.OperationType))
                sql.Where("OperationType = @0", query.OperationType);
            if (!string.IsNullOrEmpty(query.Operator))
                sql.Where("Operator like @0", "%" + query.Operator + "%");
            if (query.StartDateTime.HasValue)
                sql.Where("DateCreated >= @0", query.StartDateTime.Value);
            if (query.EndDateTime.HasValue)
                sql.Where("DateCreated <= @0", query.EndDateTime.Value);
            if (query.OperationUserId != null && query.OperationUserId.Count > 0)
                sql.Where("OperationUserId in (@0)", query.OperationUserId);
            if (!string.IsNullOrEmpty(query.OperationUserRole))
                sql.Where("OperationUserRole like @0", "%" + query.OperationUserRole + "%");

            sql.OrderBy("Id desc");

            return GetPagingEntities(pageSize, pageIndex, sql);
          
        }


    }
}
