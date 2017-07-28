//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Tunynet.Caching;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户角色
    /// </summary>
    [TableName("tn_Roles")]
    [PrimaryKey("RoleId", autoIncrement = false)]
    [CacheSetting(true, ExpirationPolicy = EntityCacheExpirationPolicies.Stable)]
    [Serializable]
    public class Role : IEntity
    {
        #region 需持久化属性

        private long roleId = 0;
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId
        {
            get { return roleId; }
            set { roleId = value; }
        }

        private string roleName = string.Empty;
        /// <summary>
        /// 角色友好名称（用于对外显示）
        /// </summary>
        public string RoleName
        {
            get { return roleName; }
            set { roleName = value; }
        }

        private bool isBuiltIn;
        /// <summary>
        /// 是否是系统内置的
        /// </summary>
        public bool IsBuiltIn
        {
            get { return isBuiltIn; }
            set { isBuiltIn = value; }
        }

        private bool connectToUser;
        /// <summary>
        /// 是否直接关联到用户（例如：版主、注册用户 无需直接赋给用户）
        /// </summary>
        public bool ConnectToUser
        {
            get { return connectToUser; }
            set { connectToUser = value; }
        }

      

        private bool isPublic;
        /// <summary>
        /// 是否对外显示
        /// </summary>
        public bool IsPublic
        {
            get { return isPublic; }
            set { isPublic = value; }
        }

        private string description = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private long roleImageAttachmentId = 0;
        /// <summary>
        /// 角色标识图片Id
        /// </summary>
        public long RoleImageAttachmentId
        {
            get { return roleImageAttachmentId; }
            set { roleImageAttachmentId = value; }
        }

      

        #endregion
        
        #region IEntity 成员

        object IEntity.EntityId { get { return this.RoleId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

        #region 拓展方法
        /// <summary>
        /// 获取角色标志图
        /// </summary>
        /// <returns></returns>
        public string RoleImageUrl()
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Role());
            Attachment attachment= attachmentService.Get(RoleImageAttachmentId);
            string url=string.Empty;
            if (attachment!=null)
            {
                url= attachment.GetDirectlyUrl("Small");
            }
            return url;
        }
        #endregion
    }
}
