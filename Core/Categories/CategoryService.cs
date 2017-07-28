//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Events;
using Tunynet.Common.Repositories;
using Tunynet.Caching;
//using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 分类业务逻辑类
    /// </summary>
    public class CategoryService
    {
        private ICategoryRepository categoryRepository;
        private IItemInCategoryRepository itemInCategoryRepository;
        //private ISectionRepository sectionRepository;

        /// <summary>
        /// 可设置repository的构造函数
        /// </summary>
        public CategoryService(ICategoryRepository categoryRepository, IItemInCategoryRepository itemInCategoryRepository)
        {
            this.categoryRepository = categoryRepository;
            this.itemInCategoryRepository = itemInCategoryRepository;
            //this.sectionRepository = sectionRepository;
        }

        #region Create & Update & Delete & Merge & Move

        /// <summary>
        /// 创建类别
        /// </summary>
        /// <param name="category">待创建的类别</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public bool Create(Category category)
        {
            //写入数据库前，触发相关事件
            EventBus<Category>.Instance().OnBefore(category, new CommonEventArgs(EventOperationType.Instance().Create()));
            //将数据插入数据库
            categoryRepository.Insert(category);

            //若插入数据成功，则触发数据入库后的相关事件
            if (category.CategoryId > 0)
            {
                EventBus<Category>.Instance().OnAfter(category, new CommonEventArgs(EventOperationType.Instance().Create()));

                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新类别，注意：不能更新ParentId属性！
        /// </summary>
        /// <param name="category">待更新的类别</param>
        public void Update(Category category)
        {
            //更新数据前，触发的相关事件
            EventBus<Category>.Instance().OnBefore(category, new CommonEventArgs(EventOperationType.Instance().Update()));
            //更新到数据库
            categoryRepository.Update(category);
            //更新数据后，触发的相关事件
            EventBus<Category>.Instance().OnAfter(category, new CommonEventArgs(EventOperationType.Instance().Update()));

        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="categoryId">待删除类别Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool Delete(long categoryId)
        {
            //首先获取要删除的分类，若获取不到则返回false
            Category category = categoryRepository.Get(categoryId);
            if (category == null)
                throw new ExceptionFacade(string.Format("Id为:{0}的分类不存在", categoryId));

            //删除数据前，触发相关事件
            EventBus<Category>.Instance().OnBefore(category, new CommonEventArgs(EventOperationType.Instance().Delete()));

            #region 删除分类

            //删除所有子分类以及子分类下面的内容
            if (category.ChildCount > 0)
            {
                IEnumerable<Category> childCategories = GetCategoriesOfChildren(categoryId);
                foreach (Category childCategoy in childCategories)
                {
                    Delete(childCategoy.CategoryId);
                }
            }

            //删除分类下的内容项
            ClearItemsFromCategory(categoryId);
            //从数据库删除数据
            bool isDeleted = (categoryRepository.Delete(category) > 0);


            #endregion

            //若删除成功，触发删除后相关事件
            if (isDeleted)
            {
                EventBus<Category>.Instance().OnAfter(category, new CommonEventArgs(EventOperationType.Instance().Delete()));

            }

            #region 处理父级缓存

            Tunynet.Caching.RealTimeCacheHelper cacheHelper = EntityData.ForType(typeof(Category)).RealTimeCacheHelper;
            if (category.ParentId > 0)
            {
                //处理实体缓存
                cacheHelper.IncreaseEntityCacheVersion(category.ParentId);
                //处理区域缓存
                //cacheHelper.IncreaseAreaVersion("ParentId", category.ParentId);
            }

            #endregion

            return isDeleted;
        }

        /// <summary>
        /// 根据用户删除用户类别（删除用户时使用）
        /// </summary>
        public void CleanByUser(long userId)
        {
            categoryRepository.CleanByUser(userId);
        }
        /// <summary>
        /// 从fromCategoryId并入到toCategoryId
        /// </summary>
        /// <remarks>
        /// 例如：将分类fromCategoryId合并到分类toCategoryId，那么fromCategoryId分类下的所有子分类和实体全部归到toCategoryId分类，同时删除fromCategoryId分类
        /// </remarks>
        /// <param name="fromCategoryId">合并分类源类别</param>
        /// <param name="toCategoryId">合并分类目标类别</param>
        public void MergeCategory(long fromCategoryId, long toCategoryId)
        {
            #region 验证输入参数的有效性

            //会影响到数据层，判断输入参数的合理性
            if (fromCategoryId < 1 || toCategoryId < 1)
                throw new ExceptionFacade(string.Format("输入参数fromCategoryId-{0}，或者toCategoryId-{1}，不合理！", fromCategoryId, toCategoryId));

            //若formCategory，在数据库中不存在则返回
            Category formCategory = categoryRepository.Get(fromCategoryId);
            if (formCategory == null)
                throw new ExceptionFacade("fromCategory不存在！");

            //若toCategory，在数据库中不存在则返回
            Category toCategory = categoryRepository.Get(toCategoryId);
            if (toCategory == null)
                throw new ExceptionFacade("toCategory不存在！");

            if (GetCategoriesOfDescendants(fromCategoryId).Select(n => n.CategoryId).ToList().Contains(toCategoryId)
                || fromCategoryId == toCategoryId)
                throw new ExceptionFacade("不能合并到指定分类！");

            #endregion

            //直接调用仓储提供的方法，处理缓存
            categoryRepository.MergeCategory(fromCategoryId, toCategoryId);

        }

        /// <summary>
        /// 把fromCategoryId移动到toCategoryId
        /// </summary>
        /// <remarks>
        /// 将一个分类移动到另一个分类，并作为另一个分类的子分类
        /// </remarks>
        /// <param name="fromCategoryId">被移动类别</param>
        /// <param name="toCategoryId">目标类别</param>
        public void MoveCategoryToOther(long fromCategoryId, long toCategoryId)
        {
            #region 验证输入参数的有效性

            //会影响到数据层，判断输入参数的合理性
            if (fromCategoryId < 1 || toCategoryId < 1)
                throw new ExceptionFacade(string.Format("输入参数fromCategoryId-{0}，或者toCategoryId-{1}，不合理！", fromCategoryId, toCategoryId));

            //若formCategory，在数据库中不存在则返回
            Category formCategory = categoryRepository.Get(fromCategoryId);
            if (formCategory == null)
                throw new ExceptionFacade("fromCategory不存在！");

            //若toCategory，在数据库中不存在则返回
            Category toCategory = categoryRepository.Get(toCategoryId);
            if (toCategory == null)
                throw new ExceptionFacade("toCategory不存在！");

            //父及分类不能合并到子分类中
            if (GetCategoriesOfDescendants(fromCategoryId).Select(n => n.CategoryId).ToList().Contains(toCategoryId)
                || fromCategoryId == toCategoryId)
                throw new ExceptionFacade("不能移动到指定分类下！");

            #endregion

            //直接调用仓储提供的方法，处理缓存
            categoryRepository.MoveCategoryToOther(fromCategoryId, toCategoryId);


        }

        #endregion

        #region Get & Gets

        /// <summary>
        /// 获取Category
        /// </summary>
        /// <param name="categoryId">CategoryId</param>
        public Category Get(long categoryId)
        {
            //根据标识获取实体
            return categoryRepository.Get(categoryId);
        }

        /// <summary>
        /// 获取拥有者的类别列表
        /// </summary>
        /// <param name="ownerId">类别拥有者Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns>按树状排序的</returns>
        public IEnumerable<Category> GetOwnerCategories(long ownerId, string tenantTypeId)
        {
            //1 从数据库取出全部该用户的该应用的分类
            //2 取出所有的顶级节点
            //3 使用递归组织每个分类下的所有分类,可以直接显示为tree
            //4 将数据放入缓存 - 缓存分区：OwnerId
            //注意：从数据库中取出来的数据是同级按照DisplayOrder正排序，这样就不需要再进行排序了
            return categoryRepository.GetOwnerCategories(ownerId, tenantTypeId);
        }

        /// <summary>
        /// 获取Onwer的所有根类别
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">OwnerId</param>
        /// <returns></returns>
        public IEnumerable<Category> GetRootCategoriesOfOwner(string tenantTypeId, long ownerId = 0)
        {
            return categoryRepository.GetRootCategoriesOfOwner(ownerId, tenantTypeId);
        }

        /// <summary>
        /// 获取子类别
        /// </summary>
        /// <param name="categoryId">父类别Id</param>
        /// <returns></returns>
        public IEnumerable<Category> GetCategoriesOfChildren(long categoryId)
        {
            //根据parentCategoryId获取所有的直属子分类的ID，并且按照DisplayOrder正序
            //获取后放入缓存，使用ParentId进行分区
            return categoryRepository.GetCategoriesOfChildren(categoryId);
        }

        /// <summary>
        /// 获取后代类别
        /// </summary>
        /// <param name="categoryId">父类别Id</param>
        /// <returns></returns>
        public IEnumerable<Category> GetCategoriesOfDescendants(long categoryId)
        {
            //获取当前分类，若不存在则返回null
            Category currentParentCategory = Get(categoryId);
            if (currentParentCategory == null)
                return null;

            //从OwnerId、tenantTypeId对应的类别集合中获取，排序：DisplayOrder正序 
            IEnumerable<Category> orgCategories = GetOwnerCategories(currentParentCategory.OwnerId, currentParentCategory.TenantTypeId);
            if (orgCategories == null)
                return null;
            IList<Category> orgCategoriesList = new List<Category>(orgCategories);
            if (orgCategoriesList == null || orgCategoriesList.Count < 1)
                return null;

            //调用递归获取所有子集
            IList<Category> descendantsList = new List<Category>();
            categoryRepository.RecurseGetChildren(currentParentCategory, descendantsList, orgCategoriesList);

            return descendantsList;
        }
        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Category> GetAll()
        {
            return categoryRepository.GetAll();
        }

        #endregion

        #region 类别与内容项关系

        /// <summary>
        /// 批量为内容项设置类别
        /// </summary>
        /// <param name="itemIds">内容项Id集合</param>
        /// <param name="categoryId">类别Id</param>
        public void AddItemsToCategory(IEnumerable<long> itemIds, long categoryId)
        {




            //首先输入参数是否合理
            if (itemIds == null || categoryId < 1)
                throw new ExceptionFacade("输入参数不合理！");

            //获取关联的分类
            Category category = Get(categoryId);
            if (category == null)
                throw new ExceptionFacade(string.Format("Id为：{0}的分类不存在！", categoryId));





            //插入数据库
            int effectLineCount = itemInCategoryRepository.AddItemsToCategory(itemIds, categoryId);

            //修改category的内容项
            category.ItemCount = category.ItemCount + effectLineCount;
            categoryRepository.UpdateItemCount(category);
            //移除内容项缓存
            RemoveParentCache(categoryId);

        }
        /// <summary>
        /// 递归移除父级内容项个数的缓存
        /// </summary>
        /// <param name="categoryId"></param>

        public void RemoveParentCache(long categoryId)
        {
            var cacheService = DIContainer.Resolve<ICacheService>();
            Category category = Get(categoryId);
            if (category != null)
            {
                var cacheKey = "CumulateItemCount:CategoryId-" + categoryId;
                cacheService.Remove(cacheKey);
                if (category.ParentId > 0)
                    RemoveParentCache(category.ParentId);
            }

        }
        /// <summary>
        /// 为内容项批量设置类别
        /// </summary>
        /// <param name="categoryIds">类别Id集合</param>
        /// <param name="itemId">内容项Id</param>
        /// <param name="ownerId">类别拥有者Id</param>
        public void AddCategoriesToItem(IEnumerable<long> categoryIds, long itemId)
        {
            //若输入参数有问题，则直接返回
            if (categoryIds == null || itemId < 1)
                throw new ExceptionFacade("输入参数不合理！");

            string tenantTypeId = "100201";
            if (categoryIds.Count() > 0)
            {
                long categoryId = categoryIds.First();
                Category category = Get(categoryId);
                tenantTypeId = category.TenantTypeId;
            }

            EventBus<long, CategoryEventArgs>.Instance().OnBatchBefore(categoryIds, new CategoryEventArgs(EventOperationType.Instance().Create(), tenantTypeId, itemId));
            //数据库插入操作
            itemInCategoryRepository.AddCategoriesToItem(categoryIds, itemId);

            EventBus<long, CategoryEventArgs>.Instance().OnBatchAfter(categoryIds, new CategoryEventArgs(EventOperationType.Instance().Create(), tenantTypeId, itemId));


        }

        /// <summary>
        /// 清除内容项的所有类别(某个租户和用户的)
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <param name="ownerId">分类所有者</param>
        /// <param name="tenantTypeId">租户Id</param>
        public void ClearCategoriesFromItem(long itemId, long? ownerId, string tenantTypeId)
        {
            //操作设计到的分类
            IEnumerable<Category> categories = GetCategoriesOfItem(itemId, ownerId, tenantTypeId);
            if (categories != null && categories.Count() > 0)
            {
                foreach (Category category in categories)
                {
                    if (category != null)
                    {
                        category.ItemCount--;
                        if (category.ItemCount < 0)
                            category.ItemCount = 0;
                        categoryRepository.UpdateItemCount(category);
                    }
                }
            }
            //操作数据库
            itemInCategoryRepository.ClearCategoriesFromItem(itemId, ownerId, tenantTypeId);
        }

        /// <summary>
        /// 删除分类下的所有的关联项
        /// </summary>
        /// <param name="category">要处理的分类</param>
        /// <param name="ownerId">拥有者Id</param>
        public void ClearItemsFromCategory(Category category)
        {
            //获取关联的Category
            if (category == null)
                throw new ExceptionFacade("category不能为空！");

            //操作数据库
            int effectLineCount = itemInCategoryRepository.ClearItemsFromCategory(category.CategoryId);

            //处理关联的category
            if (effectLineCount > 0)
            {
                category.ItemCount = category.ItemCount - effectLineCount;
                if (category.ItemCount < 0)
                    category.ItemCount = 0;
                categoryRepository.UpdateItemCount(category);
            }
        }

        /// <summary>
        /// 删除分类下的所有的关联项
        /// </summary>
        /// <param name="categoryId">分类Id</param>
        public void ClearItemsFromCategory(long categoryId)
        {
            //获取关联的Category
            Category category = Get(categoryId);
            ClearItemsFromCategory(category);
        }

        /// <summary>
        /// 删除分类同内容的关联项
        /// </summary>
        /// <param name="categoryId">分类Id</param>
        /// <param name="itemId">内容项Id</param>
        /// <param name="ownerId">拥有者Id</param>
        public void DeleteItemInCategory(long categoryId, long itemId)
        {
            Category category = categoryRepository.Get(categoryId);
            if (category == null)
                throw new ExceptionFacade(string.Format("Id为:{0}的分类不存在", categoryId));

            EventBus<long, CategoryEventArgs>.Instance().OnBatchBefore(new List<long> { categoryId }, new CategoryEventArgs(EventOperationType.Instance().Delete(), category.TenantTypeId, itemId));

            int effectLineCount = itemInCategoryRepository.DeleteItemInCategory(categoryId, itemId);

            if (effectLineCount > 0)
            {
                EventBus<long, CategoryEventArgs>.Instance().OnBatchAfter(new List<long> { categoryId }, new CategoryEventArgs(EventOperationType.Instance().Delete(), category.TenantTypeId, itemId));
            }

        }

        /// <summary>
        /// 将内容项从fromCategoryId转移到toCategoryId
        /// </summary>
        /// <param name="itemIds">要转移的内容项</param>
        /// <param name="toCategoryId">目标分类Id</param>
        /// <param name="ownerId">分类所有者</param>
        /// <param name="tenantTypeId">租户Id</param>
        public void MoveItemsToCategory(IEnumerable<long> itemIds, long toCategoryId, long? ownerId, string tenantTypeId)
        {
            //删除内容项原来的关联
            foreach (long itemId in itemIds)
            {
                ClearCategoriesFromItem(itemId, ownerId, tenantTypeId);
            }

            //将内容项关联到toCategoryId
            AddItemsToCategory(itemIds, toCategoryId);
        }

        /// <summary>
        /// 获取类别的内容项集合
        /// </summary>
        /// <param name="categoryId">分类的Id集合</param>
        /// <param name="includeDescendant">是否包括子分类</param>
        /// <returns>内容项的ID集合</returns>
        public IEnumerable<long> GetItemsOfCategory(long categoryId, bool includeDescendant)
        {
            #region 组装分类的ID列表

            List<long> ids = new List<long>();
            ids.Add(categoryId);

            //若包括子分类则获取所有子孙
            if (includeDescendant)
            {
                ids.AddRange(GetCategoriesOfDescendants(categoryId).Select(n => n.CategoryId));
            }

            #endregion
            IEnumerable<long> itemIds = itemInCategoryRepository.GetItemsOfCategory(categoryId, ids);
            return itemIds;
        }

        /// <summary>
        /// 获取类别的内容项集合
        /// </summary>
        /// <param name="categoryId">类别Id</param>
        /// <param name="includeDescendant">是否包括所有后代类别节点的内容项</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">当前页码(从1开始)</param
        /// <param name="totalRecords">符合查询条件的记录数</param>
        /// <returns>返回指定页码的内容项Id集合</returns>
        public IEnumerable<long> GetItemIds(long categoryId, bool includeDescendant, int pageSize, int pageIndex, out long totalRecords)
        {
            #region 组装分类的ID列表

            List<long> ids = new List<long>();
            ids.Add(categoryId);

            //若包括子分类则获取所有子孙
            if (includeDescendant)
                ids.AddRange(GetCategoriesOfDescendants(categoryId).Select(n => n.CategoryId));

            #endregion

            //从数据库中获取数据
            IEnumerable<long> itemIds = itemInCategoryRepository.GetItemIdsOfCategory(categoryId, ids, pageSize, pageIndex, out totalRecords);

            return itemIds;
        }

        /// <summary>
        /// 获取内容项的所有类别
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <returns>返回内容项的类别集合</returns>
        /// <param name="ownerId">分类所有者</param>
        /// <param name="tenantTypeId">租户Id</param>
        public IEnumerable<Category> GetCategoriesOfItem(long itemId, long? ownerId, string tenantTypeId)
        {
            //内容项所对应的分类的列表
            IList<Category> categories = new List<Category>();

            //从数据库中获取内容项对应的分类的ID列表
            IEnumerable<long> categoryIds = itemInCategoryRepository.GetCategoriesOfItem(itemId, ownerId, tenantTypeId);

            if (categoryIds == null)
                return categories;

            //从categoryIds 组装成 categories
            foreach (long categoryId in categoryIds)
            {
                Category category = Get(categoryId);
                if (category != null)
                    categories.Add(category);
            }

            return categories;
        }
        /// <summary>
        /// 获取分类及其成员项的关系集合
        /// </summary>
        /// <param name="itemId">成员Id</param>
        /// <param name="categoryId">分类Id</param>
        /// <returns></returns>
        public IEnumerable<ItemInCategory> GetItemsInCategory(long itemId, long categoryId)
        {
            return itemInCategoryRepository.GetItemsInCategories(itemId, categoryId);
        }
        /// <summary>
        /// 根据ItemId获取分类
        /// </summary>
        /// <param name="itemId">成员Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <returns></returns>
       public ItemInCategory GetItems(long itemId, string tenantTypeId)
        {
            return itemInCategoryRepository.GetItems(itemId,tenantTypeId);
        }

        #endregion

    }


}
