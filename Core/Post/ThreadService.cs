//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tunynet;
using Tunynet.Common;
using Tunynet.Events;
using Tunynet.Settings;

namespace Tunynet.Post
{

    /// <summary>
    /// 主题贴业务逻辑类
    /// </summary>
    public class ThreadService
    {
        private IUserService userService;
        private IThreadRepository barThreadRepository = null;
        private SiteSettings siteSetting;
        private CommentService commentService;
        private SectionService barSectionService;
        private AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Thread());
        private CategoryService categoryService;
        private AuditService auditService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="barThreadRepository"></param>
        public ThreadService(IThreadRepository barThreadRepository, CommentService commentService, IUserService userService, SectionService barSectionService, CategoryService categoryService, AuditService auditService, ISettingsManager<SiteSettings> siteSettings)
        {
            this.barThreadRepository = barThreadRepository;
            this.commentService = commentService;
            this.userService = userService;
            this.barSectionService = barSectionService;
            this.categoryService = categoryService;
            this.auditService = auditService;
            this.siteSetting = siteSettings.Get();

        }

        #region 维护主题贴

        /// <summary>
        /// 创建主题贴
        /// </summary>
        /// <param name="thread">主题贴</param>
        /// <param name="isMobile">是否手机端</param>
        /// <param name="isManager">是否有权限管理</param>
        public bool Create(Thread thread, bool isManager = false, bool isMobile = false)
        {
            EventBus<Thread>.Instance().OnBefore(thread, new CommonEventArgs(EventOperationType.Instance().Create()));
            auditService.ChangeAuditStatusForCreate(thread.UserId, thread, isManager);

            barThreadRepository.Insert(thread);

            if (thread.ThreadId > 0)
            {
                //执行事件  
                EventBus<Thread>.Instance().OnAfter(thread, new CommonEventArgs(EventOperationType.Instance().Create()));
                //发布通过审核的贴子增加积分
                EventBus<Thread, AuditEventArgs>.Instance().OnAfter(thread, new AuditEventArgs(null, thread.ApprovalStatus, EventOperationType.Instance().Create()));
                //记录操作日志
                EventBus<Thread, CommonEventArgs>.Instance().OnAfter(thread, new CommonEventArgs(EventOperationType.Instance().Create()));
                EventBus<Thread, AttachmentEventArgs>.Instance().OnAfter(thread, new AttachmentEventArgs(EventOperationType.Instance().Create(), thread.TenantTypeId, isMobile));
            }
            return thread.ThreadId > 0;
        }

        /// <summary>
        /// 更新主题贴
        /// </summary>
        /// <param name="thread">主题贴</param>
        /// <param name="operatorUserId">当前操作人</param>
        /// <param name="isManager">是否有权限管理</param>
        public void Update(Thread thread, long operatorUserId, bool isManager = false)
        {
            EventBus<Thread>.Instance().OnBefore(thread, new CommonEventArgs(EventOperationType.Instance().Update()));
            AuditStatus oldAuditStatus = thread.ApprovalStatus;

            auditService.ChangeAuditStatusForUpdate(thread.UserId, thread, isManager);

            barThreadRepository.Update(thread);


            EventBus<Thread>.Instance().OnAfter(thread, new CommonEventArgs(EventOperationType.Instance().Update()));

            //积分
            EventBus<Thread, AuditEventArgs>.Instance().OnAfter(thread, new AuditEventArgs(oldAuditStatus, thread.ApprovalStatus, EventOperationType.Instance().Update()));
            //记录操作日志
            EventBus<Thread, CommonEventArgs>.Instance().OnAfter(thread, new CommonEventArgs(EventOperationType.Instance().Update()));

        }

