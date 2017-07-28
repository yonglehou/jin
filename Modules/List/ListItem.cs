//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PetaPoco;
using Tunynet.Caching;

namespace Tunynet.Common
{
    /// <summary>
    /// 列表项管理实体
    /// </summary>
    [TableName("tn_ListItems")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true, PropertyNamesOfArea = "ListCode,ParentCode")]
    [Serializable]
    public class ListItem : IEntity
    {
        #region 需持久化属性

        /// <summary>
        /// 列表项Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 项编码（同一ListCode内唯一）
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 列表编码
        /// </summary>
        public string ListCode { get; set; }

        /// <summary>
        /// 父级编码（根级为空字符串）
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 子级数目
        /// </summary>
        public int ChildrenCount { get; set; }

        /// <summary>
        /// 深度（从0开始）
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// 排列顺序
        /// </summary>
        public int DisplayOrder { get; set; }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion IEntity 成员

        /// <summary>
        /// 获取所有子列表项
        /// </summary>
        [Ignore]
        public IEnumerable<ListItem> Children
        {
            get
            {
                if (ChildrenCount > 0)
                    return DIContainer.Resolve< ListItemService >().GetChildren(this.ListCode,this.ItemCode);
                else
                    return new List<ListItem>();
            }
        }

        

    }
}
