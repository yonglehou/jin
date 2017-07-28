//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Events;
using Tunynet.Repositories;
using Tunynet.Common.Repositories;
using System.Text.RegularExpressions;

namespace Tunynet.Common
{
    /// <summary>
    /// 评论业务逻辑
    /// </summary>
    public class CommentService
    {
        //Comment Repository
        private ICommentRepository commentRepository;
        private ICommentBodyProcessor commentBodyProcessor;
        private AuditService auditService;
        /// <summary>
        /// 可设置repository的构造函数（主要用于测试用例）
        /// </summary>
        public CommentService(ICommentRepository commentRepository, ICommentBodyProcessor commentBodyProcessor, AuditService auditService)
        {
            this.commentRepository = commentRepository;
            this.commentBodyProcessor = commentBodyProcessor;
            this.auditService = auditService;
        }


        #region Create & Delete & Update

        public void Update(Comment comment)
        {
            commentRepository.Update(comment);
        }

        /// <summary>
        /// 创建评论
        /// </summary>
        /// <param name="comment">待创建评论</param>
        /// <param name="isManager">是否有权限管理</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public bool Create(Comment comment)
        {
            //获取评论用户的权限并发送
            ICommentUrlGetter urlGetter = CommentUrlGetterFactory.Get(comment.TenantTypeId);
            //触发事件
            EventBus<Comment>.Instance().OnBefore(comment, new CommonEventArgs(EventOperationType.Instance().Create()));
            //审核
            auditService.ChangeAuditStatusForCreate(comment.UserId, comment, urlGetter.IsManager(comment.UserId));
            //评论创建
            commentRepository.Insert(comment);


            comment.Body = commentBodyProcessor.Process(comment.Body, TenantTypeIds.Instance().Comment(), comment.Id, comment.UserId);
            commentRepository.Update(comment);

            //触发事件
            EventBus<Comment>.Instance().OnAfter(comment, new CommonEventArgs(EventOperationType.Instance().Create()));
            EventBus<Comment, AuditEventArgs>.Instance().OnAfter(comment, new AuditEventArgs(null, comment.ApprovalStatus, EventOperationType.Instance().Create()));
            EventBus<Comment, CommonEventArgs>.Instance().OnAfter(comment, new CommonEventArgs(EventOperationType.Instance().Create()));


            return comment.Id > 0;
        }

        /// <summary>
        /// 删除评论 
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool Delete(long id)
        {
            Comment comment = commentRepository.Get(id);
            int count = 0;
            if (comment != null)
            {
                //触发事件
                EventBus<Comment>.Instance().OnAfter(comment, new CommonEventArgs(EventOperationType.Instance().Delete()));
                EventBus<Comment, AuditEventArgs>.Instance().OnAfter(comment, new AuditEventArgs(comment.ApprovalStatus, null, EventOperationType.Instance().Delete()));
                EventBus<Comment, CommonEventArgs>.Instance().OnAfter(comment, new CommonEventArgs(EventOperationType.Instance().Delete()));
                count = commentRepository.Delete(id);
            }
            return count > 0;
        }