        /// <summary>
        /// 移动主题
        /// </summary>
        /// <param name="threadId">要移动主题的ThreadId</param>
        /// <param name="moveToSectionId">转移到贴吧的SectionId</param>
        public void MoveThread(long threadId, long moveToSectionId)
        {
            Thread thread = barThreadRepository.Get(threadId);
            if (thread.SectionId == moveToSectionId)
                return;
            long oldSectionId = thread.SectionId;

            var oldSection = barSectionService.Get(oldSectionId);
            if (oldSection == null)
                return;
            var newSection = barSectionService.Get(moveToSectionId);
            if (newSection == null)
                return;
            barThreadRepository.MoveThread(threadId, moveToSectionId);

            CountService countService = new CountService(TenantTypeIds.Instance().Section());
            countService.ChangeCount(CountTypes.Instance().ThreadCount(), oldSection.SectionId, oldSection.UserId, -1, true);
            countService.ChangeCount(CountTypes.Instance().ThreadAndPostCount(), oldSection.SectionId, oldSection.UserId, -1, true);

            countService.ChangeCount(CountTypes.Instance().ThreadCount(), newSection.SectionId, newSection.UserId, 1, true);
            countService.ChangeCount(CountTypes.Instance().ThreadAndPostCount(), newSection.SectionId, newSection.UserId, 1, true);

        }


        /// <summary>
        /// 设置置顶
        /// </summary>
        /// <param name="threadId">待操作的主题贴Id</param>
        /// <param name="isSticky">待更新至的置顶状态</param>
        public bool SetSticky(long threadId, bool isSticky)
        {
            Thread thread = barThreadRepository.Get(threadId);
            if (thread == null)
                return false;
			 if (thread.IsSticky != isSticky)
            {
            thread.IsSticky = isSticky;
            barThreadRepository.Update(thread);
			}
            return true;
        }

        /// <summary>
        /// 更新审核状态
        /// </summary>
        /// <param name="threadId">待被更新的主题贴Id</param>
        /// <param name="isApproved">是否通过审核</param>
        public void UpdateAuditStatus(long threadId, bool isApproved)
        {
            Thread thread = barThreadRepository.Get(threadId);
            AuditStatus auditStatus = isApproved ? AuditStatus.Success : AuditStatus.Fail;
            if (thread.ApprovalStatus == auditStatus)
                return;
            AuditStatus oldAuditStatus = thread.ApprovalStatus;
            thread.ApprovalStatus = auditStatus;
            barThreadRepository.Update(thread);

            EventBus<Thread>.Instance().OnAfter(thread, new CommonEventArgs(EventOperationType.Instance().Update()));
            //待审核贴子通过审核增加积分
            EventBus<Thread, AuditEventArgs>.Instance().OnAfter(thread, new AuditEventArgs(oldAuditStatus, auditStatus, isApproved ? EventOperationType.Instance().Approved() : EventOperationType.Instance().Disapproved()));
            //记录操作日志
            EventBus<Thread, CommonEventArgs>.Instance().OnAfter(thread, new CommonEventArgs(isApproved ? EventOperationType.Instance().Approved() : EventOperationType.Instance().Disapproved()));

        }


        /// <summary>
        /// 删除主题贴
        /// </summary>
        /// <param name="threadId">主题贴Id</param>
        public void Delete(long threadId)
        {
            Thread thread = barThreadRepository.Get(threadId);
            if (thread == null)
                return;
            EventBus<Thread>.Instance().OnBefore(thread, new CommonEventArgs(EventOperationType.Instance().Delete()));

            Section barSection = barSectionService.Get(thread.SectionId);
            if (barSection != null)
            {
                //贴子分类

                categoryService.ClearCategoriesFromItem(threadId, barSection.SectionId, TenantTypeIds.Instance().Thread());
            }

            int affectCount = barThreadRepository.Delete(thread);
            if (affectCount > 0)
            {
                //更新贴吧的计数
                CountService countService = new CountService(TenantTypeIds.Instance().Section());
                countService.ChangeCount(CountTypes.Instance().ThreadCount(), barSection.SectionId, barSection.UserId, -1, true);
                countService.ChangeCount(CountTypes.Instance().ThreadAndPostCount(), barSection.SectionId, barSection.UserId, -1, true);
                EventBus<Thread>.Instance().OnAfter(thread, new CommonEventArgs(EventOperationType.Instance().Delete()));
                //积分处理
                EventBus<Thread, AttachmentEventArgs>.Instance().OnAfter(thread, new AttachmentEventArgs(EventOperationType.Instance().Delete(), TenantTypeIds.Instance().Thread()));
                //相关联删除处理
                EventBus<Thread, AuditEventArgs>.Instance().OnAfter(thread, new AuditEventArgs(thread.ApprovalStatus, null, EventOperationType.Instance().Delete()));
                //记录操作日志
                EventBus<Thread, CommonEventArgs>.Instance().OnAfter(thread, new CommonEventArgs(EventOperationType.Instance().Delete()));



            }



        }

