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
    /// 权限项目与角色关联
    /// </summary>
    [TableName("tn_Permissions")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true, PropertyNamesOfArea = "PermissionItemKey")]
    [Serializable]
    public class Permission : IEntity
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static Permission New()
        {
            Permission permissionItemsInUserRole = new Permission()
            {
                PermissionItemKey = string.Empty,
                OwnerId = 0
            };
            return permissionItemsInUserRole;
        }

        #region 需持久化属性

        /// <summary>
        ///Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///权限项目标识
        /// </summary>
        public string PermissionItemKey { get; set; }

        /// <summary>
        ///被授权对象Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        ///被授权对象类型（用户=1、角色=11）
        /// </summary>
        public OwnerType OwnerType { get; set; }

        /// <summary>
        ///是否锁定
        /// </summary>
        public bool IsLocked { get; set; }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
