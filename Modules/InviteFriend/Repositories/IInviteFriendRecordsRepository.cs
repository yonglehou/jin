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
    /// 邀请好友记录表
    /// </summary>
    public interface IInviteFriendRecordsRepository : IRepository<InviteFriendRecord>
    {
        /// <summary>
        /// 获取我的邀请好友记录
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>被邀请的好友Id列表</returns>
        IEnumerable<long> GetMyInviteFriendRecords(long userId, int pageSize, int pageIndex, out long totalRecords);

        /// <summary>
        /// 通过被邀请人ID获取邀请人
        /// </summary>
        /// <param name="userId">被邀请人ID</param>
        /// <returns></returns>
        InviteFriendRecord GetInvitingUserId(long userId);

        /// <summary>
        /// 删除用户的所有邀请好友记录（删除用户的时候使用）
        /// </summary>
        /// <param name="userId">用户id</param>
        void CleanByUser(long userId);

        /// <summary>
        /// 记录邀请用户奖励
        /// </summary>
        /// <param name="userId">用户Id</param>
        void RewardingUser(long userId);
    }
}
