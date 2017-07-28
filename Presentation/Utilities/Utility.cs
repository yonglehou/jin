//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Tunynet;
using Tunynet.Common;
using Tunynet.Common.Configuration;
using Tunynet.Utilities;
using Tunynet.Logging;
using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using Tunynet.Settings;
using System.Text;

namespace Tunynet.Common
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Utility
    {
        ///// <summary>
        ///// 获取隐私的特别类型的
        ///// </summary>
        ///// <param name="PrivacySpecifyUsers">隐私的用户的id字符串</param>
        ///// <param name="PrivacySpecifyUserGroup">隐私的分组的id字符串</param>
        ///// <param name="tenantTypeId">类型id</param>
        ///// <param name="contentId">对象id</param>
        ///// <returns>隐私特别对象集合</returns>
        //public static Dictionary<int, IEnumerable<ContentPrivacySpecifyObject>> GetContentPrivacySpecifyObjects(string PrivacySpecifyUsers, string PrivacySpecifyUserGroup, string tenantTypeId, long contentId)
        //{
        //    IUserService userService = DIContainer.Resolve<IUserService>();
        //    CategoryService categoryService = new CategoryService();
        //    Dictionary<int, IEnumerable<ContentPrivacySpecifyObject>> privacySpecifyObjects = new Dictionary<int, IEnumerable<ContentPrivacySpecifyObject>>();

        //    if (!string.IsNullOrEmpty(PrivacySpecifyUsers))
        //    {
        //        string[] userIds = PrivacySpecifyUsers.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        //        IEnumerable<User> users = userService.GetFullUsers(userIds.Select(n => long.Parse(n)));
        //        List<ContentPrivacySpecifyObject> contentPrivacySpecifyObjects = new List<ContentPrivacySpecifyObject>();
        //        foreach (User user in users)
        //        {
        //            ContentPrivacySpecifyObject contentPrivacySpecifyObject = ContentPrivacySpecifyObject.New();
        //            contentPrivacySpecifyObject.TenantTypeId = tenantTypeId;
        //            contentPrivacySpecifyObject.ContentId = contentId;
        //            contentPrivacySpecifyObject.SpecifyObjectId = user.UserId;
        //            contentPrivacySpecifyObject.SpecifyObjectName = user.DisplayName;
        //            contentPrivacySpecifyObject.SpecifyObjectTypeId = SpecifyObjectTypeIds.Instance().User();

        //            contentPrivacySpecifyObjects.Add(contentPrivacySpecifyObject);
        //        }

        //        privacySpecifyObjects[SpecifyObjectTypeIds.Instance().User()] = contentPrivacySpecifyObjects;
        //    }

        //    if (!string.IsNullOrEmpty(PrivacySpecifyUserGroup))
        //    {
        //        List<ContentPrivacySpecifyObject> contentPrivacySpecifyObjects = new List<ContentPrivacySpecifyObject>();
        //        if (PrivacySpecifyUserGroup.Contains("-1"))
        //        {
        //            ContentPrivacySpecifyObject contentPrivacySpecifyObject = ContentPrivacySpecifyObject.New();
        //            contentPrivacySpecifyObject.TenantTypeId = tenantTypeId;
        //            contentPrivacySpecifyObject.ContentId = contentId;
        //            contentPrivacySpecifyObject.SpecifyObjectId = -1;
        //            contentPrivacySpecifyObject.SpecifyObjectName = "我关注的所有人";
        //            contentPrivacySpecifyObject.SpecifyObjectTypeId = SpecifyObjectTypeIds.Instance().UserGroup();

        //            contentPrivacySpecifyObjects.Add(contentPrivacySpecifyObject);
        //        }
        //        else
        //        {
        //            string[] userGroupIds = PrivacySpecifyUserGroup.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        //            foreach (string categoryId in userGroupIds)
        //            {
        //                ContentPrivacySpecifyObject contentPrivacySpecifyObject = ContentPrivacySpecifyObject.New();
        //                contentPrivacySpecifyObject.TenantTypeId = tenantTypeId;
        //                contentPrivacySpecifyObject.ContentId = contentId;
        //                contentPrivacySpecifyObject.SpecifyObjectTypeId = SpecifyObjectTypeIds.Instance().UserGroup();

        //                if (categoryId == "-2")
        //                {
        //                    contentPrivacySpecifyObject.SpecifyObjectId = -2;
        //                    contentPrivacySpecifyObject.SpecifyObjectName = "相互关注";

        //                    contentPrivacySpecifyObjects.Add(contentPrivacySpecifyObject);
        //                }
        //                else
        //                {
        //                    Category category = categoryService.Get(long.Parse(categoryId));
        //                    contentPrivacySpecifyObject.SpecifyObjectId = category.CategoryId;
        //                    contentPrivacySpecifyObject.SpecifyObjectName = category.CategoryName;

        //                    contentPrivacySpecifyObjects.Add(contentPrivacySpecifyObject);
        //                }
        //            }
        //        }

        //        privacySpecifyObjects[SpecifyObjectTypeIds.Instance().UserGroup()] = contentPrivacySpecifyObjects;
        //    }

        //    return privacySpecifyObjects;
        //}

        #region 加密与解密

        /// <summary>
        /// 加密上传图片所属用户的Id
        /// </summary>
        /// <param name="timeliness">加密有效期</param>
        /// <param name="userId">要加密的用户Id</param>
        /// <returns>加密令牌</returns>
        public static string EncryptTokenForUploadfile(double timeliness, long userId)
        {
            
            return EncryptToken(timeliness, userId);
        }

        /// <summary>
        /// 解密上传附件的用户Id
        /// </summary>
        /// <param name="token">要解密的令牌</param>
        /// <param name="isTimeout">输出参数：令牌是否过期</param>
        /// <returns>解密后的用户Id</returns>
        public static long DecryptTokenForUploadfile(string token, out bool isTimeout)
        {
            return DecryptToken(token, out isTimeout);
        }


        /// <summary>
        /// 用于邀请好友的Token加密方法
        /// </summary>
        /// <param name="timeliness">实效（天）</param>
        /// <param name="userId">用户id</param>
        /// <returns>生成的token</returns>
        public static string EncryptTokenForInviteFriend(double timeliness, long userId)
        {
            return EncryptToken(timeliness, userId);
        }

        /// <summary>
        /// 用户邀请好友的Toke解析方法
        /// </summary>
        /// <param name="token">邀请链接</param>
        /// <param name="isTimeout">是否超时</param>
        /// <returns>用户id</returns>
        public static long DecryptTokenForInviteFriend(string token, out bool isTimeout)
        {
            return DecryptToken(token, out isTimeout);
        }

        /// <summary>
        /// 生成忘记密码Token
        /// </summary>
        /// <param name="timeliness">实效（天）</param>
        /// <param name="userId">用户id</param>
        /// <returns>生成的token</returns>
        public static string EncryptTokenFindPassword(double timeliness, long userId)
        {
            return EncryptToken(timeliness, userId);
        }

        /// <summary>
        /// 解析忘记密码Token
        /// </summary>
        /// <param name="token">链接</param>
        /// <param name="isTimeout">是否超时</param>
        /// <returns>用户id</returns>
        public static long DecryptTokenForFindPassword(string token, out bool isTimeout)
        {
            return DecryptToken(token, out isTimeout);
        }


        /// <summary>
        /// 获取身份认证Token(加密Token)
        /// </summary>
        /// <param name="timeliness">实效（天）</param>
        /// <param name="userId">用户id</param>
        /// <returns>生成的token</returns>
        public static string EncryptTokenForQuickLogin(double timeliness, long userId)
        {
            return EncryptToken(timeliness, userId);
        }
        /// <summary>
        /// 解密Token
        /// </summary>
        /// <param name="token">链接</param>
        /// <param name="isTimeout">是否超时</param>
        /// <returns>用户id</returns>
        public static long DecryptTokenForQuickLogin(string token, out bool isTimeout)
        {
            return DecryptToken(token, out isTimeout);
        }

        /// <summary>
        /// 生成验证邮箱的token
        /// </summary>
        /// <param name="timeliness">时间限制</param>
        /// <param name="userId">用户id</param>
        /// <returns>生成的token</returns>
        public static string EncryptTokenForValidateEmail(double timeliness, long userId)
        {
            return EncryptToken(timeliness, userId);
        }

        /// <summary>
        /// 解析验证邮箱的token
        /// </summary>
        /// <param name="token">被验证的token</param>
        /// <param name="isTimeout">标识是否过期</param>
        /// <returns>用户的id</returns>
        public static long DecryptTokenForValidateEmail(string token, out bool isTimeout)
        {
            return DecryptToken(token, out isTimeout);
        }

        /// <summary>
        /// 获取加密附件下载的token
        /// </summary>
        /// <param name="timeliness">加密有效期</param>
        /// <param name="ip">要加密的用户ip</param>
        /// <returns>加密令牌</returns>
        public static string EncryptTokenForAttachmentDownload(double timeliness, long attachmentId)
        {
            return EncryptToken(timeliness, attachmentId);
        }

        /// <summary>
        /// 附件下载的token解密
        /// </summary>
        /// <param name="token">加密串</param>
        /// <param name="isTimeout">是否失效</param>
        /// <returns></returns>
        public static long DecryptTokenForAttachmentDownload(string token, out bool isTimeout)
        {
            return DecryptToken(token, out isTimeout);
        }

        /// <summary>
        /// 加密的操作类
        /// </summary>
        /// <param name="timeliness">时限</param>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        public static string EncryptToken(double timeliness, long id)
        {
            string tonkenStr = id + "," + DateTime.Now.AddDays(timeliness).Ticks;
            MachineKeySection section = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
            string token = EncryptionUtility.AES_Encrypt(tonkenStr, section.DecryptionKey);

            //string token = EncryptionUtility.SymmetricEncrypt(SymmetricEncryptType.DES, tonkenStr, iv, key);
            return WebUtility.UrlEncode(token);
        }

        /// <summary>
        /// 解密操作类
        /// </summary>
        /// <param name="token">网络令牌</param>
        /// <param name="isTimeout">是否失效</param>
        /// <returns></returns>
        private static long DecryptToken(string token, out bool isTimeout)
        {
            long id = 0;
            isTimeout = true;
            try
            {
                token = token.Replace(" ", "+");
                MachineKeySection section = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
                string tokenStr = EncryptionUtility.AES_Decrypt(token, section.DecryptionKey);

                //string tokenStr = EncryptionUtility.SymmetricDncrypt(SymmetricEncryptType.DES, token, iv, key);

                string[] parts = tokenStr.Split(',');
                if (parts.Length > 1)
                {
                    long.TryParse(parts[0], out id);

                    long ticks;
                    long.TryParse(parts[1], out ticks);

                    if (ticks > DateTime.Now.Ticks)
                        isTimeout = false;
                }
            }
            catch (Exception ex)
            {
                throw new ExceptionFacade("解密操作的时候发生错误", ex);
            }
            return id;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source">需要加密的字符</param>
        /// <returns></returns>
        public static string MD5Encrypt(string source)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(source));

            // Create a new Stringbuilder to collect the bytes and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        #endregion

        #region 验证信息
        /// <summary>
        /// 验证用户名
        /// </summary>
        /// <param name="userName">待验证的用户名</param>
        /// <param name="errorMessage">输出出错信息</param>
        /// <returns>是否通过验证</returns>
        public static bool ValidateUserName(string userName, out string errorMessage, bool isWeb = true)
        {
            if (string.IsNullOrEmpty(userName))
            {
                if (isWeb)
                    errorMessage = ResourceAccessor.GetString("Validate_UserNameRequired");
                else
                    errorMessage = ResourceAccessor.GetString("Validate_UserNameRequiredForMobileClient");
                return false;
            }
            ISettingsManager<UserSettings> userSettingsManager = DIContainer.Resolve<ISettingsManager<UserSettings>>();
            UserSettings userSettings = userSettingsManager.Get();

            if (userName.Contains("*"))
            {
                if (isWeb)
                    errorMessage = string.Format(ResourceAccessor.GetString("Validate_UserNameHasSensitiveWord"));
                else
                    errorMessage = string.Format(ResourceAccessor.GetString("Validate_UserNameHasSensitiveWordForMobileClient"));
                return false;
            }

            if (userName.Length < userSettings.MinUserNameLength)
            {
                if (isWeb)
                    errorMessage = string.Format(ResourceAccessor.GetString("Validate_UserNameTooShort"), userSettings.MinUserNameLength);
                else
                    errorMessage = string.Format(ResourceAccessor.GetString("Validate_UserNameTooShortForMobileClient"), userSettings.MinUserNameLength);
                return false;
            }

            if (userName.Length > userSettings.MaxUserNameLength)
            {
                if (isWeb)
                    errorMessage = string.Format(ResourceAccessor.GetString("Validate_UserNameTooLong"), userSettings.MaxUserNameLength);
                else
                    errorMessage = string.Format(ResourceAccessor.GetString("Validate_UserNameTooLongForMobileClient"), userSettings.MaxUserNameLength);
                return false;
            }

            //Regex regex = new Regex(userSettings.UserNameRegex);
            //if (!regex.IsMatch(userName))
            //{
            //    errorMessage = ResourceAccessor.GetString("Validate_UserNameRegex");
            //    return false;
            //}

            AuthorizationService authorizationService = DIContainer.Resolve<AuthorizationService>();
            authorizationService.IsSuperAdministrator(UserContext.CurrentUser);
            //验证UserName是否被禁止使用
            if (!authorizationService.IsSuperAdministrator(UserContext.CurrentUser) &&
                userSettings.DisallowedUserNames.Split(',', '，').Any(n => n.Equals(userName, StringComparison.CurrentCultureIgnoreCase)))
            {

                if (isWeb)
                    errorMessage = ResourceAccessor.GetString("Validate_UserNameIsDisallowed");
                else
                    errorMessage = ResourceAccessor.GetString("Validate_UserNameIsDisallowedForMobileClient");
                return false;
            }


            //验证UserName是否已经存在
            IUserService userService = DIContainer.Resolve<IUserService>();
            IUser user = userService.GetUser(userName);
            if (user != null)
            {
                if (isWeb)
                    errorMessage = ResourceAccessor.GetString("Validate_UserNameIsExisting");
                else
                    errorMessage = ResourceAccessor.GetString("Validate_UserNameIsExistingForMobileClient");
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="userName">待验证的手机号</param>
        /// <param name="errorMessage">输出出错信息</param>
        /// <returns>是否通过验证</returns>
        public static bool ValidateUserNameForPhone(string userName, out string errorMessage, bool isWeb = true)
        {
            if (string.IsNullOrEmpty(userName))
            {
                if (isWeb)
                    errorMessage = ResourceAccessor.GetString("Validate_PhoneRequired");
                else
                    errorMessage = ResourceAccessor.GetString("Validate_PhoneRequiredForMobileClient");
                return false;
            }
            ISettingsManager<UserSettings> userSettingsManager = DIContainer.Resolve<ISettingsManager<UserSettings>>();
            UserSettings userSettings = userSettingsManager.Get();

            Regex regex = new Regex(userSettings.PhoneRegex);
            if (!regex.IsMatch(userName))
            {
                errorMessage = ResourceAccessor.GetString("Validate_PhoneRegex");
                return false;
            }

            //AuthorizationService authorizationService = new AuthorizationService();
            //authorizationService.IsSuperAdministrator(UserContext.CurrentUser);
            ////验证UserName是否被禁止使用
            //if (!authorizationService.IsSuperAdministrator(UserContext.CurrentUser) &&
            //    userSettings.DisallowedUserNames.Split(',', '，').Any(n => n.Equals(userName, StringComparison.CurrentCultureIgnoreCase)))
            //{

            //    if (isWeb)
            //        errorMessage = ResourceAccessor.GetString("Validate_UserNameIsDisallowed");
            //    else
            //        errorMessage = ResourceAccessor.GetString("Validate_UserNameIsDisallowedForMobileClient");
            //    return false;
            //}

            //验证UserName是否已经存在
            IUserService userService = DIContainer.Resolve<IUserService>();
            IUser user = userService.GetUser(userName);
            if (user != null)
            {
                if (isWeb)
                    errorMessage = ResourceAccessor.GetString("Validate_PhoneIsExisting");
                else
                    errorMessage = ResourceAccessor.GetString("Validate_PhoneIsExistingForMobileClient");
                return false; 
            }
            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="email">待验证的邮箱</param>
        /// <param name="errorMessage">输出出错信息</param>
        /// <returns>是否通过验证</returns>
        public static bool ValidateEmail(string email, out string errorMessage, Boolean isWeb = true)
        {
            ISettingsManager<UserSettings> userSettingsManager = DIContainer.Resolve<ISettingsManager<UserSettings>>();
            UserSettings userSettings = userSettingsManager.Get();

            if (string.IsNullOrEmpty(email))
            {
                errorMessage = ResourceAccessor.GetString("Validate_EmailRequired");
                return false;
            }

            Regex regex = new Regex(userSettings.EmailRegex, RegexOptions.ECMAScript);
            if (!regex.IsMatch(email))
            {
                errorMessage = ResourceAccessor.GetString("Validate_EmailStyle");
                return false;
            }

            IUserService userService = DIContainer.Resolve<IUserService>();
            IUser user = userService.GetUserByEmail(email);

            //验证email是否已经存在
            if (user != null)
            {
                if (isWeb)
                    errorMessage = string.Format(ResourceAccessor.GetString("Message_Pattern_RegisterFailedForDuplicateEmailAddress"), SiteUrls.Instance().FindPassword(email));
                else
                    errorMessage = string.Format(ResourceAccessor.GetString("Validate_UserEmailIsExisting"));
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 检测用户密码是否适合站点设置
        /// </summary>
        /// <param name="newPassword">Password to be verified.</param>
        /// <param name="errorMessage">Error message to return.</param>
        /// <returns>True if compliant, otherwise False.</returns>
        public static bool ValidatePassword(string newPassword, out string errorMessage)
        {

            ISettingsManager<UserSettings> userSettingsManager = DIContainer.Resolve<ISettingsManager<UserSettings>>();
            UserSettings userSettings = userSettingsManager.Get();
            int minRequiredPasswordLength = userSettings.MinPasswordLength;
            int minRequiredNonAlphanumericCharacters = userSettings.MinRequiredNonAlphanumericCharacters;

            errorMessage = "";

            if (string.IsNullOrEmpty(newPassword))
            {
                errorMessage = ResourceAccessor.GetString("Validate_RequiredPassword");
                return false;
            }

            if (newPassword.Length < minRequiredPasswordLength)
            {
                errorMessage = string.Format(ResourceAccessor.GetString("Validate_Pattern_MinRequiredPasswordLength"), minRequiredPasswordLength);
                return false;
            }

            int nonAlphaNumChars = 0;
            for (int i = 0; i < newPassword.Length; i++)
            {
                if (!char.IsLetterOrDigit(newPassword, i))
                    nonAlphaNumChars++;
            }
            if (nonAlphaNumChars < minRequiredNonAlphanumericCharacters)
            {
                errorMessage = string.Format(ResourceAccessor.GetString("Validate_Pattern_MinRequiredNonAlphanumericCharacters"), minRequiredNonAlphanumericCharacters);
                return false;
            }

            return true;
        }

       

        #endregion

        #region 生成随机码
        /// <summary>
        /// 随机种子
        /// </summary>
        private static Random RndSeed = new Random();
        /// <summary>
        /// 生成一个随机码
        /// </summary>
        /// <returns></returns>
        public static string GenerateRndNonce()
        {
            return string.Concat(
            Utility.RndSeed.Next(1, 99999999).ToString("00000000"),
            Utility.RndSeed.Next(1, 99999999).ToString("00000000"),
            Utility.RndSeed.Next(1, 99999999).ToString("00000000"),
            Utility.RndSeed.Next(1, 99999999).ToString("00000000"));
        }
        #endregion

        /// <summary>
        /// 是不是合法的请求
        /// </summary>
        /// <remarks>
        /// 用于防盗链的检测、防洪攻击
        /// </remarks>
        /// <returns></returns>
        public static bool IsAllowableReferrer(HttpRequestBase httpRequest)
        {
            if (httpRequest == null || httpRequest.UrlReferrer == null)
                return false;
            string[] domainRules = { };

            string urlReferrerDomain = WebUtility.GetServerDomain(httpRequest.UrlReferrer, domainRules);
            string urlDomain = WebUtility.GetServerDomain(httpRequest.Url, domainRules);

            return urlReferrerDomain.Equals(urlDomain, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 加密的操作类
        /// </summary>
        /// <param name="encryptString">encryptString</param>
        /// <returns></returns>
        public static string EncryptTokenForCookie(string encryptString)
        {
            string tonkenStr = encryptString;
            MachineKeySection section = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
            return EncryptionUtility.AES_Encrypt(tonkenStr, section.DecryptionKey);

            //return EncryptionUtility.SymmetricEncrypt(SymmetricEncryptType.DES, tonkenStr, iv, key);
        }

        /// <summary>
        /// 解密操作类
        /// </summary>
        /// <param name="token">网络令牌</param>
        /// <returns></returns>
        public static string DecryptTokenForCookie(string token)
        {
            string encryptString = string.Empty;
            try
            {
                token = token.Replace(" ", "+");
                MachineKeySection section = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
                encryptString = EncryptionUtility.AES_Decrypt(token, section.DecryptionKey);
            }
            catch (Exception ex)
            {
                throw new ExceptionFacade("解密操作的时候发生错误", ex);
            }
            return encryptString;
        }

        /// <summary>
        /// 检查是否分布式运行环境
        /// </summary>
        /// <returns></returns>
        public static bool IsDistributedDeploy()
        {
            bool distributedDeploy = false;
            if (ConfigurationManager.AppSettings["DistributedDeploy"] != null)
            {
                if (!bool.TryParse(ConfigurationManager.AppSettings["DistributedDeploy"], out distributedDeploy))
                {
                    distributedDeploy = false;
                }
            }

            return distributedDeploy;
        }

        /// <summary>
        /// 检查文件存储是否为分布式运行环境
        /// </summary>
        /// <returns></returns>
        public static bool IsFileDistributedDeploy()
        {
            bool fileDistributedDeploy = false;
            if (ConfigurationManager.AppSettings["FileDistributedDeploy"] != null)
            {
                if (!bool.TryParse(ConfigurationManager.AppSettings["FileDistributedDeploy"], out fileDistributedDeploy))
                {
                    fileDistributedDeploy = false;
                }
            }

            return fileDistributedDeploy;
        }


        /// <summary>
        /// 是否启用MiniProfiler
        /// </summary>
        /// <returns></returns>
        public static bool IsMiniProfilerEnabled()
        {
            bool isMiniProfilerEnabled = false;
            if (ConfigurationManager.AppSettings["MiniProfiler:Enabled"] != null)
            {
                if (!bool.TryParse(ConfigurationManager.AppSettings["MiniProfiler:Enabled"], out isMiniProfilerEnabled))
                {
                    isMiniProfilerEnabled = false;
                }
            }

            return isMiniProfilerEnabled;
        }

        /// <summary>
        /// 检查启用的应用并呈现
        /// </summary>
        /// <returns></returns>
        public static bool CheckApplication(string key)
        {
            bool result = false;
            string keys = ConfigurationManager.AppSettings["ApplicationsEnabled"];
            if (keys!=null&&keys.ToLower().IndexOf(key.ToLower()) > -1)
                result = true;
            return result;
           
        }
        /// <summary>
        /// 检查启用第三方登录
        /// </summary>
        /// <returns></returns>
        public static bool CheckThirdPartyLogin( )
        {
            bool result = false;
            string khirdPartyLogin = ConfigurationManager.AppSettings["ThirdPartyLogin"];
            if (khirdPartyLogin != null&&bool.TryParse(khirdPartyLogin,  out result)&& result)
            return result;
            return result;
        }

    }
}
