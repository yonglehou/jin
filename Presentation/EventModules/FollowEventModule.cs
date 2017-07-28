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

namespace Tunynet.Common
{
    public class FollowEventModule : IEventMoudle
    {
        private PointService pointService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public FollowEventModule(PointService pointService)
        {
            this.pointService = pointService;
        }

        /// <summary>
        /// 注册事件处理方法
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<FollowEntity>.Instance().After += new CommonEventHandler<FollowEntity, CommonEventArgs>(FollowPointEventModule_After);
        }
        /// <summary>
        /// 关注用户积分事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void FollowPointEventModule_After(FollowEntity sender, CommonEventArgs eventArgs)
        {
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                var pointItemKey = PointItemKeys.Instance().FollowUser();
                string description = string.Format("关注用户");
                pointService.GenerateByRole(sender.UserId, sender.UserId, pointItemKey, description);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                var pointItemKey = PointItemKeys.Instance().CancelFollowUser();
                string description = string.Format("取消关注用户");
                pointService.GenerateByRole(sender.UserId, sender.UserId, pointItemKey, description);
            }

        }
    }
}
