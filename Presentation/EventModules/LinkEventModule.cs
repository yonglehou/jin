//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Tunynet.Events;
using Tunynet.Logging;

namespace Tunynet.Common
{
    /// <summary>
    ///链接管理事件
    /// </summary>
    public class LinkEventModule : IEventMoudle
    {
        private OperationLogService operationLogService;
        private RoleService roleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LinkEventModule(OperationLogService operationLogService, RoleService roleService)
        {
            this.operationLogService = operationLogService;
            this.roleService = roleService;
        }

        /// <summary>
        /// 注册事件处理方法
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<LinkEntity, CommonEventArgs>.Instance().After += new CommonEventHandler<LinkEntity, CommonEventArgs>(LinkEventModuleForOperationLog_After);

            EventBus<LinkEntity, AttachmentEventArgs>.Instance().After += new CommonEventHandler<LinkEntity, AttachmentEventArgs>(LinksAttachmentForManagerOperation_After);
        }

        /// <summary>
        ///友情链接事件处理
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="eventArgs"></param>
        private void LinksAttachmentForManagerOperation_After(LinkEntity link, AttachmentEventArgs eventArgs)
        {
            AttachmentService attachmentService = new AttachmentService(eventArgs.TenantTypeId);
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create() || eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                attachmentService.ToggleTemporaryAttachments(eventArgs.OperatorInfo.OperationUserId, eventArgs.TenantTypeId, link.LinkId, new List<long>() { link.ImageAttachmentId });
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
                attachmentService.Delete(link.ImageAttachmentId);


        }

        /// <summary>
        /// 第三方登录日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void LinkEventModuleForOperationLog_After(LinkEntity sender, CommonEventArgs eventArgs)
        {
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.LinkId;
            newLog.OperationObjectName = sender.LinkName;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Link();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));


            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = string.Format("添加链接 {0}", sender.LinkName);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = string.Format("编辑链接 {0}", sender.LinkName);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除链接 {0}", sender.LinkName);

            }

            operationLogService.Create(newLog);
        }
    }
}
