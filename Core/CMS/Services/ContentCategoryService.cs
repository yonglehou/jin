//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Tunynet.Common;
using Tunynet.Events;
using Tunynet.Repositories;

namespace Tunynet.CMS
{
    /// <summary>
    /// 栏目业务逻辑
    /// </summary>
    public class ContentCategoryService
    {
        private IRepository<ContentCategory> contentCategoryrepository;
        private IContentItemRepository contentitemrepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContentCategoryService(IRepository<ContentCategory> contentCategoryrepositor, IContentItemRepository contentitemrepository)
        {
            this.contentCategoryrepository = contentCategoryrepositor;
            this.contentitemrepository = contentitemrepository;

        }

        /// <summary>
        /// 创建栏目
        /// </summary>
        /// <param name="contentcategory">栏目实体</param>
        public void Create(ContentCategory contentcategory)
        {
            if (contentcategory.ParentId > 0)
            {
                ContentCategory parentContentCategory = Get(contentcategory.ParentId);
                if (parentContentCategory != null)
                {
                    contentcategory.Depth = parentContentCategory.Depth + 1;
                    contentcategory.ParentIdList = parentContentCategory.ParentIdList + "," + contentcategory.ParentId;
                    parentContentCategory.ChildCount += 1;

                    contentCategoryrepository.Update(parentContentCategory);
                }
                else
                {
                    contentcategory.ParentId = 0;
                }
            }
            contentCategoryrepository.Insert(contentcategory);
            if (contentcategory.CategoryId > 0)
            {
                contentcategory.DisplayOrder = contentcategory.CategoryId;
                contentCategoryrepository.Update(contentcategory);
            }

            //执行事件
            EventBus<ContentCategory>.Instance().OnAfter(contentcategory, new CommonEventArgs(EventOperationType.Instance().Create(), 0));
            //生成操作日志
            EventBus<ContentCategory, CommonEventArgs>.Instance().OnAfter(contentcategory, new CommonEventArgs(EventOperationType.Instance().Create()));
        }

