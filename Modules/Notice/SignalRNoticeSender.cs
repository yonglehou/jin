//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Microsoft.AspNet.SignalR;
using SignalRChat;
using System;
using System.Collections.Generic;

namespace Tunynet.Common
{
    public class SignalRNoticeSender : INoticeSender
    {
        private IUserService userService = DIContainer.Resolve<IUserService>();
        private NoticeService noticeService = DIContainer.Resolve<NoticeService>();
        private IKvStore kvStore = DIContainer.Resolve<IKvStore>();

        /// <summary>
        /// 发送方式(0是站内SingalR)
        /// </summary>
        /// <returns></returns>
        public int SendMode()
        {
            return 0;
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="notice">通知实体</param>
        /// <returns></returns>
        public bool Send(Notice notice)
        {
            var userAvatarUrl = userService.GetAvatarDirectlyUrl(userService.GetFullUser(notice.LeadingActorUserId), AvatarSizeType.Big);
            string message = NoticeBuilder.Instance().Resolve(notice);
            //SignalrHub.Instance().Clients.Group(notice.ReceiverId.ToString()).noticeUser(Utilities.HtmlUtility.StripForPreview(message).Trim(), userAvatarUrl);
            SignalrHub.Instance().Clients.User(notice.ReceiverId.ToString()).noticeUser(notice.Id, Utilities.HtmlUtility.StripForPreview(message).Trim(), userAvatarUrl, notice.RelativeObjectUrl);

            //加入队列,个推依次发送
            kvStore.Append(KvKeys.Instance().GetuiNotice(), notice);

            return true;
        }

        /// <summary>
        /// 通知发送
        /// </summary>
        /// <param name="notices"></param>
        /// <returns></returns>
        public bool Send(IEnumerable<Notice> notices)
        {
            //判断用户是否接受 或者 开启桌面 。。。等等
            var context = GlobalHost.ConnectionManager.GetHubContext<NoticeHub>();
            foreach (var notice in notices)
            {
                //更新上次发送时间
                notice.LastSendDate = DateTime.Now;
                notice.Times = notice.Times++;
                noticeService.Update(notice);

                var users = userService.GetUser(notice.ReceiverId);
                //if (!users.HasAvatar)
                //{
                //    string usersID = string.Format("{0}#{1}", notice.NoticeTypeKey, notice.ReceiverId);
                //    context.Clients.All.newNotice(notice.Id.ToString(), notice.Body, users.Avatar, notice.MobileResolvedBody, notice.RelativeObjectUrl);
                //}

            }

            //过滤出开启桌面通知的用户
            //List<long> receivers = new List<long>();
            //foreach (var user in users)
            //{
            //    if ((user.UserProfile.DisabledNoticeWay & (int)NoticeWay.Web) != (int)NoticeWay.Web)
            //    {
            //        receivers.Add(user.Id);
            //    }
            //}



            //推送通知

            return true;
        }
    }
}
