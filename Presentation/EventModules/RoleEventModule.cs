//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tunynet.CMS;
using Tunynet.Common;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Utilities;

namespace Tunynet.Common
{
    /// <summary>
    /// 角色管理事件
    /// </summary>
    public class RoleEventModule : IEventMoudle
    {

        private OperationLogService operationLogService;
        private RoleService roleService;
        private PermissionService permissionService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public RoleEventModule() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="operationLogService"></param>
        /// <param name="roleService"></param>
        public RoleEventModule(OperationLogService operationLogService, RoleService roleService,PermissionService permissionService)
        {
            this.operationLogService = operationLogService;
            this.roleService = roleService;
            this.permissionService = permissionService;
        }
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Role>.Instance().After += new CommonEventHandler<Role, CommonEventArgs>(RoleEventModuleForOperationLog_After);
            EventBus<Role>.Instance().After += new CommonEventHandler<Role, CommonEventArgs>(RoleEventModuleForEmptyPermissions_After);
        }

        /// <summary>
        /// 角色操作生成日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void RoleEventModuleForOperationLog_After(Role sender, CommonEventArgs eventArgs)
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Role());
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.RoleId;
            newLog.OperationObjectName = sender.RoleName;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Role();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                attachmentService.ToggleTemporaryAttachments(eventArgs.OperatorInfo.OperationUserId, TenantTypeIds.Instance().Role(), sender.RoleId,new List<long>() { sender.RoleImageAttachmentId});
                newLog.Description = "创建角色：" + sender.RoleName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                attachmentService.DeletesByAssociateId(sender.RoleId);
                newLog.Description = "删除角色：" + sender.RoleName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                attachmentService.ToggleTemporaryAttachments(eventArgs.OperatorInfo.OperationUserId, TenantTypeIds.Instance().Role(), sender.RoleId, new List<long>() { sender.RoleImageAttachmentId });
                newLog.Description = "更新角色："+ sender.RoleName;
            }

            operationLogService.Create(newLog);

        }

        /// <summary>
        /// 角色删除时清空该角色权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void RoleEventModuleForEmptyPermissions_After(Role sender, CommonEventArgs eventArgs)
        {
            if (eventArgs.EventOperationType==EventOperationType.Instance().Delete())
            {
                IEnumerable<string> permissionItemKeys = new List<string>();
                permissionService.UpdatePermissionsInUserRole(permissionItemKeys, sender.RoleId, OwnerType.Role);
            }
        }
    }
}