        /// <summary>
        /// 更新栏目
        /// </summary>
        /// <remarks>
        /// 不要修改ParentId，如需修改请使用Move()
        /// </remarks>
        /// <param name="contentcategory">栏目实体</param>
        public void Update(ContentCategory contentcategory)
        {
            contentCategoryrepository.Update(contentcategory);
            //执行事件
            EventBus<ContentCategory>.Instance().OnAfter(contentcategory, new CommonEventArgs(EventOperationType.Instance().Update(), 0));

            EventBus<ContentCategory, CommonEventArgs>.Instance().OnAfter(contentcategory, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        /// 更改栏目显示顺序
        /// </summary>
        /// <param name="fromContentCategoryId"></param>
        /// <param name="toContentCategoryId"></param>
        public void ChangeContentCategory(int fromContentCategoryId, int toContentCategoryId)
        {
            var fromContentCategory = Get(fromContentCategoryId);
            var toContentCategory = Get(toContentCategoryId);
            //更改栏目显示顺序
            var temp = fromContentCategory.DisplayOrder;
            fromContentCategory.DisplayOrder = toContentCategory.DisplayOrder;
            contentCategoryrepository.Update(fromContentCategory);
            toContentCategory.DisplayOrder = temp;
            contentCategoryrepository.Update(toContentCategory);
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <param name="contentcategory">栏目实体</param>
        public void Delete(ContentCategory contentcategory)
        {
            if (contentcategory == null)
                return;

            //更新父栏目的ChildCount
            if (contentcategory.ParentId > 0)
            {
                ContentCategory parentcontentcategory = Get(contentcategory.ParentId);
                if (parentcontentcategory != null)
                {
                    parentcontentcategory.ChildCount -= 1;
                    contentCategoryrepository.Update(parentcontentcategory);
                }
            }

            //ContentItemService contentItemService = new ContentItemService();

            //所有后代栏目
            IEnumerable<ContentCategory> descendantcontentcategories = GetCategoryDescendants(contentcategory.CategoryId);
            if (descendantcontentcategories != null)
            {
                foreach (var item in descendantcontentcategories)
                {
                    DeleteContentItemOfCategory(item.CategoryId);
                    contentCategoryrepository.Delete(item);
                }
            }
            contentCategoryrepository.Delete(contentcategory);
            DeleteContentItemOfCategory(contentcategory.CategoryId);

            //执行事件
            EventBus<ContentCategory>.Instance().OnAfter(contentcategory, new CommonEventArgs(EventOperationType.Instance().Delete(), 0));

            EventBus<ContentCategory, CommonEventArgs>.Instance().OnAfter(contentcategory, new CommonEventArgs(EventOperationType.Instance().Delete()));
        }

        /// <summary>
        /// 删除categoryId下的所有ContentItem
        /// </summary>
        /// <param name="categoryId"></param>
        public void DeleteContentItemOfCategory(int categoryid)
        {
            ContentItemQuery query = new ContentItemQuery()
            {
                CategoryId = categoryid
            };

            PagingDataSet<ContentItem> items = contentitemrepository.GetContentItems(query, int.MaxValue, 1);
            if (items != null)
            {
                foreach (var item in items)
                {
                    contentitemrepository.Delete(item);

                    EventBus<ContentItem, CommonEventArgs>.Instance().OnAfter(item, new CommonEventArgs(EventOperationType.Instance().Delete()));
                }
            }
        }

        /// <summary>
        /// 把fromCategoryId合并到toCategoryId
        /// </summary>
        /// <remarks>
        /// 例如：将栏目fromCategoryId合并到栏目toCategoryId，那么fromCategoryId栏目下的所有子栏目和ContentItem全部归到toCategoryId栏目，同时删除fromCategoryId栏目
        /// </remarks>
        /// <param name="fromCategoryId">被合并的栏目ID</param>
        /// <param name="toCategoryId">合并到的栏目ID</param>
        public void MergeCategory(int fromCategoryId, int toCategoryId)
        {
            ContentCategory toCategory = Get(toCategoryId);
            if (toCategory == null)
                return;

            ContentCategory fromCategory = Get(fromCategoryId);
            if (fromCategory == null)
                return;

            foreach (var childSection in fromCategory.Children)
            {
                childSection.ParentId = toCategoryId;
                childSection.Depth = toCategory.Depth + 1;

                if (childSection.Depth == 1)
                    childSection.ParentIdList = childSection.ParentId.ToString();
                else
                    childSection.ParentIdList = toCategory.ParentIdList + "," + childSection.ParentId;

                RecursiveUpdateDepthAndParentIdList(childSection);

                ContentItemQuery contentItemQuery = new ContentItemQuery()
                {
                    CategoryId = childSection.CategoryId
                };

                PagingDataSet<ContentItem> contentItems = contentitemrepository.GetContentItems(contentItemQuery, int.MaxValue, 1);

                foreach (var contentItem in contentItems)
                {
                    contentItem.ContentCategoryId = toCategoryId;
                    contentitemrepository.Update(contentItem);
                }
            }

            ContentItemQuery currentContentItemQuery = new ContentItemQuery()
            {
                CategoryId = fromCategoryId
            };
            PagingDataSet<ContentItem> currentContentItems = contentitemrepository.GetContentItems(currentContentItemQuery, int.MaxValue, 1);

            foreach (var item in currentContentItems)
            {
                item.ContentCategoryId = toCategoryId;
                contentitemrepository.Update(item);
            }

            if (fromCategory.ParentId > 0)
            {
                ContentCategory fromParentCategory = Get(fromCategory.ParentId);
                if (fromParentCategory != null)
                    fromParentCategory.ChildCount -= 1;
            }

            toCategory.ChildCount += fromCategory.ChildCount;
            contentCategoryrepository.Update(toCategory);
            contentCategoryrepository.Delete(fromCategory);
        }

        /// <summary>
        /// 把fromCategoryId移动到toCategoryId，作为toCategoryId的子栏目
        /// </summary>
        /// <remarks>
        /// 例如：将栏目fromCategoryId合并到栏目toCategoryId，那么fromCategoryId栏目下的所有子栏目和ContentItem全部归到toCategoryId栏目，同时删除fromCategoryId栏目
        /// </remarks>
        /// <param name="fromCategoryId">被移动的栏目ID</param>
        /// <param name="toCategoryId">移动到的栏目ID</param>
        public void MoveCategoryToOther(int fromCategoryId, int toCategoryId)
        {
            ContentCategory fromCategory = Get(fromCategoryId);
            if (fromCategory == null)
                return;

            if (fromCategory.ParentId > 0)
            {
                ContentCategory fromParentCategory = Get(fromCategory.ParentId);
                if (fromParentCategory != null)
                {
                    fromParentCategory.ChildCount -= 1;
                    contentCategoryrepository.Update(fromParentCategory);
                }
            }

            if (toCategoryId > 0)
            {
                ContentCategory toCategory = Get(toCategoryId);
                if (toCategory == null)
                    return;

                toCategory.ChildCount += 1;
                contentCategoryrepository.Update(toCategory);

                fromCategory.ParentId = toCategoryId;
                fromCategory.Depth = toCategory.Depth + 1;
                if (fromCategory.Depth == 1)
                    fromCategory.ParentIdList = fromCategory.ParentId.ToString();
                else
                    fromCategory.ParentIdList = toCategory.ParentIdList + "," + fromCategory.ParentId;
            }
            else //移动到顶层
            {
                fromCategory.Depth = 0;
                fromCategory.ParentIdList = string.Empty;
                fromCategory.ParentId = 0;
            }
            contentCategoryrepository.Update(fromCategory);

            if (fromCategory.Children != null)
            {
                foreach (var childCategory in fromCategory.Children)
                {
                    childCategory.Depth = fromCategory.Depth + 1;
                    childCategory.ParentIdList = fromCategory.ParentIdList + "," + fromCategory.CategoryId;
                    RecursiveUpdateDepthAndParentIdList(childCategory);
                }
            }
        }

        /// <summary>
        /// 获取栏目的累计内容数
        /// </summary>
        /// <param name="categoryId">栏目ID</param>
        /// <returns>栏目的累计内容数</returns>
        public int GetCategoryCumulateItemCount(int categoryId)
        {
            ContentCategory contentcategory = Get(categoryId);
            if (contentcategory == null)
                return 0;
            int cumulateItemCount = contentcategory.ContentCount;
            IEnumerable<ContentCategory> contentCategorys = GetCategoryDescendants(categoryId);
            foreach (var folder in contentCategorys)
            {
                cumulateItemCount += folder.ContentCount;
            }
            return cumulateItemCount;
        }

        /// <summary>
        /// 获取顶级栏目
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ContentCategory> GetRootCategories()
        {
            return GetAll().Where(x => x.ParentId == 0);
        }

        /// <summary>
        /// 更新后代的Depth和ParentIdList
        /// </summary>
        /// <param name="parentCategory">栏目实体</param>
        private void RecursiveUpdateDepthAndParentIdList(ContentCategory parentCategory)
        {
            contentCategoryrepository.Update(parentCategory);
            if (parentCategory.ChildCount > 0)
            {
                foreach (var folder in parentCategory.Children)
                {
                    folder.ParentId = parentCategory.CategoryId;
                    folder.Depth = parentCategory.Depth + 1;
                    folder.ParentIdList = parentCategory.ParentIdList + "," + parentCategory.CategoryId;

                    contentCategoryrepository.Update(folder);
                    RecursiveUpdateDepthAndParentIdList(folder);
                }
            }
        }

        /// <summary>
        /// 以缩进排序方式获取所有栏目
        /// </summary>
        public IEnumerable<ContentCategory> GetIndentedAllCategories()
        {
            IEnumerable<ContentCategory> rootCategorys = GetRootCategories();
            List<ContentCategory> organizedCategorys = new List<ContentCategory>();
            foreach (var folder in rootCategorys)
            {
                organizedCategorys.Add(folder);
                CategoryForIndented(folder, organizedCategorys);
            }

            return organizedCategorys;
        }

        /// <summary>
        /// 把栏目组织成缩进格式
        /// </summary>
        private static void CategoryForIndented(ContentCategory parentCategory, List<ContentCategory> organizedCategorys)
        {
            if (parentCategory.ChildCount > 0)
            {
                foreach (ContentCategory child in parentCategory.Children)
                {
                    organizedCategorys.Add(child);
                    CategoryForIndented(child, organizedCategorys);
                }
            }
        }

        /// <summary>
        /// 获取子栏目
        /// </summary>
        /// <param name="parentId">parentId</param>
        /// <returns></returns>
        public IEnumerable<ContentCategory> GetCategoryChildren(int parentId)
        {
            return GetAll().Where(x => x.ParentId == parentId);
        }

        /// <summary>
        /// 获取所有后代栏目
        /// </summary>
        /// <param name="categoryId">栏目ID</param>
        /// <returns></returns>
        public IEnumerable<ContentCategory> GetCategoryDescendants(int categoryId)
        {
            ContentCategory contentcategory = Get(categoryId);
            if (contentcategory == null || contentcategory.ChildCount == 0)
                return null;


            string descendantParentIdListPrefix = "," + contentcategory.CategoryId.ToString();
            if (contentcategory.ParentId == 0)
                //return GetAllCategorys().Where(x => x.ParentIdList.StartsWith(contentcategory.ContentCategoryId.ToString()));
                return GetAll().Where(x => x.ParentId == contentcategory.CategoryId);
            else
            {
                //所有后代栏目
                //   IEnumerable<ContentCategory> descendantContentCategorys = GetAllCategorys().Where(x => x.ParentIdList.EndsWith(descendantParentIdListPrefix));
                return GetAll().Where(x => x.ParentIdList.EndsWith(descendantParentIdListPrefix));
            }
        }

        /// <summary>
        /// 获取栏目
        /// </summary>
        /// <param name="categoryId">栏目ID</param>
        /// <returns></returns>
        public ContentCategory Get(int categoryId)
        {
            return contentCategoryrepository.Get(categoryId);
        }

        /// <summary>
        /// 获取所有ContentCategory
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ContentCategory> GetAll()
        {
            return contentCategoryrepository.GetAll("DisplayOrder");
        }

    }
}
