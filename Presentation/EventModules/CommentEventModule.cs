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
using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 评论创建和删除的事件
    /// </summary>
    public class CommentEventModule : IEventMoudle
    {

        private PointService pointService;
        private AuditService auditService;
        private TenantTypeService tenantTypeService;
        private ContentItemService contentItemService;
        private ThreadRepository threadRepository;
        private ThreadService threadService;
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;
        private CommentService commentService;
        private IKvStore kvStore;
        private NoticeService noticeService;
        private INoticeSender noticeSender;
        //构造
        public CommentEventModule(PointService pointService, AuditService auditService, TenantTypeService tenantTypeService,
            ContentItemService contentItemService, ThreadService threadService, OperationLogService operationLogService, UserService userService, RoleService roleService, CommentService commentService, ThreadRepository threadRepository, IKvStore kvStore, NoticeService noticeService, INoticeSender noticeSender)
        {
            this.pointService = pointService;
            this.auditService = auditService;
            this.tenantTypeService = tenantTypeService;
            this.contentItemService = contentItemService;
            this.threadService = threadService;
            this.operationLogService = operationLogService;
            this.userService = userService;
            this.commentService = commentService;
            this.roleService = roleService;
            this.threadRepository = threadRepository;
            this.kvStore = kvStore;
            this.noticeService = noticeService;
            this.noticeSender = noticeSender;
        }
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Comment>.Instance().After += new CommonEventHandler<Comment, CommonEventArgs>(CommentModuleForManagerOperation_After);
            EventBus<Comment>.Instance().After += new CommonEventHandler<Comment, CommonEventArgs>(CommentIndex_After);
            EventBus<Comment, AuditEventArgs>.Instance().After += new CommonEventHandler<Comment, AuditEventArgs>(CommentPointEventModule_After);
            EventBus<Comment, CommonEventArgs>.Instance().After += new CommonEventHandler<Comment, CommonEventArgs>(CommentModuleForOperationLog_After);
        }



        /// <summary>
        /// 评论创建处理数量事件
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="eventArgs"></param> 
        private void CommentModuleForManagerOperation_After(Comment comment, CommonEventArgs eventArgs)
        {

            var commentService = DIContainer.Resolve<CommentService>();
            Comment commentNew = commentService.Get(comment.ParentId);
            if (commentNew != null)
            {
                if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
                    //维护 子集个数和用户评论计数
                    commentNew.ChildrenCount++;
                else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
                    //维护 子集个数和用户评论计数
                    commentNew.ChildrenCount--;
                commentService.Update(commentNew);
            }
            CountService countService = new CountService(comment.TenantTypeId);
            CountService threadAndPostCountService = new CountService(TenantTypeIds.Instance().Section());

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
				//对象评论计数
                countService.ChangeCount(CountTypes.Instance().CommentCount(), comment.CommentedObjectId, 0, 1, true);
                if (comment.TenantTypeId == TenantTypeIds.Instance().Thread())
                {
                    //评论贴子则更新回复时间 （调用Repository不触发service中update事件
                    var thread = threadService.Get(comment.CommentedObjectId);
                    thread.LastModified = DateTime.Now;
                    threadRepository.Update(thread);
                    //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                    kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, TenantTypeIds.Instance().Thread(), comment.ApprovalStatus));
                    kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, TenantTypeIds.Instance().Thread()));
                    threadAndPostCountService.ChangeCount(CountTypes.Instance().ThreadAndPostCount(), thread.BarSection.SectionId, 0, 1, true);
                }

              
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, null, comment.ApprovalStatus));
                kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, null));

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
				 //对象评论计数
                countService.ChangeCount(CountTypes.Instance().CommentCount(), comment.CommentedObjectId, 0, -1, true);
                if (comment.TenantTypeId == TenantTypeIds.Instance().Thread())
                {
                    var thread = threadService.Get(comment.CommentedObjectId);
                    //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                    kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, TenantTypeIds.Instance().Thread(), comment.ApprovalStatus), -1);
                    kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, TenantTypeIds.Instance().Thread()), -1);

                    threadAndPostCountService.ChangeCount(CountTypes.Instance().ThreadAndPostCount(), thread.BarSection.SectionId, 0, -1, true);
                }
             
                   
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, null, comment.ApprovalStatus), -1);
                kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, null), -1);
            }
        }

        /// <summary>
        /// *评论通过审核事件处理
        /// *积分操作事件
        /// *发送评论审核通过通知
        /// *处理用户评论计数
        /// </summary>
        /// <param name="comment">评论实体</param>
        /// <param name="eventArgs">审核状态</param>
        private void CommentPointEventModule_After(Comment comment, AuditEventArgs eventArgs)
        {
            string pointItemKey = string.Empty;
            string eventOperationType = string.Empty;
            string description = string.Empty;
            string tenantTypeId = comment.TenantTypeId;

            if (comment.TenantTypeId == TenantTypeIds.Instance().ContentItem())
            {
                var contentItem = contentItemService.Get(comment.CommentedObjectId);

                if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Article() || contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Contribution())
                {
                    tenantTypeId = TenantTypeIds.Instance().CMS_Article();
                }
                else if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Image())
                {
                    tenantTypeId = TenantTypeIds.Instance().CMS_Image();
                }
                else if (contentItem.ContentModel.ModelKey == ContentModelKeys.Instance().Video())
                {
                    tenantTypeId = TenantTypeIds.Instance().CMS_Video();
                }
            }



            //评论审核通过则发送通知给相应用户
            Notice notice = Notice.New();
            notice.LeadingActor = comment.Author;
            notice.LeadingActorUserId = comment.UserId;
            notice.RelativeObjectUrl = comment.GetCommentedObjectUrl();
            notice.LeadingActorUrl = SiteUrls.Instance().SpaceHome(comment.UserId);
            notice.Body = comment.Body;
            notice.ObjectId = comment.Id;
            if (comment.ParentId != 0)
            {
                var parentComment = commentService.Get(comment.ParentId);
                notice.ReceiverId = parentComment.OwnerId;
                notice.NoticeTypeKey = NoticeTypeKeys.Instance().NewReply(tenantTypeId);
                notice.RelativeObjectId = comment.CommentedObjectId;
                notice.RelativeObjectName = parentComment.Body;
            }
            else
            {
                notice.ReceiverId = comment.GetCommentedObject().UserId;
                notice.NoticeTypeKey = NoticeTypeKeys.Instance().NewReply(tenantTypeId);
                notice.RelativeObjectId = comment.CommentedObjectId;
                notice.RelativeObjectName = comment.GetCommentedObject().Name;
            }

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                //发送通知
                if (comment.ApprovalStatus == AuditStatus.Success && notice.ReceiverId != eventArgs.OperatorInfo.OperationUserId && notice.NoticeTypeKey != NoticeTypeKeys.Instance().NewReply(null))
                {
                    noticeService.Create(notice);
                    noticeSender.Send(notice);
                }
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Approved())
            {
                if (comment.TenantTypeId == TenantTypeIds.Instance().Thread())
                {
                    //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                    kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, TenantTypeIds.Instance().Thread(), eventArgs.OldAuditStatus), -1);
                    kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, TenantTypeIds.Instance().Thread(), eventArgs.NewAuditStatus));
                }
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, null, eventArgs.OldAuditStatus), -1);
                kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, null, eventArgs.NewAuditStatus));
                //发送通知
                if (comment.ApprovalStatus == AuditStatus.Success && notice.ReceiverId != eventArgs.OperatorInfo.OperationUserId)
                {
                    noticeService.Create(notice);
                    noticeSender.Send(notice);
                }



            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Disapproved())
            {
                if (comment.TenantTypeId == TenantTypeIds.Instance().Thread())
                    //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                    kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, TenantTypeIds.Instance().Thread(), eventArgs.OldAuditStatus), -1);
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserCommentCount(comment.UserId, null, eventArgs.OldAuditStatus), -1);

            }


            bool? auditDirection = auditService.ResolveAuditDirection(eventArgs.OldAuditStatus, eventArgs.NewAuditStatus);
            if (auditDirection == true) //加积分
            {
                pointItemKey = PointItemKeys.Instance().CreateComment();
                if (eventArgs.OldAuditStatus == null)
                    eventOperationType = EventOperationType.Instance().Create();
                else
                    eventOperationType = EventOperationType.Instance().Approved();
            }
            else if (auditDirection == false) //减积分
            {
                pointItemKey = PointItemKeys.Instance().DeleteComment();
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
                    description = string.Format("发布评论");
                }
                else if (eventOperationType == EventOperationType.Instance().Delete())
                {
                    description = string.Format("删除评论");
                }
                else if (eventOperationType == EventOperationType.Instance().Approved())
                {
                    description = string.Format("评论通过审核");
                }
                else if (eventOperationType == EventOperationType.Instance().Disapproved())
                {
                    description = string.Format("评论不通过审核");
                }
                pointService.GenerateByRoles(comment.UserId, eventArgs.OperatorInfo.OperationUserId, pointItemKey, description, comment.TenantTypeId, comment.Id, comment.Subject, false, true);
            }
        }
        /// <summary>
        /// 评论操作日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void CommentModuleForOperationLog_After(Comment sender, CommonEventArgs eventArgs)
        {
            var commentedObject = commentService.Get(sender.Id).GetCommentedObject();

            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.Id;
            newLog.OperationObjectName = "<a class=\"a\" target=\"_blank\" href=" + commentedObject.DetailUrl + ">" + commentedObject.Name + "</a>";
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Comment();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = "发布评论：" + sender.Body;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Approved())
            {
                newLog.Description = "评论通过审核：" + sender.Body;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Disapproved())
            {
                newLog.Description = "评论不通过审核：" + sender.Body;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除评论: {0}", sender.Body);
            }

            operationLogService.Create(newLog);


        }


        #region 评论增量索引

        /// <summary>
        /// 评论增量索引
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="eventArgs"></param>
        private void CommentIndex_After(Comment comment, CommonEventArgs eventArgs)
        {
            if (comment == null)
            {
                return;
            }

            //添加索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                kvStore.Append(KvKeys.Instance().CommentSearch(), comment.Id);
            }

            //删除索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                kvStore.Append(KvKeys.Instance().CommentDeleteSerach(), comment.Id);
            }

            //更新索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                kvStore.Append(KvKeys.Instance().CommentUpdateSearch(), comment.Id);
            }
        }

        #endregion
    }
}