//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tunynet.CMS;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 广告管理事件
    /// </summary>
    public class AdvertisingEventModule : IEventMoudle
    {   
        private OperationLogService operationLogService;
        private RoleService roleService;

        //构造
        public AdvertisingEventModule(OperationLogService operationLogService, RoleService roleService)
        {
            this.operationLogService = operationLogService;           
            this.roleService = roleService;
           
        }
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Advertising, CommonEventArgs>.Instance().After += new CommonEventHandler<Advertising, CommonEventArgs>(AdvertisingModuleForOperationLog_After);
            EventBus<AdvertisingPosition,CommonEventArgs>.Instance().After += new CommonEventHandler<AdvertisingPosition, CommonEventArgs>(AdvertisingPositionModuleForOperationLog_After);

        }

        /// <summary>
        /// 广告操作日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void AdvertisingModuleForOperationLog_After(Advertising sender, CommonEventArgs eventArgs)
        {

            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.AdvertisingId;
            newLog.OperationObjectName = sender.Name;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Advertising();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Advertising());
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                attachmentService.ToggleTemporaryAttachments(eventArgs.OperatorInfo.OperationUserId, TenantTypeIds.Instance().Advertising(), sender.AdvertisingId, new List<long> { sender.ImageAttachmentId });
                newLog.Description = "添加广告：" + sender.Body;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                attachmentService.ToggleTemporaryAttachments(eventArgs.OperatorInfo.OperationUserId, TenantTypeIds.Instance().Advertising(), sender.AdvertisingId, new List<long> { sender.ImageAttachmentId });
                newLog.Description = "更新广告：" + sender.Body;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Approved())
            {

                newLog.Description = "启用广告：" + sender.Body;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Disapproved())
            {
                newLog.Description = "禁用广告：" + sender.Body;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                attachmentService.DeletesByAssociateId(sender.AdvertisingId);
                newLog.Description = string.Format("删除广告: {0}", sender.Body);
            }

            operationLogService.Create(newLog);


        }

        /// <summary>
        /// 广告位操作日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void AdvertisingPositionModuleForOperationLog_After(AdvertisingPosition sender, CommonEventArgs eventArgs)
        {

            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.PositionId;
            newLog.OperationObjectName = sender.Description;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().AdvertisingPosition();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().AdvertisingPosition());
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                attachmentService.ToggleTemporaryAttachments(eventArgs.OperatorInfo.OperationUserId, TenantTypeIds.Instance().AdvertisingPosition(), sender.PositionId, new List<long> { sender.ImageAttachmentId });
                newLog.Description = "添加广告位：" + sender.Description;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                attachmentService.ToggleTemporaryAttachments(eventArgs.OperatorInfo.OperationUserId, TenantTypeIds.Instance().AdvertisingPosition(), sender.PositionId, new List<long> { sender.ImageAttachmentId });
                newLog.Description = "更新广告位：" + sender.Description;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                attachmentService.DeletesByAssociateId(sender.PositionId);
                newLog.Description = string.Format("删除广告位: {0}", sender.Description);
            }

            operationLogService.Create(newLog);


        }
    }
}