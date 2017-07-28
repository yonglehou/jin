//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 通知数据访问接口
    /// </summary>
    public interface INoticeRepository : IRepository<Notice>
    {
        /// <summary>
        /// 清空接收人的通知记录
        /// </summary>
        /// <param name="userId">接收人Id</param>
        /// <param name="status">通知状态</param>
        void ClearAll(long userId, NoticeStatus? status = null);

        /// <summary>
        /// 删除用户的记录（删除用户时调用）
        /// </summary>
        /// <param name="userId"></param>
        void CleanByUser(long userId);

        /// <summary>
        /// 将通知设置为已处理状态
        /// </summary>
        /// <param name="id">通知Id</param>
        /// <param name="noticestatus">处理状态</param>
        void SetIsHandled(long id, NoticeStatus noticestatus);

        /// <summary>
        /// 批量将所有未处理通知修改为已处理状态
        /// </summary>
        /// <param name="userId">接收人Id</param>
        /// <param name="noticestatus">处理状态</param>
        void BatchSetIsHandled(long userId, NoticeStatus noticestatus);

        /// <summary>
        /// 获取某人的未处理通知数
        /// </summary>
        /// <param name="noticestatus">处理状态</param>
        int GetUnhandledCount(long userId, NoticeStatus noticestatus);
        /// <summary>
        /// 根据触发通知对象的ID获取通知 (用于评论审核通过时发送通知)
        /// </summary>
        /// <param name="objectId">触发通知对象ID</param>
        /// <returns></returns>
         Notice GetNoticeByObjectId(long objectId);
        


        /// <summary>
        /// 获取某人的未处理通知数（不包括评论）
        /// </summary>
        int GetUnhandledCountNoComment(long userId);

        /// <summary>
        /// 获取用户最近几条未处理的通知
        /// </summary>
        /// <param name="topNumber"></param>
        /// <param name="userId">通知接收人Id</param>
        IEnumerable<Notice> GetTops(long userId, int topNumber);
        /// <summary>
        ///获取用户通知的分页集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">通知状态</param>
        /// <param name="typeId">通知类型Id</param>
        /// <param name="applicationId">应用Id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>通知分页集合</returns>
        PagingDataSet<Notice> Gets(long userId, NoticeStatus? status, int pageIndex);
        /// <summary>
        /// 根据条件获取所有通知
        /// </summary>
        /// <param name="noticeTypeKey">通知key</param>
        /// <param name="Interval">上次间隔 （秒）</param>
        /// <param name="Status">状态</param>
        /// <returns></returns>
        IEnumerable<Notice> GetNotices(string noticeTypeKey, int? Interval = null, int? Status = null
             );
     
            /// <summary>
            /// 获取用户通知的分页集合
            /// </summary>
            /// <param name="userId">用户Id</param>
            /// <param name="status">通知状态</param>
            /// <param name="typeId">通知类型Id</param>
            /// <param name="applicationId">应用Id</param>
            /// <param name="pageIndex">页码</param>
            /// <returns>通知分页集合</returns>
            PagingDataSet<Notice> Gets(long userId, NoticeStatus? status, int? typeId, int? applicationId, int pageIndex);
        /// <summary>
        /// 获取用户通知的分页集合(不包括评论)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">通知状态</param>
        /// <param name="typeId">通知类型Id</param>
        /// <param name="applicationId">应用Id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>通知分页集合</returns>
        PagingDataSet<Notice> GetsForNoComment(long userId, NoticeStatus? status, int? typeId, int? applicationId, int pageIndex);

        /// <summary>
        /// 获取通知需提醒信息
        /// </summary>
        /// <returns></returns>
        //IEnumerable<UserReminderInfo> GetUserReminderInfos();
    }
}