        /// <summary>
        /// 删除贴吧下的所有主题
        /// </summary>
        /// <param name="sectionId"></param>
        public void DeletesBySectionId(long sectionId)
        {
            IEnumerable<Thread> barThreads = barThreadRepository.GetAllThreadsOfSection(sectionId);
            foreach (var barThread in barThreads)
            {

                Delete(barThread.ThreadId);
            }
        }

        /// <summary>
        /// 删除用户下的所有贴子
        /// </summary>

        /// <param name="operatorUserId">操作人用户ID</param>

        private void DeletesByUserId(long userId)
        {
            IEnumerable<Thread> barThreads = barThreadRepository.GetAllThreadsOfUser(userId);
            foreach (var barThread in barThreads)
            {
                Delete(barThread.ThreadId);
            }
        }
        /// <summary>
        /// 删除 owner的所有贴子
        /// </summary>
        /// <param name="ownerId">拥有者ID</param>
        public IEnumerable<Thread> GetAllThreadsOfOwner(long ownerId)
        {
            IEnumerable<Thread> barThreads = barThreadRepository.GetAllThreadsOfOwner(ownerId);
            return barThreads;
        }

        /// <summary>
        /// 删除用户记录（删除用户时使用）
        /// </summary>
        /// <param name="userId">被删除用户</param>
        /// <param name="takeOverUserName">接管用户名</param>
        /// <param name="takeOverAll">是否接管被删除用户的所有内容</param>
        public void DeleteUser(long userId, string takeOverUserName, bool takeOverAll)
        {
            long takeOverUserId = UserIdToUserNameDictionary.GetUserId(takeOverUserName);

            User takeOver = userService.GetFullUser(takeOverUserId);
            //删除用户时，不删除贴吧，把贴吧转让，如果没有指定转让人，那就转给网站初始管理员
            if (takeOver == null)
            {
                takeOver = userService.GetFullUser(takeOverUserId);
            }
            barThreadRepository.DeleteUser(userId, takeOver, takeOverAll);
            if (takeOver != null)
            {
                if (!takeOverAll)
                {
                    DeletesByUserId(userId);
                }
            }

        }

        #endregion

        #region 获取主题贴

        /// <summary>
        /// 获取单个主题贴实体
        /// </summary>
        /// <param name="threadId">主题贴Id</param>
        /// <returns>主题贴</returns>
        public Thread Get(long threadId)
        {
            return barThreadRepository.Get(threadId);
        }

        /// <summary>
        /// 获取主题贴内容
        /// </summary>
        /// <param name="threadId">主题贴Id</param>
        /// <returns>主题贴内容</returns>
        public string GetBody(long threadId)
        {
            return barThreadRepository.GetBody(threadId);
        }

        /// <summary>
        /// 获取排序的列表ID集合去获取贴子实体
        /// </summary>
        /// <param name="threadIds">主题贴Id集合</param>
        /// <returns>主题贴</returns>
        public IEnumerable<Thread> Gets(IEnumerable<long> threadIds)
        {
            var threads = barThreadRepository.PopulateEntitiesByEntityIds(threadIds);
            if (siteSetting.AuditStatus == PubliclyAuditStatus.Success)
                return threads.Where(n => n.ApprovalStatus == AuditStatus.Success);
            else if (siteSetting.AuditStatus == PubliclyAuditStatus.Again)
                return threads.Where(n => n.ApprovalStatus > AuditStatus.Pending);
            else if (siteSetting.AuditStatus == PubliclyAuditStatus.Pending)
                return threads.Where(n => n.ApprovalStatus > AuditStatus.Fail);
            return threads;
        }

