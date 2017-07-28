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
    /// 收藏事件
    /// </summary>
    public class FavoriteEventModule : IEventMoudle
    {
        private IKvStore kvStore;
        public FavoriteEventModule (IKvStore kvStore)
        {
            this.kvStore = kvStore;

        }


        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<long, FavoriteEventArgs>.Instance().After += new CommonEventHandler<long, FavoriteEventArgs>(FavoriteEventModule_After);

        }

        /// <summary>
        /// 收藏计数事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param> 
        private void FavoriteEventModule_After(long userId, FavoriteEventArgs eventArgs)
        {
            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                kvStore.Increase(KvKeys.Instance().UserFavoriteCount(userId));

            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                kvStore.Increase(KvKeys.Instance().UserFavoriteCount(userId),-1);
            }

        }

       



    }
}