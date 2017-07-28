//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 贴子相关事件
    /// </summary>
    public class ThreadEventModule : IEventMoudle
    {
        private CountService countService = new CountService(TenantTypeIds.Instance().Section());
        private SpecialContentitemService specialContentitemService;
        private IOperatorInfoGetter operatorInfoGetter;
        private CommentService commentService;
        private PointService pointService;
        private AuditService auditService;
        private FavoriteService favoriteService;
        private UserService userService;
        private RoleService roleService;
        private OperationLogService operationLogService;
        private AttachmentService attachmentService;
        private IKvStore kvStore;
        private NoticeService noticeService;
        private INoticeSender noticeSender;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ThreadEventModule(SpecialContentitemService specialContentitemService,
            IOperatorInfoGetter operatorInfoGetter,
            CommentService commentService,
            PointService pointService,
            RoleService roleService,
            UserService userService,
            AuditService auditService,
            OperationLogService operationLogService,
            IKvStore kvStore,
            NoticeService noticeService,
             INoticeSender noticeSender
            )
        {
            this.commentService = commentService;
            this.operatorInfoGetter = operatorInfoGetter;
            this.specialContentitemService = specialContentitemService;
            this.pointService = pointService;
            this.auditService = auditService;
            this.userService = userService;
            this.roleService = roleService;
            this.operationLogService = operationLogService;
            this.kvStore = kvStore;
            favoriteService = new FavoriteService(TenantTypeIds.Instance().Thread());
            attachmentService = new AttachmentService(TenantTypeIds.Instance().Thread());
            this.noticeService = noticeService;
            this.noticeSender = noticeSender;
        }
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Thread, AttachmentEventArgs>.Instance().After += new CommonEventHandler<Thread, AttachmentEventArgs>(ThreadModuleForManagerOperation_After);
            EventBus<Thread>.Instance().After += new CommonEventHandler<Thread, CommonEventArgs>(ThreadIndex_After);
            EventBus<Thread, AuditEventArgs>.Instance().After += new CommonEventHandler<Thread, AuditEventArgs>(ThreadPointModule_After);
            EventBus<Thread, CommonEventArgs>.Instance().After += new CommonEventHandler<Thread, CommonEventArgs>(ThreadModuleForOperationLog_After);
        }





        /// <summary>
        /// 执行贴子操作时操作关联内容
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="eventArgs"></param>
        private void ThreadModuleForManagerOperation_After(Thread thread, AttachmentEventArgs eventArgs)
        {
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                if (!eventArgs.IsMobile)
                {
                    attachmentService.ToggleTemporaryAttachments(thread.UserId, thread.TenantTypeId, thread.ThreadId, thread.BodyImageAttachmentIds);
                }
                //计数处理
                countService.ChangeCount(CountTypes.Instance().ThreadCount(), thread.BarSection.SectionId, thread.BarSection.SectionId, 1, true);
                countService.ChangeCount(CountTypes.Instance().ThreadAndPostCount(), thread.BarSection.SectionId, thread.BarSection.SectionId, 1, true);

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                //计数处理
                countService.ChangeCount(CountTypes.Instance().ThreadCount(), thread.BarSection.SectionId, thread.BarSection.SectionId, -1, true);
                countService.ChangeCount(CountTypes.Instance().ThreadAndPostCount(), thread.BarSection.SectionId, thread.BarSection.SectionId, -1, true);

                //删除推荐
                specialContentitemService.DeleteSpecialContentItem(thread.ThreadId, TenantTypeIds.Instance().Thread());
                //删除评论
                commentService.DeleteCommentedObjectComments(thread.ThreadId, TenantTypeIds.Instance().Thread());
                //删除附件
                attachmentService.DeletesByAssociateId(thread.ThreadId);
                //删除收藏
                //var userIdList = favoriteService.GetUserIdsOfObject(sender.ThreadId);
                //foreach (var item in userIdList)
                //{
                //    favoriteService.CancelFavorite(sender.ThreadId, item);
                //}
                favoriteService.DeleteTrashDatas(MultiTenantServiceKeys.Instance().Favorites());


            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                attachmentService.ToggleTemporaryAttachments(thread.UserId, thread.TenantTypeId, thread.ThreadId, thread.BodyImageAttachmentIds);
            }

        }
        /// <summary>
        /// 执行贴子操作时改变积分
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="eventArgs"></param>
        private void ThreadPointModule_After(Thread thread, AuditEventArgs eventArgs)
        {

            string pointItemKey = string.Empty;
            string eventOperationType = string.Empty;
            string description = string.Empty;

            //贴子计数处理
            if (eventArgs.EventOperationType == EventOperationType.Instance().Approved() || eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserThreadCount(thread.UserId, thread.TenantTypeId, eventArgs.OldAuditStatus), -1);
                kvStore.Increase(KvKeys.Instance().UserThreadCount(thread.UserId, thread.TenantTypeId, eventArgs.NewAuditStatus));
                // 通知被评论用户
                var currentUser = userService.GetUser(thread.UserId);
                Notice notice = Notice.New();
                notice.LeadingActor = currentUser.DisplayName;
                notice.LeadingActorUserId = currentUser.UserId;
                notice.RelativeObjectUrl = SiteUrls.Instance().ThreadDetail(thread.ThreadId);
                notice.LeadingActorUrl = SiteUrls.Instance().SpaceHome(currentUser.UserId);
                notice.ObjectId = thread.ThreadId;
                notice.ReceiverId = thread.UserId;
                notice.RelativeObjectId = thread.ThreadId;
                notice.RelativeObjectName = thread.Subject;
                notice.Status = NoticeStatus.Unhandled;
                if (eventArgs.EventOperationType == EventOperationType.Instance().Approved())
                {
                    notice.NoticeTypeKey = NoticeTypeKeys.Instance().ManagerApproved(thread.TenantTypeId);
                    noticeService.Create(notice);
                    noticeSender.Send(notice);
                }

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Disapproved())
            {
                kvStore.Increase(KvKeys.Instance().UserThreadCount(thread.UserId, thread.TenantTypeId, eventArgs.OldAuditStatus), -1);

                //// 通知被评论用户
                //var currentUser = userService.GetUser(thread.UserId);
                //Notice notice = Notice.New();
                //notice.LeadingActor = currentUser.DisplayName;
                //notice.LeadingActorUserId = currentUser.UserId;
                //notice.RelativeObjectUrl = SiteUrls.Instance().ThreadDetail(thread.ThreadId);
                //notice.LeadingActorUrl = SiteUrls.Instance().SpaceHome(currentUser.UserId);
                //notice.ObjectId = thread.ThreadId;
                //notice.ReceiverId = thread.UserId;
                //notice.RelativeObjectId = thread.ThreadId;
                //notice.RelativeObjectName = thread.Subject;
                //notice.Status = NoticeStatus.Unhandled;
                //notice.NoticeTypeKey = NoticeTypeKeys.Instance().ManagerDisapproved(thread.TenantTypeId);
                //noticeService.Create(notice);
                //noticeSender.Send(notice);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserThreadCount(thread.UserId, thread.TenantTypeId, thread.ApprovalStatus));
                kvStore.Increase(KvKeys.Instance().UserThreadCount(thread.UserId, thread.TenantTypeId));
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                //计数分为两个一个为个人空间计数,一个为发布时候的每个审核状态 计数
                kvStore.Increase(KvKeys.Instance().UserThreadCount(thread.UserId, thread.TenantTypeId, thread.ApprovalStatus), -1);
                kvStore.Increase(KvKeys.Instance().UserThreadCount(thread.UserId, thread.TenantTypeId), -1);
            }



            bool? auditDirection = auditService.ResolveAuditDirection(eventArgs.OldAuditStatus, eventArgs.NewAuditStatus);
            if (auditDirection == true) //加积分
            {
                pointItemKey = PointItemKeys.Instance().CreateThread();
                if (eventArgs.OldAuditStatus == null)
                    eventOperationType = EventOperationType.Instance().Create();
                else
                    eventOperationType = EventOperationType.Instance().Approved();
            }
            else if (auditDirection == false) //减积分
            {
                pointItemKey = PointItemKeys.Instance().DeleteThread();
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

                    description = string.Format("发布贴子");
                }
                else if (eventOperationType == EventOperationType.Instance().Delete())
                {

                    description = string.Format("删除贴子");
                }
                else if (eventOperationType == EventOperationType.Instance().Approved())
                {

                    description = string.Format("贴子通过审核");
                }
                else if (eventOperationType == EventOperationType.Instance().Disapproved())
                {
                    description = string.Format("贴子不通过审核");
                }
                pointService.GenerateByRoles(thread.UserId, eventArgs.OperatorInfo.OperationUserId, pointItemKey, description, thread.TenantTypeId, thread.ThreadId, thread.Subject, false, false);
            }
        }

        /// <summary>
        /// 生成操作日志事件
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="eventArgs"></param> 
        private void ThreadModuleForOperationLog_After(Thread thread, CommonEventArgs eventArgs)
        {

            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = thread.ThreadId;
            newLog.OperationObjectName = "<a class=\"a\" target=\"_blank\" href=\"" + SiteUrls.Instance().ThreadDetail(thread.ThreadId) + "\">" + thread.Subject + "</a>";
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Thread();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = "发布贴子：" + thread.Subject;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = "更新贴子：" + thread.Subject;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Approved())
            {
                newLog.Description = "贴子通过审核：" + thread.Subject;


            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Disapproved())
            {
                newLog.Description = "贴子不通过审核：" + thread.Subject;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().SetSticky())
            {
                newLog.Description = "置顶贴子：" + thread.Subject;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().CancelSticky())
            {
                newLog.Description = "取消置顶贴子：" + thread.Subject;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除贴子：{0}", thread.Subject);
            }

            operationLogService.Create(newLog);
        }

        #region 贴子增量索引


        /// <summary>
        /// 贴子增量索引
        /// </summary>
        private void ThreadIndex_After(Thread thread, CommonEventArgs eventArgs)
        {
            if (thread == null)
            {
                return;
            }

            //添加索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                kvStore.Append(KvKeys.Instance().ThreadSearch(), thread.ThreadId);
            }

            //删除索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                kvStore.Append(KvKeys.Instance().ThreadDeleteSearch(), thread.ThreadId);
            }

            //更新索引
            if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                kvStore.Append(KvKeys.Instance().ThreadUpdateSearch(), thread.ThreadId);
            }
        }

        #endregion
    }
}