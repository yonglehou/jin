//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Common.Repositories;
using Tunynet.Events;
using Tunynet.Settings;
using Tunynet.Utilities;

namespace Tunynet.Common
{
    /// <summary>
    /// 邀请好友业务逻辑类
    /// </summary>
    public class InviteFriendService
    {
        private ISettingsManager<InviteFriendSettings> inviteFriendSettingsManager;
        private IInvitationCodesRepository invitationCodesRepository;
        private IInviteFriendRecordsRepository inviteFriendRecordsRepository;
        /// <summary>
        /// 不带参数的构造方法
        /// </summary>
        public InviteFriendService(ISettingsManager<InviteFriendSettings> inviteFriendSettingsManager, IInvitationCodesRepository invitationCodesRepository, IInviteFriendRecordsRepository inviteFriendRecordsRepository)
        {
            this.invitationCodesRepository = invitationCodesRepository;
            this.inviteFriendSettingsManager = inviteFriendSettingsManager;
            this.inviteFriendRecordsRepository = inviteFriendRecordsRepository;
        }

       

        

        //关于缓存期限：
        //1、用户邀请码配额 使用CachingExpirationType.SingleObject
        //2、邀请码实体、列表 使用CachingExpirationType.SingleObject、ObjectCollection
        //3、邀请好友记录列表 使用CachingExpirationType.ObjectCollection

        #region 邀请码

        /// <summary>
        /// 获取邀请码
        /// </summary>
        /// <param name="userId">申请人</param>
        public string GetInvitationCode(long userId)
        {        
            string code = EncryptionUtility.MD5_16(userId.ToString() + DateTime.Now.Ticks.ToString());
            InviteFriendSettings inviteFriendSettings = inviteFriendSettingsManager.Get();
            if (inviteFriendSettings.AllowInvitationCodeUseOnce)
            {
              
                    InvitationCode invitationCode = new InvitationCode
                    {
                        Code = code,
                        DateCreated = DateTime.Now,
                        ExpiredDate = DateTime.Now.AddDays(inviteFriendSettings.InvitationCodeTimeLiness),
                        IsMultiple = !inviteFriendSettings.AllowInvitationCodeUseOnce,
                        UserId = userId
                    };
                    invitationCodesRepository.Insert(invitationCode);
              
                //1.用户未使用邀请码配额减1，然后调用ChangeUserInvitationCodeCount进行更新
                //2.过期时间根据InviteFriendSettings.InvitationCodeTimeLiness确定
            }
            else
            {
                string todayCode = invitationCodesRepository.GetTodayCode(userId);
                if (string.IsNullOrEmpty(todayCode))
                {
                    InvitationCode invitationCode = new InvitationCode
                    {
                        Code = code,
                        DateCreated = DateTime.Now,
                        ExpiredDate = DateTime.Now.AddDays(inviteFriendSettings.InvitationCodeTimeLiness),
                        IsMultiple = !inviteFriendSettings.AllowInvitationCodeUseOnce,
                        UserId = userId
                    };
                    invitationCodesRepository.Insert(invitationCode);
                }
                else
                {
                    code = todayCode;
                }
                //检查今日是否有生成过的可多次使用的邀请码，若没有，则生成；否则，直接返回
                //过期时间根据LinktimelinessSettings.Lowlinktimeliness确定
                //设置IsMultiple为true
            }
            //向邀请码表中插入数据库
            return code;
        }

        /// <summary>
        /// 获取邀请码实体
        /// </summary>
        /// <param name="invitationCode">邀请码</param>
        public InvitationCode GetInvitationCodeEntity(string invitationCode)
        {
            InvitationCode invitation = invitationCodesRepository.Get(invitationCode);
            InviteFriendSettings inviteFriendSettings = inviteFriendSettingsManager.Get();
            if (invitation == null || inviteFriendSettings.AllowInvitationCodeUseOnce == invitation.IsMultiple)
                return null;
            return invitation;
        }

    

        /// <summary>
        /// 删除邀请码（当邀请码被使用时进行调用）
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="invitationCode">邀请码</param>
        public bool DeleteInvitationCode(long userId, string invitationCode)
        {
            return invitationCodesRepository.DeleteInvitationCode(userId, invitationCode);
        }

        /// <summary>
        /// 批量删除过期的邀请码
        /// </summary>
        public void DeleteTrashInvitationCodes()
        {

            invitationCodesRepository.DeleteTrashInvitationCodes();
        }

        //done:zhengw,by mazq
        //1、什么情况会用到？
        //2、如何实现？
        //zhengw回复：已删除

        /// <summary>
        /// 获取我的未使用邀请码列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>未使用邀请码列表</returns>
        public PagingDataSet<InvitationCode> GetMyInvitationCodes(long userId, int pageIndex = 1)
        {
            return invitationCodesRepository.GetMyInvitationCodes(userId, pageIndex);
        }

        #endregion

       
        #region 邀请好友记录

        /// <summary>
        /// 创建邀请好友记录
        /// </summary>
        /// <param name="inviteFriendRecord">被创建的记录实体</param>
        public void CreateInviteFriendRecord(InviteFriendRecord inviteFriendRecord)
        {
            inviteFriendRecordsRepository.Insert(inviteFriendRecord);
            EventBus<InviteFriendRecord>.Instance().OnAfter(inviteFriendRecord, new CommonEventArgs(EventOperationType.Instance().Create(), 0));
        }

        /// <summary>
        /// 通过被邀请人ID获取邀请人
        /// </summary>
        /// <param name="userId">被邀请人ID</param>
        /// <returns></returns>
        public InviteFriendRecord GetInvitingUserId(long userId)
        {
            return inviteFriendRecordsRepository.GetInvitingUserId(userId);
        }

        /// <summary>
        /// 获取我的邀请好友记录
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns>被邀请的好友Id列表</returns>
        public IEnumerable<long> GetMyInviteFriendRecords(long userId, int pageSize, int pageIndex, out long totalRecords)
        {        

            return inviteFriendRecordsRepository.GetMyInviteFriendRecords(userId, pageSize, pageIndex, out totalRecords);
        }

        #endregion

        /// <summary>
        /// 删除用户的所有邀请好友记录（删除用户的时候使用）
        /// </summary>
        /// <param name="userId">用户id</param>
        public void CleanByUser(long userId)
        {
            invitationCodesRepository.CleanByUser(userId);
            inviteFriendRecordsRepository.CleanByUser(userId);
        }

        /// <summary>
        /// 记录邀请用户奖励
        /// </summary>
        /// <param name="userId">用户Id</param>
        public void RewardingUser(long userId)
        {

            inviteFriendRecordsRepository.RewardingUser(userId);
        }
    }
}
