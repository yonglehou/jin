//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tunynet;
using Tunynet.Common;
using Tunynet.Common.Configuration;
using Tunynet.Email;
using Tunynet.Events;
using Tunynet.Settings;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户账户业务逻辑
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private IUserRepository userRepository ;
        private ISettingsManager<UserSettings> userSettingsManager  ;
     
        public MembershipService(IUserRepository userRepository, ISettingsManager<UserSettings> userSettingsManager)
        {
            this.userRepository = userRepository;
            this.userSettingsManager = userSettingsManager;
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user">待创建的用户</param>
        /// <param name="password">密码</param>
        /// <param name="userCreateStatus">用户帐号创建状态</param>
        /// <returns>创建成功返回IUser，创建失败返回null</returns>
        public IUser CreateUser(IUser user, string password, out UserCreateStatus userCreateStatus)
        {

            return CreateUser(user, password, string.Empty, string.Empty, false, out userCreateStatus);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user">待创建的用户</param>
        /// <param name="password">密码</param>
        /// <param name="passwordQuestion">密码问题</param>
        /// <param name="passwordAnswer">密码答案</param>
        /// <param name="ignoreDisallowedUsername">是否忽略禁用的用户名称</param>
        /// <param name="userCreateStatus">用户帐号创建状态</param>
        /// <returns>创建成功返回IUser，创建失败返回null</returns>
        public IUser CreateUser(IUser user, string password, string passwordQuestion, string passwordAnswer, bool ignoreDisallowedUsername, out UserCreateStatus userCreateStatus)
        {
            User user_object = user as User;
            if (user_object == null)
            {
                userCreateStatus = UserCreateStatus.UnknownFailure;
                return null;
            }

            UserSettings userSettings = userSettingsManager.Get();
            user_object.PasswordFormat = (int)userSettings.UserPasswordFormat;
            user_object.Password = UserPasswordHelper.EncodePassword(password, userSettings.UserPasswordFormat);
            user_object.IsModerated = userSettings.AutomaticModerated;
            EventBus<User, CreateUserEventArgs>.Instance().OnBefore(user_object, new CreateUserEventArgs(password));
            user = userRepository.CreateUser(user_object, ignoreDisallowedUsername, out userCreateStatus);

            if (userCreateStatus == UserCreateStatus.Created)
            {
                UserIdToUserNameDictionary.RemoveUserId(user.UserId);
                UserIdToUserNameDictionary.RemoveUserName(user.UserName);
                EventBus<User, CreateUserEventArgs>.Instance().OnAfter(user_object, new CreateUserEventArgs(password));
                EventBus<User>.Instance().OnAfter(user_object,new CommonEventArgs(EventOperationType.Instance().Create()));
            }
                
            
            return user;
        }

        /// <summary>
        /// 用户名验证
        /// </summary>
        /// <param name="userName">待创建的用户名</param>
        /// <param name="ignoreDisallowedUsername">是否忽略禁用的用户名称</param>
        /// <param name="userCreateStatus">用户帐号创建状态</param>
        public void RegisterValidate(string userName, bool ignoreDisallowedUsername, out UserCreateStatus userCreateStatus)
        {
            userRepository.RegisterValidate(userName, ignoreDisallowedUsername, out userCreateStatus);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="takeOverUserName">用于接管删除用户时不能删除的内容(例如：用户创建的群组)</param>
        /// <returns></returns>
        public UserDeleteStatus DeleteUser(long userId, string takeOverUserName)
        {
            return DeleteUser(userId, takeOverUserName, false);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="takeOverUserName">用于接管删除用户的内容(例如：用户创建的群组)</param>
        /// <param name="takeOverAll">是否接管被删除用户的所有内容</param>
        /// <remarks>接管被删除用户的所有内容</remarks>
        /// <returns></returns>
        public UserDeleteStatus DeleteUser(long userId, string takeOverUserName, bool takeOverAll)
        {
            User user = userRepository.Get(userId);
            if (user == null)
                return UserDeleteStatus.DeletingUserNotFound;

            if (takeOverAll)
            {
                long takeOverUserId = userRepository.GetUserIdByUserName(takeOverUserName);
                User takeOverUser = userRepository.Get(takeOverUserId);
                if (takeOverUser == null)
                    return UserDeleteStatus.InvalidTakeOverUsername;
            }

            user.Status = UserStatus.Delete;
            userRepository.Update(user);

                UserIdToUserNameDictionary.RemoveUserId(userId);
                UserIdToUserNameDictionary.RemoveUserName(user.UserName);
                EventBus<User, DeleteUserEventArgs>.Instance().OnAfter(user, new DeleteUserEventArgs(takeOverUserName, takeOverAll));
                EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(EventOperationType.Instance().Delete()));
                return UserDeleteStatus.Deleted;
        
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(IUser user)
        {
            User user_object = user as User;
            if (user_object == null)
                return;
            EventBus<User>.Instance().OnBefore(user_object, new CommonEventArgs(EventOperationType.Instance().Update()));
            userRepository.Update(user_object);
         
            EventBus<User>.Instance().OnAfter(user_object, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        /// 批量激活用户
        /// </summary>
        /// <param name="userIds">用户Id集合</param>
        /// <param name="status">用户账号状态(-1=已删除,1=已激活,0=未激活)</param>
        public void ActivateUsers(IEnumerable<long> userIds, UserStatus status = UserStatus.IsActivated)
        {
            List<User> users = new List<User>();
            foreach (var userId in userIds)
            {
                User user = userRepository.GetUser(userId);
                if (user == null)
                    continue;

                if (user.Status == status)
                    continue;

                user.Status = status;
                user.ForceLogin = false;

                userRepository.Update(user);
                users.Add(user);
            }
            if (users.Count > 0)
            {
                string eventOperationType=string.Empty;
                switch (status)
                {
                    case UserStatus.Delete:
                        eventOperationType = EventOperationType.Instance().DeleteUser();
                        break;
                    case UserStatus.IsActivated:
                        eventOperationType = EventOperationType.Instance().ActivateUser();
                        break;
                    case UserStatus.NoActivated:
                        eventOperationType = EventOperationType.Instance().CancelActivateUser();
                        break;
                  
                }
             
                foreach (var user in users)
                {
                    EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(eventOperationType));
                }
                
            }
        }

        ///	<summary>
        ///	更新密码（需要验证当前密码）
        ///	</summary>
        /// <param name="username">用户名</param>
        ///	<param name="password">当前密码</param>
        ///	<param name="newPassword">新密码</param>
        ///	<returns>更新成功返回true，否则返回false</returns>
        public bool ChangePassword(string username, string password, string newPassword)
        {
            if (ValidateUser(username, password) == UserLoginStatus.Success)
            {
                long userId = userRepository.GetUserIdByUserName(username);
                var user=userRepository.GetUser(userId);
                EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(EventOperationType.Instance().ResetPassword()));
                return ResetPassword(username, newPassword);
            }
                
            return false;
        }

        ///	<summary>
        ///	重设密码（无需验证当前密码，供管理员或忘记密码时使用含手机和邮箱重置）
        ///	</summary>
        /// <param name="username">用户名</param>
        ///	<param name="newPassword">新密码</param>
        ///	<remarks>成功时，会自动发送密码已修改邮件</remarks>
        ///	<returns>更新成功返回true，否则返回false</returns>
        public bool ResetPassword(string username, string newPassword)
        {
            long userId = userRepository.GetUserIdByUserName(username);
            if (userId == 0L)
            {
                //如果是手机号登录
                var mobileRegex = new Regex("^1[3-8]\\d{9}$");
                if (mobileRegex.IsMatch(username))
                {
                    userId = userRepository.GetUserIdByMobile(username);
                }
            }
            if (userId == 0L)
            {
                //如果是邮箱登录
                var emailRegex = new Regex("^([a-zA-Z0-9_.-]+)@([0-9A-Za-z.-]+).([a-zA-Z.]{2,6})$");
                if (emailRegex.IsMatch(username))
                {
                    userId = userRepository.GetUserIdByEmail(username);
                }
            }
            User user = userRepository.Get(userId);
            if (user == null)
                return false;

            string storedPassword = UserPasswordHelper.EncodePassword(newPassword, (UserPasswordFormat)user.PasswordFormat);
            EventBus<User>.Instance().OnBefore(user, new CommonEventArgs(EventOperationType.Instance().ResetPassword()));
            bool result = userRepository.ResetPassword(user, storedPassword);

            if (result)
                EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(EventOperationType.Instance().ResetPassword()));

            return result;
        }

        /// <summary>
        /// 验证提供的用户名和密码是否匹配
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回<see cref="UserLoginStatus"/></returns>
        public UserLoginStatus ValidateUser(string username, string password)
        {
            User loginUser = new User();
            return ValidateUser(username, password, out loginUser);
        }

        /// <summary>
        /// 验证提供的用户名和密码是否匹配(含手机登录和邮箱登录)
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="loginUser">当前用户实体</param>
        /// <returns>返回<see cref="UserLoginStatus"/></returns>
        public UserLoginStatus ValidateUser(string username, string password, out User loginUser)
        {
            var userSetting = DIContainer.Resolve<ISettingsManager<UserSettings>>().Get();
            long userId = UserIdToUserNameDictionary.GetUserId(username);
            if (userId == 0L)
            {
                var mobileRegex = new Regex("^1[3-8]\\d{9}$");
                var emailRegex = new Regex("^([a-zA-Z0-9_.-]+)@([0-9A-Za-z.-]+).([a-zA-Z.]{2,6})$");
                loginUser = null;
                switch (userSetting.RegisterType)
                {
                    case RegisterType.Mobile:
                        //如果是手机号登录
                        if (mobileRegex.IsMatch(username))
                            userId = userRepository.GetUserIdByMobile(username);
                        if (userId == 0L)
                            return UserLoginStatus.NoEmail;
                        break;
                    case RegisterType.Email:
                        //如果是邮箱登录
                        if (emailRegex.IsMatch(username))
                            userId = userRepository.GetUserIdByEmail(username);
                        if (userId == 0L)
                            return UserLoginStatus.NoMobile;
                        break;
                    case RegisterType.MobileOrEmail:
                    case RegisterType.EmailOrMobile:
                        if ( emailRegex.IsMatch(username))
                           userId = userRepository.GetUserIdByEmail(username);
                        if (userId == 0L && mobileRegex.IsMatch(username))
                                userId = userRepository.GetUserIdByMobile(username);
                        break;                   
                } 
            }
           
            loginUser = userRepository.Get(userId);
            if (loginUser == null)
                return UserLoginStatus.InvalidCredentials;

            if (!UserPasswordHelper.CheckPassword(password, loginUser.Password, (UserPasswordFormat)loginUser.PasswordFormat))
                return UserLoginStatus.InvalidCredentials;

            if (loginUser.Status!=  UserStatus.IsActivated)
                return UserLoginStatus.NotActivated;
            if (loginUser.IsBanned)
            {
                if (loginUser.BanDeadline >= DateTime.Now)
                    return UserLoginStatus.Banned;
                else
                {
                    loginUser.IsBanned = false;
                    loginUser.BanDeadline = DateTime.Now;
                    userRepository.Update(loginUser);
                }
            }
            return UserLoginStatus.Success;
        }
        
    }
}