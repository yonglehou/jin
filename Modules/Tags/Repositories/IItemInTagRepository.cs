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
    /// 标签和内容关联项，需要的数据服务接口
    /// </summary>
    public interface IItemInTagRepository : IRepository<ItemInTag>
    {
        /// <summary>
        /// 为多个内容项添加相同标签
        /// </summary>
        /// <param name="itemIds">内容项Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="tagName">标签名</param>
        int AddItemsToTag(IEnumerable<long> itemIds, string tenantTypeId ,string tagName);

        /// <summary>
        /// 为内容项批量设置标签
        /// </summary>
        /// <param name="tagNames">标签名称集合</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="itemId">内容项Id</param>
        int AddTagsToItem(string[] tagNames, string tenantTypeId,  long itemId);

      

        /// <summary>
        /// 清除内容项的所有标签
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="itemId">内容项Id</param>
        int ClearTagsFromItem(long itemId, string tenantTypeId);

        /// <summary>
        /// 获取标签的内容项集合
        /// </summary>
        /// <param name="tagName">标签名称</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>返回指定的内容项Id集合</returns>
        IEnumerable<long> GetTagOfItemIds(string tagName, string tenantTypeId);

        ///// <summary>
        ///// 获取标签的内容项集合
        ///// </summary>
        ///// <param name="tagName">标签名称</param>
        ///// <param name="tenantTypeId">租户id</param>
        ///// <param name="pageSize">页数</param>
        ///// <param name="pageIndex">页码</param>
        ///// <returns></returns>
        PagingDataSet<ItemInTag> GetItemIds(string tagName, string tenantTypeId, int pageSize, int pageIndex);

        /// <summary>
        ///  获取多个标签的内容项集合
        /// </summary>
        /// <param name="tagNames">多个名称</param>
        /// <param name="tenantTypeId">租户id</param>
        /// <param name="pageSize">页数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        PagingDataSet<ItemInTag> GetItemIds(IEnumerable<string> tagNames, string tenantTypeId, int pageSize, int pageIndex);

        /// <summary>
        /// 获取内容项的所有标签
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <returns>返回内容项的标签Id集合,无返回时返回空集合</returns>
        IEnumerable<long> GetTagIdsOfItem(long itemId, string tenantTypeId);

        ///// <summary>
        ///// 获取标签标签与拥有者关系Id集合
        ///// </summary>
        ///// <param name="itemId">内容项Id</param>
        ///// <param name="ownerId">拥有者Id</param>
        ///// <param name="tenantTypeId">租户类型Id</param>
        //IEnumerable<long> GetTagInOwnerIdsOfItem(long itemId, long ownerId, string tenantTypeId);


        /// <summary>
        /// 获取内容项与标签关联Id集合
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <param name="tenantTypeId">租户类型</param>
        /// <returns>返回内容项的标签Id集合,无返回时返回空集合</returns>
        IEnumerable<long> GetItemInTagIdsOfItem(long itemId, string tenantTypeId);

        /// <summary>
        /// 根据用户ID列表获取用户tag，本方法现用于用户搜索功能的索引生成
        /// </summary>
        /// <param name="userIds">用户ID列表</param>
        /// <returns></returns>
        IEnumerable<dynamic> GetTagNamesOfUsers(IEnumerable<long> userIds);

        /// <summary> 
        /// 根据用户ID列表获取ItemInTag的ID列表，本方法现用于用户搜索功能的索引生成
        /// </summary>
        /// <param name="userIds">用户ID列表</param>
        /// <returns>ItemInTag的ID列表</returns>
        IEnumerable<long> GetEntityIdsByUserIds(IEnumerable<long> userIds);

        /// <summary>
        /// 根据成员获取标签名及标签Id
        /// </summary>
        /// <param name="itemId">成员Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="tagInOwnerId">标签与拥有者关联Id</param>
        /// <returns></returns>
        Dictionary<string, long> GetTagNamesWithIdsOfItem(long itemId, string tenantTypeId);
       
    }
}