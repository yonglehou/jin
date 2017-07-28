//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Mail;
using Tunynet;
using Tunynet.Caching;
using Tunynet.Common;
using Tunynet.Email;
using Tunynet.Settings;
using Tunynet.Utilities;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 扩展邮件模板构建器
    /// </summary>
    public static class EmailBuilderExtensions
    {
        /// <summary>
        /// 注册成功
        /// </summary>
        /// <param name="emailBuilder">Email模板</param>
        /// <param name="password">密码</param>
        /// <param name="user">用户</param>
        /// <returns></returns>
        public static dynamic RegisterSuccess(this EmailBuilder emailBuilder, string password, IUser user)
        {
            string templateName = "RegisterSuccess";
            dynamic model = emailBuilder.GetUserEmailModel(user);
            model.Password = password;
            return emailBuilder.Resolve(templateName, model, new string[] { user.AccountEmail });
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="emailBuilder"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static MailMessage ResetPassword(this EmailBuilder emailBuilder, IUser user)
        {
            string templateName = "ResetPassword";
            dynamic model = emailBuilder.GetUserEmailModel(user);
            model.IP = WebUtility.GetIP();
            return emailBuilder.Resolve(templateName, model, new string[] { user.AccountEmail });
        }

        /// <summary>
        /// 修改密码页面
        /// </summary>
        /// <param name="emailBuilder"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static MailMessage ChangedPassword(this EmailBuilder emailBuilder, IUser user)
        {
            string templateName = "ChangedPassword";
            dynamic model = emailBuilder.GetUserEmailModel(user);
            return emailBuilder.Resolve(templateName, model, new string[] { user.AccountEmail });
        }

        ///// <summary>
        ///// 提醒
        ///// </summary>
        ///// <param name="emailBuilder"></param> 
        ///// <param name="userReminderInfos">用户提醒信息集合</param>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public static MailMessage Reminder(this EmailBuilder emailBuilder, IList<UserReminderInfo> userReminderInfos, IUser user)
        //{
        //    string templateName = "Reminder";
        //    dynamic model = emailBuilder.GetUserEmailModel(user);
        //    model.UserReminderInfos = userReminderInfos;
        //    return emailBuilder.Resolve(templateName, model, new string[] { user.AccountEmail });
        //}


        /// <summary>
        /// 邀请好友加入站点
        /// </summary>
        /// <param name="emailBuilder"></param>
        /// <param name="email"></param>
        /// <param name="invitationBody">邀请附注</param>
        /// <param name="inviteUrl">邀请链接</param>
        /// <returns></returns>
        public static MailMessage InviteFriend(this EmailBuilder emailBuilder, string email, string inviteUrl, string invitationBody)
        {
            string templateName = "InviteFriend";
            IAuthenticationService authenticationService = DIContainer.ResolvePerHttpRequest<IAuthenticationService>();
            IUser user = authenticationService.GetAuthenticatedUser();
            dynamic model = emailBuilder.GetUserEmailModel(user);
            model.InviteUrl = SiteUrls.FullUrl(inviteUrl);
            model.InvitationBody = invitationBody;
            return emailBuilder.Resolve(templateName, model, new string[] { email });
        }

        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="emailBuilder"></param>
        /// <param name="user"></param>
        /// <param name="change">是否完善资料中的绑定邮箱</param>
        /// <returns></returns>
        public static MailMessage RegisterValidateEmail(this EmailBuilder emailBuilder, IUser user, bool change = false)
        {
            string templateName = "RegisterValidateEmail";
            string token = Utility.EncryptTokenForValidateEmail(0.004, user.UserId);
            dynamic model = emailBuilder.GetUserEmailModel(user);
            model.EmailValidateUrl = SiteUrls.FullUrl(SiteUrls.Instance().ValideMailActive(token, change));
            return emailBuilder.Resolve(templateName, model, new string[] { user.AccountEmail });
        }


        /// <summary>
        /// 验证码验证邮箱
        /// </summary>
        /// <param name="emailBuilder"></param>
        /// <param name="user">被发送用户</param>
        /// <param name="operation">当前操作（例如：您正在进行{operation}）</param>
        /// <returns></returns>
        public static MailMessage EmailVerfyCode(this EmailBuilder emailBuilder, IUser user,string operation)
        {
            ICacheService cacheService = DIContainer.Resolve<ICacheService>();
            string validateCode = new Random().Next(999999).ToString().PadLeft(6, '0');
            string templateName = "EmailVerfyCode";
            dynamic model = emailBuilder.GetUserEmailModel(user);
            model.VerfyCode = validateCode;
            model.Operation = operation;
            cacheService.Set(user.AccountEmail + "_EmailVerificationCode", EncryptionUtility.MD5(validateCode), new TimeSpan(0, 30, 0));
            return emailBuilder.Resolve(templateName, model, new string[] { user.AccountEmail });
        }



        /// <summary>
        /// 获取邮件模板的全局数据字典
        /// </summary>
        /// <param name="emailBuilder"></param>
        /// <returns></returns>
        public static dynamic GetCommonEmailModel(this EmailBuilder emailBuilder)
        {
            ISettingsManager<SiteSettings> siteSettingsManager = DIContainer.Resolve<ISettingsManager<SiteSettings>>();
            SiteSettings siteSettings = siteSettingsManager.Get();
            dynamic model = new ExpandoObject();
            model.SiteName = siteSettings.SiteName;
            model.AdminAddress = "admin@tunynet.com";
            model.LoginUrl = SiteUrls.FullUrl(SiteUrls.Instance().Login());
            model.HomeUrl = SiteUrls.FullUrl(SiteUrls.Instance().Home());
            model.DateCreated = DateTime.Now.ToString();
            return model;
        }

        /// <summary>
        /// 获取用户邮件模板的数据字典
        /// </summary>
        /// <param name="emailBuilder"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static dynamic GetUserEmailModel(this EmailBuilder emailBuilder, IUser user)
        {
            //ISettingsManager<InviteFriendSettings> manager = DIContainer.Resolve<ISettingsManager<InviteFriendSettings>>();
            dynamic model = emailBuilder.GetCommonEmailModel();//合并全局数据字典
            model.UserName = user.UserName;
            model.UserDisplayName = user.DisplayName;
            model.PublicEmail = "PublicEmail";
            model.LastLogin = SiteUrls.FullUrl(SiteUrls.Instance().Login());
            model.ProfileUrl = SiteUrls.FullUrl(SiteUrls.Instance().SpaceHome(user.UserName));
            //model.ChangePasswordUrl = SiteUrls.FullUrl(SiteUrls.Instance().ChangePassword(user.UserName));
            //model.UserAvatarUrl = SiteUrls.FullUrl(SiteUrls.Instance().UserAvatarUrl(user, AvatarSizeType.Medium, false));
            //model.ResetPassword = SiteUrls.FullUrl(SiteUrls.Instance().ResetPassword(user.UserId));
            //model.ExpirationTime = DateTime.Now.AddDays(manager.Get().InvitationCodeTimeLiness);
            //model.InvitationCodeTimeLiness = manager.Get().InvitationCodeTimeLiness;

            //model.Highlinktimeliness =1;
            //model.HighlinktimelinessExpirationTime = DateTime.Now.AddDays(model.Highlinktimeliness);
            return model;
        }
    }

}