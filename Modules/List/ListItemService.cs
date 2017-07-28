//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Tunynet.Common
{
    /// <summary>
    /// 列表项管理业务逻辑
    /// </summary>
    public class ListItemService
    {
        private IListItemRepository repository ;

        /// <summary>
        /// 构造器
        /// </summary>
        public ListItemService(IListItemRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// 列表项管理数据存入
        /// </summary>
        /// <param name="listItem">列表项管理实体</param>
        public void Create(ListItem listItem)
        {
            repository.Create(listItem);
        }

        /// <summary>
        /// 删除列表项及所有后代列表项
        /// </summary>
        /// <param name="Id">主键Id</param>
        public void Delete(long Id)
        {
            repository.DeleteListItems(Id);
        }

        /// <summary>
        /// 修改列表项管理数据
        /// </summary>
        /// <param name="listItem">列表项管理实体</param>
        public void Update(ListItem listItem)
        {
            repository.Update(listItem);
        }


        /// <summary>
        /// 获取一条列表项管理数据
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns>一条列表项管理数据</returns>
        public ListItem Get(long id)
        {
            return repository.Get(id);
        }

        /// <summary>
        /// 获取某一列表下的所有列表项
        /// </summary>
        /// <returns>所有列表项管理数据</returns>
        public IEnumerable<ListItem> GetItemsOfList(string listCode)
        {
            return repository.GetItemsOfList(listCode);
        }

        /// <summary>
        /// 根据项目编码获取一个列表项
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="listCode">列表编码</param>
        /// <returns>一个列表项</returns>
        public ListItem GetItemByItemCode(string listCode, string itemCode)
        {
            return repository.GetItemByItemCode(listCode, itemCode);
        }

        /// <summary>
        /// 获取根级列表项
        /// </summary>
        /// <param name="listCode">列表编码</param>        
        /// <returns>返回根级列表项集合</returns>
        public IEnumerable<ListItem> GetRootItems(string listCode)
        {
            IEnumerable<ListItem> items = repository.GetItemsOfList(listCode);
            return items.Where(n => n.ParentCode == string.Empty);
        }

        /// <summary>
        /// 获取子级列表项
        /// </summary>
        /// <param name="listCode">列表编码</param>
        /// <param name="parentCode">父级列表项编码</param>
        /// <returns>执行返回子集</returns>
        public IEnumerable<ListItem> GetChildren(string listCode, string parentCode)
        {
            IEnumerable<ListItem> items = repository.GetItemsOfList(listCode);
            return items.Where(n => n.ParentCode == parentCode);
        }

        /// <summary>
        /// 根据ItemCodes获取所有列表项
        /// </summary>
        /// <param name="listcode"></param>
        /// <param name="itemCodes"></param>
        /// <returns></returns>
        public IEnumerable<ListItem> GetItemsByItemCodes(string listcode, IEnumerable<string> itemCodes)
        {
            return repository.GetItemsByItemCodes(listcode, itemCodes);
        }

    }
}
