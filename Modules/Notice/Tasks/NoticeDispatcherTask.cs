//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Microsoft.AspNet.SignalR;
using SignalRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Common;
using Tunynet.Repositories;
using Tunynet.Settings;
using Tunynet.Tasks;

namespace Tunynet.Common
{
   public class NoticeDispatcherTask : ITask
    {
       private IEnumerable<INoticeSender> noticeSenders = DIContainer.Resolve<IEnumerable<INoticeSender>>();
       

       private NoticeService noticeService = DIContainer.Resolve<NoticeService>();

        /// <summary>
        /// 通知发送
        /// </summary>
        /// <param name="notices"></param>
        /// <returns></returns>
        public void Execute(TaskDetail taskDetail = null)
        {
            IEnumerable<NoticeTypeSettings> Settings = null;
            var noticeTypeSettings = noticeService.GetAllTypeSetting();
            //遍历注入发送器
            foreach (var noticeSender in noticeSenders)
            {
                Settings = noticeTypeSettings.Where(n => n.SendMode == noticeSender.SendMode());
                foreach (var Setting in Settings)
                {
                    var notices = noticeService.GetNotices(Setting.NoticeTypeKey, Setting.Interval, 0);
                    noticeSender.Send(notices);
                }
            }


        }
    }
}
