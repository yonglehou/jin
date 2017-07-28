//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PetaPoco;
using Tunynet.Caching;
using Autofac;
using System.Linq;
using Tunynet.Repositories;

namespace Tunynet.CMS
{
    [TableName("tn_ContentCategories")]
    [PrimaryKey("CategoryId", autoIncrement = true)]
    [CacheSetting(false)]
    [Serializable]
    public class ContentCategory : SerializablePropertiesBase, IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public static ContentCategory New()
        {
            ContentCategory contentCategorie = new ContentCategory()
            {
                CategoryName = string.Empty,
                Description = string.Empty,
                ParentIdList = "",
                DateCreated = DateTime.Now,
                ContentModelKeys = string.Empty,                
                ParentId=0

            };
            return contentCategorie;
        }

        #region 需持久化属性

        /// <summary>
        ///主键
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///栏目名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        ///栏目描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///ParentId
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        ///所有父级CatetoryId
        /// </summary>
        public string ParentIdList { get; set; }

        /// <summary>
        ///子栏目数目
        /// </summary>
        public int ChildCount { get; set; }

        /// <summary>
        ///深度(从0开始)
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///内容计数
        /// </summary>
        public int ContentCount { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///内容模型Key集合(多个用英文逗号隔开)
        /// </summary>
        public string ContentModelKeys { get; set; }

        /// <summary>
        ///流程定义Id
        /// </summary>
        public long ProcessDefinitionId { get; set; }

        /// <summary>
        ///排列顺序，默认和CategoryId一致
        /// </summary>
        public int DisplayOrder { get; set; }

        #endregion

        /// <summary>
        /// 获取内容模型集合
        /// </summary>
        [Ignore]
        public IEnumerable<ContentModel> ContentTypes
        {
            get
            {
                var keys = this.ContentModelKeys.Split(',');
                //获取内容模型的集合.Where(n=>n.IsEnabled)
                IEnumerable<ContentModel> contentmodel = new Repository<ContentModel>().GetAll();
                return contentmodel.Where(n => keys.Contains(n.ModelKey));
            }
        }

        /// <summary>
        /// 获取所有子栏目(非即时更新)
        /// </summary>
        [Ignore]
        public IEnumerable<ContentCategory> Children
        {
            get
            {
                if (ChildCount > 0)
                    return new Repository<ContentCategory>().GetAll("DisplayOrder").Where(x => x.ParentId == this.CategoryId);
                else
                    return new List<ContentCategory>();
            }
        }

        #region IEntity 成员

        object IEntity.EntityId { get { return this.CategoryId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}