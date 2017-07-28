//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tunynet;
using Tunynet.Common;
using System.Drawing;
using System.IO;
using Tunynet.Events;
using Tunynet.FileStore;
using Tunynet.Imaging;

namespace Tunynet.Common
{
    /// <summary>
    /// 栏目贴吧管理员业务逻辑类
    /// </summary>
    public class CategoryManagerService
    {
        private ICategoryManagerRepository categoryManagerRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="barSectionRepository"></param>
        /// <param name="userService"></param>
        /// <param name="categoryService"></param>
        public CategoryManagerService(ICategoryManagerRepository categoryManagerRepository)
        {
            this.categoryManagerRepository = categoryManagerRepository;

        }

        /// <summary>
        /// 更新栏目贴吧管理员列表
        /// </summary>
        /// <param name="categoryId">(栏目ID)贴吧Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="managerIds">管理员用户Id集合</param>
        /// <param name="referenceCategoryId">从那个栏目继承</param>
        public void UpdateManagerIds(string tenantTypeId, long categoryId, IEnumerable<long> managerIds, long? referenceCategoryId = 0)
        {
            if (referenceCategoryId.Value > 0)
                categoryManagerRepository.UpdateReferenceCategoryId(tenantTypeId, categoryId, referenceCategoryId.Value);
            else
                categoryManagerRepository.UpdateManagerIds(tenantTypeId, categoryId, managerIds);
        }

        /// <summary>
        /// 获取栏目贴吧管理员用户Id列表
        /// </summary>
        /// <param name="categoryId">栏目ID/贴吧Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="isReference">是否继承上级管理员列表</param>
        /// <returns>吧管理员用户Id列表</returns>
        public IEnumerable<long> GetCategoryManagerIds(string tenantTypeId, long categoryId, out bool isReference)
        {
            return categoryManagerRepository.GetCategoryManagerIds(tenantTypeId, categoryId, out isReference);
        }

      
        /// <summary>
        /// 获取用户是否是任一栏目管理员(后台进入时判断用)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="categoryId">栏目ID</param>
        /// <returns>吧管理员用户Id列表</returns>
        public bool IsCategoryManager(string tenantTypeId, long userId, long? categoryId=null)
        {
            return categoryManagerRepository.IsCategoryManager(tenantTypeId, userId,categoryId);
        }
        /// <summary>
        /// 删除垃圾数据
        /// </summary>
        public void DeleteTrashDatas()
        {
            categoryManagerRepository.DeleteTrashDatas();
        }
    }
}