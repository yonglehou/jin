//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Tunynet.Caching;

namespace Tunynet.Common.Configuration
{
    /// <summary>
    /// 用户相关设置
    /// </summary>
    [Serializable]
    [CacheSetting(true)]
    public class UserSettings : IEntity
    {        
        private RegisterType registerType = RegisterType.EmailOrMobile;
        /// <summary>
        /// 注册类型分为 邮箱注册和手机注册(默认 优先手机)
        /// </summary>
        public RegisterType RegisterType
        {
            get { return registerType; }
            set { registerType = value; }
        }
       
        private int minUserNameLength = 2;

        /// <summary>
        /// 用户名最短长度
        /// </summary>
        public int MinUserNameLength
        {
            get { return minUserNameLength; }
            set { minUserNameLength = value > 64 ? 64 : value; }
        }

        private int maxUserNameLength = 64;

        /// <summary>
        /// 用户名的最大长度
        /// </summary>
        public int MaxUserNameLength
        {
            get { return maxUserNameLength; }
            set { maxUserNameLength = value > 64 ? 64 : value; }
        }


        string userNameRegex = @"^[\w|\u4e00-\u9fa5]{1,64}$";
        /// <summary>
        /// 用户名验证正则表达式
        /// </summary>
        public string UserNameRegex
        {
            get { return userNameRegex; }
            set { userNameRegex = value; }
        }

        string phoneRegex = @"^(13|14|15|17|18)[0-9]{9}$";
        /// <summary>
        /// 手机号验证正则表达式
        /// </summary>
        public string PhoneRegex
        {
            get { return phoneRegex; }
            set { phoneRegex = value; }
        }

        string nickNameRegex = @"^[\w|\u4e00-\u9fa5]{1,64}$";

        /// <summary>
        /// 昵称的正则
        /// </summary>
        public string NickNameRegex
        {
            get { return nickNameRegex; }
            set { nickNameRegex = value; }
        }

        int minPasswordLength = 4;
        /// <summary>
        /// 密码最小长度
        /// </summary>
        public int MinPasswordLength
        {
            get { return minPasswordLength; }
            set { minPasswordLength = value > 4 ? value : 4; }
        }

        private int minRequiredNonAlphanumericCharacters = 0;
        /// <summary>
        /// 密码中包含的最少特殊字符数
        /// </summary>
        public int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
            set
            {
                if (value < 0)
                    minRequiredNonAlphanumericCharacters = 0;
                else
                    minRequiredNonAlphanumericCharacters = value;
            }
        }

        string emailRegex = @"^([a-zA-Z0-9_.-]+)@([0-9A-Za-z.-]+).([a-zA-Z.]{2,6})$";
        /// <summary>
        /// Email验证正则表达式
        /// </summary>
        public string EmailRegex
        {
            get { return emailRegex; }
            set { emailRegex = value; }
        }

        private bool enableTrackAnonymous = true;
        /// <summary>
        /// 是否启用匿名用户跟踪
        /// </summary>
        public bool EnableTrackAnonymous
        {
            get { return enableTrackAnonymous; }
            set { enableTrackAnonymous = value; }
        }

        private int userOnlineTimeWindow = 20;
        /// <summary>
        /// 指定用户在最近一次活动时间之后多长时间视为在线的分钟数
        /// </summary>
        public int UserOnlineTimeWindow
        {
            get { return userOnlineTimeWindow; }
            set { userOnlineTimeWindow = value; }
        }

        private bool enableNotActivatedUsersToLogin = false;
        /// <summary>
        /// 允许未激活的用户登录
        /// </summary>
        public bool EnableNotActivatedUsersToLogin
        {
            get { return enableNotActivatedUsersToLogin; }
            set { enableNotActivatedUsersToLogin = value; }
        }

        private bool requiresUniqueMobile = false;
        /// <summary>
        /// 用户注册时是否允许手机号重复
        /// </summary>
        public bool RequiresUniqueMobile
        {
            get { return requiresUniqueMobile; }
            set { requiresUniqueMobile = value; }
        }

        private UserPasswordFormat userPasswordFormat = UserPasswordFormat.MD5;
        /// <summary>
        /// 用户密码加密方式
        /// </summary>
        public UserPasswordFormat UserPasswordFormat
        {
            get { return userPasswordFormat; }
            set { userPasswordFormat = value; }
        }

