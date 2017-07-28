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
    /// 用户等级事件
    /// </summary>
    public class UserRankEventModule : IEventMoudle
    {
        private OperationLogService operationLogService;
        private RoleService roleService;

		/// <summary>
        /// 构造函数
        /// </summary>
        public UserRankEventModule() {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserRankEventModule(OperationLogService operationLogService, RoleService roleService)
        {
            this.operationLogService = operationLogService;
            this.roleService = roleService;
        }

		/// <summary>
        /// 注册事件处理方法
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<UserRank>.Instance().After += new CommonEventHandler<UserRank, CommonEventArgs>(UserRankEventModuleForOperationLog_After) ;
        }

        /// <summary>
        /// 用户等级日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void UserRankEventModuleForOperationLog_After(UserRank sender, CommonEventArgs eventArgs)
        {
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId =0;
            newLog.OperationObjectName = sender.RankName;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().User();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = string.Format("添加用户等级 {0}", sender.RankName);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = string.Format("编辑用户等级 {0}", sender.RankName);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除用户等级 {0}", sender.RankName);
            }

            operationLogService.Create(newLog);
        }
    }
}
