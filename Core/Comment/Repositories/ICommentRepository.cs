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

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// ICommentRepository接口
    /// </summary>
    public interface ICommentRepository : IRepository<Comment>
    {
        #region   Delete & Update

        /// <summary>
        /// 删除评论 
        /// </summary>
        /// <param name="id">评论Id</param>
        int Delete(long id);

        /// <summary>
        /// 删除被评论对象的所有评论
        /// </summary>
        /// <remarks>
        /// 供被评论对象删除时调用
        /// </remarks>
        /// <param name="commentedObjectId"></param>
        /// <returns></returns>
        int DeleteCommentedObjectComments(long commentedObjectId, string tenantTypeId);

        /// <summary>
        ///  删除用户发布的评论
        /// </summary>
        /// <remarks>
        /// 供用户删除时处理用户相关信息时调用
        /// </remarks>
        /// <param name="userId">UserId</param>
        /// <param name="reserveCommnetsAsAnonymous">true=保留用户发布的评论，但是修改为匿名用户；false=直接删除评论</param>
        /// <returns></returns>
        int DeleteUserComments(long userId, bool reserveCommnetsAsAnonymous);

        #endregion


        #region  Get

        /// <summary>
        /// 获取被评论对象的所有评论（用于删除被评论对象时的积分处理）
        /// </summary>
        /// <param name="commentedObjectId">被评论对象ID</param>
        /// <returns></returns>
        IEnumerable<Comment> GetCommentedObjectComments(long commentedObjectId, string tenantTypeId);


        /// <summary>
        /// 获取顶级评论列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="sortBy">排序字段</param> 
        /// <returns></returns>
        PagingDataSet<Comment> GetRootComments(string tenantTypeId, long commentedObjectId, int pageIndex, SortBy_Comment sortBy);

        /// <summary>
        /// 获取子级评论列表
        /// </summary>
        /// <param name="parentId">父评论Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageIndex">个数</param>
        /// <param name="sortBy">排序字段</param> 
        /// <param name="includeCommentDescendants">是否包含所有后代评论</param>
        /// <returns></returns>
        PagingDataSet<Comment> GetChildren(long parentId, int pageIndex, int pageSize, SortBy_Comment sortBy, bool includeCommentDescendants);

        /// <summary>
        /// 获取拥有者的评论
        /// </summary>
        /// <param name="ownerId">评论拥有者Id</param>
        /// <param name="userId">评论发布人UserId</param>
        /// <param name="tenantTypeId">租户类型Id（如果为null，则获取该拥有者所有评论）</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        PagingDataSet<Comment> GetUserComments(long? ownerId, long? userId, string tenantTypeId, DateTime? startDate, DateTime? endDate, int pageIndex);

        /// <summary>
        /// 获取前topNumber条评论
        /// </summary>
        /// <param name="ownerId">评论拥有者Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">获取的评论数量</param>
        /// <param name="sortBy">排序字段</param>
        /// <returns></returns>
        IEnumerable<Comment> GetTopComments(long ownerId, string tenantTypeId, int topNumber, SortBy_Comment sortBy);

        /// <summary>
        /// 获取对象的所有评论列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="sortBy">排序字段</param>
        /// <returns></returns>
        PagingDataSet<Comment> GetObjectComments(string tenantTypeId, long commentedObjectId, int pageIndex, SortBy_Comment sortBy, bool? IsPrivate, long? UserId, long? parentId);
        /// <summary>
        /// 根据贴吧获取评论
        /// </summary>
        /// <param name="sectionId">贴吧Id</param>
        /// <param name="auditStatus">审核状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        PagingDataSet<Comment> GetSectionComments(long sectionId, AuditStatus? auditStatus, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex);
        /// <summary>
        /// 查询用户评论
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="publiclyAuditStatus">审核状态</param>
        /// <param name="userId">评论发布人UserId</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        PagingDataSet<Comment> GetComments(string tenantTypeId, PubliclyAuditStatus? publiclyAuditStatus, long? userId, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex);
        /// <summary>
        /// 查询用户评论
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="userId">评论发布人UserId</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        PagingDataSet<Comment> GetUserComments(string tenantTypeId, long? userId, long? parentId, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex);

        #endregion

        /// <summary>
        /// 评论计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        int GetCommentCount(AuditStatus? approvalStatus, bool is24Hours);
    


        /// <summary>
        /// 获取解析后的内容
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns></returns>
        string GetResolvedBody(long id);

    }
}
