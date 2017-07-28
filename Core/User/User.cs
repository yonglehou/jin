//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Linq;
using PetaPoco;
using Tunynet;
using Tunynet.Caching;
using Tunynet.Common;
using Tunynet.Common.Configuration;
using System.Collections.Generic;
using Tunynet.Settings;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户帐号
    /// </summary>
    [TableName("tn_Users")]
    [PrimaryKey("UserId", autoIncrement = false)]
    [CacheSetting(true)]
    [Serializable]
    public class User : IUser, IEntity
    {
        #region 构造器

        /// <summary>
        /// 构造器
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="userGuid">用户Guid</param>
        /// <param name="userName">用户名称</param>
        public static User New(string userGuid, string userName)
        {
            User user = New();
            //user.UserId = userId;
            user.UserName = userName;
            return user;
        }

        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static User New()
        {
            User user = new User();
            user.UserName = string.Empty;
            user.UserGuid = string.Empty;
            user.Password = string.Empty;
            user.AccountEmail = string.Empty;
            user.AccountMobile = string.Empty;
            user.TrueName = string.Empty;
            user.DateCreated = DateTime.Now;
            user.IpCreated = string.Empty;
            user.LastActivityTime = DateTime.Now;
            user.LastAction = string.Empty;
            user.IpLastActivity = string.Empty;
            user.BanReason = string.Empty;
            user.BanDeadline = DateTime.Now;
            user.FollowedCount = 0;
            user.FollowerCount = 0;
            user.HasAvatar = 0;
            user.HasCover = 0;
            user.Rank = 1;
            //用户创建后 根据配置设置是否管制
            if (DIContainer.Resolve<ISettingsManager<UserSettings>>().Get().AutomaticModerated)
                user.IsModerated = true;

            return user;
        }

        #endregion 构造器

        #region 需持久化属性

        /// <summary>
        ///UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户GUID/OpenId
        /// </summary>
        public string UserGuid { get; set; }
        /// <summary>
        ///用户名（昵称）
        /// </summary>
        public string UserName { get; set; }

        
        /// <summary>
        ///密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///0=Clear（明文）1=标准MD5
        /// </summary>
        public int PasswordFormat { get; set; }
      
        /// <summary>
        ///帐号邮箱
        /// </summary>
        public string AccountEmail { get; set; }

        /// <summary>
        ///帐号邮箱是否通过验证
        /// </summary>
        public bool IsEmailVerified { get; set; }

        /// <summary>
        ///手机号码
        /// </summary>
        public string AccountMobile { get; set; }

        /// <summary>
        ///帐号手机是否通过验证
        /// </summary>
        public bool IsMobileVerified { get; set; }

        /// <summary>
        ///个人姓名 或 企业名称
        /// </summary>
        public string TrueName { get; set; }

      
        /// <summary>
        ///是否强制用户登录
        /// </summary>
        public bool ForceLogin { get; set; }

        /// <summary>
        /// 用户账号状态(-1=已删除,1=已激活,0=未激活)
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///创建用户时的ip
        /// </summary>
        public string IpCreated { get; set; }

        /// <summary>
        ///用户类别
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        ///上次活动时间
        /// </summary>
        public DateTime LastActivityTime { get; set; }

        /// <summary>
        ///上次操作
        /// </summary>
        public string LastAction { get; set; }

        /// <summary>
        ///上次活动时的ip
        /// </summary>
        public string IpLastActivity { get; set; }

        /// <summary>
        ///是否封禁
        /// </summary>
        public bool IsBanned { get; set; }

        /// <summary>
        ///封禁原因
        /// </summary>
        public string BanReason { get; set; }

        /// <summary>
        ///封禁截止日期
        /// </summary>
        public DateTime BanDeadline { get; set; }

        /// <summary>
        ///用户是否被监管
        /// </summary>
        public bool IsModerated { get; set; }

        /// <summary>
        ///强制用户管制（不会自动解除）
        /// </summary>
        public bool IsForceModerated { get; set; }

        /// <summary>
        /// 头像 是否存在
        /// </summary>
        public int HasAvatar { get; set; }

        /// <summary>
        /// 封面图 是否存在
        /// </summary>
        public int HasCover { get; set; }

        /// <summary>
        ///磁盘配额
        /// </summary>
        public int DatabaseQuota { get; set; }

        /// <summary>
        ///已用磁盘空间
        /// </summary>
        public int DatabaseQuotaUsed { get; set; }

       
        /// <summary>
        /// 关注用户数
        /// </summary>
        public int FollowedCount { get; set; }

        /// <summary>
        /// 粉丝数
        /// </summary>
        public int FollowerCount { get; set; }

        /// <summary>
        /// 经验积分值
        /// </summary>
        public int ExperiencePoints { get; set; }

        /// <summary>
        /// 威望积分值
        /// </summary>
        public int ReputationPoints { get; set; }

        /// <summary>
        /// 交易积分值
        /// </summary>
        public int TradePoints { get; set; }

        /// <summary>
        /// 交易积分值2
        /// </summary>
        public int TradePoints2 { get; set; }

        /// <summary>
        /// 交易积分值3
        /// </summary>
        public int TradePoints3 { get; set; }

        /// <summary>
        /// 交易积分值4
        /// </summary>
        public int TradePoints4 { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 冻结的交易积分
        /// </summary>
        public int FrozenTradePoints { get; set; }


        #endregion


        #region 显示实现接口

        long IUser.UserId
        {
            get { return UserId; }
        }

        string IUser.UserName
        {
            get { return UserName; }
        }

        int IUser.UserType
        {
            get { return UserType; }
        }

        string IUser.AccountEmail
        {
            get { return AccountEmail; }
        }

        bool IUser.IsEmailVerified
        {
            get { return IsEmailVerified; }
        }

        string IUser.AccountMobile
        {
            get { return AccountMobile; }
        }

        bool IUser.IsMobileVerified
        {
            get { return IsMobileVerified; }
        }

        string IUser.TrueName
        {
            get { return TrueName; }
        }

        string IUser.DisplayName
        {
            get { return DisplayName; }
        }

        bool IUser.ForceLogin
        {
            get { return ForceLogin; }
        }

        UserStatus IUser.Status
        {
            get { return Status; }
        }

        DateTime IUser.DateCreated
        {
            get { return DateCreated; }
        }

        DateTime IUser.LastActivityTime
        {
            get { return LastActivityTime; }
        }

        string IUser.LastAction
        {
            get { return LastAction; }
        }

        string IUser.IpCreated
        {
            get { return IpCreated; }
        }

        string IUser.IpLastActivity
        {
            get { return IpLastActivity; }
        }

        bool IUser.IsBanned
        {
            get { return IsBanned; }
        }

        bool IUser.IsModerated
        {
            get { return IsModerated; }
        }

        int IUser.HasAvatar
        {
            get { return HasAvatar; }
        }
        

        /// <summary>
        /// 经验积分值
        /// </summary>
        int IUser.ExperiencePoints
        {
            get { return this.ExperiencePoints; }
        }

        /// <summary>
        /// 威望积分值
        /// </summary>
        int IUser.ReputationPoints
        {
            get { return this.ReputationPoints; }
        }

        /// <summary>
        /// 交易积分值
        /// </summary>
        int IUser.TradePoints
        {
            get { return this.TradePoints; }
        }

        /// <summary>
        /// 交易积分值2
        /// </summary>
        int IUser.TradePoints2
        {
            get { return this.TradePoints2; }
        }

        /// <summary>
        /// 交易积分值3
        /// </summary>
        int IUser.TradePoints3
        {
            get { return this.TradePoints3; }
        }

        /// <summary>
        /// 交易积分值4
        /// </summary>
        int IUser.TradePoints4
        {
            get { return this.TradePoints4; }
        }

        /// <summary>
        /// 用户等级
        /// </summary>
        int IUser.Rank
        {
            get { return this.Rank; }
        }

        /// <summary>
        /// 冻结的交易积分
        /// </summary>
        int IUser.FrozenTradePoints
        {
            get { return this.FrozenTradePoints; }
        }

        #endregion 显示实现接口

        #region 扩展属性
        
        /// <summary>
        /// 对外显示名称
        /// </summary>
        [Ignore]
        public string DisplayName
        {
            get
            {
                ISettingsManager<UserSettings> userSettingsManager = DIContainer.Resolve<ISettingsManager<UserSettings>>();
                DisplayNameType displayNameType = userSettingsManager.Get().DisplayNameType;
                if (displayNameType == DisplayNameType.UserNameFirst)
                {
                    return this.UserName;
                }
                else if (displayNameType == DisplayNameType.TrueNameFirst)
                {
                    if (!string.IsNullOrEmpty(this.TrueName))
                        return this.TrueName;
                }

                return this.UserName;
            }
        }


        #endregion

        /// <summary>
        /// 用户资料
        /// </summary>
        [Ignore]
        public UserProfile Profile
        {
            get { return DIContainer.Resolve<UserProfileService>().Get(this.UserId); }
        }
        
        /// <summary>
        /// 总浏览数
        /// </summary>
        [Ignore]
        public int HitTimes
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().User());
                return countService.Get(CountTypes.Instance().HitTimes(), this.UserId);
            }
        }

        /// <summary>
        /// 内容数
        ///// </summary>
        //[Ignore]
        public long ContentCount
        {
            get
            {
               
                //IKvStore IKvStore = DIContainer.Resolve<IKvStore>();
                //int Value;
                //if (IKvStore.TryGet<int>(KvKeys.Instance().UserContributeCount(this.UserId), out Value))
                //{
                //    return Value;
                //}

                return 0;
            }
        }

        /// <summary>
        /// 最近七天的威望数
        /// </summary>
        [Ignore]
        public int PreWeekReputationPointsCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().User());
                int count = countService.GetStageCount(CountTypes.Instance().ReputationPointsCounts(), 7, this.UserId);
                if (count < 0)
                    return 0;
                return count;
            }
        }

        /// <summary>
        /// 最近七天浏览数
        /// </summary>
        [Ignore]
        public int PreWeekHitTimes
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().User());
                int count = countService.GetStageCount(CountTypes.Instance().HitTimes(), 7, this.UserId);
                if (count < 0)
                    return 0;
                return count;
            }
        }
        
        #region IEntity 成员

        object IEntity.EntityId { get { return this.UserId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
        #region 拓展方法
        /// <summary>
        /// 获取用户角色信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Role> GetUserRoles()
        {
            RoleService roleService = DIContainer.Resolve<RoleService>();
            var userRoles = roleService.GetRoleIdsOfUser(UserId);
            return roleService.GetRoles(userRoles);
        }
        #endregion

    }

}