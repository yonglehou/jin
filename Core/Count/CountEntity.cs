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
using Tunynet;

namespace Tunynet.Common
{
    /// <summary>
    /// 计数实体
    /// </summary>
    [TableName("tn_Counts")]
    [PrimaryKey("CountId", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class CountEntity : IEntity
    {

        #region 需持久化属性

        /// <summary>
        ///id
        /// </summary>
        public long CountId { get; protected set; }

        /// <summary>
        ///拥有者id
        /// </summary>
        public long OwnerId { get; set; }
        /// <summary>
        ///租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }
        /// <summary>
        ///计数对象id
        /// </summary>
        public long ObjectId { get; set; }

        /// <summary>
        ///计数类型
        /// </summary>
        public string CountType { get; set; }

        /// <summary>
        ///计数
        /// </summary>
        public int StatisticsCount { get; set; }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.CountId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
