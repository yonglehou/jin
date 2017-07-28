//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Tunynet.Common;
using Tunynet.Events;
using Tunynet.Repositories;
using Tunynet.Settings;

namespace Tunynet.CMS
{
    /// <summary>
    /// ContentItem 业务逻辑
    /// </summary>
    public class ContentItemService
    {
        private IContentItemRepository contentItemRepository;
        private IRepository<ContentCategory> contentCategoryrepository = new Repository<ContentCategory>();
        private IKvStore ikvstore;
        private AuditService auditService;
        private SiteSettings siteSetting;
        /// <summary>
        /// 构造器
        /// </summary>
        public ContentItemService(IContentItemRepository contentItemRepository, IRepository<ContentCategory> contentCategoryrepository, IKvStore ikvstore,
         AuditService auditService, ISettingsManager<SiteSettings> siteSettings)
        {
            this.contentItemRepository = contentItemRepository;
            this.contentCategoryrepository = contentCategoryrepository;
            this.ikvstore = ikvstore;
            this.auditService = auditService;
            this.siteSetting = siteSettings.Get();
        }

        /// <summary>
        /// 创建ContentItem
        /// </summary>
        /// <param name="contentItem">ContentItem</param>
        /// <param name="tenantTypeId">租户ID</param>
        ///  <param name="isManager">是否有权限管理</param>
        /// <param name="isMobile">是手机端</param>
        public void Create(ContentItem contentItem, string tenantTypeId, bool isManager = false, bool isMobile = false)
        {
            //执行事件
            EventBus<ContentItem>.Instance().OnBefore(contentItem, new CommonEventArgs(EventOperationType.Instance().Create()));
            if (contentItem.ApprovalStatus != 0)
                auditService.ChangeAuditStatusForCreate(contentItem.UserId, contentItem, isManager);

            contentItemRepository.Insert(contentItem);


            IEnumerable<Attachment> attachments = new AttachmentService(tenantTypeId).GetsByAssociateId(contentItem.ContentItemId);

            //执行事件(fanggm)
            EventBus<ContentItem>.Instance().OnAfter(contentItem, new CommonEventArgs(EventOperationType.Instance().Update(), 0));

            //执行事件  
            EventBus<ContentItem, AttachmentEventArgs>.Instance().OnAfter(contentItem, new AttachmentEventArgs(EventOperationType.Instance().Create(), tenantTypeId, isMobile));
            //添加操作日志
            EventBus<ContentItem, CommonEventArgs>.Instance().OnAfter(contentItem, new CommonEventArgs(EventOperationType.Instance().Create()));
            EventBus<ContentItem, AuditEventArgs>.Instance().OnAfter(contentItem, new AuditEventArgs(null, contentItem.ApprovalStatus, EventOperationType.Instance().Create()));
        }

        /// <summary>
        /// 更新ContentItem
        /// </summary>
        /// <param name="contentItem">ContentItem</param>
        ///  <param name="isManager">是否有权限管理</param>
        public void Update(ContentItem contentItem, string tenantTypeId, bool isManager = false)
        {
            //执行事件
            EventBus<ContentItem>.Instance().OnBefore(contentItem, new CommonEventArgs(EventOperationType.Instance().Update()));
            if (contentItem.ApprovalStatus != 0 && contentItem.IsDraft == false)
                auditService.ChangeAuditStatusForUpdate(contentItem.UserId, contentItem, isManager);
            if (contentItem.ApprovalStatus != 0 && contentItem.IsDraft == true)
                auditService.ChangeAuditStatusForCreate(contentItem.UserId, contentItem, isManager);

            contentItemRepository.Update(contentItem);

            //执行事件  
            EventBus<ContentItem, AttachmentEventArgs>.Instance().OnAfter(contentItem, new AttachmentEventArgs(EventOperationType.Instance().Update(), tenantTypeId));
            EventBus<ContentItem, AuditEventArgs>.Instance().OnAfter(contentItem, new AuditEventArgs(null, contentItem.ApprovalStatus, EventOperationType.Instance().Update()));
            //添加操作日志
            EventBus<ContentItem, CommonEventArgs>.Instance().OnAfter(contentItem, new CommonEventArgs(EventOperationType.Instance().Update()));

        }

