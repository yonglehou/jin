//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using PetaPoco;
using Tunynet.Caching;

namespace Tunynet.Common
{
    /// <summary>
    /// 积分类型配置类（便于使用PointItemKey）
    /// </summary>
    public class PointItemKeys
    {
        #region Instance
        private static PointItemKeys _instance = new PointItemKeys();

        /// <summary>
        /// 获取该类的单例
        /// </summary>
        /// <returns></returns>
        public static PointItemKeys Instance()
        {
            return _instance;
        }

        private PointItemKeys()
        { }
        #endregion

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public string Register()
        {
            return "Register";
        }
        /// <summary>
        /// 发表评论
        /// </summary>
        /// <returns></returns>
        public string CreateComment()
        {
            return "CreateComment";
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <returns></returns>
        public string DeleteComment()
        {
            return "DeleteComment";
        }
        /// <summary>
        /// 首次上传头像
        /// </summary>
        /// <returns></returns>
        public string FirstUploadAvatar()
        {
            return "FirstUploadAvatar";
        }
        /// <summary>
        /// 关注用户
        /// </summary>
        /// <returns></returns>
        public string FollowUser()
        {
            return "FollowUser";
        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <returns></returns>
        public string CancelFollowUser()
        {
            return "CancelFollowUser";
        }
        /// <summary>
        /// 发表评价
        /// </summary>
        /// <returns></returns>
        public string CreateEvaluation()
        {
            return "CreateEvaluation";
        }
        /// <summary>
        /// 取消评价
        /// </summary>
        /// <returns></returns>
        public string CancelEvaluation()
        {
            return "CancelEvaluation";
        }
        /// <summary>
        ///发布贴子
        /// </summary>
        /// <returns></returns>
        public string CreateThread()
        {
            return "CreateThread";
        }
        /// <summary>
        ///删除贴子
        /// </summary>
        /// <returns></returns>
        public string DeleteThread()
        {
            return "DeleteThread";
        }
        /// <summary>
        ///邀请用户注册 
        /// </summary>
        /// <returns></returns>
        public string InviteUserRegister()
        {
            return "InviteUserRegister";
        }
        /// <summary>
        ///发布资讯
        /// </summary>
        /// <returns></returns>
        public string CreateContentItem()
        {
            return "CreateContentItem";
        }
        /// <summary>
        ///删除资讯
        /// </summary>
        /// <returns></returns>
        public string DeleteContentItem()
        {
            return "DeleteContentItem";
        }
    }


}
