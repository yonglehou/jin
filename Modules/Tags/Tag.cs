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
    /// 标签实体类
    /// </summary>
    [TableName("tn_Tags")]
    [PrimaryKey("TagId", autoIncrement = true)]
    [CacheSetting(true, PropertyNamesOfArea = "TenantTypeId")]
    [Serializable]
    public class Tag : SerializablePropertiesBase, IEntity
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static Tag New()
        {
            Tag tag = new Tag()
            {
                TagName = string.Empty,
                Description = string.Empty,
                ImageAttachmentId = 0,
                TenantTypeId = string.Empty,
                ItemCount = 0,
                DateCreated = DateTime.Now
            };
            return tag;
        }

        #region 需持久化属性

        /// <summary>
        ///标签Id
        /// </summary>
        public long TagId { get; protected set; }

        /// <summary>
        ///租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        ///标签名称
        /// </summary>
        public string TagName { get; set; }

     

        /// <summary>
        ///描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///标签标题图Id
        /// </summary>
        public long ImageAttachmentId { get; set; }

        /// <summary>
        ///是否为特色标签
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// 内容项数目
        /// </summary>
        public int ItemCount { get; set; }

      

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get; protected set; }

        #endregion 需持久化属性

        /// <summary>
        /// 相关对象Id
        /// </summary>
        [Ignore]
        public string RelatedObjectIds
        {
            get { return GetExtendedProperty<string>("RelatedObjectIds"); }
            set { SetExtendedProperty("RelatedObjectIds", value); }
        }

        /// <summary>
        /// 拥有者Id
        /// </summary>
        [Ignore]
        public long OwnerId { get; set; }

        /// <summary>
        /// 24小时内的讨论次数
        /// </summary>
        [Ignore]
        public int PreDayItemCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Tag());
                return countService.GetStageCount(CountTypes.Instance().ItemCounts(), 1, this.TagId);
            }
        }

        /// <summary>
        /// 最近7天的讨论次数
        /// </summary>
        [Ignore]
        public int PreWeekItemCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Tag());
                return countService.GetStageCount(CountTypes.Instance().ItemCounts(), 7, this.TagId);
            }
        }
      
        #region IEntity 成员

        object IEntity.EntityId { get { return this.TagId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion IEntity 成员
    }
}