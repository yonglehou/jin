//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Tunynet;
using Tunynet.Common;
using Tunynet.Events;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户资料业务逻辑类
    /// </summary>
    public class UserProfileService
    {
        private IProfileRepository profileRepository;
        private IUserService userService;
        private IUserRepository userRepository;
        public UserProfileService(IProfileRepository profileRepository, IUserService userService, IUserRepository userRepository)
        {
            this.profileRepository = profileRepository;
            this.userService = userService;
            this.userRepository = userRepository;
        }

        #region Create & Update & Get

        /// <summary>
        /// 创建用户资料（基本资料）
        /// </summary>
        /// <param name="userProfile">用户资料</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public bool Create(UserProfile userProfile)
        {
            if (userProfile == null)
                return false;

            //需要添加检查是否存在的方法
            //如果存在则更新用户资料
            if (profileRepository.UserIdIsExist(userProfile.UserId))
            {
                this.Update(userProfile);
                return false;
            }

            EventBus<UserProfile>.Instance().OnBefore(userProfile, new CommonEventArgs(EventOperationType.Instance().Create()));
                        
            profileRepository.Insert(userProfile);
            profileRepository.UpdateIntegrity(userProfile.UserId);

            EventBus<UserProfile>.Instance().OnAfter(userProfile, new CommonEventArgs(EventOperationType.Instance().Create()));
            return userProfile.UserId > 0;
        }

        /// <summary>
        /// 更新用户资料（基本资料）
        /// </summary>
        /// <param name="userProfile">用户资料</param>
        public void Update(UserProfile userProfile)
        {
            if (userProfile == null)
                return;
            EventBus<UserProfile>.Instance().OnBefore(userProfile, new CommonEventArgs(EventOperationType.Instance().Update()));
            profileRepository.Update(userProfile);
            profileRepository.UpdateIntegrity(userProfile.UserId);

            User user = userService.GetFullUser(userProfile.UserId);
            if (user.HasAvatar==1)
            {
                userRepository.UpdateAvatar(user, user.HasAvatar);
            }

            EventBus<UserProfile>.Instance().OnAfter(userProfile, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        /// 删除用户资料
        /// </summary>
        /// <param name="userProfile">用户资料</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool Delete(long userId)
        {
            UserProfile userProfile = profileRepository.Get(userId);
            if (userProfile == null)
                return false;
            EventBus<UserProfile>.Instance().OnBefore(userProfile, new CommonEventArgs(EventOperationType.Instance().Delete()));
            profileRepository.Delete(userProfile);

         
            EventBus<UserProfile>.Instance().OnAfter(userProfile, new CommonEventArgs(EventOperationType.Instance().Delete()));
            //同时删除教育经历、工作经历、个人标签、头像文件等
            return true;
        }


        #endregion Create & Update & Get

        #region Get & Gets

        /// <summary>
        /// 获取用户资料
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        public UserProfile Get(long userId)
        {
            return profileRepository.Get(userId);
        }
        
        /// <summary>
        /// 根据Id列表获取UserProfile的实体列表
        /// </summary>
        /// <param name="entityIds">UserProfile的Id列表</param>
        /// <returns>UserProfile的实体列表</returns>
        public IEnumerable<UserProfile> GetUserProfiles(IEnumerable<long> entityIds)
        {
            return profileRepository.PopulateEntitiesByEntityIds<long>(entityIds);
        }

        #endregion Get & Gets


        #region 辅助方法

        ///// <summary>
        /////  获取基本用户资料的完成度
        ///// </summary>
        ///// <param name="userProfile">用户资料</param>
        ///// <returns></returns>
        //private int CountIntegrity(UserProfile userProfile)
        //{
        //    IUserProfileSettingsManager userProfileSettingsManager = DIContainer.Resolve<IUserProfileSettingsManager>();
        //    UserProfileSettings userProfileSettings = userProfileSettingsManager.GetUserProfileSettings();
        //    int[] integrityItems = userProfileSettings.IntegrityProportions;
        //    int integrity = integrityItems[(int)ProfileIntegrityItems.Birthday];

        //    IUser user = new UserService().GetUser(userProfile.UserId, false);
        //    integrity += (user.HasAvatar ? integrityItems[(int)ProfileIntegrityItems.Avatar] : 0);
        //    integrity += (userProfile.HasHomeAreaCode ? integrityItems[(int)ProfileIntegrityItems.HomeArea] : 0);
        //    integrity += (userProfile.HasIM ? integrityItems[(int)ProfileIntegrityItems.IM] : 0);
        //    integrity += (userProfile.HasIntroduction ? integrityItems[(int)ProfileIntegrityItems.Introduction] : 0);
        //    integrity += (userProfile.HasMobile ? integrityItems[(int)ProfileIntegrityItems.Mobile] : 0);
        //    integrity += (userProfile.HasNowAreaCode ? integrityItems[(int)ProfileIntegrityItems.NowArea] : 0);
        //    return integrity;
        //}

        #endregion 辅助方法
    }
}