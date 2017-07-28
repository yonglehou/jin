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
using Tunynet.CMS;
using Tunynet.Events;
using Tunynet.Logging;

namespace Tunynet.Common
{
    class ContentCategoryEventModule : IEventMoudle
    {
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;

        public ContentCategoryEventModule()
        {

        }

        public ContentCategoryEventModule(
            OperationLogService operationLogService,
            UserService userService, 
            RoleService roleService)
        {
            this.operationLogService = operationLogService;
            this.userService = userService;
            this.roleService = roleService;
        }
        public void RegisterEventHandler()
        {
            EventBus<ContentCategory, CommonEventArgs>.Instance().After += new CommonEventHandler<ContentCategory, CommonEventArgs>(ContentCategoryModuleForOperationLog_After);
        }

        /// <summary>
        /// 生成操作日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void ContentCategoryModuleForOperationLog_After(ContentCategory sender, CommonEventArgs eventArgs)
        {
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.CategoryId;
            newLog.OperationObjectName = sender.CategoryName;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().CategoryManagers();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));


            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = string.Format("添加栏目 {0}", sender.CategoryName);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = string.Format("更新栏目 {0}", sender.CategoryName);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除栏目 {0}", sender.CategoryName);
            }

            operationLogService.Create(newLog);
        }


    }
}
