//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tunynet.Common;
using Tunynet.Logging.Repositories;
using Tunynet.Utilities;

namespace Tunynet.Logging
{
    /// <summary>
    /// 操作日志业务逻辑类
    /// </summary>
    public class OperationLogService
    {
        //OperationLogEntry Repository
        private IOperationLogRepository repository;
        private IOperatorInfoGetter operatorInfoGetter;
     
        /// <summary>
        /// 可设置repository的构造函数
        /// </summary>
        public OperationLogService(IOperationLogRepository repository, IOperatorInfoGetter operatorInfoGetter)
        {
            this.repository = repository;
            this.operatorInfoGetter = operatorInfoGetter;
        }

      
        /// <summary>
        /// 创建操作日志
        /// </summary>
        /// <param name="entry">操作日志实体</param>
        /// <returns>返回创建的操作日志对象的Id</returns>
        public long Create(OperationLog entry)
        {
            repository.Insert(entry);
            return entry.Id;
        }

        /// <summary>
        /// 创建操作日志
        /// </summary>
        /// <typeparam name="TEntity">操作对象类型</typeparam>
        /// <param name="entity">操作对象实体</param>
        /// <param name="operationType">操作类型</param>
        /// <returns>返回创建的操作日志对象</returns>r
        public OperationLog Create<TEntity>(TEntity entity, string operationType) where TEntity : class
        {
            return Create<TEntity>(entity, operationType, null);
        }

        /// <summary>
        /// 创建操作日志
        /// </summary>
        /// <typeparam name="TEntity">操作对象类型</typeparam>
        /// <param name="entity">操作对象实体</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="historyData">历史数据</param>
        /// <returns>返回创建的操作日志对象</returns>
        public OperationLog Create<TEntity>(TEntity entity, string operationType, TEntity historyData) where TEntity : class
        {
            
            if (operatorInfoGetter == null)
                throw new ApplicationException("IOperatorInfoGetter not registered to DIContainer");
            var operatorInfo = operatorInfoGetter.GetOperatorInfo();

            OperationLog entry = new OperationLog(operatorInfo);
            IOperationLogSpecificPartProcesser<TEntity> operationLogSpecificPartProcesser = DIContainer.Resolve<IOperationLogSpecificPartProcesser<TEntity>>();
            if (operationLogSpecificPartProcesser == null)
                throw new ApplicationException(string.Format("IOperationLogSpecificPartProcesser<{0}> not registered to DIContainer", typeof(TEntity).Name));

            if (historyData == null)
                operationLogSpecificPartProcesser.Process(entity, operationType, entry);
            else
                operationLogSpecificPartProcesser.Process(entity, operationType, historyData, entry);

            repository.Insert(entry);

            return entry;
        }

        /// <summary>
        /// 删除entryId相应的操作日志
        /// </summary>
        /// <param name="entryId">操作日志Id</param>
        public void Delete(long entryId)
        {
            repository.DeleteByEntityId(entryId);
        }

        /// <summary>
        /// 删除指定时间段内的日志列表
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        public int Clean(DateTime? startDate, DateTime? endDate)
        {
            return repository.Clean(startDate, endDate);
        }

        /// <summary>
        /// 根据DiscussQuestionQuery查询获取可分页的数据集合
        /// </summary>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">当前页码(从1开始)</param>
        public PagingDataSet<OperationLog> GetLogs(string tenantTypeId, string operationUserRole, List<long> operationUserId, string operationType,
         DateTime? minDate = null, DateTime? maxDate = null, int pageSize = 20, int pageIndex = 1)
        {
            var query = new OperationLogQuery
            {
                TenantTypeId = tenantTypeId,
                OperationUserRole = operationUserRole,
                OperationUserId = operationUserId,
                OperationType = operationType,
                StartDateTime = minDate,
                EndDateTime = maxDate
            };

            return repository.GetLogs(query, pageSize, pageIndex);
        }
    }
}
