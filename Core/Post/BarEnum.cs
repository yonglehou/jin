//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Repositories;
using Tunynet;

namespace Tunynet.Post
{
    /// <summary>
    /// 贴吧排序依据
    /// </summary>
    public enum SortBy_BarSection
    {
        /// <summary>
        /// 创建时间倒序
        /// </summary>
        DateCreated_Desc,
        /// <summary>
        /// 主题贴数
        /// </summary>
        ThreadCount,

        /// <summary>
        /// 主题贴和回贴总数
        /// </summary>
        ThreadAndPostCount,

        /// <summary>
        /// 阶段主题贴和回贴总数
        /// </summary>
        StageThreadAndPostCount,
        /// <summary>
        /// 被关注数
        /// </summary>
        FollowedCount
    }

    /// <summary>
    /// 贴子排序依据
    /// </summary>
    public enum SortBy_BarThread
    {
        /// <summary>
        /// 发布时间倒序
        /// </summary>
        DateCreated_Desc=0,
        /// <summary>
        /// 更新时间倒序
        /// </summary>
        LastModified_Desc=1,
        /// <summary>
        /// 浏览数
        /// </summary>
        HitTimes,
        /// <summary>
        /// 阶段浏览数
        /// </summary>
        StageHitTimes,
        /// <summary>
        /// 回贴数
        /// </summary>
        PostCount
    }
    /// <summary>
    /// 贴子时间排序依据
    /// </summary>
    public enum SortBy_BarDateThread
    {
        /// <summary>
        /// 全部时间
        /// </summary>
        All=0,
        /// <summary>
        /// 近三天
        /// </summary>
        ThreeDay=1,
        /// <summary>
        /// 近一个周
        /// </summary>
        SevenDay=2,
        /// <summary>
        /// 近一个月
        /// </summary>
        AMonth=3

    }
    /// <summary>
    /// 回贴排序依据
    /// </summary>
    public enum SortBy_BarPost
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateCreated,
        /// <summary>
        /// 创建时间倒序
        /// </summary>
        DateCreated_Desc
    }

    /// <summary>
    /// 主题分类状态
    /// </summary>
    public enum ThreadCategoryStatus
    {
        //0=禁用；1=启用（不强制）；2=启用（强制）
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// 启用（不强制）
        /// </summary>
        NotForceEnabled = 1,
        /// <summary>
        /// 启用（强制）
        /// </summary>
        ForceEnabled = 2

    }
}