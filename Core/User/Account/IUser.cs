﻿//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tunynet.Common
{

    /// <summary>
    /// 用户实体接口
    /// </summary>
    public interface IUser
    {

        /// <summary>
        /// 用户Id
        /// </summary>
        long UserId { get; }

        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 用户GUID/OpenId
        /// </summary>
        string UserGuid { get; }


        /// <summary>
        /// 用户类型
        /// </summary>
        int UserType { get; }

        /// <summary>
        /// 帐号邮箱
        /// </summary>
        string AccountEmail { get; }

        /// <summary>
        /// 帐号邮箱是否通过验证
        /// </summary>
        bool IsEmailVerified { get; }

        /// <summary>
        /// 帐号手机号码
        /// </summary>
        string AccountMobile { get; }

        /// <summary>
        /// 手机号码是否通过验证
        /// </summary>
        bool IsMobileVerified { get; }

        /// <summary>
        /// 真实姓名(或名称)
        /// </summary>
        string TrueName { get; }

        /// <summary>
        /// 是否强制用户登录
        /// </summary>
        bool ForceLogin { get; }

        /// <summary>
        /// 用户账号状态(-1=已删除,1=已激活,0=未激活)
        /// </summary>
        UserStatus Status { get; }

        /// <summary>
        /// 创建日期
        /// </summary>
        DateTime DateCreated { get; }

        /// <summary>
        /// 上传活动时间
        /// </summary>
        DateTime LastActivityTime { get; }

        /// <summary>
        /// 上次操作
        /// </summary>
        string LastAction { get; }

        /// <summary>
        /// 注册用户时IP地址
        /// </summary>
        string IpCreated { get; }

        /// <summary>
        /// 上次操作时IP地址 
        /// </summary>
        string IpLastActivity { get; }

        /// <summary>
        /// 是否被封禁
        /// </summary>
        bool IsBanned { get; }

        /// <summary>
        /// 是否被管制
        /// </summary>
        bool IsModerated { get; }

        /// <summary>
        /// 对外显示名称
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 头像 是否存在
        /// </summary>
        int HasAvatar { get; }

        /// <summary>
        /// 封面图 是否存在
        /// </summary>
        int HasCover { get; }

        ///// <summary>
        ///// 头像上传时间
        ///// </summary>
        //long DateAvatar { get; }

        /// <summary>
        /// 经验积分值
        /// </summary>
        int ExperiencePoints { get; }

        /// <summary>
        /// 威望积分值
        /// </summary>
        int ReputationPoints { get; }

        /// <summary>
        /// 交易积分值
        /// </summary>
        int TradePoints { get; }

        /// <summary>
        /// 交易积分值2
        /// </summary>
        int TradePoints2 { get; }

        /// <summary>
        /// 交易积分值3
        /// </summary>
        int TradePoints3 { get; }

        /// <summary>
        /// 交易积分值4
        /// </summary>
        int TradePoints4 { get; }

        /// <summary>
        /// 用户等级
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// 冻结的交易积分
        /// </summary>
        int FrozenTradePoints { get; }

    }
}
