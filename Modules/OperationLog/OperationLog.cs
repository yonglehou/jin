//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using Tunynet.Caching;
using PetaPoco;
using Tunynet.Common;
using Tunynet.Events;

namespace Tunynet.Logging
{
    /// <summary>
    /// 操作日志实体
    /// </summary>
    [TableName("tn_OperationLogs")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class OperationLog : IEntity, IOperationLogSpecificPart
    {
        //此空构造函数是因为此方法被构造过 所以需 留一个空的构造 防止 反射的时候 找不到此类的实例
        public OperationLog()
        { }
        /// <summary>
        /// 构造函数
        /// </summary>
        public OperationLog(OperatorInfo operatorInfo)
        {
            this.OperationUserId = operatorInfo.OperationUserId;
            this.OperatorIP = operatorInfo.OperatorIP;
            this.Operator = operatorInfo.Operator;
            this.AccessUrl = operatorInfo.AccessUrl;
            this.DateCreated = DateTime.Now;
            this.TenantTypeId = string.Empty;
            this.OperationType = string.Empty;
            this.OperationObjectName = string.Empty;
            this.OperationObjectId = 0;
            this.Description = string.Empty;
            this.OperationUserRole = string.Empty;
           
        }

        #region 需持久化属性

        /// <summary>
        ///Id
        /// </summary>
        public long Id { get; protected set; }

        /// <summary>
        ///租户Id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        ///操作类型标识
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        ///操作对象名称
        /// </summary>
        public string OperationObjectName { get; set; }

        /// <summary>
        ///OperationObjectId
        /// </summary>
        public long OperationObjectId { get; set; }

        /// <summary>
        ///操作描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///操作者UserId
        /// </summary>
        public long OperationUserId { get; set; }


        /// <summary>
        ///操作者角色
        /// </summary>
        public string OperationUserRole { get; set; }

        /// <summary>
        ///操作者名称
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        ///操作者IP
        /// </summary>
        public string OperatorIP { get; set; }

        /// <summary>
        ///操作访问的url
        /// </summary>
        public string AccessUrl { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取OperationType对外显示的名称
        /// </summary>
        /// <returns></returns>
        public string GetOperationTypeDisplayName()
        {
            if (OperationType == EventOperationType.Instance().Approved())
            {
                return "审核通过";
            }
            else if (OperationType == EventOperationType.Instance().BanUser())
            {
                return "封禁用户";
            }
            else if (OperationType == EventOperationType.Instance().Create())
            {
                return "创建";
            }
            else if (OperationType == EventOperationType.Instance().Delete())
            {
                return "删除";
            }
            else if (OperationType == EventOperationType.Instance().Disapproved())
            {
                return "审核不通过";
            }
            else if (OperationType == EventOperationType.Instance().Update())
            {
                return "更新";
            }
            else if (OperationType == EventOperationType.Instance().UnbanUser())
            {
                return "解禁用户";
            }
            else if (OperationType == EventOperationType.Instance().SignIn())
            {
                return "登录";
            }
            else if (OperationType == EventOperationType.Instance().SignOut())
            {
                return "登出";
            }

            return OperationType;
        }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }

}

