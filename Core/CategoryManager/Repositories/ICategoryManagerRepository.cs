//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Repositories;
using Tunynet;
using Tunynet.Common;
using Tunynet.Caching;

namespace Tunynet.Common
{
    /// <summary>
    /// 贴吧仓储接口
    /// </summary>
    public interface ICategoryManagerRepository : IRepository<CategoryManager>
    {
        /// <summary>
        /// 更新管理员列表
        /// </summary>
        /// <param name="categoryId">(栏目ID)贴吧Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="managerIds">管理员用户Id集合</param>
        /// <param name="referenceCategoryId">从那个栏目继承</param>
        void UpdateManagerIds(string tenantTypeId, long categoryId, IEnumerable<long> managerIds);
        /// <summary>
        /// 更新继承栏目管理员记录
        /// </summary>
        /// <param name="categoryId">(栏目ID)贴吧Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="managerIds">管理员用户Id集合</param>
        /// <param name="referenceCategoryId">从那个栏目继承</param>
        void UpdateReferenceCategoryId(string tenantTypeId, long categoryId, long referenceCategoryId);
        /// <summary>
        /// 获取管理员用户Id列表
        /// </summary>
        /// <param name="categoryId">栏目ID/贴吧Id</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="isReference">是否继承上级管理员列表</param>
        /// <returns>吧管理员用户Id列表</returns>
        IEnumerable<long> GetCategoryManagerIds(string tenantTypeId, long categoryId, out bool isReference);

        /// <summary>
        /// 获取用户是否是任一栏目管理员(后台进入时判断用)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="categoryId">栏目ID</param>
        /// <returns>吧管理员用户Id列表</returns>
        bool IsCategoryManager(string tenantTypeId, long userId, long? categoryId);
       
        /// <summary>
        /// 删除垃圾数据
        /// </summary>
        void DeleteTrashDatas();


        }
}