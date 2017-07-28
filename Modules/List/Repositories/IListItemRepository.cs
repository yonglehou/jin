//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 列表项管理数据访问接口
    /// </summary>
    public interface IListItemRepository : IRepository<ListItem>
    {
        /// <summary>
        /// 列表项管理数据存入
        /// </summary>
        /// <param name="listItem">列表项管理实体</param>
        void Create(ListItem listItem);

        /// <summary>
        /// 删除列表项及所有后代列表项
        /// </summary>
        /// <param name="Id">主键Id</param>
        void DeleteListItems(long Id);


        /// <summary>
        /// 根据项目编码获取一个列表项
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="listCode">列表编码</param>
        /// <returns>一个列表项</returns>
        ListItem GetItemByItemCode(string listCode, string itemCode);

        /// <summary>
        /// 获取一个列表下的所有列表项
        /// </summary>
        /// <param name="listCode"></param>
        /// <returns></returns>
        IEnumerable<ListItem> GetItemsOfList(string listCode);

        /// <summary>
        /// 获取parentCode的所有后代列表项
        /// </summary>
        /// <param name="listCode"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        IEnumerable<ListItem> GetDescendants(string listCode, string parentCode);


        /// <summary>
        /// 删除ListCode对应的所有列表项
        /// </summary>
        /// <param name="listCode">列表项</param>
        void DeleteItemsByListCode(string listCode);

        IEnumerable<ListItem> GetItemsByItemCodes(string listcode, IEnumerable<string> itemCodes);
    }
}
