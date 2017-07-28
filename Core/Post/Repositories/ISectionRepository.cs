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

namespace Tunynet.Post
{
    /// <summary>
    /// 贴吧仓储接口
    /// </summary>
    public interface ISectionRepository : IRepository<Section>
    {


        /// <summary>
        /// 依据OwnerId获取单个贴吧（用于OwnerId与贴吧一对一关系）
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>贴吧</returns>
        Section GetByOwnerId(string tenantTypeId, long ownerId);

   

        /// <summary>
        /// 获取拥有者的贴吧列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="userId">吧主UserId</param>
        /// <returns>贴吧列表</returns>
        IEnumerable<Section> GetsByUserId(string tenantTypeId, long userId);

        /// <summary>
        /// 获取贴吧的排行数据
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">前多少条</param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="sortBy">贴吧排序依据</param>
        /// <returns></returns>
        IEnumerable<Section> GetTops(string tenantTypeId, int topNumber, long? categoryId, SortBy_BarSection sortBy);

        /// <summary>
        /// 获取贴吧列表
        /// </summary>
        /// <remarks>在频道贴吧分类页使用</remarks>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="nameKeyword"></param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="sortBy">贴吧排序依据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴吧列表</returns>
        PagingDataSet<Section> Gets(string tenantTypeId, string nameKeyword, long? categoryId, SortBy_BarSection sortBy, int pageIndex);

        /// <summary>
        /// 贴吧管理时查询贴吧分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="query">贴吧查询条件</param>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴吧分页集合</returns>
        PagingDataSet<Section> Gets(string tenantTypeId, SectionQuery query, int pageSize, int pageIndex);




    }
}