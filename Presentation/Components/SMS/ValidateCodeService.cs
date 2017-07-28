//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tunynet.Events;
using Tunynet;
using System.Text;
using System.Configuration;
using System.Web.Mvc;
using System.Web;
using Tunynet.Utilities;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Sms.Model.V20160927;
using Tunynet.Logging;
using Aliyun.Acs.Dm.Model.V20151123;
using Tunynet.Settings;
using Tunynet.Caching;
using Tunynet.Email;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Tunynet.Common
{
    /// <summary>
    /// 手机验证码业务逻辑
    /// </summary>
    public class ValidateCodeService
    {
        private ISettingsManager<SiteSettings> siteSettings;
        private ICacheService cacheService;
        public ValidateCodeService(ISettingsManager<SiteSettings> siteSettings, ICacheService cacheService)
        {
            this.siteSettings = siteSettings;
            this.cacheService = cacheService;
        }


        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phoneNum">手机号</param>
        /// <param name="expiredMinutes">到期时间限制</param>
        /// <returns></returns>
        public bool Send(string phoneNum, string templateCode, int expiredMinutes = 30)
        {
            string time;
            //给发送短信加了个时间限制 1分钟之内多发 都会失败
            cacheService.TryGetValue(phoneNum + "_Time", out time);
            if (!string.IsNullOrEmpty(time))
            {
                return false;
            }
            int count;
            //给发送短信加了个每日时间限制 1日之内过量就失败
            cacheService.TryGetValue(phoneNum + "_Day", out count);
            if (count >= Convert.ToInt32(SMSConfig.ShortCreedNumber))
            {
                return false;
            }

            string validateCode = new Random().Next(999999).ToString().PadLeft(6, '0');
            var regionId = "cn-hangzhou";
            var accessKey = SMSConfig.AccessKey;
            var secretKey = SMSConfig.AccessSecret;

            var siteSetting = siteSettings.Get();

            string endPointName = "cn-hangzhou";
            string productName = "Sms";
            string domain = "sms.aliyuncs.com";
            /**
            *根据自己需要访问的区域选择Region，并设置对应的接入点
            */
            try
            {
                DefaultProfile.AddEndpoint(endPointName, regionId, productName, domain);
            }
            catch (ClientException e)
            {
                var ws = e.Message;
            }

            IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKey, secretKey);

            IAcsClient client = new DefaultAcsClient(profile);

            SingleSendSmsRequest request = new SingleSendSmsRequest();

            try
            {
                request.SignName = SMSConfig.SignName;//"管理控制台中配置的短信签名（状态必须是验证通过）"
                request.TemplateCode = templateCode;//管理控制台中配置的审核通过的短信模板的模板CODE（状态必须是验证通过）"
                request.RecNum = phoneNum;//"接收号码，多个号码可以逗号分隔"97077
                request.ParamString = "{\"code\":\"" + validateCode + "\",\"product\":\"" + siteSetting.SiteName + "\"}";//短信模板中的变量；数字需要转换为字符串；个人用户每个变量长度必须小于15个字符。"
                SingleSendSmsResponse httpResponse = client.GetAcsResponse(request, true, 10);
                if (httpResponse.HttpResponse.Status == 200)
                {
                    //验证码储存到缓存中
                    cacheService.Set(phoneNum + "_VerificationCode", EncryptionUtility.MD5(validateCode), new TimeSpan(0, expiredMinutes, 0));
                    //一分钟间隔 超过一分钟才能进行发送
                    cacheService.Set(phoneNum + "_Time", EncryptionUtility.MD5(validateCode), new TimeSpan(0, 1, 0));
                    count++;
                    //每日上限+1
                    cacheService.Set(phoneNum + "_Day", count, new TimeSpan(24, 0, 0));
                    return true;
                }
                return false;
               

                //HttpCookie cookies = new HttpCookie(phoneNum + "_VerificationCode");
                //cookies.Value = EncryptionUtility.MD5(validateCode);
                //cookies.Expires = DateTime.Now.AddMinutes(expiredMinutes);
                //httpContext.Response.Cookies.Add(cookies);
            }
            catch (ServerException e)
            {
                var ws = e.Message;
                return false;
            }
            catch (ClientException e)
            {
                var ws = e.Message;
                return false;
            }

        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="user">被发送用户</param>
        /// <param name="subject">标题</param>
        /// <param name="model">发送的内容</param>
        /// <param name="change">是否完善资料中的绑定邮箱</param>
        /// <param name="newEmailAddress">新邮箱地址</param>
        /// <returns></returns>
        public bool EmailSend(IUser user, string subject, MailMessage model, bool change = false, string newEmailAddress = null)
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", SMSConfig.AccessKey, SMSConfig.AccessSecret);
            IAcsClient client = new DefaultAcsClient(profile);
            SingleSendMailRequest request = new SingleSendMailRequest();
            try
            {
                request.AccountName = SMSConfig.AccountName;
                request.FromAlias = model.From.DisplayName.Length>13? model.From.DisplayName.Substring(0, 13): model.From.DisplayName;
                request.AddressType = 1;
                request.TagName = SMSConfig.TagName;
                request.ReplyToAddress = true;
                if (change)
                {
                    if (string.IsNullOrEmpty(newEmailAddress))
                    {
                        request.ToAddress = user.UserGuid;
                    }
                    else
                    {
                        request.ToAddress = newEmailAddress;
                    }
                }
                else
                    request.ToAddress = user.AccountEmail;
                int count;
                //给发送邮件加了个每日时间限制 1日之内过量就失败
                cacheService.TryGetValue(request.ToAddress + "_EmailDay", out count);
                if (count >= Convert.ToInt32(SMSConfig.MailArticleNumber))
                {
                    return false;
                }

                request.Subject = subject + "(" + model.From.DisplayName + ")";
                request.HtmlBody = model.Body;

                SingleSendMailResponse httpResponse = client.GetAcsResponse(request);
                if (httpResponse.HttpResponse.Status == 200)
                {
                    count++;
                    //每日上限+1
                    cacheService.Set(request.ToAddress + "_EmailDay", count, new TimeSpan(24, 0, 0));
                    return true;
                }
                return false;


            }

            catch (ServerException e)
            {
                LoggerFactory.GetLogger().Error(e.ErrorMessage);
            }

            return false;

        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public ValidateCodeStatus Check(string userName, string validateCode, bool? isMobile = true)
        {

            string cookieName = userName + "_VerificationCode";
            if (!isMobile.Value)
            {
                cookieName = userName + "_EmailVerificationCode";
            }
            string code = string.Empty;
            //从缓存里取验证码
            cacheService.TryGetValue(cookieName, out code);
            if (string.IsNullOrEmpty(code))
                return ValidateCodeStatus.Failure;

            if (EncryptionUtility.MD5(validateCode) != code)
                return ValidateCodeStatus.WrongInput;
            return ValidateCodeStatus.Passed;
        }

        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="validateCode"></param>
        public void Create(HttpContextBase httpContext, long phoneNum, string validateCode, int expiredMinutes = 2)
        {
            HttpCookie cookies = new HttpCookie("VerificationCode");
            cookies.Value = EncryptionUtility.MD5(validateCode);
            cookies.Expires = DateTime.Now.AddMinutes(expiredMinutes);
            httpContext.Response.Cookies.Add(cookies);

        }

        public string GetCodeError(ValidateCodeStatus status)
        {
            switch (status)
            {
                case ValidateCodeStatus.Empty:
                    return "未匹配到号码";
                case ValidateCodeStatus.Overtime:
                    return "验证超时，请重新获取";
                case ValidateCodeStatus.WrongInput:
                    return "验证码输入错误";
                case ValidateCodeStatus.Failure:
                    return "验证码已失效";
                default:
                    return "未知错误";
            }
        }

    }

}
