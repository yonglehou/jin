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
using Tunynet.Events;
using Tunynet.Settings;
using Tunynet.Repositories;
namespace Tunynet.Common
{
    /// <summary>
    /// 通知 设置 类型的 业务逻辑
    /// </summary>
    public class NoticeService
    {
        private INoticeRepository noticeRepository ;
        private IRepository<NoticeTypeSettings> noticeSettingsRepository;
        private IRepository<NoticeType> inoticetyperepository ;
       

        /// <summary>
        /// 构造器
        /// </summary>
        public NoticeService(INoticeRepository noticeRepository, IRepository<NoticeTypeSettings> noticeSettingsRepository, IRepository<NoticeType> inoticetyperepository)
        {
            this.noticeRepository = noticeRepository;
            this.noticeSettingsRepository = noticeSettingsRepository;
            this.inoticetyperepository = inoticetyperepository;
        }
        /// <summary>
        /// 创建通知
        /// </summary>
        /// <param name="entity">通知实体</param>
        /// <remarks>已检查通知设置</remarks>
        public void Create(Notice entity)
        {
            EventBus<Notice>.Instance().OnBefore(entity, new CommonEventArgs(EventOperationType.Instance().Create()));
         
            //entity.Body = entity.ResolvedBody;
            noticeRepository.Insert(entity);

            EventBus<Notice>.Instance().OnAfter(entity, new CommonEventArgs(EventOperationType.Instance().Create()));
        }
        /// <summary>
        /// 删除单条通知
        /// </summary>
        /// <param name="id">通知Id</param>
        public void Delete(long id)
        {

            Notice notice = noticeRepository.Get(id);
            EventBus<Notice>.Instance().OnBefore(notice, new CommonEventArgs(EventOperationType.Instance().Delete()));
            noticeRepository.DeleteByEntityId(id);
            EventBus<Notice>.Instance().OnAfter(notice, new CommonEventArgs(EventOperationType.Instance().Delete()));
        }
        /// <summary>
        /// 更新单条通知
        /// </summary>
        /// <param name="id">通知Id</param>
        public void Update(Notice entity)
        {
            EventBus<Notice>.Instance().OnBefore(entity, new CommonEventArgs(EventOperationType.Instance().Update()));
            noticeRepository.Update(entity);
            EventBus<Notice>.Instance().OnAfter(entity, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        /// 根据条件获取所有通知
        /// </summary>
        /// <param name="noticeTypeKey">通知key</param>
        /// <param name="Interval">上次间隔 （秒）</param>
        /// <param name="Status">状态</param>
        /// <returns></returns>
        public IEnumerable<Notice> GetNotices(string noticeTypeKey, int? Interval = null, int? Status = null
            )
        {
            return noticeRepository.GetNotices(noticeTypeKey, Interval, Status);
        }
        /// <summary>
        /// 获取所有通知类型
        /// </summary>
        /// <returns>通知类型</returns>
        public  IEnumerable<NoticeType> GetAllType()
        {
            return inoticetyperepository.GetAll();
        }

        /// <summary>
        ///  获取通知类型
        /// </summary>
        /// <param name="NoticeTypeKey">通知类型的Key</param>
        /// <returns></returns>
        public NoticeType GetType(string NoticeTypeKey)
        {
            return inoticetyperepository.GetAll().Where(n=>n.NoticeTypeKey == NoticeTypeKey).SingleOrDefault();
        }


        /// <summary>
        /// 获取通知通知设置
        /// </summary>
        /// <param name="NoticeTypeKey"></param>
        /// <returns></returns>
        public IEnumerable<NoticeTypeSettings> GetAllTypeSetting()
        {
            return noticeSettingsRepository.GetAll();
        }

        /// <summary>
        ///获取单条通知 
        /// </summary>
        /// <param name="id">通知Id</param>
        public Notice Get(long id)
        {
           return  noticeRepository.Get(id);
          
        }

        /// <summary>
        ///将通知设置为已处理状态
        /// </summary>
        /// <param name="id">通知Id</param>
        public void SetIsHandled(long id, NoticeStatus noticestatus)
        {
            var notice = noticeRepository.Get(id);
            EventBus<Notice>.Instance().OnBefore(notice, new CommonEventArgs(EventOperationType.Instance().Update()));
            noticeRepository.SetIsHandled(id, noticestatus);
            EventBus<Notice>.Instance().OnAfter(notice, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        ///获取用户最近几条未处理的通知
        /// </summary>
        /// <param name="topNumber"></param>
        /// <param name="userId">通知接收人Id</param>
        public IEnumerable<Notice> GetTops(long userId, int topNumber)
        {
            //按照创建日期倒序排序，并注意只查询未处理的通知
            return noticeRepository.GetTops(userId, topNumber);
        }

        /// <summary>
        /// 根据触发通知对象的ID获取通知 (用于评论审核通过时发送通知)
        /// </summary>
        /// <param name="objectId">触发通知对象ID</param>
        /// <returns></returns>
        public Notice GetNoticeByObjectId(long objectId)
        {
            return noticeRepository.GetNoticeByObjectId(objectId);
        }

        /// <summary>
        ///获取用户通知的分页集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">通知状态</param>
        /// <param name="typeId">通知类型Id</param>
        /// <param name="applicationId">应用Id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>通知分页集合</returns>
        public PagingDataSet<Notice> Gets(long userId, NoticeStatus? status, int? pageIndex = null)
        {
            //按照创建日期倒序排序
            return noticeRepository.Gets(userId, status, pageIndex ?? 1);
        }
        /// <summary>
        /// 删除用户数据（删除用户时使用）
        /// </summary>
        /// <param name="userId">用户id</param>
        public void CleanByUser(long userId)
        {
            noticeRepository.CleanByUser(userId);
        }


    }
}
