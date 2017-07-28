//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


namespace Tunynet.Logging
{
    /// <summary>
    /// 具体的操作日志信息接口
    /// </summary>
    public interface IOperationLogSpecificPart
    {
        /// <summary>
        ///租户Id
        /// </summary>
        string TenantTypeId { get; set; }

        /// <summary>
        ///操作类型标识
        /// </summary>
        string OperationType { get; set; }

        /// <summary>
        ///操作对象名称
        /// </summary>
        string OperationObjectName { get; set; }

        /// <summary>
        ///OperationObjectId
        /// </summary>
        long OperationObjectId { get; set; }

        /// <summary>
        ///操作描述
        /// </summary>
        string Description { get; set; }

    }
}
