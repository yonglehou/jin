//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Tunynet.CMS;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.UI;

namespace Tunynet.Common
{
    /// <summary>
    /// 导航的事件
    /// </summary>
    public class NavigationEventModule : IEventMoudle
    {
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;
        public NavigationEventModule()
        {

        }

        public NavigationEventModule( OperationLogService operationLogService, UserService userService, RoleService roleService)
        {
            this.operationLogService = operationLogService;
            this.userService = userService;
            this.roleService = roleService;
        }


        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Navigation>.Instance().After += new CommonEventHandler<Navigation, CommonEventArgs>(NavigationModuleForOperationLog_After);
        }

        /// <summary>
        /// 生成操作日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void NavigationModuleForOperationLog_After(Navigation sender, CommonEventArgs eventArgs)
        {

            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.NavigationId;
            newLog.OperationObjectName = sender.NavigationText;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Navigation();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if(eventArgs.EventOperationType==EventOperationType.Instance().Create())
            {
                newLog.Description = string.Format("添加导航 {0}", sender.NavigationText);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = string.Format("更新导航 {0}", sender.NavigationText);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除导航 {0}", sender.NavigationText);
            }

            operationLogService.Create(newLog);
        }


    }
}