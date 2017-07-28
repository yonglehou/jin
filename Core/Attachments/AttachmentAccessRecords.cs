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
using Tunynet.Utilities;
using System.Web;

namespace Tunynet.Common
{
    /// <summary>
    /// 附件下载记录
    /// </summary>
    [TableName("tn_AttachmentAccessRecords")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class AttachmentAccessRecords : IEntity
    {
        /// <summary>
        /// 实例化下载记录对象
        /// </summary>
        /// <param name="attachment">附件实体（用来为下载记录提供一些信息）</param>
        /// <returns></returns>
        public static AttachmentAccessRecords New(Attachment attachment)
        {
            return new AttachmentAccessRecords()
            {
                UserId = attachment.UserId,
                UserDisplayName = attachment.UserDisplayName,
                AttachmentId = attachment.AttachmentId,
                IP = WebUtility.GetIP(),
                Price = attachment.Price,
                DownloadDate = DateTime.Now,
                LastDownloadDate = DateTime.Now
            };
        }


        #region 需持久化属性

        /// <summary>
        ///Id
        /// </summary>
        public long Id { get; protected set; }

        /// <summary>
        ///附件Id
        /// </summary>
        public long AttachmentId { get; set; }

        /// <summary>
        ///1=下载；2=浏览
        /// </summary>
        public int AccessType { get; set; }


        /// <summary>
        ///UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///下载人DisplayName
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        ///消费的积分
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        ///最近下载日期
        /// </summary>
        public DateTime LastDownloadDate { get; set; }

        /// <summary>
        ///下载日期
        /// </summary>
        public DateTime DownloadDate { get; set; }


        /// <summary>
        ///附件下载人IP
        /// </summary>
        public string IP { get; set; }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