        /// <summary>
        /// 获取用户的主题贴分页集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="ignoreAudit">是否忽略审核状态（作者或管理员查看时忽略审核状态）</param>
        /// <param name="isPosted">是否是取我回复过的</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns>主题贴列表</returns>
        public PagingDataSet<Thread> GetUserThreads(string tenantTypeId, long userId, bool ignoreAudit, bool isPosted = false, int pageSize = 6, int pageIndex = 1, long? sectionId = null)
        {
            //不必筛选审核状态
            //缓存周期：对象集合，需要维护即时性
            //排序：发布时间（倒序）
            return barThreadRepository.GetUserThreads(tenantTypeId, userId, ignoreAudit, isPosted, pageSize, pageIndex, sectionId);
        }

        /// <summary>
        /// 获取主题贴的排行数据
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">前多少条</param>
        /// <param name="isEssential">是否为精华贴</param>
        /// <param name="sortBy">主题贴排序依据</param>
        /// <returns></returns>
        public IEnumerable<Thread> GetTops(string tenantTypeId, int topNumber, bool? isEssential = null, SortBy_BarThread sortBy = SortBy_BarThread.StageHitTimes)
        {
            //只获取可对外显示审核状态的主题贴
            //缓存周期：常用对象集合，不用维护即时性
            return barThreadRepository.GetTops(tenantTypeId, topNumber, isEssential, sortBy);
        }




        /// <summary>
        /// 根据标签名获取主题贴排行分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="tagName">标签名</param>
        /// <param name="isEssential">是否为精华贴</param>
        /// <param name="sortBy">贴子排序依据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>主题贴列表</returns>
        public PagingDataSet<Thread> Gets(string tenantTypeId, string tagName, bool? isEssential = null, SortBy_BarThread sortBy = SortBy_BarThread.StageHitTimes, int pageIndex = 1)
        {
            //只获取可对外显示审核状态的主题贴
            //缓存周期：对象集合，不用维护即时性
            return barThreadRepository.Gets(tenantTypeId, tagName, isEssential, sortBy, pageIndex);
        }

        /// <summary>
        /// 根据贴吧获取主题贴分页集合
        /// </summary>
        /// <param name="sectionId">贴吧Id</param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="orderBySticky">是否显示置顶</param>
        /// <param name="sortBy">贴子排序依据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>主题贴列表</returns>
        public PagingDataSet<Thread> Gets(long sectionId, long? ownerId = null, long? categoryId = null, bool? orderBySticky = null, SortBy_BarThread sortBy = SortBy_BarThread.LastModified_Desc, int pageSize = 20, int pageIndex = 1, SortBy_BarDateThread BarDate = SortBy_BarDateThread.All)
        {
            //只获取可对外显示审核状态的主题贴
            //缓存周期：对象集合，需要维护即时性
            return barThreadRepository.Gets(sectionId, ownerId, categoryId, orderBySticky, sortBy, pageSize, pageIndex, BarDate);
        }

        /// <summary>
        /// 贴子管理时查询贴子分页集合
        /// </summary>
        /// <param name="query">贴子查询条件</param>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴子分页集合</returns>
        public PagingDataSet<Thread> Gets(string tenantTypeId, ThreadQuery query, int pageSize, int pageIndex)
        {
            //当SubjectKeyword、StartDate、EndDate为null时，进行缓存
            //当SectionId不为null时，使用分区版本，分区名为：SectionId，否则使用全局版本
            //缓存周期：对象集合，需要维护即时性
            //使用用户选择器设置query.UserId参数
            return barThreadRepository.Gets(tenantTypeId, query, pageSize, pageIndex);
        }

        /// <summary>
        /// 帖子计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        public int GetThreadCount(AuditStatus? approvalStatus = null, bool is24Hours = false)
        {
            return barThreadRepository.GetThreadCount(approvalStatus, is24Hours);
        }

        #endregion



    }
}