        private bool enableNickname = true;
        /// <summary>
        /// 是否启用昵称
        /// </summary>
        public bool EnableNickname
        {
            get { return enableNickname; }
            set { enableNickname = value; }
        }

        private bool enablePhone = true;
        /// <summary>
        /// 是否启用电话
        /// </summary>
        public bool EnablePhone
        {
            get { return enablePhone; }
            set { enablePhone = value; }
        }

        private DisplayNameType displayNameType = DisplayNameType.UserNameFirst;
        /// <summary>
        /// 用户对外显示哪个名称（如果未启用昵称，则该选项无需设置）
        /// </summary>
        public DisplayNameType DisplayNameType
        {
            get { return displayNameType; }
            set { displayNameType = value; }
        }


        private bool automaticModerated = false;
        /// <summary>
        /// 新注册用户是否自动处于管制状态
        /// </summary>
        public bool AutomaticModerated
        {
            get { return automaticModerated; }
            set { automaticModerated = value; }
        }

        private int noModeratedUserPoint = 100;
        ///	<summary>
        ///	用户自动接触管制状态所需的积分（用户综合积分）
        ///	</summary>
        public int NoModeratedUserPoint
        {
            get { return noModeratedUserPoint; }
            set { noModeratedUserPoint = value; }
        }
        
        private string disallowedUserNames = "admin，administrator，super";
        /// <summary>
        /// 不允许使用的用户名
        /// </summary>
        /// <remarks>
        /// 多个用户名之间用逗号分割
        /// </remarks>
        public string DisallowedUserNames
        {
            get { return disallowedUserNames; }
            set { disallowedUserNames = value; }
        }
                
        private long superAdministratorRoleId = 101;
        /// <summary>
        /// 超级管理员角色Id
        /// </summary>
        public long SuperAdministratorRoleId
        {
            get { return superAdministratorRoleId; }
            set { superAdministratorRoleId = value; }
        }

        private long anonymousRoleId = 122;
        /// <summary>
        /// 匿名用户角色Id
        /// </summary>
        public long AnonymousRoleId
        {
            get { return anonymousRoleId; }
            set { anonymousRoleId = value; }
        }

        private bool enableAudit = true;
        /// <summary>
        /// 是否启用人工审核
        /// </summary>
        public bool EnableAudit
        {
            get { return enableAudit; }
            set { enableAudit = value; }
        }

        private List<long> noAuditedRoleNames = new List<long> { RoleIds.Instance().SuperAdministrator(),RoleIds.Instance().TrustedUser() };
        /// <summary>
        /// 不需要审核的角色集合
        /// </summary>
        public List<long> NoAuditedRoleNames
        {
            get { return noAuditedRoleNames; }
            set { noAuditedRoleNames = value; }
        }

        private List<long> noCreatedRoleIds = new List<long> { RoleIds.Instance().RegisteredUsers(), RoleIds.Instance().ModeratedUser(), RoleIds.Instance().Anonymous() };
        /// <summary>
        /// 不可以创建的角色ID集合
        /// </summary>
        public List<long> NoCreatedRoleIds
        {
            get { return noCreatedRoleIds; }
            set { noCreatedRoleIds = value; }
        }

        private int minNoAuditedUserRank = 18;
        /// <summary>
        /// 最小不需要审核的用户等级
        /// </summary>
        public int MinNoAuditedUserRank
        {
            get { return minNoAuditedUserRank; }
            set { minNoAuditedUserRank = value; }
        }

        #region IEntity 成员

        object IEntity.EntityId
        {
            get { return typeof(UserSettings).FullName; }
        }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }

    /// <summary>
    ///用户注册类型(昵称也可以登录)
    /// </summary>
    public enum RegisterType
    {
        /// <summary>
        /// 手机注册
        /// </summary>
        [Display(Name = "手机注册")]
        Mobile = 10,
        /// <summary>
        /// 邮箱注册
        /// </summary>
        [Display(Name = "邮箱注册")]
        Email = 20,
        /// <summary>
        /// 手机或者邮箱同时可以注册手机优先
        /// </summary>
        [Display(Name = "手机或者邮箱同时可以注册手机优先")]
        MobileOrEmail = 30,
        /// <summary>
        /// 手机或者邮箱同时可以注册邮箱优先
        /// </summary>
        [Display(Name = "手机或者邮箱同时可以注册邮箱优先")]
        EmailOrMobile = 40
    }
}
