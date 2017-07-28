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
    /// 通知设置类
    /// </summary>
    [TableName("tn_NoticeTypeSettings")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class NoticeTypeSettings : IEntity
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public NoticeTypeSettings (string NoticeTypeKey)
        {
            this.NoticeTypeKey = NoticeTypeKey;
        }

        #region 需持久化属性

        /// <summary>
        ///Id
        /// </summary>
        public long Id { get; protected set; }

        /// <summary>
        ///通知类型 Key
        /// </summary>
        public string NoticeTypeKey { get; set; }

        /// <summary>
        ///第几次通知
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 距离上次通知的时间间隔(秒)
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// 发送方式（0=站内，1=Email，2=手机短信）
        /// </summary>
        public int SendMode { get; set; }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