        /// <summary>
        /// 删除ContentItem
        /// </summary>
        /// <param name="contentItem">ContentItem</param>
        public void Delete(ContentItem contentItem)
        {
            if (contentItem != null)
            {
                //执行事件
                EventBus<ContentItem>.Instance().OnBefore(contentItem, new CommonEventArgs(EventOperationType.Instance().Delete()));
                contentItemRepository.Delete(contentItem);
                //执行事件
                EventBus<ContentItem, AttachmentEventArgs>.Instance().OnAfter(contentItem, new AttachmentEventArgs(EventOperationType.Instance().Delete(),null));
                EventBus<ContentItem>.Instance().OnAfter(contentItem, new CommonEventArgs(EventOperationType.Instance().Delete(), 0));
                EventBus<ContentItem, AuditEventArgs>.Instance().OnAfter(contentItem, new AuditEventArgs(contentItem.ApprovalStatus, null, EventOperationType.Instance().Delete()));
                //添加操作日志
                EventBus<ContentItem, CommonEventArgs>.Instance().OnAfter(contentItem, new CommonEventArgs(EventOperationType.Instance().Delete()));

            }
        }

        /// <summary>
        /// 批量移动ContentItem
        /// </summary>
        /// <param name="contentItemIds">需要移动的内容项ID</param>
        /// <param name="toContentCategoryId">移动到的栏目ID</param>
        public void MoveContentItemToCategory(IEnumerable<long> contentItemIds, int toContentCategoryId)
        {
            ContentCategory toContentCategory = contentCategoryrepository.Get(toContentCategoryId);
            if (toContentCategory == null)
                return;

            contentItemIds = contentItemIds.Distinct();
            IEnumerable<ContentItem> contentItemForMove = contentItemRepository.PopulateEntitiesByEntityIds(contentItemIds).Where(c => c.ContentCategoryId != toContentCategoryId);

            contentItemRepository.Move(contentItemForMove, toContentCategoryId);
            foreach (var contentItem in contentItemForMove)
            {
                //执行事件
                EventBus<ContentItem>.Instance().OnAfter(contentItem, new CommonEventArgs(EventOperationType.Instance().Update(), 0));
            }
        }

        /// <summary>
        /// 删除用户时处理所有资讯相关的数据（删除用户时使用）
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <remarks>接管被删除用户的所有内容</remarks>
        public void DeleteContentItemOfUser(long userId)
        {
            int pageSize = 100;     //批量删除，每次删100条
            int pageIndex = 1;
            int pageCount = 1;
            do
            {
                PagingDataSet<ContentItem> ContentItem = GetContentItems(null, null, userId, pageSize, pageIndex);
                foreach (ContentItem contentItem in ContentItem)
                {
                    Delete(contentItem);
                }
                pageCount = ContentItem.PageCount;
                pageIndex++;
            } while (pageIndex <= pageCount);
        }

        /// <summary>
        /// 获取ContentItem
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns></returns>
        public ContentItem Get(long contentItemId)
        {
            return contentItemRepository.Get(contentItemId);
        }

        /// <summary>
        /// 获取集合资讯
        /// </summary>
        /// <param name="contentItemIds"></param>
        /// <returns></returns>
        public IEnumerable<ContentItem> Gets(IEnumerable<long> contentItemIds)
        {
            var contentItems = contentItemRepository.PopulateEntitiesByEntityIds(contentItemIds);
            if (siteSetting.AuditStatus == PubliclyAuditStatus.Success)
                return contentItems.Where(n => n.ApprovalStatus == AuditStatus.Success);
            else if (siteSetting.AuditStatus == PubliclyAuditStatus.Again)
                return contentItems.Where(n => n.ApprovalStatus > AuditStatus.Pending);
            else if (siteSetting.AuditStatus == PubliclyAuditStatus.Pending)
                return contentItems.Where(n => n.ApprovalStatus > AuditStatus.Fail);
            return contentItems;
        }

