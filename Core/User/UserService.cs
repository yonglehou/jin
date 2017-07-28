//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tunynet.Common;
using Tunynet.Events;
using Tunynet;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户业务逻辑
    /// </summary>
    public class UserService : IUserService
    {
        private IUserRepository userRepository;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="userRepository"></param>
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// 封禁用户 
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="banDeadline">封禁截止日期</param>
        /// <param name="banReason">封禁原因</param>
        public void BanUser(long userId, DateTime banDeadline, string banReason)
        {
            if (banDeadline <= DateTime.Now)
                return;

            User user = userRepository.GetUser(userId);
            user.IsBanned = true;
            user.BanReason = banReason;
            user.BanDeadline = banDeadline;
            user.ForceLogin = true;
            userRepository.Update(user);
            EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(EventOperationType.Instance().BanUser()));
        }

        /// <summary>
        /// 解禁用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        public void UnbanUser(long userId)
        {
            User user = userRepository.GetUser(userId);
            user.IsBanned = false;
            user.BanReason = string.Empty;
            user.BanDeadline = DateTime.Now;
            user.ForceLogin = false;
            userRepository.Update(user);
            EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(EventOperationType.Instance().UnbanUser()));
        }

        /// <summary>
        /// 设置用户管制状态
        /// </summary>
        /// <param name="userIds">用户Id集合</param>
        /// <param name="isModerated">是否被管制</param>
        public void SetModeratedStatus(IEnumerable<long> userIds, bool isModerated)
        {
            List<User> users = new List<User>();
            foreach (var userId in userIds)
            {
                User user = userRepository.GetUser(userId);
                if (user == null)
                    continue;
                if (user.IsModerated == isModerated)
                    continue;
                user.IsModerated = isModerated;
                user.IsForceModerated = isModerated;
                userRepository.Update(user);
                users.Add(user);
            }
            if (users.Count > 0)
            {
                string eventOperationType = isModerated ? EventOperationType.Instance().ModerateUser() : EventOperationType.Instance().CancelModerateUser();
                foreach (var user in users)
                {
                    EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(eventOperationType));
                }
                
            }
        }


        #region Get & Gets

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userIsOnline">该用户是否在线，更新用户在线状态</param>
        public IUser GetUser(long userId)
        {
            if (userId <= 0)
                return null;
            return userRepository.GetUser(userId);
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userGuid">用户Guid</param>
        public IUser GetUserByGuid(string userGuid)
        {
            if (!string.IsNullOrEmpty(userGuid))
            {
                return userRepository.GetUserByGuid(userGuid);
            }
            return null;
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userName">用户名</param>
        public IUser GetUser(string userName)
        {
            long userId = UserIdToUserNameDictionary.GetUserId(userName);
            return GetUser(userId);
        }

        /// <summary>
        /// 根据帐号邮箱获取用户
        /// </summary>
        /// <param name="accountEmail">帐号邮箱</param>
        public IUser GetUserByEmail(string accountEmail, UserStatus? statue = UserStatus.IsActivated)
        {
            long userId = userRepository.GetUserIdByEmail(accountEmail, statue);
            return GetUser(userId);
        }

        /// <summary>
        /// 根据手机号获取用户
        /// </summary>
        /// <param name="accountMobile">手机号</param>
        public IUser GetUserByMobile(string accountMobile, UserStatus? statue = UserStatus.IsActivated)
        {
            long userId = userRepository.GetUserIdByMobile(accountMobile, statue);
            return GetUser(userId);
        }

        /// <summary>
        /// 依据UserId集合组装IUser集合
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public IEnumerable<IUser> GetUsers(IEnumerable<long> userIds)
        {
            return userRepository.PopulateEntitiesByEntityIds<long>(userIds);
        }

        #endregion

        /// <summary>
        /// 更新用户等级
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rank"></param>
        public void UpdateRank(long userId, int rank)
        {
            IUser user = GetUser(userId);
            if (user == null)
                return;
            EventBus<IUser, UpdateRankEventArgs>.Instance().OnBefore(user, new UpdateRankEventArgs(rank));
            if (userRepository.UpdateRank(user, rank) > 0)
                EventBus<IUser, UpdateRankEventArgs>.Instance().OnAfter(user, new UpdateRankEventArgs(rank));
        }

        /// <summary>
        /// 更新用户积分
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="experiencePoints">经验值</param>
        /// <param name="reputationPoints">威望值</param>
        /// <param name="tradePoints">交易经验值</param>
        /// <param name="tradePoints2">交易经验值2</param>
        /// <param name="tradePoints3">交易经验值3</param>
        /// <param name="tradePoints4">交易经验值4</param>
        public void ChangePoints(long userId, int experiencePoints, int reputationPoints, int tradePoints, int tradePoints2 = 0, int tradePoints3 = 0, int tradePoints4 = 0)
        {
            IUser user = GetUser(userId);
            if (user == null)
                return;
            EventBus<IUser, ChangePointsEventArgs>.Instance().OnBefore(user, new ChangePointsEventArgs(experiencePoints, reputationPoints, tradePoints, tradePoints2, tradePoints3, tradePoints4));
            if (userRepository.ChangePoints(user, experiencePoints, reputationPoints, tradePoints, tradePoints2, tradePoints3, tradePoints4) > 0)
                EventBus<IUser, ChangePointsEventArgs>.Instance().OnAfter(user, new ChangePointsEventArgs(experiencePoints, reputationPoints, tradePoints, tradePoints2, tradePoints3, tradePoints4));
        }

       

        /// <summary>
        /// 冻结用户的交易积分
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="tradePoints">准备冻结的积分</param>
        public void FreezeTradePoints(long userId, int tradePoints)
        {
            User user = userRepository.GetUser(userId);
            user.FrozenTradePoints += tradePoints;
            user.TradePoints -= tradePoints;
            userRepository.Update(user);
        }

        /// <summary>
        /// 解冻用户的交易积分
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="tradePoints">解冻的积分</param>
        public void UnfreezeTradePoints(long userId, int tradePoints)
        {
            User user = userRepository.GetUser(userId);
            user.FrozenTradePoints -= tradePoints;
            user.TradePoints += tradePoints;
            userRepository.Update(user);
        }

        /// <summary>
        /// 减少冻结的交易积分（完成交易时使用）
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="frozenTradePoints">需减少的冻结交易积分值</param>
        public void ReduceFrozenTradePoints(long userId, int frozenTradePoints)
        {
            User user = userRepository.GetUser(userId);
            user.FrozenTradePoints -= frozenTradePoints;
            userRepository.Update(user);
        }


        /// <summary>
        /// 根据用户Id集合获取实体集合
        /// </summary>
        /// <param name="userIds">用户Id集合</param>
        /// <param name="topNumber">获取记录数</param>
        /// <returns></returns>
        public IEnumerable<IUser> GetUsers(IEnumerable<long> userIds, int topNumber)
        {
            if (userIds == null)
                return new List<IUser>();

            return userRepository.PopulateEntitiesByEntityIds(userIds);
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="roleIds">用户角色Id</param>
        /// <param name="minRank">最低等级</param>
        /// <param name="maxRank">最高等级</param>
        public IEnumerable<IUser> GetUsers(List<long> roleIds, int minRank = 0, int maxRank = 0)
        {
            return userRepository.GetUsers(roleIds, minRank, maxRank);
        }

        /// <summary>
        /// 依据UserGuid集合组装IUser集合
        /// </summary>
        /// <param name="userGuids">用户Guid集合</param>
        /// <returns></returns>
        public IEnumerable<IUser> GetUsersByGuids(IEnumerable<string> userGuids)
        {
            //todo:暂时还用不着
            throw new NotImplementedException();
        }


    }

}
