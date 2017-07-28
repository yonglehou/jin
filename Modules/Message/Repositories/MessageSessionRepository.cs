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
using Tunynet.Utilities;

namespace Tunynet.Common
{
    /// <summary>
    /// 私信会话数据访问
    /// </summary>
    public class MessageSessionRepository : Repository<MessageSession>, IMessageSessionRepository
    {


        /// <summary>
        /// 删除私信会话
        /// </summary>
        /// <param name="entity">待删除实体</param>
        /// <returns>操作后影响行数</returns>
        public override int Delete(MessageSession entity)
        {
            var dao = CreateDAO();

            List<Sql> sqls = new List<Sql>();
            int affectCount = 0;

            dao.OpenSharedConnection();

            var sql = Sql.Builder;
            sql.Select("SessionId")
               .From("tn_MessageSessions")
               .Where("UserId = @0", entity.UserId)
               .Where("OtherUserId = @0", entity.OtherUserId);
            affectCount = dao.Execute(sql);

            using (var transaction = dao.GetTransaction())
            {
                if (affectCount <= 0)
                {
                    //清除私信实体脚本

                    sqls.Add(Sql.Builder.Append("delete from tn_Messages where exists(")
                                        .Append("select 1 from (select 1 from tn_MessagesInSessions MIS where tn_Messages.MessageId = MIS.MessageId and MIS.SessionId = @0) as a)", entity.SessionId));
                }

                //清除私信与会话关联脚本
                sqls.Add(Sql.Builder.Append("delete from tn_MessagesInSessions  where SessionId = @0", entity.SessionId));

                dao.Execute(sqls);
                affectCount = base.Delete(entity);
                transaction.Complete();
            }

            dao.CloseSharedConnection();

            EntityData.ForType(typeof(Message)).RealTimeCacheHelper.IncreaseAreaVersion("ReceiverUserId", entity.UserId);
            EntityData.ForType(typeof(Message)).RealTimeCacheHelper.IncreaseAreaVersion("ReceiverUserId", entity.OtherUserId);

            return affectCount;
        }

        /// <summary>
        /// 获取私信会话
        /// </summary>
        /// <param name="userId">用户的id</param>
        /// <param name="otherUserId">其他人的id</param>
        /// <returns>私信会话</returns>
        public MessageSession Get(long userId, long otherUserId)
        {
            string cacheKey = GetCacheKeyForGetMessageSession(userId, otherUserId);
            MessageSession session = cacheService.Get<MessageSession>(cacheKey);
            if (session != null)
                return session;

            Sql sql_select = Sql.Builder.Select("SessionId").From("tn_MessageSessions").Where("UserId=@0", userId).Where("OtherUserId=@0", otherUserId);
            long id = CreateDAO().FirstOrDefault<long>(sql_select);

            session = Get(id);
            if (session != null)
            {
                cacheService.Set(cacheKey, session, CachingExpirationType.SingleObject);
                return session;
            }

            session = MessageSession.New();
            session.LastModified = DateTime.Now;
            session.MessageType = (int)MessageType.Common;
            session.OtherUserId = otherUserId;
            session.UserId = userId;

            Insert(session);

            cacheService.Set(cacheKey, session, CachingExpirationType.SingleObject);
            return session;
        }

        /// <summary>
        /// 获取用户私信会话
        /// </summary>
        ///<param name="userId">用户Id</param>
        ///<param name="pageIndex">当前页码</param>
        public PagingDataSet<MessageSession> GetSessionsOfUser(long userId, int pageIndex, int pageSize )
        {
            var sql = Sql.Builder;
            sql.Where("UserId = @0", userId);

            sql.OrderBy("UnreadMessageCount desc");
            sql.OrderBy("LastMessageId desc");
            return GetPagingEntities(pageSize, pageIndex, sql);
            
        }

        /// <summary>
        /// 获取主会话Id
        /// </summary>
        ///<param name="userId">用户Id</param>
        ///<param name="pageIndex">当前页码</param>
        public MessageSession GetSenderSessionId(long sessionId)
        {
            var sql = Sql.Builder;
            sql.Select("*").From("tn_MessageSessions");
            sql.Where("SenderSessionId = @0", sessionId);
            return CreateDAO().SingleOrDefault<MessageSession>(sql);

        }
        /// <summary>
        /// 获取私信会话分页数据（后台用）
        /// </summary>
        /// <param name="type">私信类型</param>
        /// <param name="userId">用户Id（用来搜索用户Id相关的私信会话）</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>私信会话分页数据</returns>
        public PagingDataSet<MessageSession> GetSessions(MessageType? type, long? userId,  bool asAnonymous, int pageIndex, int pageSize)
        {

            var sql = Sql.Builder;
            if (type.HasValue)
                sql.Where("MessageType = @0", (int)type);
            if (userId.HasValue && userId.Value > 0)
            {
                sql.Where("UserId = @0", userId);
                if (!asAnonymous)
                    sql.Where("AsAnonymous = 0");
                else
                    sql.Where("SenderSessionId>0 or AsAnonymous = 0");
            }
            else
                sql.Where("SenderSessionId>0");
            sql.Where("MessageCount>0 ");
            sql.OrderBy("LastMessageId desc");
            sql.OrderBy("UnreadMessageCount desc");

            return GetPagingEntities(pageSize, pageIndex, sql);
            

        }

        /// <summary>
        /// 获取前N个私信会话
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="topNumber">获取记录条数</param>
        /// <param name="hasUnread">是否仅获取未读会话</param>
        public IEnumerable<MessageSession> GetTopSessions(long userId, int topNumber, bool hasUnread = false)
        {
            var sql = Sql.Builder;

            sql.Where("userId = @0", userId);
            if (hasUnread)
                sql.Where("UnreadMessageCount > 0");

            sql.OrderBy("SessionId desc");
            return GetTopEntities(topNumber, sql);
           

        }

