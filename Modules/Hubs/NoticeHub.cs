//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRChat;
using Tunynet.Common;

namespace SignalRChat
{
    [HubName("NoticeHub")]
    public class NoticeHub : Hub
    {
        public override Task OnConnected()
        {

            //var cookie = Context.RequestCookies[UserAuthentication.currentUserCookie];
            //if (cookie != null)
            //{
            //    string cookieValue = Encoding.UTF8.GetString(Convert.FromBase64String(cookie.Value));
            //    cookieValue = AES.Decrypt(cookieValue);
            //    long userId = long.Parse(cookieValue);
            //    Groups.Add(Context.ConnectionId, "User#" + userId);
            //}

            return base.OnConnected();
        }
        public void Send(string name, string message)
        {
            message = string.Format(message + "   当前时间：{0}", DateTime.Now.ToString());
            // Call the addNewMessageToPage method to update clients.
            Clients.All.notice(message);
        }
    }
}

/// <summary>
/// 自定义用户Id提供器
/// </summary>
public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(IRequest request)
    {
        if (request.User != null && request.User.Identity != null)
            return request.User.Identity.Name;
        //return HttpContext.Current.User.Identity.Name;
        else
            return null;
    }
}

/// <summary>
///Hub的单例模式
/// </summary>
public class SignalrHub
{

    private static volatile IHubContext _instance = null;
    private static readonly object lockObject = new object();
    /// <summary>
    /// 创建主页实体
    /// </summary>
    /// <returns></returns>
    public static IHubContext Instance()
    {
        if (_instance == null)
        {
            lock (lockObject)
            {
                if (_instance == null)
                {
                    _instance = GlobalHost.ConnectionManager.GetHubContext<NoticeHub>();
                }
            }
        }
        return _instance;
    }

    private SignalrHub()
    { }
}