//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using PetaPoco;
using Tunynet.Caching;

namespace Tunynet.Attitude
{
    /// <summary>
    /// 顶踩实体
    /// </summary>
    [TableName("tn_Attitudes")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class Attitude : IEntity
    {
        /// <summary>
        /// 实体初始化方法
        /// </summary>
        /// <returns></returns>
        public static Attitude New()
        {
            Attitude attitude = new Attitude()
            {
                ObjectId = 0,
                SupportCount = 0,
                TenantTypeId = string.Empty,
               
            };
            return attitude;
        }

        #region 需持久化属性

        /// <summary>
        ///Id
        /// </summary>
        public long Id { get; protected set; }

        /// <summary>
        ///操作对象Id
        /// </summary>
        public long ObjectId { get; set; }

        /// <summary>
        ///点赞数
        /// </summary>
        public int SupportCount { get; set; }

        /// <summary>
        ///租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }

    

        #endregion 需持久化属性

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion IEntity 成员
    }
}