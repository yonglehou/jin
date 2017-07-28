//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Tunynet.Repositories;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 标签仓储接口，实现特殊方法
    /// </summary>
    public interface ITagRepository<T> : IRepository<T> where T : Tag
    {
        /// <summary>
        /// 获取标签实体
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        T Get(string tagName, string tenantTypeId);

        /// <summary>
        /// 获取前N个标签
        /// </summary>
        /// <remarks>智能提示时也使用该方法获取数据</remarks>
        ///<param name="tenantTypeId">租户类型Id</param>
        ///<param name="topNumber">前N条数据</param>
        ///<param name="isFeatured">是否为特色标签</param>
        ///<param name="sortBy">标签排序字段</param>
        ///<param name="isTagCloud">为true时则不启用缓存</param>
        IEnumerable<T> GetTopTags(string tenantTypeId, int topNumber, bool? isFeatured, SortBy_Tag? sortBy);

        /// <summary>
        /// 获取前N个标签
        /// </summary>
        /// <remarks>用于智能提示</remarks>
        ///<param name="tenantTypeId">租户类型Id</param>
        ///<param name="ownerId">拥有者Id</param>
        ///<param name="keyword">标签名称关键字</param>
        ///<param name="topNumber">前N条数据</param>
        IEnumerable<string> GetTopTagNames(string tenantTypeId,  string keyword, int topNumber);

        /// <summary>
        ///分页检索标签
        /// </summary>
        ///<param name="query">查询条件</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        PagingDataSet<T> GetTags(TagQuery query, int pageIndex, int pageSize);
        /// <summary>
        /// 标签内容数减1（删除资讯时使用）
        /// </summary>
        /// <param name="tagName"></param>
        void SetItemCount(string tagName);




    }
}