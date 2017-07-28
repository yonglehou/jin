//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Common;
using Tunynet;
using Tunynet.Common.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 栏目管理员（用于贴吧、资讯栏目等） 
    /// </summary>
    [TableName("tn_CategoryManagers")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class CategoryManager : IEntity
    {

        #region 需持久化属性

        /// <summary>
        ///ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///所属栏目Id（或贴吧Id）
        /// </summary>
        public long CategoryId { get; set; }
        /// <summary>
        ///租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        ///从哪个栏目继承权限
        /// </summary>
        public long ReferenceCategoryId { get; set; }

        /// <summary>
        ///用户Id
        /// </summary>
        public long UserId { get; set; }
      

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

      
    }
}