        /// <summary>
        /// 批量删除评论
        /// </summary>
        /// <param name="ids">待删除的评论Id列表</param>
        /// <returns>返回删除的评论数量</returns>
        public int Delete(IEnumerable<long> ids)
        {
            int commentCount = 0;
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    Delete(id);
                    commentCount++;
                }
            }
            return commentCount;
        }


        /// <summary>
        /// 删除被评论对象的所有评论
        /// </summary>
        /// <remarks>
        /// 供被评论对象删除时调用
        /// </remarks>
        /// <param name="commentedObjectId"></param>
        /// <returns></returns>
        public int DeleteCommentedObjectComments(long commentedObjectId, string tenantTypeId)
        {
            //积分操作
            var commentList = commentRepository.GetCommentedObjectComments(commentedObjectId, tenantTypeId);
            foreach (var comment in commentList)
            {
                EventBus<Comment>.Instance().OnAfter(comment, new CommonEventArgs(EventOperationType.Instance().Delete()));
                EventBus<Comment, AuditEventArgs>.Instance().OnAfter(comment, new AuditEventArgs(comment.ApprovalStatus, null, EventOperationType.Instance().Delete()));
            }
            return commentRepository.DeleteCommentedObjectComments(commentedObjectId, tenantTypeId);
        }

        /// <summary>
        ///  删除用户发布的评论
        /// </summary>
        /// <remarks>
        /// 供用户删除时处理用户相关信息时调用
        /// </remarks>
        /// <param name="userId">UserId</param>
        /// <param name="reserveCommnetsAsAnonymous">true=保留用户发布的评论，但是修改为匿名用户；false=直接删除评论</param>
        /// <returns></returns>
        public int DeleteUserComments(long userId, bool reserveCommnetsAsAnonymous)
        {
            return commentRepository.DeleteUserComments(userId, reserveCommnetsAsAnonymous);
        }

        #endregion

        #region 列表

        /// <summary>
        /// 获取单个评论实体
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns>评论</returns>
        public Comment Get(long id)
        {
            return commentRepository.Get(id);
        }

        /// <summary>
        /// 获取集合评论
        /// </summary>
        /// <param name="commentIds"></param>
        /// <returns></returns>
        public IEnumerable<Comment> Gets(IEnumerable<long> commentIds)
        {
            return commentRepository.PopulateEntitiesByEntityIds(commentIds);
        }


        /// <summary>
        /// 获取顶级评论列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="sortBy">排序字段</param> 
        /// <returns></returns>
        public PagingDataSet<Comment> GetRootComments(string tenantTypeId, long commentedObjectId, int pageIndex, SortBy_Comment sortBy)
        {
            //排序：Id正序
            //缓存分区：CommentedObjectId
            //仅显示可公开对外显示的 PubliclyAuditStatus 

            return commentRepository.GetRootComments(tenantTypeId, commentedObjectId, pageIndex, sortBy);
        }

        /// <summary>
        /// 获取评论对象的所有评论 
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="sortBy">排序字段</param> 
        /// <returns></returns>
        public PagingDataSet<Comment> GetObjectComments(string tenantTypeId, long commentedObjectId, int pageIndex, SortBy_Comment sortBy, bool? IsPrivate = false, long? UserId = null, long? parentId = null)
        {
            return commentRepository.GetObjectComments(tenantTypeId, commentedObjectId, pageIndex, sortBy, IsPrivate, UserId, parentId);
        }
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
        public PagingDataSet<Comment> GetSectionComments(long sectionId, AuditStatus? auditStatus, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            return commentRepository.GetSectionComments(sectionId, auditStatus, startDate, endDate, pageSize, pageIndex);
        }
        /// <summary>
        /// 获取子级评论列表
        /// </summary>
        /// <param name="parentId">父评论Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">个数</param>
        /// <param name="sortBy">排序字段</param> 
        /// <param name="IncludeCommentDescendants">是否包含所有后代评论</param> 
        /// <returns></returns>
        public PagingDataSet<Comment> GetChildren(long parentId, int pageIndex, int pageSize, SortBy_Comment sortBy, bool IncludeCommentDescendants = true)
        {
            //排序：Id正序
            //缓存分区：ParentId
            //仅显示可公开对外显示的 PubliclyAuditStatus 
            return commentRepository.GetChildren(parentId, pageIndex, pageSize, sortBy, IncludeCommentDescendants);
        }

        /// <summary>
        /// 获取拥有者的评论
        /// </summary>
        /// <param name="ownerId">评论拥有者Id</param>
        /// <param name="tenantTypeId">租户类型Id（如果为null，则获取该拥有者所有评论）</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="isMobileClient">是否是移动客户端</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetOwnerComments(long ownerId, string tenantTypeId, DateTime? startDate, DateTime? endDate, int pageIndex, bool? isMobileClient = false)
        {
            //排序：Id倒序
            //缓存分区：OwnerId
            return commentRepository.GetUserComments(ownerId, null, tenantTypeId, startDate, endDate, pageIndex);
        }

        /// <summary>
        /// 获取用户发布的评论
        /// </summary>
        /// <param name="userId">评论发布人UserId</param>
        /// <param name="tenantTypeId">租户类型Id（如果为null，则获取该拥有者所有评论）</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetUserComments(long userId, string tenantTypeId, DateTime? startDate, DateTime? endDate, int pageIndex)
        {
            //排序：Id倒序
            //缓存分区：UserId
            return commentRepository.GetUserComments(null, userId, tenantTypeId, startDate, endDate, pageIndex);
        }

        /// <summary>
        /// 获取一个评论的所有引用评论
        /// </summary>
        /// <param name="commentId">评论Id</param>
        /// <param name="Isstretch">是否展开隐藏</param>
        /// <returns></returns>
        public List<Comment> GetParentComments(long commentId, bool Isstretch)
        {
            List<Comment> comments = new List<Comment>();
            var comment = commentRepository.Get(commentId);
            var ParentIds = Regex.Split(comment.ParentIds, ",", RegexOptions.IgnoreCase);
            //如果 超过10个 并且 没有展开隐藏 默认只取四个
            if (ParentIds.Length > 11 && !Isstretch)
            {
                for (int i = 1; i < 4; i++)
                {
                    comments.Add(commentRepository.Get(ParentIds[i]));
                }
                comments.Add(commentRepository.Get(ParentIds[ParentIds.Length - 1]));
            }
            else
            {
                for (int i = 1; i < ParentIds.Length; i++)
                {
                    comments.Add(commentRepository.Get(ParentIds[i]));
                }
            }

            return comments;
        }
        /// <summary>
        /// 更新审核状态
        /// </summary>
        /// <param name="commentId">待被更新的评论Id</param>
        /// <param name="isApproved">是否通过审核</param>
        public void UpdateAuditStatus(long commentId, bool isApproved)
        {
            var comment = commentRepository.Get(commentId);
            AuditStatus auditStatus = isApproved ? AuditStatus.Success : AuditStatus.Fail;
            if (comment.ApprovalStatus == auditStatus)
                return;
            AuditStatus oldAuditStatus = comment.ApprovalStatus;
            comment.ApprovalStatus = auditStatus;
            commentRepository.Update(comment);

            //待审核通过审核增加积分
            EventBus<Comment>.Instance().OnAfter(comment, new CommonEventArgs(EventOperationType.Instance().Update()));
            EventBus<Comment, AuditEventArgs>.Instance().OnAfter(comment, new AuditEventArgs(oldAuditStatus, auditStatus, isApproved ? EventOperationType.Instance().Approved() : EventOperationType.Instance().Disapproved()));
            EventBus<Comment, CommonEventArgs>.Instance().OnAfter(comment, new CommonEventArgs(isApproved ? EventOperationType.Instance().Approved() : EventOperationType.Instance().Disapproved()));

        }

        /// <summary>
        /// 获取前topNumber条评论
        /// </summary>
        ///<param name="ownerId">评论拥有者Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">获取的评论数量</param>
        /// <param name="sortBy">排序字段</param>
        /// <returns></returns>
        public IEnumerable<Comment> GetTopComments(long ownerId, string tenantTypeId, int topNumber, SortBy_Comment sortBy = SortBy_Comment.DateCreated)
        {
            return commentRepository.GetTopComments(ownerId, tenantTypeId, topNumber, sortBy);
        }

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
        ///  删除了审核参数PubliclyAuditStatus? publiclyAuditStatus, 
        public PagingDataSet<Comment> GetComments(string tenantTypeId, PubliclyAuditStatus? publiclyAuditStatus, long? userId, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            //排序：Id倒序
            //缓存分区：全局版本
            return commentRepository.GetComments(tenantTypeId, publiclyAuditStatus, userId, startDate, endDate, pageSize, pageIndex);
        }

        /// <summary>
        /// 查询用户评论
        /// </summary>
        /// <param name="publiclyAuditStatus">审核状态</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="userId">评论发布人UserId</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        ///  删除了审核参数PubliclyAuditStatus? publiclyAuditStatus, 
        public PagingDataSet<Comment> GetUserComments(string tenantTypeId, long? userId, long? parentId, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            //排序：Id倒序
            //缓存分区：全局版本
            return commentRepository.GetUserComments(tenantTypeId, userId, parentId, startDate, endDate, pageSize, pageIndex);
        }

        /// <summary>
        /// 评论计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        public int GetCommentCount(AuditStatus? approvalStatus = null, bool is24Hours = false)
        {
            return commentRepository.GetCommentCount(approvalStatus, is24Hours);
        }


        #endregion

    }
}
