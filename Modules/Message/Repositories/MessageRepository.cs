//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 私信数据访问
    /// </summary>
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        MessageSessionRepository messageSessionRepository = new MessageSessionRepository();

        /// <summary>
        /// 把实体entity添加到数据库
        /// </summary>
        /// <param name="message">待创建实体</param>
        /// <returns>实体主键</returns>
        public void Create(Message message, long sessionId)
        {
            var sql = Sql.Builder;
            IList<Sql> sqls = new List<Sql>();
            long senderSessionId = 0, receiverSessionId = 0;
            MessageSessionRepository messageSessionRepository = new MessageSessionRepository();

            var dao = CreateDAO();
            dao.OpenSharedConnection();

            base.Insert(message);
            
            if (sessionId == 0)
            {
                MessageSession messageSession = new MessageSession();
                messageSession.UserId = message.SenderUserId;
                messageSession.OtherUserId = message.ReceiverUserId;
                messageSession.LastMessageId = message.MessageId;
                messageSession.MessageType = (int)message.MessageType;
                messageSession.MessageCount = 1;
                messageSession.SenderSessionId = 0;
                messageSession.LastModified = message.DateCreated;
                messageSession.AsAnonymous = message.AsAnonymous;
                messageSessionRepository.Insert(messageSession);
                senderSessionId = messageSession.SessionId;

                messageSession = new MessageSession();
                messageSession.UserId = message.ReceiverUserId;
                messageSession.OtherUserId = message.SenderUserId;
                messageSession.LastMessageId = message.MessageId;
                messageSession.UnreadMessageCount = 1;
                messageSession.MessageType = (int)message.MessageType;
                messageSession.MessageCount = 1;
                messageSession.SenderSessionId = senderSessionId;
                messageSession.LastModified = message.DateCreated;
                messageSession.AsAnonymous = message.AsAnonymous;
                messageSessionRepository.Insert(messageSession);
                receiverSessionId = messageSession.SessionId;
            }
            else
            {
                var sesson = messageSessionRepository.GetSenderSessionId(sessionId);
                if (sesson != null)
                {
                    sql.Append("update tn_MessageSessions")
                       .Append("set LastMessageId = @0,UnreadMessageCount = UnreadMessageCount + 1,LastModified = @1", message.MessageId, message.DateCreated)
                       .Where("sessionId = @0", sesson.SessionId);
                    dao.Execute(sql);
                    sql = Sql.Builder;
                    sql.Append("update tn_MessageSessions")
                        .Append("set LastMessageId = @0,MessageCount = MessageCount + 1,LastModified = @1", message.MessageId, message.DateCreated)
                        .Where("sessionId = @0", sesson.SenderSessionId);
                    dao.Execute(sql);
                    senderSessionId = sessionId;
                    receiverSessionId = sesson.SessionId;
                }
                else
                {
                    var receiver = messageSessionRepository.Get(sessionId);
                    sql.Append("update tn_MessageSessions")
                         .Append("set LastMessageId = @0,UnreadMessageCount = UnreadMessageCount + 1,LastModified = @1", message.MessageId, message.DateCreated)
                         .Where("sessionId = @0", receiver.SenderSessionId);
                    dao.Execute(sql);
                    sql = Sql.Builder;
                    sql.Append("update tn_MessageSessions")
                        .Append("set LastMessageId = @0,MessageCount = MessageCount + 1,LastModified = @1", message.MessageId, message.DateCreated)
                        .Where("sessionId = @0", sessionId);
                    dao.Execute(sql);
                    senderSessionId = sessionId;
                    receiverSessionId = receiver.SenderSessionId;
                }
            }
            //添加会话与私信的关系
            sqls.Add(Sql.Builder.Append("insert into tn_MessagesInSessions (SessionId,MessageId) values (@0,@1)", senderSessionId, message.MessageId));
            sqls.Add(Sql.Builder.Append("insert into tn_MessagesInSessions (SessionId,MessageId) values (@0,@1)", receiverSessionId, message.MessageId));
            dao.Execute(sqls);

            dao.CloseSharedConnection();

            #region 缓存处理

            if (RealTimeCacheHelper.EnableCache)
            {
               

                var realTimeCacheHelper = EntityData.ForType(typeof(MessageInSession)).RealTimeCacheHelper;
       

                realTimeCacheHelper = EntityData.ForType(typeof(MessageSession)).RealTimeCacheHelper;
              
                realTimeCacheHelper.IncreaseEntityCacheVersion(senderSessionId);
                realTimeCacheHelper.IncreaseEntityCacheVersion(receiverSessionId);


                string cacheKey = realTimeCacheHelper.GetCacheKeyOfEntity(senderSessionId);
                MessageSession senderSession = cacheService.Get<MessageSession>(cacheKey);
                if (senderSession != null)
                {
                    senderSession.LastMessageId = message.MessageId;
                    senderSession.LastModified = message.DateCreated;
                    senderSession.MessageCount++;

                }

                cacheKey = realTimeCacheHelper.GetCacheKeyOfEntity(receiverSessionId);
                MessageSession receiverSession = cacheService.Get<MessageSession>(cacheKey);
                if (receiverSession != null)
                {
                    receiverSession.LastMessageId = message.MessageId;
                    receiverSession.LastModified = message.DateCreated;
                    receiverSession.MessageCount++;
                    receiverSession.UnreadMessageCount++;
                }

            }
            #endregion 缓存处理
            
        }

        /// <summary>
        /// 从数据库中删除实体
        /// </summary>
        /// <param name="entity">待删除私信实体</param>
        /// <param name="sessionId">私信会话Id</param>
        /// <returns>操作后影响行数</returns>
        public int Delete(Message entity, long sessionId)
        {
            var sql = Sql.Builder;
            IList<Sql> sqls = new List<Sql>();
            int affectCount = 0;

            var dao = CreateDAO();
            dao.OpenSharedConnection();

            List<string> dd = new List<string>();

            sql.From("tn_MessageSessions")
               .Where("(UserId = @0 and OtherUserId = @1) or (UserId = @1 and OtherUserId = @0)", entity.SenderUserId, entity.ReceiverUserId);

            List<MessageSession> sessions = dao.Fetch<MessageSession>(sql);

            if (sessions.Count > 0)
            {
                //处理相关会话的计数，如果会话中仅当前一条私信则删除会话
                foreach (var session in sessions)
                {
                    if (session.SessionId != sessionId)
                        continue;

                    if (session.MessageCount > 1)
                    {
                        sqls.Add(Sql.Builder.Append("update tn_MessageSessions")
                                            .Append("set MessageCount = MessageCount - 1")
                                            .Where("SessionId = @0", session.SessionId));
                    }
                    else
                    {
                        sqls.Add(Sql.Builder.Append("delete from tn_MessageSessions where SessionId = @0", session.SessionId));
                    }

                    //删除会话与私信的关系
                    sqls.Add(Sql.Builder.Append("delete from tn_MessagesInSessions where SessionId = @0 and MessageId = @1", session.SessionId, entity.MessageId));
                }

                using (var transaction = dao.GetTransaction())
                {
                    affectCount = dao.Execute(sqls);
                    if (sessions.Count == 1)
                    {
                        affectCount += base.Delete(entity);
                    }
                   

                    transaction.Complete();
                }
            }

            dao.CloseSharedConnection();

            #region 更新缓存

            //更新私信会话的缓存
            var sessionCacheHelper = EntityData.ForType(typeof(MessageSession)).RealTimeCacheHelper;
            sessionCacheHelper.IncreaseAreaVersion("UserId", entity.SenderUserId);
            sessionCacheHelper.IncreaseAreaVersion("UserId", entity.ReceiverUserId);
            sessions.ForEach((n) =>
            {
                sessionCacheHelper.IncreaseEntityCacheVersion(n.SessionId);
            });

             RealTimeCacheHelper.IncreaseAreaVersion("ReceiverUserId", entity.ReceiverUserId);
            #endregion 更新缓存

            return affectCount;
        }

        /// <summary>
        /// 更新私信的阅读状态
        /// </summary>
        /// <param name="sessionId">私信会话Id</param>
        /// <param name="userId">会话拥有者UserId</param>
        public bool SetIsRead(long sessionId, long userId)
        {
            var sql = Sql.Builder;
            bool isRead = false;

            var dao = CreateDAO();
            dao.OpenSharedConnection();

            sql.Select("*")
               .From("tn_Messages")
               .Append("Where ReceiverUserId = @0 and IsRead = 0", userId)
               .Append(" and exists(select 1 from tn_MessagesInSessions where tn_Messages.MessageId = tn_MessagesInSessions.MessageId and tn_MessagesInSessions.SessionId = @0)", sessionId);

            //获取未读私信Id
            List<Message> messages = dao.Fetch<Message>(sql);

            if (messages.Count() > 0)
            {
                foreach (var item in messages)
                {
                    item.IsRead = true;
                    dao.Update(item);
                    OnUpdated(item);  

                }

                //更新会话的未读信息数
                MessageSessionRepository repository = new MessageSessionRepository();
                MessageSession session = repository.Get(sessionId);
                if (session != null)
                {
                    session.UnreadMessageCount = session.UnreadMessageCount > messages.Count() ? session.UnreadMessageCount - messages.Count() : 0;
                    repository.Update(session);
                }

                #region 处理缓存

                RealTimeCacheHelper.IncreaseAreaVersion("ReceiverUserId", userId);

                #endregion 处理缓存

                isRead = true;
            }

            dao.CloseSharedConnection();

            return isRead;
        }

        /// <summary>
        /// 获取未读私信数
        /// </summary>
        public int GetUnReadCount(long userId)
        {
            string cacheKey = RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "ReceiverUserId", userId) + "UnreadCount";
            int? count = null;
            cacheService.TryGetValue<int?>(cacheKey, out count);
            if (count == null)
            {
                var sql = Sql.Builder;
            sql.Select("Count(MessageId)")
               .From("tn_Messages")
               .Where("ReceiverUserId = @0", userId)
               .Where("IsRead = 0");
                count = CreateDAO().FirstOrDefault<int?>(sql);

                count = count ?? 0;
                cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
            }
            return count.Value;
        }

        /// <summary>
        /// 获取用户的前N条私信
        /// </summary>
        /// <param name="userId">私信拥有者Id</param>
        /// <param name="sortBy">私信排序字段</param>
        /// <param name="topNumber">获取的前N条数据</param>
        public IEnumerable<Message> GetTopMessagesOfUser(long userId, SortBy_Message? sortBy, int topNumber)
        {
            var sql = Sql.Builder;
            sql.Where("ReceiverUserId = @0", userId);
            switch (sortBy)
            {
                case SortBy_Message.IsRead:
                    sql.OrderBy("IsRead asc");
                    sql.OrderBy("MessageId desc");
                    break;
                case SortBy_Message.DateCreated_Desc:
                    sql.OrderBy("MessageId desc");
                    break;
            }
            return GetTopEntities(topNumber, sql);
           
        }

        /// <summary>
        /// 获取会话最早的一条消息
        /// </summary>
        /// by liux
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public Message GetFirstMssageOfSession(long sessionId)
        {
            var sql = Sql.Builder;
            sql.Select("mi.MessageId")
                .From("tn_MessageSessions ms").InnerJoin("tn_MessagesInSessions mi").On("ms.SessionId=mi.SessionId")
                .Where("mi.SessionId=@0", sessionId);
            var id = CreateDAO().FirstOrDefault<long>(sql);
            return Get(id);
        }

       
    }
}