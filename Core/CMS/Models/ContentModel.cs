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
using Tunynet;
using Tunynet.Repositories;

namespace Tunynet.CMS
{
    [TableName("tn_ContentModels")]
    [PrimaryKey("ModelId", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class ContentModel : IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public static ContentModel New()
        {
            ContentModel contentModel = new ContentModel()
            {
                ModelName = string.Empty,
                ModelKey = string.Empty,
                PageNew = string.Empty,
                PageEdit = string.Empty,
                PageManage = string.Empty,
                PageList = string.Empty,
                PageDetail = string.Empty,
                AdditionalTableName = string.Empty

            };
            return contentModel;
        } 

        #region 需持久化属性

        /// <summary>
        ///模型Id
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        ///模型名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        ///英文标识
        /// </summary>
        public string ModelKey { get; set; }

        /// <summary>
        ///是不是内建模型（内建模型不允许删除）
        /// </summary>
        public bool IsBuiltIn { get; set; }

        /// <summary>
        ///排序序号
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        ///发布页面UrlRouteName
        /// </summary>
        public string PageNew { get; set; }

        /// <summary>
        ///修改页面UrlRouteName
        /// </summary>
        public string PageEdit { get; set; }

        /// <summary>
        ///列表管理页面UrlRouteName
        /// </summary>
        public string PageManage { get; set; }

        /// <summary>
        ///列表页面UrlRouteName
        /// </summary>
        public string PageList { get; set; }

        /// <summary>
        ///详细显示页面UrlRouteName
        /// </summary>
        public string PageDetail { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///是否启用评论
        /// </summary>
        public bool EnableComment { get; set; }

        /// <summary>
        ///附加的数据库表名
        /// </summary>
        public string AdditionalTableName { get; set; }

        #endregion

        /// <summary>
        /// 附表外键
        /// </summary>
        public static string AdditionalTableForeignKey
        {
            get { return "ContentItemId"; }
        }

        /// <summary>
        /// 所有字段
        /// </summary>
        public IEnumerable<ContentModelAdditionalFields> AdditionalFields
        {
            get { return new Repository<ContentModelAdditionalFields>().GetAll().Where(n => n.ModelId == this.ModelId).ToList(); }
        }

        #region IEntity 成员

        object IEntity.EntityId { get { return this.ModelId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
