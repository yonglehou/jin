//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Tunynet.Repositories;
using System.Text;
using Tunynet.Caching;
using System.Linq;
using PetaPoco;

namespace Tunynet.Common
{
    /// <summary>
    /// 私信与会话关联数据访问
    /// </summary>
    public class MessageInSessionRepository : Repository<MessageInSession>, IMessageInSessionRepository
    {

        /// <summary>
        /// 获取会话下的所有私信Id
        /// </summary>
        /// <param name="sessionId">会话Id</param>
        /// <param name="topNumber">获取记录数</param>
        public IEnumerable<object> GetMessageIds(long sessionId, int topNumber)
        {
            //获取缓存
            StringBuilder cacheKey = new StringBuilder(RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "SessionId", sessionId));
            cacheKey.Append("MessageIds");

            List<object> messageIds = null;
            cacheService.TryGetValue<List<object>>(cacheKey.ToString(),out messageIds);
            if (messageIds == null)
            {
                //组装sql语句
                var sql = PetaPoco.Sql.Builder;
                sql.Select("MessageId")
                   .From("tn_MessagesInSessions")
                   .Where("SessionId = @0", sessionId)
                   .OrderBy("MessageId desc");

                messageIds = CreateDAO().FetchTop<long>(topNumber, sql).Cast<object>().ToList();
                cacheService.Set(cacheKey.ToString(), messageIds, CachingExpirationType.ObjectCollection);
            }

            return messageIds;
        }

        /// <summary>
        /// 获取会话下的某条私信之前的20条私信Id(移动端使用)
        /// </summary>
        /// <param name="sessionId">会话Id</param>
        /// <param name="topNumber">某条私信的Id</param>
        public IEnumerable<object> GetMessageIds(long sessionId, long oldMessageId)
        {
            //获取缓存
            StringBuilder cacheKey = new StringBuilder(RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "SessionId", sessionId));
            cacheKey.AppendFormat("oldMessageId-{0}", oldMessageId);
            List<object> messageIds = null;
            cacheService.TryGetValue<List<object>>(cacheKey.ToString(), out messageIds);
            if (messageIds == null)
            {
                //组装sql语句
                var sql = Sql.Builder;
                if (oldMessageId == -1)
                {
                    sql.Select("MessageId")
                       .From("tn_MessagesInSessions")
                       .Where("SessionId = @0", sessionId)
                       .OrderBy("MessageId desc");
                }
                else
                {
                    sql.Select("MessageId")
                       .From("tn_MessagesInSessions")
                       .Where("SessionId = @0", sessionId)
                       .Where("MessageId < @0", oldMessageId)
                       .OrderBy("MessageId desc");
                }

                messageIds = CreateDAO().FetchTop<long>(20, sql).Cast<object>().ToList();
                cacheService.Set(cacheKey.ToString(), messageIds, CachingExpirationType.ObjectCollection);
            }

            return  messageIds;
        }
    }
}
