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
using Tunynet.Events;

namespace Tunynet.Common
{
    /// <summary>
    /// 顶踩事件处理
    /// </summary>
    public class SupportOpposeEventModules : IEventMoudle
    {
        private PointService pointService;
        private IKvStore kvStore;

        public SupportOpposeEventModules(PointService pointService, IKvStore kvStore) {
            this.pointService = pointService;
            this.kvStore = kvStore;
        }

        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<long,SupportOpposeEventArgs>.Instance().After += new CommonEventHandler<long, SupportOpposeEventArgs>(AttitudePointModule_After);
        }


        /// <summary>
        /// 处理加精、置顶等操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void AttitudePointModule_After(long sender, SupportOpposeEventArgs eventArgs)
        {
            string pointItemKey = string.Empty;
            //点赞
            if (eventArgs.EventOperationType == EventOperationType.Instance().Support())
            {
                pointItemKey = PointItemKeys.Instance().CreateEvaluation();
                string description = string.Format("点赞");
                pointService.GenerateByRole(eventArgs.UserId, eventArgs.UserId, pointItemKey, description);

                //点赞计数
                kvStore.Increase(KvKeys.Instance().UserAttitudeCount(eventArgs.UserId));
            }
            //取消点赞
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                pointItemKey = PointItemKeys.Instance().CancelEvaluation();
                string description = string.Format("取消点赞");
                pointService.GenerateByRole(eventArgs.UserId, eventArgs.UserId, pointItemKey, description);

                //点赞计数
                kvStore.Increase(KvKeys.Instance().UserAttitudeCount(eventArgs.UserId),-1);
            }

        }
    }
}
