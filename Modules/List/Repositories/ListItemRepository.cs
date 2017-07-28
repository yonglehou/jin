//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using PetaPoco;
using System.Collections.Generic;
using System.Linq;
using Tunynet.Caching;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 列表项管理数据访问
    /// </summary>
    public class ListItemRepository : Repository<ListItem>, IListItemRepository
    {
        /// <summary>
        /// 列表项管理数据存入
        /// </summary>
        /// <param name="listItem">列表项管理实体</param>
        public void Create(ListItem listItem)
        {
            Insert(listItem);
            if (string.IsNullOrEmpty(listItem.ParentCode))
            {
                CreateDAO().Execute(Sql.Builder.Append("update tn_ListItems set DisplayOrder = Id where Id =@0", listItem.Id));
            }
            else
            {
                ListItem parentListItem = GetItemByItemCode(listItem.ListCode, listItem.ParentCode);
                if (parentListItem != null)
                {
                    List<Sql> sqls = new List<Sql>();
                    Sql sql = Sql.Builder;
                    sqls.Add(sql.Append("update tn_ListItems set ChildrenCount=@0 where Id=@1", parentListItem.ChildrenCount + 1, parentListItem.Id));
                    sqls.Add(sql.Append("update tn_ListItems set Depth=@0,DisplayOrder=Id where Id=@1", parentListItem.Depth + 1, listItem.Id));
                    CreateDAO().Execute(sqls);
                }
            }
        }

        /// <summary>
        /// 删除列表项及所有后代列表项
        /// </summary>
        /// <param name="Id">主键Id</param>
        public void DeleteListItems(long Id)
        {
            ListItem li = Get(Id);
            if (li == null)
                return;
            List<Sql> sqls = new List<Sql>();
            Sql sql = Sql.Builder;
            ListItem parentListItem = GetItemByItemCode(li.ListCode, li.ParentCode);
            IEnumerable<ListItem> descendants = GetDescendants(li.ListCode, li.ItemCode);
            foreach (var item in descendants)
            {
                Delete(item);
            }
            Delete(li);
            if (parentListItem != null)
            {
                sqls.Add(sql.Append("update tn_ListItems set ChildrenCount=@0 where Id=@1", parentListItem.ChildrenCount - 1, parentListItem.Id));
                CreateDAO().Execute(sqls);
            }
        }


        /// <summary>
        /// 根据项目编码获取一个列表项
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="listCode">列表编码</param>
        /// <returns>一个列表项</returns>
        public ListItem GetItemByItemCode(string listCode, string itemCode)
        {
            Sql sql = Sql.Builder.Append("select * from tn_ListItems where ListCode=@0 and ItemCode=@1", listCode, itemCode);
            return CreateDAO().SingleOrDefault<ListItem>(sql);
        }

        /// <summary>
        /// 获取一个列表下的所有列表项
        /// </summary>
        /// <param name="listCode"></param>
        /// <returns></returns>
        public IEnumerable<ListItem> GetItemsOfList(string listCode)
        {
            string cacheKey = EntityData.ForType(typeof(ListItem)).RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "ListCode", listCode);
         
            List<ListItem> items = cacheService.Get<List<ListItem>>(cacheKey);
            if (items == null)
            {
                Sql sql = Sql.Builder.Append("select * from tn_ListItems where ListCode=@0 order by DisplayOrder", listCode);
                items = CreateDAO().Fetch<ListItem>(sql);
                cacheService.Set(cacheKey, items, CachingExpirationType.UsualObjectCollection);
            }
            return items;
        }

        /// <summary>
        /// 获取parentCode的所有后代列表项
        /// </summary>
        /// <param name="listCode"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public IEnumerable<ListItem> GetDescendants(string listCode, string parentCode)
        {
            //获取当前分类，若不存在则返回null
            ListItem currentParentItem = GetItemByItemCode(listCode, parentCode);
            if (currentParentItem == null)
                return null;

            List<ListItem> itemsOfList = GetItemsOfList(listCode).ToList();
            if (itemsOfList.Count() == 0)
                return null;

            //调用递归获取所有后代列表项
            List<ListItem> descendants = new List<ListItem>();
            RecurseGetChildren(currentParentItem, descendants, itemsOfList);

            return descendants;
        }


        /// <summary>
        /// 删除ListCode对应的所有列表项
        /// </summary>
        /// <param name="listCode">列表项</param>
        public void DeleteItemsByListCode(string listCode)
        {
            Sql sql = Sql.Builder.Append("delete tn_ListItems where ListCode=@0", listCode);
            CreateDAO().Execute(sql);
        }

        /// <summary>
        /// 根据ItemCodes获取所有列表项
        /// </summary>
        /// <param name="listcode"></param>
        /// <param name="itemCodes"></param>
        /// <returns></returns>
        public IEnumerable<ListItem> GetItemsByItemCodes(string listcode, IEnumerable<string> itemCodes)
        {
            Sql sql = Sql.Builder.Select("*").From("tn_ListItems").Where("ListCode=@0 and ItemCode in (@itemCodes)", listcode, new { itemCodes = itemCodes });
            return CreateDAO().Query<ListItem>(sql);
        }


        /// <summary>
        /// 获取所有子分类的递归方法
        /// </summary>
        /// <param name="category">当前分类</param>
        /// <param name="treeCategories">最终要组装的Tree分类</param>
        /// <param name="orgCategoriesList">原始分类列表</param>
        private void RecurseGetChildren(ListItem item, List<ListItem> descendantItems, List<ListItem> itemList)
        {
            if (item.ChildrenCount == 0)
                return;

            //获取子列表项
            List<ListItem> childer = itemList.Where(n => n.ParentCode.Equals(item.ItemCode)).ToList();

            //获取子分类的的子分类
            foreach (ListItem li in childer)
            {
                if (li == null)
                    continue;

                //将该分类加入到列表
                descendantItems.Add(li);

                //递归获取所有子分类
                RecurseGetChildren(li, descendantItems, itemList);
            }
        }


    }
}
