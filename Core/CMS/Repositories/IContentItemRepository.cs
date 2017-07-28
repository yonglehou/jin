//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tunynet.Common;
using Tunynet.Repositories;

namespace Tunynet.CMS
{
    public interface IContentItemRepository : IRepository<ContentItem>
    {

        /// <summary>
        /// 依据查询条件获取ContentItem列表
        /// </summary>
        /// <param name="query">查询实体</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="pageIndex">第几页</param>
        /// <returns></returns>
        PagingDataSet<ContentItem> GetContentItems(ContentItemQuery query, int pageSize, int pageIndex);


        /// <summary>
        /// 获取上一篇
        /// </summary>
        /// <param name="contentItem">内容项</param>
        /// <returns>上一篇内容的主键ID</returns>
        long GetPrevContentItemId(ContentItem contentItem);

        /// <summary>
        /// 获取下一篇
        /// </summary>
        /// <param name="contentItem">内容项</param>
        /// <returns>下一篇内容的主键ID</returns>
        long GetNextContentItemId(ContentItem contentItem);

        ///// <summary>
        ///// 获取资讯应用统计数据
        ///// </summary>
        ///// <param name="modelId">模型ID</param>
        ///// <returns>资讯统计数据</returns>
        //Dictionary<string, long> GetApplicationStatisticData(string modelId);

        /// <summary>
        /// 获取ContentItem附表数据
        /// </summary>
        /// <param name="contentModelId">模型ID</param>
        /// <param name="contentItemId">内容项ID</param>
        /// <returns>ContentItem附表数据</returns>
        Dictionary<string, object> GetContentItemAdditionalProperties(long contentItemId);

        /// <summary>
        /// 批量移动ContentItem
        /// </summary>
        /// <param name="ContentItem">内容项集合</param>
        /// <param name="toCategoryId">栏目ID</param>
        void Move(IEnumerable<ContentItem> ContentItem, int toCategoryId);

        /// <summary>
        /// 获取前N条数据
        /// </summary>
        /// <param name="topNumber">数量</param>
        /// <param name="categoryId">栏目Id</param>
        /// <param name="includeCategoryDescendants">是否包含CategoryId的后代</param>
        /// <param name="minDate">最小时间</param>
        /// <param name="sortBy">排序</param>
        /// <returns></returns>
        IEnumerable<ContentItem> GetTopContentItems(int topNumber = 10, int? categoryId = null, bool? includeCategoryDescendants = true,  DateTime? minDate = null, ContentItemSortBy sortBy = ContentItemSortBy.DatePublished_Desc);



        /// <summary>
        /// 前台根据ModelKey分页获取ContentItem
        /// </summary>
        /// <param name="ModelKey">内容模型key</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="sortBy">排序</param>
        /// <returns></returns>
        PagingDataSet<ContentItem> GetContentItemsofModelKey(string ModelKey = "", int pageSize = 20, int pageIndex = 1, ContentItemSortBy? sortBy = ContentItemSortBy.DatePublished_Desc);
        /// <summary>
        /// 资讯计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        int GetContentItemCount(AuditStatus? approvalStatus, bool is24Hours);
       
        /// <summary>
        /// 前台根据ModelKey前topNumber条ContentItem
        /// </summary>
        /// <param name="topNumber">数量</param>
        /// <param name="ModelKey">内容模型key</param>
        /// <param name="minDate">最小时间</param>
        /// <param name="sortBy">排序</param>
        /// <returns></returns>
        IEnumerable<ContentItem> GetTopContentItemsofModelKey(int topNumber = 10, string ModelKey = "", DateTime? minDate = null, ContentItemSortBy? sortBy = ContentItemSortBy.DatePublished_Desc);


    }
}
