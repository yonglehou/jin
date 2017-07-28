//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    public interface ISpecialContentItemRepository : IRepository<SpecialContentItem>
    {

        /// <summary>
        /// 获取某推荐类型下的前几条
        /// </summary>
        /// <param name="topNumber">前几条</param>
        /// <param name="typeId">推荐类型的ID</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="isDisplayOrderDesc">是否倒序</param>
        /// <returns></returns>
        IEnumerable<SpecialContentItem> GetTops(int topNumber, int typeId, string tenantTypeId, bool isDisplayOrderDesc);
        /// <summary>
        /// 获取分页下的推荐类型的的所有推荐内容
        /// </summary>
        /// <param name="typeId">推荐类型的ID</param>
        /// <param name="tenantTypeId">推荐内容租户Id</param>
        /// <param name="pageSize">数量</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        PagingDataSet<SpecialContentItem> Gets(int typeId, string tenantTypeId, int pageSize, int pageIndex);
        /// <summary>
        /// 根据租户ID和推荐类别去获取所有的内容
        /// </summary>
        /// <param name="typeId">推荐类型的ID</param>
        /// <param name="tenantTypeId">租户Id</param>
        ///  <param name="isDisplayOrderDesc">是否倒序</param>
        /// <returns></returns>
        IEnumerable<long> GetItemIds(string tenantTypeId, int typeId, bool isDisplayOrderDesc = false);

        /// <summary>
        /// 获取特殊内容
        /// </summary>
        SpecialContentItem GetItem(long itemId, string tenantTypeId, int typeId);

        /// <summary>
        /// 获取某内容项下的所有推荐内容
        /// </summary>
        /// <param name="itemId">内容Id</param>
        /// <returns>返回所有推荐内容</returns>
        IEnumerable<SpecialContentItem> GetItems(long itemId, string tenantTypeId);
        /// <summary>
        /// 定期移除过期的推荐内容 
        /// </summary>
        void DeleteExpiredRecommendItems();

        /// <summary>
        /// 取消特殊推荐
        /// </summary>
        void UnStick(long itemId, string tenantTypeId, int typeId);



        /// <summary>
        /// 根据内容删除推荐（用于内容删除）
        /// </summary>
        void DeleteSpecialContentItem(long itemId, string tenantTypeId);




    }
}
