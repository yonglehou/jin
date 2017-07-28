//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Common
{
    /// <summary>
    /// 短信&&邮件配置文件属性
    /// </summary>
    public static class SMSConfig 
    {
        #region 短信
        /// <summary>
        /// 访问的Key
        /// </summary>
        public static string AccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AccessKey"];
            }
        }
        /// <summary>
        /// 帐号令牌
        /// </summary>
        public static string AccessSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["AccessSecret"];
            }
        }
        /// <summary>
        /// 签名
        /// </summary>
        public static string SignName
        {
            get
            {
                return ConfigurationManager.AppSettings["SignName"];
            }
        }
        /// <summary>
        /// 短信每日发送限制数量
        /// </summary>
        public static string ShortCreedNumber
        {
            get
            {
                return ConfigurationManager.AppSettings["ShortCreedNumber"];
            }
        }

        /// <summary>
        /// 短信注册模板编码
        /// </summary>
        public static string SMSRegisterTemplateCode
        {
            get
            {
                return ConfigurationManager.AppSettings["SMSRegisterTemplateCode"];
            }
        }
        /// <summary>
        /// 短信绑定手机模板
        /// </summary>
        public static string SMSBindingTemplateCode
        {
            get
            {
                return ConfigurationManager.AppSettings["SMSBindingTemplateCode"];
            }
        }
        /// <summary>
        /// 短信找回密码模板
        /// </summary>
        public static string SMSResetPassWordTemplateCode
        {
            get
            {
                return ConfigurationManager.AppSettings["SMSResetPassWordTemplateCode"];
            }
        }


      
        #endregion 短信


        #region 邮件 


        /// <summary>
        /// 控制台创建的发信地址
        /// </summary>
        
        public static string AccountName
        {
            get
            {
                return ConfigurationManager.AppSettings["AccountName"];
            }
        }
        /// <summary>
        /// 控制台创建的标签
        /// </summary>
       
        public static string TagName
        {
            get
            {
                return ConfigurationManager.AppSettings["TagName"];
            }
        }

        /// <summary>
        /// 邮件每日发送限制数量
        /// </summary>
        public static string MailArticleNumber
        {
            get
            {
                return ConfigurationManager.AppSettings["MailArticleNumber"];
            }
        }

        #endregion 邮件



    }
}
