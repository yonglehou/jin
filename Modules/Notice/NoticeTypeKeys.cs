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

namespace Tunynet.Common
{
    /// <summary>
    /// 通知类型状态 
    /// </summary>
    public  class NoticeTypeKeys
    {
        #region Instance
        private static NoticeTypeKeys _instance = new NoticeTypeKeys();
        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static NoticeTypeKeys Instance()
        {
            return _instance;
        }

        private NoticeTypeKeys()
        { }

        #endregion
       

        /// <summary>
        /// 新评论
        /// </summary>
        /// <returns></returns>
        public string NewComment()
        {
            return "NewComment";
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
        /// 新回复
        /// </summary>
        /// <param name="tenantTypeId">租户Id</param>
        /// <returns></returns>
        public string NewReply(string tenantTypeId)
        {
            if (tenantTypeId == TenantTypeIds.Instance().Thread())
            {
                return "NewThreadReply";
            }
            else if (tenantTypeId==TenantTypeIds.Instance().CMS_Article())
            {
                return "NewArticleReply";
            }
            else if(tenantTypeId == TenantTypeIds.Instance().CMS_Image())
            {
                return "NewImageReply";
            }
            else if (tenantTypeId == TenantTypeIds.Instance().CMS_Video())
            {
                return "NewVideoReply";
            }
        
            else
            {
                return "NewCommentReply";
            }
        }

        /// <summary>
        /// 你的“XXX”通过了审核
        /// </summary>
        /// <returns></returns>
        public string ManagerApproved(string tenantTypeId)
        {
            if (tenantTypeId == TenantTypeIds.Instance().Thread())
                return "ThreadApproved";
            else if (tenantTypeId == TenantTypeIds.Instance().CMS_Article())
                return "CMSArticleApproved";
            return string.Empty;
        }

        /// <summary>
        /// 你的“XXX”未通过审核
        /// </summary>
        /// <returns></returns>
        public string ManagerDisapproved(string tenantTypeId)
        {
            if (tenantTypeId == TenantTypeIds.Instance().Thread())
                return "ThreadDisapproved";
            else if (tenantTypeId == TenantTypeIds.Instance().CMS_Article())
                return "CMSArticleDisapproved";
            return string.Empty;
        }


        /// <summary>
        /// 你的“XXX”已被管理员设为精华
        /// </summary>
        /// <returns></returns>
        public string ManagerSetEssential()
        {
            return "ManagerSetEssential";
        }

        /// <summary>
        /// 你的“XXX”已被管理员设为置顶
        /// </summary>
        /// <returns></returns>
        public string ManagerSetSticky()
        {
            return "ManagerSetSticky";
        }




    }
}
