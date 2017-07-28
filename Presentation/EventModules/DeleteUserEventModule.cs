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
using Tunynet.Attitude;
using Tunynet.CMS;
using Tunynet.Events;
using Tunynet.Logging;

namespace Tunynet.Common
{
    /// <summary>
    /// 删除用户时事件
    /// </summary>
    public class DeleteUserEventModule : IEventMoudle
    {
        private OperationLogService operationLogService;
        private RoleService roleService;
        private MessageService messageService;
        private UserProfileService userProfileService;
        private IKvStore kvStore;
        private TenantTypeService tenantTypeService;
        private ContentModelService contentModelService;
        private CategoryService categoryService;
        private CommentService commentService;
        private NoticeService noticeService;
        private FollowService followService;
        private AccountBindingService accountBindingService;
        private UserService userService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeleteUserEventModule(OperationLogService operationLogService,
            RoleService roleService, MessageService messageService,
            UserProfileService userProfileService,
            IKvStore kvStore,
            TenantTypeService tenantTypeService,
            ContentModelService contentModelService,
            CategoryService categoryService,
            CommentService commentService,
            NoticeService noticeService,
            FollowService followService,
            AccountBindingService accountBindingService,
            UserService userService)
        {
            this.operationLogService = operationLogService;
            this.roleService = roleService;
            this.messageService = messageService;
            this.userProfileService = userProfileService;
            this.kvStore = kvStore;
            this.tenantTypeService = tenantTypeService;
            this.contentModelService = contentModelService;
            this.categoryService = categoryService;
            this.commentService = commentService;
            this.noticeService = noticeService;
            this.followService = followService;
            this.accountBindingService = accountBindingService;
            this.userService = userService;

        }

        /// <summary>
        /// 注册事件处理方法
        /// </summary>
        public void RegisterEventHandler()
        {

            EventBus<User, DeleteUserEventArgs>.Instance().After += new CommonEventHandler<User, DeleteUserEventArgs>(DeleteUserEventMoudle_After);
        }

        /// <summary>
        /// 删除用户事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void DeleteUserEventMoudle_After(User sender, DeleteUserEventArgs eventArgs)
        {

            ////删除用户信息
            //userProfileService.Delete(sender.UserId);

            //删除用户所有计数相关的kvstore里计数及其他数据
            userService.DeleteUserCount(sender.UserId);


            ////清除用户关于分类的数据
            //categoryService.CleanByUser(sender.UserId);


            ////清除用户评论
            //commentService.DeleteUserComments(sender.UserId, false);

            //清除用户的私信
            messageService.ClearSessionsFromUser(sender.UserId);

            //清除通知的用户数据
            noticeService.CleanByUser(sender.UserId);

            //清除用户关于关注用户的数据
            followService.CleanByUser(sender.UserId);
            //清除帐号绑定数据
            var accountBindings = accountBindingService.GetAccountBindings(sender.UserId);
            foreach (var accountBinding in accountBindings)
            {
                accountBindingService.DeleteAccountBinding(accountBinding.UserId, accountBinding.AccountTypeKey);
            }
        }

    }
}
