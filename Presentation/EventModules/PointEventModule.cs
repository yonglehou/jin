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

namespace Tunynet.Common
{
    /// <summary>
    /// 积分操作事件
    /// </summary>
    public class PointEventModule : IEventMoudle
    {
        private CategoryService categoryService;
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;
        private OperationLogService logService;
        private IOperatorInfoGetter operatorInfoGetter;
        private PointService pointService;
        public PointEventModule(CategoryService categoryService, OperationLogService operationLogService, UserService userService, RoleService roleService, OperationLogService logService, IOperatorInfoGetter operatorInfoGetter,PointService pointService)
        {
            this.categoryService = categoryService;
            this.operationLogService = operationLogService;
            this.userService = userService;
            this.roleService = roleService;
            this.logService = logService;
            this.operatorInfoGetter = operatorInfoGetter;
            this.pointService = pointService;

        }


        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<PointRecord, CommonEventArgs>.Instance().After += new CommonEventHandler<PointRecord, CommonEventArgs>(PointModuleForManagerOperation_After);
            EventBus<PointItem>.Instance().After += new CommonEventHandler<PointItem, CommonEventArgs>(PointItemModuleForManagerOperation_After);
        }

        /// <summary>
        /// 积分奖惩事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void PointModuleForManagerOperation_After(PointRecord sender, CommonEventArgs eventArgs)
        {
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                var OperatorUser = userService.GetUser(sender.OperatorUserId);
                var user = userService.GetUser(sender.UserId);
                //3、生成操作日志            
                if (OperatorUser == null || user == null)
                    return;

                OperationLog logEntry = new OperationLog(eventArgs.OperatorInfo);
                logEntry.OperationType = eventArgs.EventOperationType;
                logEntry.OperationObjectName = user.DisplayName;
                logEntry.OperationObjectId = user.UserId;
                logEntry.TenantTypeId = TenantTypeIds.Instance().Point();
                logEntry.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(OperatorUser.UserId));

                var ExperiencePoints = pointService.GetPointCategory(PointCategoryKeys.Instance().ExperiencePoints());
                var TradePoints = pointService.GetPointCategory(PointCategoryKeys.Instance().TradePoints());

                logEntry.Description = string.Format("奖惩用户 {0}: {1} {2}  {3} {4}",user.DisplayName, ExperiencePoints.CategoryName.ToString(),sender.ExperiencePoints.ToString(), TradePoints.CategoryName.ToString(),sender.TradePoints.ToString());

                logService.Create(logEntry);

            }

        }

        /// <summary>
        /// 积分规则事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void PointItemModuleForManagerOperation_After(PointItem sender, CommonEventArgs eventArgs)
        {
            if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
               
                OperationLog logEntry = new OperationLog(eventArgs.OperatorInfo);
                logEntry.OperationType = eventArgs.EventOperationType;
                logEntry.OperationObjectName = sender.ItemName;
                logEntry.OperationObjectId = 0;
                logEntry.TenantTypeId = TenantTypeIds.Instance().Point();
                logEntry.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(
                    eventArgs.OperatorInfo.OperationUserId));
                logEntry.Description = string.Format("修改积分规则{0}", sender.ItemName);

                logService.Create(logEntry);

            }

        }




    }
}