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
using Tunynet.Caching;
using PetaPoco;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 邀请好友的记录
    /// </summary>
    public class InviteFriendRecordsRepository : Repository<InviteFriendRecord>, IInviteFriendRecordsRepository
    {
        //private int pageSize = 12;

        /// <summary>
        /// 获取我的邀请好友记录
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalRecords">总记录数</param>
        /// <returns>被邀请的好友Id列表</returns>
        public IEnumerable<long> GetMyInviteFriendRecords(long userId, int pageSize, int pageIndex, out long totalRecords)
        {
            var dao = CreateDAO();

            //仿照其他分页方法做
            var sql = Sql.Builder;
            sql.Select("tn_InviteFriendRecords.InvitedUserId")
                .From("tn_InviteFriendRecords").InnerJoin("tn_Users").On("tn_Users.UserId=tn_InviteFriendRecords.InvitedUserId")
                //done:bianchx by libsh，不需要加判断
                //回复：已经删除了对应的判断。
                .Where("tn_Users.Status=@0 and tn_InviteFriendRecords.UserId = @1", UserStatus.IsActivated,userId)
                .OrderBy("tn_InviteFriendRecords.DateCreated desc");


            IEnumerable<object> invitedUserIds;
            invitedUserIds = dao.FetchPagingPrimaryKeys(pageSize, pageIndex, "InvitedUserId", sql, out totalRecords);

            

            if (invitedUserIds != null)
                return invitedUserIds.Cast<long>();
            else
                return new List<long>();

        }

        /// <summary>
        /// 通过被邀请人ID获取邀请人
        /// </summary>
        /// <param name="userId">被邀请人ID</param>
        /// <returns></returns>
        public InviteFriendRecord GetInvitingUserId(long userId)
        {
            var sql_Get = Sql.Builder;
            var dao = CreateDAO();
            sql_Get.Select("*")
                .From("tn_InviteFriendRecords")
                .Where("InvitedUserId = @0", userId);
            InviteFriendRecord record = dao.FirstOrDefault<InviteFriendRecord>(sql_Get);
            return record;
        }

    

        /// <summary>
        /// 清除用户资料（删除用户时使用）
        /// </summary>
        /// <param name="userId">用户id</param>
        public void CleanByUser(long userId)
        {
            //清除用户数据的时候不需要考虑缓存
            Sql sql_delete = Sql.Builder.Append("Delete from tn_InviteFriendRecords where UserId=@0 or InvitedUserId=@0", userId);
            CreateDAO().Execute(sql_delete);
        }

        /// <summary>
        /// 记录邀请用户奖励
        /// </summary>
        /// <param name="userId">用户Id</param>
        public void RewardingUser(long userId)
        {
            var sql_Set = Sql.Builder
                .Append("update tn_InviteFriendRecords set IsRewarded = 1 where userId = @0", userId);
            CreateDAO().Execute(sql_Set);
        }

    }
}
