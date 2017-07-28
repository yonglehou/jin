//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using PetaPoco;
using Tunynet.Caching;
using System.Collections.Generic;

namespace Tunynet.Common
{
    /// <summary>
    /// 友情链接实体
    /// </summary>
    [TableName("tn_Links")]
    [PrimaryKey("LinkId", autoIncrement = true)]
    [CacheSetting(true, PropertyNamesOfArea = "OwnerId", ExpirationPolicy = EntityCacheExpirationPolicies.Stable)]
    [Serializable]
    public class LinkEntity : SerializablePropertiesBase, IEntity
    {
        ///// <summary>
        ///// 新建实体时使用
        ///// </summary>
        //public static LinkEntity New()
        //{
        //    LinkEntity link = new LinkEntity()
        //    {
               
        //        Description = string.Empty,
        //        DateCreated = DateTime.Now
              
        //    };
        //    return link;
        //}

        #region 需持久化属性
        /// <summary>
        ///友情链接ID
        /// </summary>
        public long LinkId { get; protected set; }

        /// <summary>
        ///链接名称
        /// </summary>
        public string LinkName { get; set; }

      
        /// <summary>
        ///链接地址
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        ///图片附件 Id
        /// </summary>
        public long ImageAttachmentId { get; set; }

        /// <summary>
        ///链接说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///排序，默认与主键相同
        /// </summary>
        public long DisplayOrder { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get;set; }

     

        /// <summary>
        ///PropertyNames
        /// </summary>
        public string PropertyNames { get; set; }

        /// <summary>
        ///PropertyValues
        /// </summary>
        public string PropertyValues { get; set; }

        #endregion

        #region 扩展属性及方法


        [Ignore]
        public IEnumerable<Category> Categories
        {
            get
            {
                return DIContainer.Resolve<CategoryService>().GetCategoriesOfItem(this.LinkId, 0, TenantTypeIds.Instance().Link());
            }
        }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.LinkId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion


        /// <summary>
        /// 获取标题图
        /// </summary>
        /// <returns></returns>
        public string GetImageUrl(string key)
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Link());
            var imageurl = string.Empty;
            var attachment = attachmentService.Get(this.ImageAttachmentId);
            if (attachment != null)
            {
                imageurl = attachment.GetDirectlyUrl(key);

            }
            return string.IsNullOrEmpty(imageurl) ? "../img/4x3.jpg" : imageurl;

        }
    }
}
