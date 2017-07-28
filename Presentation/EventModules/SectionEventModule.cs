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
using Tunynet.Common.Repositories;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 贴吧相关事件
    /// </summary>
    public class SectionEventModule : IEventMoudle
    {
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;
        private CategoryRepository categoryRepository;
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Section, CommonEventArgs>.Instance().After += new CommonEventHandler<Section, CommonEventArgs>(SectionModuleForManagerOperation_After);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SectionEventModule(OperationLogService operationLogService, UserService userService, RoleService roleService, CategoryRepository categoryRepository)
        {
            this.operationLogService = operationLogService;
            this.userService = userService;
            this.roleService = roleService;
            this.categoryRepository = categoryRepository;
        }

        /// <summary>
        /// 贴吧的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void SectionModuleForManagerOperation_After(Section sender, CommonEventArgs eventArgs)
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Section());
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.SectionId;
            newLog.OperationObjectName = "<a class=\"a\" target=\"_blank\" href=" + SiteUrls.Instance().SectionDetail(sender.SectionId) + ">" + sender.Name + "</a>";
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Bar();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));
            if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = "删除贴吧：" + sender.Name;
                //删除附件
                attachmentService.DeletesByAssociateId(sender.SectionId);

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = "创建贴吧：" + sender.Name;
                //把临时附件转换成正式附件
                attachmentService.ToggleTemporaryAttachments(sender.UserId, TenantTypeIds.Instance().Section(), sender.SectionId, new List<long> { sender.FeaturedImageAttachmentId });
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = "编辑贴吧：" + sender.Name;
                //把临时附件转换成正式附件
                attachmentService.ToggleTemporaryAttachments(sender.UserId, TenantTypeIds.Instance().Section(), sender.SectionId, new List<long> { sender.FeaturedImageAttachmentId });
            }
            operationLogService.Create(newLog);

        }

     
    }
}
