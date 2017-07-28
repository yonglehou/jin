//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;
using Tunynet.Caching;

namespace Tunynet.Common
{
    /// <summary>
    /// 推荐的内容
    /// </summary>
    [TableName("tn_SpecialContentItems")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true, PropertyNamesOfArea = "TypeId")]
    [Serializable]
    public class SpecialContentItem : SerializablePropertiesBase, IEntity
    {
        public static SpecialContentItem New(long itemId, string tenantTypeId, int typeId, long recommenderUserId, string itemName)
        {
            
            SpecialContentItem specialContentitem = new SpecialContentItem();
            specialContentitem.TypeId = typeId;
            specialContentitem.TenantTypeId = tenantTypeId;
            specialContentitem.RegionId = 0;
            specialContentitem.ItemId = itemId;
            specialContentitem.ItemName = itemName;
            specialContentitem.Recommender =new UserRepository().Get(recommenderUserId).DisplayName;
            specialContentitem.RecommenderUserId = recommenderUserId;
            specialContentitem.DateCreated = DateTime.Now;
            specialContentitem.DisplayOrder = 0;
            return specialContentitem;
        }
        /// <summary>
        /// 推荐id 自增长
        /// </summary>
        public long Id { get; protected set; }
        /// <summary>
        /// 推荐类别Id
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// 租户类型ID
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        /// 推荐内容所在区域Id（可能是版块、栏目也可能是自定义的数字）
        /// </summary>
        public long RegionId { get; set; }
        /// <summary>
        /// 内容实体ID
        /// </summary>
        public long ItemId { get; set; }
        /// <summary>
        /// 推荐标题（默认为内容名称或标题，允许推荐人修改）
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 标题图ID
        /// </summary>
        public long FeaturedImageAttachmentId { get; set; }

        /// <summary>
        /// 推荐人 DisplayName
        /// </summary>
        public string Recommender { get; set; }
        /// <summary>
        /// 推荐人用户 Id
        /// </summary>
        public long RecommenderUserId { get; set; }
        /// <summary>
        /// 推荐日期
        /// </summary>
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// 截止期限
        /// </summary>
        public DateTime ExpiredDate { get; set; }
        /// <summary>
        /// 排序顺序（默认和Id一致）
        /// </summary>
        public long DisplayOrder { get; set; }

        #region 拓展属性

        [Ignore]
        public string Link
        {
            get
            {
                return GetExtendedProperty<string>("Link");
            }
            set
            {
                SetExtendedProperty("Link", value);
            }
        }

        #endregion

        #region IEntity 成员
        object IEntity.EntityId { get { return this.Id; } }
        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

        #region 拓展方法
        /// <summary>
        /// 获取推荐图片
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public string FeaturedImageUrl(string key = null)
        {
            var attachmentService = new AttachmentService(TenantTypeIds.Instance().Recommend());
            var attachments = attachmentService.Get(FeaturedImageAttachmentId);
            if (attachments != null)

                return attachments.GetDirectlyUrl(key);
            return string.Empty;
        }

        /// <summary>
        /// 获取推荐内容的类型
        /// </summary>
        /// <returns></returns>
        public SpecialContentType GetSpecialContentType()
        {
            SpecialContentTypeService specialContentTypeService = DIContainer.Resolve<SpecialContentTypeService>();
            SpecialContentType specialContentType = specialContentTypeService.Get(this.TypeId);
            return specialContentType;
        }
        /// <summary>
        /// 获取推荐内容租户类型
        /// </summary>
        /// <returns></returns>
        public TenantType GetSpecialContentTenantType()
        {
            TenantTypeService tenantTypeService = DIContainer.Resolve<TenantTypeService>();
            TenantType tenantType = tenantTypeService.Get(this.TenantTypeId);
            return tenantType;
        }

        /// <summary>
        /// 获取被特殊推荐对象的Url
        /// </summary>
        /// <returns></returns>
        public string GetCommentedObjectUrl()
        {
            var urlGetter = SpecialContentItemUrlGetterFactory.Get(this.TenantTypeId);
            if (urlGetter != null)
                return urlGetter.GetSpecialContentItemDetailUrl(this.ItemId, this.TenantTypeId);
            else
                return string.Empty;
        }

        #endregion

    }
}
