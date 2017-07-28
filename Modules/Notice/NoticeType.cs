//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Xml.Linq;
using PetaPoco;

namespace Tunynet.Common
{
    /// <summary>
    /// 通知类型实体类
    /// </summary>
    [TableName("tn_NoticeTypes")]
    [PrimaryKey("NoticeTypeKey", autoIncrement = false)]
    [Serializable]
    public class NoticeType:IEntity
    {

        #region 需持久化属性
              

        /// <summary>
        /// 通知 类型Key
        /// </summary>
        public string  NoticeTypeKey { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型描述
        /// </summary>
        public string Description { get; set; }
        #endregion


        #region IEntity 成员
        object IEntity.EntityId { get { return this.NoticeTypeKey; } }
        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