        /// <summary>
        /// 获取客服消息
        /// </summary>
        /// <param name="userName">发件人</param>
        /// <param name="roleId">角色</param>
        /// <param name="minRank">最小等级</param>
        /// <param name="maxRank">最大等级</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页多少条</param>
        /// <returns></returns>
        public PagingDataSet<MessageSession> GetCustomerMessages(int pageIndex, int pageSize, string userName, long roleId, long minRank, long maxRank)
        {
            Sql sql = Sql.Builder;
            sql.Select("tn_MessageSessions.*")
                .From("tn_MessageSessions");

            Sql sql_Where = Sql.Builder;

            bool innerJoinTn_Users = false;
            bool innerJoinTn_UsersInRoles = false;

            if (!string.IsNullOrEmpty(userName))
            {
                if (!innerJoinTn_Users)
                    sql.InnerJoin("tn_Users")
                        .On("tn_MessageSessions.OtherUserId=tn_Users.UserId");
                innerJoinTn_Users = true;

                sql_Where.Where("tn_Users.UserName like @0 or tn_Users.TrueName like @0 or tn_Users.NickName like @0", "%" + StringUtility.StripSQLInjection(userName) + "%");
            }
            if (roleId>0)
            {
                if (!innerJoinTn_UsersInRoles)
                    sql.InnerJoin("tn_UsersInRoles")
                        .On("tn_UsersInRoles.UserId=tn_MessageSessions.OtherUserId");
                innerJoinTn_UsersInRoles = true;

                sql_Where.Where("tn_UsersInRoles.RoleId=@0", roleId);
            }
            if (minRank > 0)
            {
                if (!innerJoinTn_Users)
                    sql.InnerJoin("tn_Users")
                        .On("tn_MessageSessions.OtherUserId=tn_Users.UserId");
                innerJoinTn_Users = true;

                sql_Where.Where("tn_Users.Rank >=@0", minRank);
            }
            if (maxRank > 0)
            {
                if (!innerJoinTn_Users)
                    sql.InnerJoin("tn_Users")
                        .On("tn_MessageSessions.OtherUserId=tn_Users.UserId");
                innerJoinTn_Users = true;

                sql_Where.Where("tn_Users.Rank <=@0", maxRank);
            }
            sql_Where.Where("tn_MessageSessions.UserId=@0", (long)BuildinMessageUserId.CustomerService);
            sql.Append(sql_Where);

            sql.OrderBy("tn_MessageSessions.LastMessageId desc");

            return GetPagingEntities(pageSize, pageIndex, sql);
        }

        /// <summary>
        /// 清除用户所有私信会话
        /// </summary>
        /// <param name="userId">用户Id</param>
        public void ClearSessionsFromUser(long userId)
        {
            var dao = CreateDAO();
            dao.OpenSharedConnection();

            List<Sql> sqls = new List<Sql>();
            var sql = Sql.Builder;
            sql.Select("SessionId")
               .From("tn_MessageSessions")
               .Where("UserId = @0", userId);
            IEnumerable<object> sessionids = dao.FetchFirstColumn(sql);
            if (sessionids.Count() > 0)
            {
                //清除私信实体脚本
                sqls.Add(Sql.Builder.Append("delete from tn_Messages where exists(")
                                    .Append("select 1 from tn_MessagesInSessions MIS where tn_Messages.MessageId = MIS.MessageId and exists(")
                                    .Append("select 1 from tn_MessageSessions MS where MS.SessionId = MIS.SessionId and MS.UserId = @0 and not exists(", userId)
                                    .Append("select 1 from tn_MessageSessions SUBMS Where SUBMS.UserId = MS.OtherUserId and SUBMS.OtherUserId = MS.UserId)))"));

                //清除私信与会话关联脚本
                sqls.Add(Sql.Builder.Append("delete from tn_MessagesInSessions where exists(")
                                    .Append("select 1 from tn_MessageSessions MS where MS.SessionId = tn_MessagesInSessions.SessionId and MS.UserId = @0)", userId));

                //清除私信会话
                sqls.Add(Sql.Builder.Append("delete from tn_MessageSessions where UserId = @0", userId));

                using (var transaction = dao.GetTransaction())
                {
                    dao.Execute(sqls);
                    transaction.Complete();
                }
            }

            dao.CloseSharedConnection();

            #region 更新缓存

            RealTimeCacheHelper.IncreaseAreaVersion("UserId", userId);
        
            sessionids.ToList().ForEach((n) =>
            {
                RealTimeCacheHelper.IncreaseEntityCacheVersion(n);
            });

            #endregion
        }

        /// <summary>
        /// 获取CacheKey的信息头
        /// </summary>
        /// <returns></returns>
        private string GetCacheKeyForGetMessageSession(long userId, long otherUserId)
        {
            return string.Format("GetMessageSession::userId-{0};otherUserId-{1};", userId, otherUserId);
        }


        /// <summary>
        /// 获取私信会话ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="otherUserId">其他人的ID</param>
        public long GetSessionsId(long userId, long otherUserId)
        {
            var dao = CreateDAO();
            dao.OpenSharedConnection();

            List<Sql> sqls = new List<Sql>();
            var sql = Sql.Builder;
            sql.Select("SessionId")
                .From("tn_MessageSessions")
                .Where("UserId=@0 and OtherUserId=@1", otherUserId, userId);

            long sessionId = dao.FirstOrDefault<long>(sql);
            return sessionId;
        }
    }
}
