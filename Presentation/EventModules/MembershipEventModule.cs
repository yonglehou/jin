//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using Tunynet.CMS;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Utilities;
using System.Collections.Generic;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户管理事件
    /// </summary>
    public class MembershipEventModule : IEventMoudle
    {
        private RoleService roleService;
        private OperationLogService operationLogService;
        private UserRepository userRepository;
        private PointService pointService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MembershipEventModule(RoleService roleService, OperationLogService operationLogService, UserRepository userRepository,PointService pointService)
        {
            this.roleService = roleService;
            this.operationLogService = operationLogService;
            this.userRepository = userRepository;
            this.pointService = pointService;
        }
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<User>.Instance().After +=new CommonEventHandler<User, CommonEventArgs>(MembershipEventModuleForOperationLog_After) ;      
        }
       


        /// <summary>
        /// 用户操作日志事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void MembershipEventModuleForOperationLog_After(User sender, CommonEventArgs eventArgs)
        {
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = sender.UserId;
            newLog.OperationObjectName = "<a class=\"a\" target=\"_blank\" href=" + SiteUrls.Instance().SpaceHome(sender.UserId) + ">" + sender.UserName + "</a>";
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().User();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));

            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = "用户注册：" + sender.UserName;
                //注册增加积分
                var pointItemKey = PointItemKeys.Instance().Register();
                string description = string.Format("用户注册");
                pointService.GenerateByRole(sender.UserId, sender.UserId, pointItemKey, description);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = "删除用户：" + sender.UserName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = "编辑用户：" + sender.UserName;
                if (sender.IsModerated)
                    newLog.Description += "(管制)";
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().ActivateUser())
            {
                newLog.Description = "激活用户：" + sender.UserName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().UserEmailVerified())
            {
                newLog.Description = "通过邮箱验证：" + sender.UserName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().CancelActivateUser())
            {
                newLog.Description = "取消激活用户：" + sender.UserName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().BanUser())
            {
                newLog.Description = "封禁用户：" + sender.UserName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().ResetPassword())
            {
                newLog.Description = "用户：" + sender.UserName+" 重设密码";
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().UnbanUser())
            {
                newLog.Description = "解禁用户：" + sender.UserName;
                if (newLog.Operator == null || newLog.OperatorIP == null)
                {
                    newLog.Description = "自动解禁用户：" + sender.UserName;
                    newLog.Operator = string.Empty;
                    newLog.OperatorIP = string.Empty;
                }
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().ModerateUser())
            {
                newLog.Description = "管制用户：" + sender.UserName;
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().CancelModerateUser())
            {
                newLog.Description = "取消管制用户：" + sender.UserName;
                if (newLog.Operator == null || newLog.OperatorIP == null)
                {
                    newLog.Description = "自动取消管制用户：" + sender.UserName;
                    newLog.Operator = string.Empty;
                    newLog.OperatorIP = string.Empty;
                }

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().SignIn())
            {
                newLog.Description = "用户登录：" + sender.UserName;
                //更新用户上次登录ip和时间
                var user=userRepository.Get(sender.UserId);
                user.IpLastActivity = WebUtility.GetIP();
                user.LastActivityTime = DateTime.Now;
                userRepository.Update(user);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().SignOut())
            {
                newLog.Description = "用户登出：" + sender.UserName;
            }
          
            operationLogService.Create(newLog);
        }
    }
}