        /// <summary>
        /// 依据查询条件获取ContentItem(后台管理员使用)
        /// </summary>
        /// <param name="subjectKeyword">关键字</param>
        /// <param name="categoryId">栏目ID</param>
        /// <param name="includeCategoryDescendants">是否包含CategoryId的后代</param>
        /// <param name="contentModelId">内容模型ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="auditStatus">审核状态</param>
        /// <param name="minDate">最小时间</param>
        /// <param name="maxDate">最大时间</param>
        /// <param name="pageSize">个数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagingDataSet<ContentItem> GetContentItemForAdmin(string subjectKeyword = null, int? categoryId = null, bool? includeCategoryDescendants = true, int? contentModelId = null, long? userId = null, AuditStatus? auditStatus = null, DateTime? minDate = null, DateTime? maxDate = null, bool? orderBySticky = true, int pageSize = 15, int pageIndex = 1)
        {
            ContentItemQuery query = new ContentItemQuery()
            {
                SubjectKeyword = subjectKeyword,
                CategoryId = categoryId,
                IncludeCategoryDescendants = includeCategoryDescendants,
                ContentModelId = contentModelId,
                UserId = userId,
                MinDate = minDate,
                MaxDate = maxDate,
                OrderBySticky = orderBySticky.Value,
                SortBy = ContentItemSortBy.DatePublished_Desc,
                AuditStatus = auditStatus,
                IsAdmin = true
            };
            return contentItemRepository.GetContentItems(query, pageSize, pageIndex);
        }

        /// <summary>
        /// 分页获取ContentItem
        /// </summary>
        /// <param name="categoryId">栏目ID</param>
        /// <param name="includeCategoryDescendants">是否包含所有后代栏目资讯</param>
        /// <param name="userId">用户userid</param>
        /// <param name="pageSize">显示个数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="orderBySticky">是否置顶排序</param>
        /// <param name="sortBy">排序方式</param>
        /// <param name="IsContextUser">是否当前用户-用于获取草稿</param>
        /// <param name="contentModelId">内容模型ID</param>
        /// <param name="auditStatus">审核状态</param>
        /// <returns></returns>
        public PagingDataSet<ContentItem> GetContentItems(int? categoryId = null, bool? includeCategoryDescendants = true, long? userId = null, int pageSize = 20, int pageIndex = 1, bool? orderBySticky = true, ContentItemSortBy? sortBy = ContentItemSortBy.DatePublished_Desc, bool isContextUser = false, int? contentModelId = null, AuditStatus? auditStatus = null)
        {
            ContentItemQuery query = new ContentItemQuery()
            {
                CategoryId = categoryId,
                IncludeCategoryDescendants = includeCategoryDescendants,
                UserId = userId,
                OrderBySticky = orderBySticky.Value,
                SortBy = sortBy.Value,
                IsAdmin = false,
                IsContextUser = isContextUser,
                AuditStatus = auditStatus,
                ContentModelId = contentModelId,
            };
            return contentItemRepository.GetContentItems(query, pageSize, pageIndex);
        }

        /// <summary>
        /// 获取前topNumber条ContentItem
        /// </summary>
        /// <param name="topNumber">前几条</param>
        /// <param name="categoryId">栏目ID</param>
        /// <param name="includeCategoryDescendants">是否包含所有后代栏目资讯</param>
        /// <param name="minDate">最小时间</param>
        /// <param name="sortBy">排序方式</param>
        /// <returns>前多少条的数据</returns>
        public IEnumerable<ContentItem> GetTopContentItems(int topNumber = 10, int? categoryId = null, bool? includeCategoryDescendants = true, DateTime? minDate = null, ContentItemSortBy sortBy = ContentItemSortBy.DatePublished_Desc)
        {
            return contentItemRepository.GetTopContentItems(topNumber, categoryId, includeCategoryDescendants, minDate, sortBy);
        }

