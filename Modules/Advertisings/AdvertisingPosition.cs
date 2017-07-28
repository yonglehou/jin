//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tunynet.Imaging;
using Tunynet.Caching;
using PetaPoco;
using Tunynet.Common.Configuration;
using Tunynet.FileStore;

namespace Tunynet.Common
{
    /// <summary>
    /// 广告位实体
    /// </summary>
    [TableName("tn_AdvertisingPositions")]
    [PrimaryKey("PositionId", autoIncrement = false)]
    [CacheSetting(true, ExpirationPolicy = EntityCacheExpirationPolicies.Stable)]
    [Serializable]
    public class AdvertisingPosition : IEntity
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static AdvertisingPosition New()
        {
            AdvertisingPosition advertisingPosition = new AdvertisingPosition()
            {
                Description = string.Empty,
                ImageAttachmentId = 0,
                IsEnable = true,
                PositionId =0
            };
            return advertisingPosition;
        }

        #region 需持久化属性

        /// <summary>
        ///广告位Id
        /// </summary>
        public long PositionId { get; set; }

      

        /// <summary>
        ///描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///示意图
        /// </summary>
        public long ImageAttachmentId { get; set; }

        /// <summary>
        ///宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLocked { get; set; }


        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.PositionId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

        #region 拓展方法
        /// <summary>
        /// 获取示例图url
        /// </summary>
        /// <returns></returns>
        public string GetImageUrl()
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().AdvertisingPosition());
            Attachment attachment = attachmentService.Get(ImageAttachmentId);
            string url = string.Empty;
            if (attachment != null)
            {
                url = attachment.GetDirectlyUrl();
            }
            return url;
        }
        #endregion
    }
}
