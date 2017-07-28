
//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Common;

namespace Tunynet
{
    /// <summary>
    /// KvKey管理方法
    /// </summary>
    public static class KvKeysExtensions
    {
        /// <summary>
        /// 获取用户的Cms投稿计数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string UserContentItemCount(this KvKeys kvKeys, long userId, string contentModelKey, AuditStatus? auditStatus = null)
        {
            var key = "CMS-ContentModelKey-" + contentModelKey + "-ContentItemCount-UserId-" + userId;
            if (auditStatus.HasValue)
                key = key + "-AuditStatus" + auditStatus.Value;
            return key;
        }



        /// <summary>
        /// 获取用户的点赞计数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string UserAttitudeCount(this KvKeys kvKeys, long userId)
        {
            return "Attitude-AttitudeCount-UserId-" + userId;
        }

        /// <summary>
        /// 获取用户的问题被采纳计数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string UserAnswerAcceptCount(this KvKeys kvKeys, long userId)
        {
            return "Ask-AnswerAcceptCount-UserId-" + userId;
        }
        /// <summary>
        /// 获取用户的Thread发布计数
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <returns></returns>
        public static string UserThreadCount(this KvKeys kvKeys, long userId, string tenantTypeId, AuditStatus? auditStatus = null)
        {
            var key = "Post-TenantTypeId-" + tenantTypeId + "-ThreadCount-UserId-" + userId;
            if (auditStatus.HasValue)
                key = key + "-AuditStatus" + auditStatus.Value;
            return key;
        }
        /// <summary>
        /// 获取用户的Comment计数
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <returns></returns>
        public static string UserCommentCount(this KvKeys kvKeys, long userId, string tenantTypeId, AuditStatus? auditStatus = null)
        {
            var key = "Comment-TenantTypeId-" + tenantTypeId + "-CommentCount-UserId-" + userId;
            if (auditStatus.HasValue)
                key = key + "-AuditStatus" + auditStatus.Value;
            return key;
        }

        /// <summary>
        /// 获取用户收藏计数
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string UserFavoriteCount(this KvKeys kvKeys, long userId)
        {
            return "Favorite-FavoriteCount-UserId-" + userId;
        }

        /// <summary>
        /// 交易积分
        /// </summary>
        /// <returns></returns>
        public static string TradePoints(this KvKeys kvKeys)
        {
            return "TradePoints";
        }

        /// <summary>
        /// 资讯全文索引Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string CmsSearch(this KvKeys kvKeys)
        {
            return "CmsSearch";
        }

        /// <summary>
        /// 资讯全文索引需要删除索引Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string CmsDeleteSearch(this KvKeys kvKeys)
        {
            return "CmsDeleteSearch";
        }

        /// <summary>
        /// 资讯全文索引更新Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string CmsUpdateSearch(this KvKeys kvKeys)
        {
            return "CmsUpdateSearch";
        }


       

        /// <summary>
        /// 贴子全文索引Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string ThreadSearch(this KvKeys kvKeys)
        {
            return "ThreadSearch";
        }

        /// <summary>
        /// 贴子全文索引需要删除索引Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string ThreadDeleteSearch(this KvKeys kvKeys)
        {
            return "ThreadDeleteSearch";
        }

        /// <summary>
        /// 贴子全文索引更新Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string ThreadUpdateSearch(this KvKeys kvKeys)
        {
            return "ThreadUpdateSearch";
        }

        /// <summary>
        /// 评论全文索引Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string CommentSearch(this KvKeys kvKeys)
        {
            return "CommentSearch";
        }

        /// <summary>
        /// 评论全文索引需要删除索引Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string CommentDeleteSerach(this KvKeys kvKeys)
        {
            return "CommentDeleteSerach";
        }

        /// <summary>
        /// 评论全文索引更新Key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string CommentUpdateSearch(this KvKeys kvKeys)
        {
            return "CommentUpdateSearch";
        }

        /// <summary>
        /// 个推通知key
        /// </summary>
        /// <param name="kvKeys"></param>
        /// <returns></returns>
        public static string GetuiNotice(this KvKeys kvKeys)
        {
            return "GetuiNotice";
        }

    }
}
