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
using Tunynet.Attitude;
using Tunynet.Events;
using Tunynet.Logging;

namespace Tunynet.Common
{
	/// <summary>
    /// 第三方登录事件
    /// </summary>
    public class ThirdAccountEventModule : IEventMoudle
    {
        private OperationLogService operationLogService;
        private RoleService roleService;

		/// <summary>
        /// 构造函数
        /// </summary>
        public ThirdAccountEventModule() {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ThirdAccountEventModule(OperationLogService operationLogService, RoleService roleService)
        {
            this.operationLogService = operationLogService;
            this.roleService = roleService;
        }

		/// <summary>
        /// 注册事件处理方法
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<AccountType>.Instance().After += new CommonEventHandler<AccountType, CommonEventArgs>(ThirdAccountEventModuleForOperationLog_After) ;
        }

		/// <summary>
        /// 第三方登录日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ThirdAccountEventModuleForOperationLog_After(AccountType sender, CommonEventArgs eventArgs)
        {
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = 0;
            newLog.OperationObjectName = sender.AccountTypeKey;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = "";
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = string.Format("添加第三方登录类型 {0}", sender.AccountTypeKey);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = string.Format("编辑第三方登录类型 {0}", sender.AccountTypeKey);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除第三方登录类型 {0}", sender.AccountTypeKey);
            }

            operationLogService.Create(newLog);
        }
    }
}
