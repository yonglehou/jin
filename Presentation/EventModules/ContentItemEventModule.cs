//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Tunynet.CMS;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Spacebuilder;

namespace Tunynet.Common
{
    /// <summary>
    /// ContentItem事件
    /// </summary>
    public class ContentItemEventModule : IEventMoudle
    {
        private PointService pointService;
        private AuditService auditService;
        private ContentItemService contentItemService;
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;
        private IKvStore kvStore;
        private SpecialContentitemService specialContentitemService;
        private TagService tagService = new TagService(TenantTypeIds.Instance().ContentItem());
        private FavoriteService favoriteService;
        private NoticeService noticeService;
        private INoticeSender noticeSender;
        public ContentItemEventModule()
        {

        }
        public ContentItemEventModule(PointService pointService,
            AuditService auditService,
            ContentItemService contentItemService,
            RoleService roleService,
            UserService userService,
            OperationLogService operationLogService,
            SpecialContentitemService specialContentitemService,
            IKvStore kvStore,
            FavoriteService favoriteService,
             NoticeService noticeService,
             INoticeSender noticeSender)
        {
            this.pointService = pointService;
            this.auditService = auditService;
            this.contentItemService = contentItemService;
            this.operationLogService = operationLogService;
            this.userService = userService;
            this.roleService = roleService;
            this.kvStore = kvStore;
            this.specialContentitemService = specialContentitemService;
            this.favoriteService = favoriteService;
            this.noticeService = noticeService;
            this.noticeSender = noticeSender;
        }

        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {

            EventBus<ContentItem, AttachmentEventArgs>.Instance().After += new CommonEventHandler<ContentItem, AttachmentEventArgs>(ContentItemModuleForManagerOperation_After);
            EventBus<ContentItem, CommonEventArgs>.Instance().After += new CommonEventHandler<ContentItem, CommonEventArgs>(ContentItemIndex_After);
            EventBus<ContentItem, AuditEventArgs>.Instance().After += new CommonEventHandler<ContentItem, AuditEventArgs>(ContentItemPointModule_After);
            EventBus<ContentItem, CommonEventArgs>.Instance().After += new CommonEventHandler<ContentItem, CommonEventArgs>(ContentItemModuleForOperationLog_After);
        }
        /// <summary>
        /// 资讯相关事件
        /// </summary>
        /// <param name="contentItem"></param>
        /// <param name="eventArgs"></param> 
        private void ContentItemModuleForManagerOperation_After(ContentItem contentItem, AttachmentEventArgs eventArgs)
        {
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                if (!eventArgs.IsMobile)
                {
                    var attachmentService = new AttachmentService(eventArgs.TenantTypeId);
                    attachmentService.ToggleTemporaryAttachments(contentItem.UserId, eventArgs.TenantTypeId, contentItem.ContentItemId, contentItem.BodyImageAttachmentIds);
                }

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {


                //删除附件
                var attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Article());
                if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Image())
                {
                    attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Image());
                    attachmentService.DeletesByAssociateId(contentItem.ContentItemId);
                }
                else if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Video())
                {
                    attachmentService = new AttachmentService(TenantTypeIds.Instance().CMS_Video());
                    attachmentService.DeletesByAssociateId(contentItem.ContentItemId);
                }
                else
                {
                    attachmentService.DeletesByAssociateId(contentItem.ContentItemId);
                }
                //删除推荐
                specialContentitemService.DeleteSpecialContentItem(contentItem.ContentItemId, TenantTypeIds.Instance().ContentItem());
                specialContentitemService.DeleteSpecialContentItem(contentItem.ContentItemId, TenantTypeIds.Instance().CMS_Article());
                specialContentitemService.DeleteSpecialContentItem(contentItem.ContentItemId, TenantTypeIds.Instance().CMS_Image());
                specialContentitemService.DeleteSpecialContentItem(contentItem.ContentItemId, TenantTypeIds.Instance().CMS_Video());
                //标签内容数减1
                var tags = tagService.attiGetItemInTagsOfItem(contentItem.ContentItemId);
                foreach (var item in tags)
                {
                    tagService.SetItemCount(item.TagName);
                }
                //删除标签与内容项的关联
                tagService.DeleteTagFromItem(contentItem.ContentItemId);
                //删除收藏
                favoriteService.DeleteTrashDatas(MultiTenantServiceKeys.Instance().Favorites());

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                var attachmentService = new AttachmentService(eventArgs.TenantTypeId);
                attachmentService.ToggleTemporaryAttachments(contentItem.UserId, eventArgs.TenantTypeId, contentItem.ContentItemId, contentItem.BodyImageAttachmentIds);
            }

        }
        /// <summary>
        /// *资讯通过审核处理事件
        /// *处理用户积分
        /// *处理用户个人计数
        /// *发送通知给用户
        /// </summary>
        /// <param name="contentItem"></param>
        /// <param name="eventArgs"></param>
        private void ContentItemPointModule_After(ContentItem contentItem, AuditEventArgs eventArgs)
        {
            string pointItemKey = string.Empty;
            string eventOperationType = string.Empty;
            string description = string.Empty;
            // 通知被评论用户
            var currentUser = userService.GetUser(contentItem.UserId);
          
          
          

            //资讯计数处理
            if (eventArgs.EventOperationType == EventOperationType.Instance().Approved())
            {
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution(), eventArgs.OldAuditStatus), -1);
                kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution(), eventArgs.NewAuditStatus));

                //发送通知
                if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Contribution())
                {
                    Notice notice = Notice.New();
                    notice.LeadingActor = currentUser.DisplayName;
                    notice.LeadingActorUserId = currentUser.UserId;
                    notice.RelativeObjectUrl = SiteUrls.Instance().CMSDetail(contentItem.ContentItemId);
                    notice.LeadingActorUrl = SiteUrls.Instance().SpaceHome(currentUser.UserId);
                    notice.ObjectId = contentItem.ContentItemId;
                    notice.ReceiverId = contentItem.UserId;
                    notice.RelativeObjectId = contentItem.ContentItemId;
                    notice.RelativeObjectName = contentItem.Subject;
                    notice.Status = NoticeStatus.Unhandled;
                    notice.NoticeTypeKey = NoticeTypeKeys.Instance().ManagerApproved(TenantTypeIds.Instance().CMS_Article());
                    noticeService.Create(notice);
                    noticeSender.Send(notice);
                }



            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Disapproved())
            {
                kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution(), eventArgs.OldAuditStatus), -1);

                ////发送通知
                //if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Contribution())
                //{
                //    Notice notice = Notice.New();
                //    notice.LeadingActor = currentUser.DisplayName;
                //    notice.LeadingActorUserId = currentUser.UserId;
                //    notice.RelativeObjectUrl = SiteUrls.Instance().CMSDetail(contentItem.ContentItemId);
                //    notice.LeadingActorUrl = SiteUrls.Instance().SpaceHome(currentUser.UserId);
                //    notice.ObjectId = contentItem.ContentItemId;
                //    notice.ReceiverId = contentItem.UserId;
                //    notice.RelativeObjectId = contentItem.ContentItemId;
                //    notice.RelativeObjectName = contentItem.Subject;
                //    notice.Status = NoticeStatus.Unhandled;
                //    notice.NoticeTypeKey = NoticeTypeKeys.Instance().ManagerDisapproved(TenantTypeIds.Instance().CMS_Article());
                //    noticeService.Create(notice);
                //    noticeSender.Send(notice);
                //}

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                if (contentItem.ApprovalStatus != 0)
                {
                    kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution(), contentItem.ApprovalStatus));
                    kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution()));
                }
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution(), contentItem.ApprovalStatus), -1);
                kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution()), -1);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {

                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                if (contentItem.ApprovalStatus != 0 && contentItem.IsDraft == true)
                {

                    kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution(), contentItem.ApprovalStatus));
                    kvStore.Increase(KvKeys.Instance().UserContentItemCount(contentItem.UserId, ContentModelKeys.Instance().Contribution()));
                }

            }
         

            bool? auditDirection = auditService.ResolveAuditDirection(eventArgs.OldAuditStatus, eventArgs.NewAuditStatus);
            if (auditDirection == true) //加积分
            {
                pointItemKey = PointItemKeys.Instance().CreateContentItem();
                if (eventArgs.OldAuditStatus == null)
                    eventOperationType = EventOperationType.Instance().Create();
                else
                    eventOperationType = EventOperationType.Instance().Approved();
            }
            else if (auditDirection == false) //减积分
            {
                pointItemKey = PointItemKeys.Instance().DeleteContentItem();
                if (eventArgs.NewAuditStatus == null)
                    eventOperationType = EventOperationType.Instance().Delete();
                else
                    eventOperationType = EventOperationType.Instance().Disapproved();
            }
            if (!string.IsNullOrEmpty(pointItemKey))
            {
                //描述
                if (eventOperationType == EventOperationType.Instance().Create())
                {
                    description = string.Format("发布资讯");

                }
                else if (eventOperationType == EventOperationType.Instance().Delete())
                {
                    description = string.Format("删除资讯");
                }
                else if (eventOperationType == EventOperationType.Instance().Approved())
                {

                    string cmsSubject = contentItemService.Get(contentItem.ContentItemId).Subject;
                    description = string.Format("资讯通过审核");
                }
                else if (eventOperationType == EventOperationType.Instance().Disapproved())
                {
                    string cmsSubject = contentItemService.Get(contentItem.ContentItemId).Subject;
                    description = string.Format("资讯不通过审核");
                }

                pointService.GenerateByRoles(contentItem.UserId, eventArgs.OperatorInfo.OperationUserId, pointItemKey, description, TenantTypeIds.Instance().ContentItem(), contentItem.ContentItemId, contentItem.Subject, false, false);
            }
        }

        /// <summary>
        /// 生成操作日志事件
        /// </summary>
        /// <param name="contentItem"></param>
        /// <param name="eventArgs"></param> 
        private void ContentItemModuleForOperationLog_After(ContentItem contentItem, CommonEventArgs eventArgs)
        {

            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = contentItem.ContentItemId;
            newLog.OperationObjectName = "<a class=\"a\" target=\"_blank\" href=" + SiteUrls.Instance().CMSDetail(contentItem.ContentItemId) + ">" + contentItem.Subject + "</a>";
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().ContentItem();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = "发布资讯：" + contentItem.Subject;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = "更新资讯：" + contentItem.Subject;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Approved())
            {
                newLog.Description = "资讯通过审核：" + contentItem.Subject;
             
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Disapproved())
            {
                newLog.Description = "资讯不通过审核：" + contentItem.Subject;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除资讯 {0}", contentItem.Subject);
            }

            operationLogService.Create(newLog);
        }

        #region 资讯增量索引

        /// <summary>
        /// 资讯增量索引
        /// </summary>
        private void ContentItemIndex_After(ContentItem contentItem, CommonEventArgs eventArgs)
        {
            if (contentItem == null)
            {
                return;
            }

            //添加索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                if (contentItem.ApprovalStatus == 0)
                    return;
                kvStore.Append(KvKeys.Instance().CmsSearch(), contentItem.ContentItemId);
            }

            //删除索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                kvStore.Append(KvKeys.Instance().CmsDeleteSearch(), contentItem.ContentItemId);
            }

            //更新索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                kvStore.Append(KvKeys.Instance().CmsUpdateSearch(), contentItem.ContentItemId);

            }
        }

        #endregion


    }
}