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
    /// 扩展短信发送模板
    /// </summary>
    public static class SMSExtensions
    {
        /// <summary>
        /// 注册成功
        /// </summary>
        /// <param name="phoneNum">手机号码</param>
        /// <param name="expiredMinutes">过期时间/分钟(默认30分钟)</param>
        /// <returns></returns>
        public static bool RegisterSuccess(this ValidateCodeService validateCodeService, string phoneNum, int expiredMinutes = 30)
        {
            string templateName = SMSConfig.SMSRegisterTemplateCode;
            return validateCodeService.Send(phoneNum, templateName, expiredMinutes);
        }

        /// <summary>
        /// 绑定手机发送验证码
        /// </summary>
        /// <param name="phoneNum">手机号码</param>
        /// <param name="expiredMinutes">过期时间/分钟(默认30分钟)</param>
        /// <returns></returns>
        public static bool Binding(this ValidateCodeService validateCodeService, string phoneNum, int expiredMinutes = 30)
        {
            string templateName = SMSConfig.SMSBindingTemplateCode;
            return validateCodeService.Send(phoneNum, templateName, expiredMinutes);
        }
        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="phoneNum">手机号码</param>
        /// <param name="expiredMinutes">过期时间/分钟(默认30分钟)</param>
        /// <returns></returns>
        public static bool ResetPassWord(this ValidateCodeService validateCodeService, string phoneNum, int expiredMinutes = 30)
        {
            string templateName = SMSConfig.SMSResetPassWordTemplateCode;
            return validateCodeService.Send(phoneNum, templateName, expiredMinutes);
        }

    }

}