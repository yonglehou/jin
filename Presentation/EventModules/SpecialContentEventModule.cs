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
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 推荐内容事件
    /// </summary>
    public class SpecialContentEventModule : IEventMoudle
    {
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;
        private SpecialContentTypeService specialContentTypeService;
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<SpecialContentItem, CommonEventArgs>.Instance().After += new CommonEventHandler<SpecialContentItem, CommonEventArgs>(SpecialContentEventModuleForOperationLog_After);
            EventBus<SpecialContentType, CommonEventArgs>.Instance().After += new CommonEventHandler<SpecialContentType, CommonEventArgs>(SpecialContentTypeEventModuleForOperationLog_After);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="operationLogService"></param>
        /// <param name="userService"></param>
        /// <param name="roleService"></param>
        public SpecialContentEventModule(OperationLogService operationLogService,
            UserService userService, RoleService roleService, SpecialContentTypeService specialContentTypeService)
        {
            this.operationLogService = operationLogService;
            this.roleService = roleService;
            this.userService = userService;
            this.specialContentTypeService = specialContentTypeService;
        }

        /// <summary>
        /// 推荐类别日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SpecialContentTypeEventModuleForOperationLog_After(SpecialContentType sender, CommonEventArgs eventArgs)
        {
            
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.TypeId;
            newLog.OperationObjectName = sender.Name;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Recommend();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));
           
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
             
                newLog.Description = string.Format("添加推荐类别：{0}", sender.Name);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除推荐类别：{0}", sender.Name);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = string.Format("编辑推荐类别：{0}", sender.Name);
            }

            operationLogService.Create(newLog);
        }

        /// <summary>
        /// 推荐内容日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SpecialContentEventModuleForOperationLog_After(SpecialContentItem sender, CommonEventArgs eventArgs)
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Recommend());
            var specialContentType = specialContentTypeService.Get(sender.TypeId);

            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.ItemId;

            if (sender.TenantTypeId == TenantTypeIds.Instance().Bar())
            {
                newLog.OperationObjectName = "<a class=\"a\" target=\"_blank\" href=\"" + SiteUrls.Instance().SectionDetail(sender.ItemId) + "\">" + sender.ItemName + "</a>";
            }
            else if (sender.TenantTypeId == TenantTypeIds.Instance().CMS_Article()|| sender.TenantTypeId == TenantTypeIds.Instance().CMS_Image()||sender.TenantTypeId== TenantTypeIds.Instance().CMS_Video())
            {
                newLog.OperationObjectName = "<a class=\"a\" target=\"_blank\" href=\"" + SiteUrls.Instance().CMSDetail(sender.ItemId) + "\">" + sender.ItemName + "</a>";
            }          
            else if (sender.TenantTypeId == TenantTypeIds.Instance().Thread())
            {
                newLog.OperationObjectName = "<a class=\"a\" target=\"_blank\" href=\"" + SiteUrls.Instance().ThreadDetail(sender.ItemId) + "\">" + sender.ItemName + "</a>";
            }

            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Recommend();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));
            

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                attachmentService.ToggleTemporaryAttachments(sender.RecommenderUserId, TenantTypeIds.Instance().Recommend(), sender.Id, new List<long> { sender.FeaturedImageAttachmentId });
                if (sender.TenantTypeId == TenantTypeIds.Instance().Bar())
                {
                    newLog.Description = "推荐贴吧："+ sender.ItemName;
                }
                else if (sender.TenantTypeId == TenantTypeIds.Instance().CMS_Article())
                {
                    newLog.Description = "推荐文章：" + sender.ItemName + " 推荐类别：" + specialContentType.Name;
                }
                else if (sender.TenantTypeId == TenantTypeIds.Instance().CMS_Video())
                {
                    newLog.Description = "推荐视频：" + sender.ItemName + " 推荐类别：" + specialContentType.Name;
                }
                else if (sender.TenantTypeId == TenantTypeIds.Instance().CMS_Image())
                {
                    newLog.Description = "推荐组图：" + sender.ItemName + " 推荐类别：" + specialContentType.Name;
                }
                else if (sender.TenantTypeId == TenantTypeIds.Instance().Thread())
                {
                    newLog.Description = "推荐贴子：" + sender.ItemName + " 推荐类别：" + specialContentType.Name;
                }

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                attachmentService.DeletesByAssociateId(sender.Id);
                newLog.Description = "取消推荐：" + sender.ItemName + " 推荐类别：" + specialContentType.Name; ;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                attachmentService.ToggleTemporaryAttachments(sender.RecommenderUserId, TenantTypeIds.Instance().Recommend(), sender.Id, new List<long> { sender.FeaturedImageAttachmentId });
                newLog.Description = "编辑推荐内容：" + sender.ItemName;
            }

            operationLogService.Create(newLog);
        }

    }
}
