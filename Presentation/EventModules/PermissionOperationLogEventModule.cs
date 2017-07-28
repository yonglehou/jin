//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Tunynet;
using Tunynet.Common;
using Tunynet.Events;
using Tunynet.Logging;

namespace Tunynet.Common
{
    /// <summary>
    /// Permission操作日志处理
    /// </summary>
    public class PermissionOperationLogEventMoudle : IEventMoudle
    {
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public PermissionOperationLogEventMoudle()
        {

        }

        public PermissionOperationLogEventMoudle(RoleService roleService, OperationLogService operationLogService, UserService userService)
        {
            this.roleService = roleService;
            this.operationLogService = operationLogService;
            this.userService = userService;
        }
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Permission>.Instance().BatchAfter += new BatchEventHandler<Permission, CommonEventArgs>(PermissionOperationLogEventMoudle_BatchAfter);
            EventBus<Permission>.Instance().After += new CommonEventHandler<Permission, CommonEventArgs>(PermissionOperationLogEventMoudle_After);
        }
        /// <summary>
        /// 生成操作日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void PermissionOperationLogEventMoudle_After(Permission sender, CommonEventArgs eventArgs)
        {
          

            var permissionObjectName =sender.OwnerType==OwnerType.Role? roleService.Get(sender.OwnerId).RoleName:userService.GetUser(sender.OwnerId).DisplayName;

            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.Id;
            newLog.OperationObjectName = permissionObjectName;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Permission();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));


            if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = string.Format("更新{0}{2}授权：{1}", sender.OwnerType.GetDisplayName(), sender.PermissionItemKey, permissionObjectName);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("移除{0}{2}授权： {1}", sender.OwnerType.GetDisplayName(), sender.PermissionItemKey, permissionObjectName);
            }

            operationLogService.Create(newLog);
        }

        public void PermissionOperationLogEventMoudle_BatchAfter(IEnumerable<Permission> senders, CommonEventArgs eventArgs)
        {
            //只记录批量更新操作
            if (eventArgs.EventOperationType != EventOperationType.Instance().Update())
                return;
            //OperationLogService logService = Tunynet.DIContainer.Resolve<OperationLogService>();
            //PermissionService permissionService = new PermissionService();

            //OperationLogEntry entry = new OperationLogEntry(eventArgs.OperatorInfo);

            //entry.ApplicationId = 0;
            //entry.Source = string.Empty;
            //entry.OperationType = eventArgs.EventOperationType;
            //IEnumerable<string> roleNames = senders.Select(n => n.RoleName).Distinct();
            //entry.OperationObjectName = string.Join(",", roleNames);
            //entry.OperationObjectId = 0;
            //entry.Description = string.Format(ResourceAccessor.GetString("OperationLog_Pattern_" + eventArgs.EventOperationType), "权限", entry.OperationObjectName);
            //logService.Create(entry);
        }

    }
}