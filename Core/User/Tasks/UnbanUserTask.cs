//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using Tunynet.Tasks;
using System.Collections.Generic;
using System.Linq;
using Tunynet.Common.Configuration;
using Tunynet.Settings;

namespace Tunynet.Common
{
    /// <summary>
    /// 执行解封用户任务
    /// </summary>
    public class UnbanUserTask : ITask
    {
        /// <summary>
        /// 任务执行的内容
        /// </summary>
        /// <param name="taskDetail">任务配置状态信息</param>
        public void Execute(TaskDetail taskDetail)
        {
            IUserService userService = DIContainer.Resolve<IUserService>();
            UserQuery userQuery = new UserQuery();
            userQuery.IsBanned = true;
            IEnumerable<User> users= userService.GetUsers(userQuery, 10000, 1);
            foreach (var user in users)
            {
                if (user.BanDeadline<=DateTime.Now)
                {
                    userService.UnbanUser(user.UserId);
                }
            }
            UserSettings userSetting = DIContainer.Resolve<ISettingsManager<UserSettings>>().Get();
            ///经验达到一定数值后自动解除管制
            userQuery = new UserQuery();
            userQuery.IsModerated = true;
            users = userService.GetUsers(userQuery, 10000, 1);
              userService.SetModeratedStatus(users.Where(n=>n.ExperiencePoints> userSetting.NoModeratedUserPoint|| n.ExperiencePoints ==userSetting.NoModeratedUserPoint).Select(n=>n.UserId),false);
           




        }
    }
}