        /// <summary>
        /// 前台根据ModelKey分页获取ContentItem
        /// </summary>
        /// <param name="ModelKey">内容模型key</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="sortBy">排序</param>
        /// <returns></returns>
        public PagingDataSet<ContentItem> GetContentItemsofModelKey(string ModelKey = "", int pageSize = 20, int pageIndex = 1, ContentItemSortBy? sortBy = ContentItemSortBy.DatePublished_Desc)
        {
            return contentItemRepository.GetContentItemsofModelKey(ModelKey, pageSize, pageIndex);
        }

        /// <summary>
        /// 前台根据ModelKey前topNumber条ContentItem
        /// </summary>
        /// <param name="topNumber">数量</param>
        /// <param name="ModelKey">内容模型key</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="minDate">最小时间</param>
        /// <param name="sortBy">排序</param>
        /// <returns></returns>
        public IEnumerable<ContentItem> GetTopContentItemsofModelKey(int topNumber = 10, string ModelKey = "", DateTime? minDate = null, ContentItemSortBy? sortBy = ContentItemSortBy.DatePublished_Desc)
        {
            return contentItemRepository.GetTopContentItemsofModelKey(topNumber, ModelKey, minDate, sortBy);
        }

        /// <summary>
        /// 资讯计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        public int GetContentItemCount(AuditStatus? approvalStatus = null, bool is24Hours = false)
        {
            return contentItemRepository.GetContentItemCount(approvalStatus, is24Hours);
        }

        /// <summary>
        /// 资讯置顶
        /// </summary>
        /// <param name="contentItemId">资讯Id</param>
        /// <param name="isSticky">是否置顶</param>
        public void ContentItemIsSticky(object contentItemId, bool isSticky)
        {
            var contentItem = contentItemRepository.Get(contentItemId);
            if (contentItem == null)
                return;
            contentItem.IsSticky = isSticky;
            contentItemRepository.Update(contentItem);
        }

        /// <summary>
        /// 更新审核状态
        /// </summary>
        /// <param name="contentItemId">资讯Id</param>
        /// <param name="isApproved">是否通过审核</param>
        public void UpdateAuditStatus(long contentItemId, bool isApproved)
        {
            var contentItem = contentItemRepository.Get(contentItemId);
            AuditStatus auditStatus = isApproved ? AuditStatus.Success : AuditStatus.Fail;
            if (contentItem.ApprovalStatus == auditStatus)
                return;

            //执行事件
            EventBus<ContentItem>.Instance().OnAfter(contentItem, new CommonEventArgs(EventOperationType.Instance().Update(), 0));
            //待审核通过审核 增加积分
            EventBus<ContentItem, AuditEventArgs>.Instance().OnAfter(contentItem, new AuditEventArgs(contentItem.ApprovalStatus, auditStatus, isApproved ? EventOperationType.Instance().Approved() : EventOperationType.Instance().Disapproved()));
            //添加操作日志
            EventBus<ContentItem, CommonEventArgs>.Instance().OnAfter(contentItem, new CommonEventArgs(isApproved ? EventOperationType.Instance().Approved() : EventOperationType.Instance().Disapproved()));

            contentItem.ApprovalStatus = auditStatus;
            contentItemRepository.Update(contentItem);
        }

    }
    /// <summary>
    /// ContentItem排序字段
    /// </summary>
    public enum ContentItemSortBy
    {
        /// <summary>
        /// 发布日期倒序
        /// </summary>
        DatePublished_Desc,

        /// <summary>
        /// 阶段浏览次数
        /// </summary>
        StageHitTimes,

        /// <summary>
        /// 浏览次数
        /// </summary>
        HitTimes,

        /// <summary>
        /// 阶段评论数
        /// </summary>
        StageCommentCount,

        /// <summary>
        /// 评论总数
        /// </summary>
        CommentCount,
    }
}
