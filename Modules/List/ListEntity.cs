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
    /// 列表管理实体
    /// </summary>
    [TableName("tn_Lists")]
    [PrimaryKey("Code",autoIncrement =false)]
    [CacheSetting(true)]
    [Serializable]
    public class ListEntity : IEntity
    {

        #region 需持久化属性
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否多层级
        /// </summary>
        public int IsMultilevel { get; set; }
        
        /// <summary>
        /// 是否允许添加或删除
        /// </summary>
        public int AllowAddOrDelete { get; set; }
        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Code; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion IEntity 成员
    }
}
