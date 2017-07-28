//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;
using Tunynet.Common;
using Tunynet.Utilities;

namespace Tunynet.CMS
{
    /// <summary>
    /// 内容模型
    /// </summary>
    [TableName("tn_ContentItems")]
    [PrimaryKey("ContentItemId", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class ContentItem : SerializablePropertiesBase, IEntity, IAuditable
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static ContentItem New()
        {
            ContentItem contentItem = new ContentItem()
            {
                Subject = string.Empty,
                Author = string.Empty,
                Body = string.Empty,
                Summary = string.Empty,
                IP = WebUtility.GetIP(),
                IsSticky = false,
                DatePublished = DateTime.Now,
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now,
                IsVisible = true,
                ApprovalStatus = AuditStatus.Fail
            };
            return contentItem;
        }

        #region 需持久化属性

        /// <summary>
        /// 主键标识
        /// </summary>
        public long ContentItemId { get; set; }

        /// <summary>
        /// 栏目Id
        /// </summary>
        public int ContentCategoryId { get; set; }

        /// <summary>
        /// 内容模型Id
        /// </summary>
        public int ContentModelId { get; set; }

        /// <summary>
        /// 标题允许录入及修改
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 标题图Id
        /// </summary>
        public long FeaturedImageAttachmentId { get; set; }

        /// <summary>
        /// 发布部门Id
        /// </summary>
        public string DepartmentGuid { get; set; }
		
        /// <summary>
        /// 发布资讯获得的积分(备用)
        /// </summary>
        public int Points { get; set; }
		
        /// <summary>
        /// 发布者UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 发布者DisplayName
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        ///审批状态(-1=已删除(看需求是否需要)，0 草稿状态，10未通过审核，20=待审核，30=需再审核，40=已通过审核)
        /// </summary>
        public AuditStatus ApprovalStatus { get; set; }

        /// <summary>
        ///是否锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        ///是否置顶
        /// </summary>
        public bool IsSticky { get; set; }

        /// <summary>
        ///IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///发布时间
        /// </summary>
        public DateTime DatePublished { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///最后更新时间
        /// </summary>
        public DateTime LastModified { get; set; }

        #endregion

        #region 扩展

        #region 提交表单扩展

        /// <summary>
        /// Body Imgids
        /// </summary>
        [Ignore]
        public IEnumerable<long> BodyImageAttachmentIds { get; set; }

        #endregion


        /// <summary>
        /// 是否允许评论
        /// </summary>
        [Ignore]
        public bool IsComment
        {
            get
            {
                return GetExtendedProperty<bool>("IsComment");
            }
            set
            {
                SetExtendedProperty("IsComment", value);
            }
        }

        /// <summary>
        /// 编辑前是否为草稿
        /// </summary>
        [Ignore]
        public bool IsDraft { get; set; }

        /// <summary>
        /// 是否允许内容末尾显示附件列表
        /// </summary>
        [Ignore]
        public bool IsVisible
        {
            get
            {
                return GetExtendedProperty<bool>("IsVisible");
            }
            set
            {
                SetExtendedProperty("IsVisible", value);
            }
        }

        private IDictionary<string, object> additionalProperties = null;
        public object contentItemService;

        /// <summary>
        /// 附表中的字段
        /// </summary>
        [Ignore]
        public IDictionary<string, object> AdditionalProperties
        {
            get
            {
                if (additionalProperties == null || additionalProperties.Count == 0)
                {
                    IContentItemRepository contentItemRepository = new ContentItemRepository();
                    additionalProperties = contentItemRepository.GetContentItemAdditionalProperties(this.ContentItemId);
                    if (additionalProperties == null)
                        additionalProperties = new Dictionary<string, object>();
                }
                return additionalProperties;
            }
            set { additionalProperties = value; }
        }

        /// <summary>
        /// 内容模型
        /// </summary>
        [Ignore]
        public ContentModel ContentModel
        {

            get { return new Repository<ContentModel>().Get((long)this.ContentModelId); }
        }

        /// <summary>
        /// 栏目
        /// </summary>
        [Ignore]
        public ContentCategory ContentCategory
        {
            get { return new Repository<ContentCategory>().Get(this.ContentCategoryId); }
        }

        /// <summary>
        /// 下一内容项ContentItemId
        /// </summary>
        [Ignore]
        public long NextContentItemId
        {
            get { return new ContentItemRepository().GetNextContentItemId(this); }
        }

        /// <summary>
        /// 上一内容项ContentItemId
        /// </summary>
        [Ignore]
        public long PrevContentItemId
        {
            get { return new ContentItemRepository().GetPrevContentItemId(this); }
        }

        /// <summary>
        /// 下一篇内容项
        /// </summary>
        [Ignore]
        public ContentItem NextContentItem
        {
            get
            {
                return new ContentItemRepository().Get(NextContentItemId);
            }
        }

        /// <summary>
        /// 上一篇内容项
        /// </summary>
        [Ignore]
        public ContentItem PrevContentItem
        {
            get
            {
                return new ContentItemRepository().Get(PrevContentItemId);
            }
        }

        /// <summary>
        /// 获取ContentItem的解析过的内容
        /// </summary>
        public string GetResolvedBody()
        {
            return DIContainer.Resolve<ContentItemRepository>().GetResolvedBody(this.ContentItemId);
        }

        #endregion

        #region IAuditable 实现

        /// <summary>
        /// 审核项Key 
        /// </summary>
        public string AuditItemKey
        {
            get { return AuditItemKeys.Instance().ContentItem(); }
        }


        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.ContentItemId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}