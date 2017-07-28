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

namespace Tunynet.Common
{
    /// <summary>
    /// 广告实体
    /// </summary>
    [TableName("tn_Advertisings")]
    [PrimaryKey("AdvertisingId", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class Advertising : SerializablePropertiesBase, IEntity
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static Advertising New()
        {
            Advertising advertising = new Advertising()
            {
                Name = string.Empty,
                ImageAttachmentId = 0,
                LinkUrl = "http://",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(6),
                DateCreated = DateTime.Now,
                AdvertisingType = AdvertisingType.Image,
                IsEnable = true,
            };
            return advertising;
        }

        #region 序列化属性

        /// <summary>
        /// 问题
        /// </summary>
        [Ignore]
        public int Width
        {
            get { return GetExtendedProperty<int>("Width"); }
            set { SetExtendedProperty("Width", value); }
        }

        /// <summary>
        /// 答案
        /// </summary>
        [Ignore]
        public int Height
        {
            get { return GetExtendedProperty<int>("Height"); }
            set { SetExtendedProperty("Height", value); }
        }

        #endregion

        #region 需持久化属性

        /// <summary>
        ///广告Id
        /// </summary>
        public long AdvertisingId { get; protected set; }

        /// <summary>
        ///广告名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///呈现方式
        /// </summary>
        public AdvertisingType AdvertisingType { get; set; }

        /// <summary>
        ///广告内容
        /// </summary>
        public string Body { get; set; }


        /// <summary>
        ///图片附件Id
        /// </summary>
        public long ImageAttachmentId { get; set; }

        /// <summary>
        ///广告链接地址
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        //排序
        public long DisplayOrder { get; set; }
        /// <summary>
        ///是否新开窗口
        /// </summary>
        public bool TargetBlank { get; set; }

        /// <summary>
        ///开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }



        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.AdvertisingId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 该广告所在的广告位
        /// </summary>
        public IEnumerable<AdvertisingPosition> AdvertisingsPositions
        {
            get
            {
                return new AdvertisingRepository().GetPositionsByAdvertisingId(this.AdvertisingId);
            }
        }

        /// <summary>
        /// 广告是否超期、未开始
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            if (StartDate > DateTime.Now)
            {
                return true;
            }
            if (EndDate.AddDays(1) < DateTime.Now)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取广告图
        /// </summary>
        public string ImageUrl(string Key = "")
        {
            Attachment attachment = new AttachmentService(TenantTypeIds.Instance().Advertising()).GetByAssociateId(this.AdvertisingId);
            return attachment != null ? attachment.GetDirectlyUrl(Key) : string.Empty;
        }

        #endregion
    }
}
