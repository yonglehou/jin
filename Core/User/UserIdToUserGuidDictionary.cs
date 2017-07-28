//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Common
{
    /// <summary>
    /// 查询UserID与UserGuid的查询器
    /// </summary>
    public abstract class UserIdToUserGuidDictionary
    {

        private static ConcurrentDictionary<long, string> dictionaryOfUserIdToUserGuid = new ConcurrentDictionary<long, string>();
        private static ConcurrentDictionary<string, long> dictionaryOfUserGuidToUserId = new ConcurrentDictionary<string, long>();

        #region Instance

        private static volatile UserIdToUserGuidDictionary _defaultInstance = null;
        private static readonly object lockObject = new object();

        /// <summary>
        /// 获取UserIdToUserGuidDictionary实例
        /// </summary>
        /// <returns></returns>
        private static UserIdToUserGuidDictionary Instance()
        {
            if (_defaultInstance == null)
            {
                lock (lockObject)
                {
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = DIContainer.Resolve<UserIdToUserGuidDictionary>();
                        if (_defaultInstance == null)
                            throw new ExceptionFacade("未在DIContainer注册UserIdToUserGuidDictionary的具体实现类");
                    }
                }
            }
            return _defaultInstance;
        }

        #endregion

        /// <summary>
        /// 根据用户Id获取用户Guid
        /// </summary>
        /// <returns>
        /// 用户名
        /// </returns>
        protected abstract string GetUserGuidByUserId(long userId);

        /// <summary>
        /// 根据用户Guid获取用户Id
        /// </summary>
        /// <returns>
        /// 用户Id
        /// </returns>
        protected abstract long GetUserIdByUserGuid(string userGuid);


        /// <summary>
        /// 通过UserId获取UserGuid
        /// </summary>
        /// <param name="userId">userId</param>
        public static string GetUserGuid(long userId)
        {
            if (dictionaryOfUserIdToUserGuid.ContainsKey(userId))
                return dictionaryOfUserIdToUserGuid[userId];
            string userGuid = Instance().GetUserGuidByUserId(userId);
            if (!string.IsNullOrEmpty(userGuid))
            {
                dictionaryOfUserIdToUserGuid[userId] = userGuid;
                if (!dictionaryOfUserGuidToUserId.ContainsKey(userGuid))
                    dictionaryOfUserGuidToUserId[userGuid] = userId;
                return userGuid;
            }
            return string.Empty;
        }

        /// <summary>
        /// 通过UserGuid获取UserId
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public static long GetUserId(string userGuid)
        {
            if (dictionaryOfUserGuidToUserId.ContainsKey(userGuid))
                return dictionaryOfUserGuidToUserId[userGuid];
            long userId = Instance().GetUserIdByUserGuid(userGuid);
            if (userId > 0)
            {
                dictionaryOfUserGuidToUserId[userGuid] = userId;
                if (!dictionaryOfUserIdToUserGuid.ContainsKey(userId))
                    dictionaryOfUserIdToUserGuid[userId] = userGuid;
            }
            return userId;
        }

        /// <summary>
        /// 移除UserId
        /// </summary>
        /// <param name="userId">userId</param>
        internal static void RemoveUserId(long userId)
        {
            string userName;
            dictionaryOfUserIdToUserGuid.TryRemove(userId, out userName);
        }

        /// <summary>
        /// 移除UserGuid
        /// </summary>
        /// <param name="userGuid">userGuid</param>
        internal static void RemoveUserGuid(string userGuid)
        {
            long userId;
            dictionaryOfUserGuidToUserId.TryRemove(userGuid, out userId);
        }
    }
}
