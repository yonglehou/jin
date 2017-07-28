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
    /// 贴子仓储接口
    /// </summary>
    public interface IThreadRepository : IRepository<Thread>
    {
        /// <summary>
        /// 移动贴子
        /// </summary>
        /// <param name="threadId">要移动贴子的ThreadId</param>
        /// <param name="moveToSectionId">转移到贴吧的SectionId</param>
        void MoveThread(long threadId, long moveToSectionId);

        /// <summary>
        /// 删除用户记录（删除用户时使用）
        /// </summary>
        /// <param name="userId">被删除用户</param>
        /// <param name="takeOver">接管用户</param>
        /// <param name="takeOverAll">是否接管被删除用户的所有内容</param>
        void DeleteUser(long userId, User takeOver, bool takeOverAll);

        /// <summary>
        /// 获取某个贴吧下的所有贴子（用于删除贴子）
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        IEnumerable<Thread> GetAllThreadsOfSection(long sectionId);

        /// <summary>
        /// 获取某个用户的所有贴子（用于删除用户）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<Thread> GetAllThreadsOfUser(long userId);
        /// <summary>
        /// 获取某个拥有者时删除所有贴子（用于删除对象）
        /// </summary>
        /// <param name="OwnerId"></param>
        /// <returns></returns>
        IEnumerable<Thread> GetAllThreadsOfOwner(long OwnerId);
      

        /// <summary>
        /// 获取BarThread内容
        /// </summary>
        /// <param name="threadId"></param>
        string GetBody(long threadId);

        /// <summary>
        /// 获取解析过正文
        /// </summary>
        /// <returns></returns>
        string GetResolvedBody(long threadId);

        /// <summary>
        /// 获取用户的主题贴分页集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="ignoreAudit">是否忽略审核状态（作者或管理员查看时忽略审核状态）</param>
        /// <param name="isPosted">是否是取我回复过的</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns>主题贴列表</returns>
        PagingDataSet<Thread> GetUserThreads(string tenantTypeId, long userId, bool ignoreAudit, bool isPosted, int pageSize, int pageIndex, long? sectionId);

        /// <summary>
        /// 获取主题贴的排行数据
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">前多少条</param>
        /// <param name="sortBy">主题贴排序依据</param>
        /// <returns></returns>
        IEnumerable<Thread> GetTops(string tenantTypeId, int topNumber, bool? isEssential, SortBy_BarThread sortBy);

      

        /// <summary>
        /// 根据标签名获取主题贴排行分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="categoryId">主题贴分类Id</param>
        /// <param name="isEssential">是否为精华贴</param>
        /// <param name="sortBy">贴子排序依据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>主题贴列表</returns>
        PagingDataSet<Thread> Gets(string tenantTypeId, string tageName, bool? isEssential, SortBy_BarThread sortBy, int pageIndex);


       
        /// <summary>
        /// 根据贴吧获取主题贴分页集合
        /// </summary>
        /// <param name="sectionId">贴吧Id</param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="orderBySticky">是否为置顶贴</param>
        /// <param name="sortBy">贴子排序依据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>主题贴列表</returns>
        PagingDataSet<Thread> Gets(long sectionId, long? ownerId, long? categoryId, bool? orderBySticky, SortBy_BarThread sortBy, int pageSize, int pageIndex, SortBy_BarDateThread BarDate);

        /// <summary>
        /// 贴子管理时查询贴子分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="query">贴子查询条件</param>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴子分页集合</returns>
        PagingDataSet<Thread> Gets(string tenantTypeId, ThreadQuery query, int pageSize, int pageIndex);

        /// <summary>
        /// 帖子计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        int GetThreadCount(AuditStatus? approvalStatus, bool is24Hours);
      

    }
}
