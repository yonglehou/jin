//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 创建标签的事件
    /// </summary>
    public class TagEventModule : IEventMoudle
    {

        private OperationLogService operationLogService;
        private RoleService roleService;
        private AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Tag());

        /// <summary>
        /// 构造函数
        /// </summary>
        public TagEventModule()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public TagEventModule(RoleService roleService,OperationLogService operationLogService)
        {
            this.roleService = roleService;
            this.operationLogService = operationLogService;
        }
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Tag>.Instance().After += new CommonEventHandler<Tag, CommonEventArgs>(TagModuleForManagerOperation_After);
            EventBus<Tag>.Instance().After +=new CommonEventHandler<Tag, CommonEventArgs>(TagEventModuleForOperationLog_After);
        }

        /// <summary>
        /// 标签操作日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void TagEventModuleForOperationLog_After(Tag sender, CommonEventArgs eventArgs)
        {
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.TagId;
            newLog.OperationObjectName = sender.TagName;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Tag();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = "添加标签："+sender.TagName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = "删除标签：" + sender.TagName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = "更新标签：" + sender.TagName;
            }

            operationLogService.Create(newLog);
        }

        /// <summary>
        /// 关联标题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void TagModuleForManagerOperation_After(Tag sender, CommonEventArgs eventArgs)
        {
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                attachmentService.ToggleTemporaryAttachments(sender.OwnerId, TenantTypeIds.Instance().Tag(),sender.TagId,new List<long>() { sender.ImageAttachmentId });

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                //删除标题图
                attachmentService.Delete(sender.ImageAttachmentId);
            }

        }
    }
}