//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Tunynet.CMS;
using Tunynet.Common.Repositories;
using Tunynet.Events;
using Tunynet.Logging;
using Tunynet.Post;

namespace Tunynet.Common
{
    /// <summary>
    /// 分类的事件
    /// </summary>
    public class CategoryEventModule : IEventMoudle
    {
        private CategoryService categoryService;
        private OperationLogService operationLogService;
        private UserService userService;
        private RoleService roleService;
        private ICategoryRepository categoryRepository;
        private SectionService sectionService;

        public CategoryEventModule(CategoryService categoryService, OperationLogService operationLogService, UserService userService, RoleService roleService, ICategoryRepository categoryRepository, SectionService sectionService)
        {
            this.categoryService = categoryService;
            this.operationLogService = operationLogService;
            this.userService = userService;
            this.roleService = roleService;
            this.categoryRepository = categoryRepository;
            this.sectionService = sectionService;
        }
        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        public void RegisterEventHandler()
        {
            EventBus<Category>.Instance().After += new CommonEventHandler<Category, CommonEventArgs>(CategoryModuleForManagerOperation_After);
            EventBus<long, CategoryEventArgs>.Instance().BatchAfter += new BatchEventHandler<long, CategoryEventArgs>(CategoryModuleForItemCount_BatchAfter);
            EventBus<long, CategoryEventArgs>.Instance().BatchBefore += new BatchEventHandler<long, CategoryEventArgs>(CategoryModuleForItemCount_BatchBefore);
        }
        /// <summary>
        /// 分类操作事件
        /// </summary>
        /// <param name="category"></param>
        /// <param name="eventArgs"></param> 
        private void CategoryModuleForManagerOperation_After(Category category, CommonEventArgs eventArgs)
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Categorie());
            OperationLog newLog = new OperationLog(eventArgs.OperatorInfo);
            newLog.OperationObjectId = category.CategoryId;
            newLog.OperationObjectName = category.CategoryName;
            newLog.OperationType = eventArgs.EventOperationType;
            newLog.TenantTypeId = TenantTypeIds.Instance().Categorie();
            newLog.OperationUserRole = string.Join(",", roleService.GetRoleNamesOfUser(eventArgs.OperatorInfo.OperationUserId));
            if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
            {
                newLog.Description = string.Format("删除类别 {0}", category.CategoryName);
                //删除后处理父类别子节点数量
                var categoryService = DIContainer.Resolve<CategoryService>();
                //获取父类别以更新其子节点个数
                var parentCategory = categoryService.Get(category.ParentId);
                parentCategory.ChildCount--;
                categoryService.Update(parentCategory);
                //删除后将类别与内容取消关联
                categoryService.ClearItemsFromCategory(category);
                //删除分类附件
                attachmentService.DeletesByAssociateId(category.CategoryId);
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
            {
                newLog.Description = string.Format("添加类别 {0}", category.CategoryName);
                //临时附件转换成分类的正式附件
                attachmentService.ToggleTemporaryAttachments(category.OwnerId, TenantTypeIds.Instance().Categorie(), category.CategoryId, new List<long> { category.ImageAttachmentId });
            }
            else if (eventArgs.EventOperationType == EventOperationType.Instance().Update())
            {
                newLog.Description = string.Format("更新类别 {0}", category.CategoryName);
                //临时附件转换成分类的正式附件
                attachmentService.ToggleTemporaryAttachments(category.OwnerId, TenantTypeIds.Instance().Categorie(), category.CategoryId, new List<long> { category.ImageAttachmentId });
            }
            operationLogService.Create(newLog);
        }

        /// <summary>
        /// 更新类别内容项数量的事件
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <param name="eventArgs"></param>
        private void CategoryModuleForItemCount_BatchBefore(IEnumerable<long> categoryIds, CategoryEventArgs eventArgs)
        {
            lock (this)
            {
                //处理涉及分类的ItemCount计数
                foreach (long categoryId in categoryIds)
                {
                    Category category = categoryRepository.Get(categoryId);
                    //不是贴吧直接更新计数
                    if (category != null && category.TenantTypeId != TenantTypeIds.Instance().Bar())
                    {
                        if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
                        {
                            category.ItemCount--;
                            if (category.ItemCount < 0)
                            {
                                category.ItemCount = 0;
                            }
                            categoryRepository.UpdateItemCount(category);
                        }
                    }
                    else if (category != null && category.TenantTypeId == TenantTypeIds.Instance().Bar())
                    {
                        if (eventArgs.EventOperationType == EventOperationType.Instance().Delete())
                        {
                            if (sectionService.Get(eventArgs.ItemId).IsEnabled)
                            {
                                category.ItemCount--;
                                if (category.ItemCount < 0)
                                {
                                    category.ItemCount = 0;
                                }
                                categoryRepository.UpdateItemCount(category);
                            }

                        }

                    }
                }
            }



        }

        /// <summary>
        /// 更新类别内容项数量的事件
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <param name="eventArgs"></param>
        private void CategoryModuleForItemCount_BatchAfter(IEnumerable<long> categoryIds, CategoryEventArgs eventArgs)
        {
            lock (this)
            {
                //处理涉及分类的ItemCount计数
                if (categoryIds.Count() > 0)
                {
                    foreach (long categoryId in categoryIds)
                    {
                        Category category = categoryRepository.Get(categoryId);
                        //不是贴吧直接更新计数
                        if (category != null && category.TenantTypeId != TenantTypeIds.Instance().Bar())
                        {
                            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
                            {
                                category.ItemCount++;
                                categoryRepository.UpdateItemCount(category);
                            }
                        }

                        else if (category != null && category.TenantTypeId == TenantTypeIds.Instance().Bar())
                        {
                            if (eventArgs.EventOperationType == EventOperationType.Instance().Create())
                            {
                                if (sectionService.Get(eventArgs.ItemId).IsEnabled)
                                {
                                    category.ItemCount++;
                                    categoryRepository.UpdateItemCount(category);
                                }

                            }
                        }

                    }
                }
            }
        }

    }
}