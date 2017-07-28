//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Tunynet;
using Tunynet.Caching;
using Tunynet.Common.Repositories;
using Tunynet.Common;

namespace Tunynet.Common
{
    /// <summary>
    /// 扩展用户业务逻辑
    /// </summary>
    public static class UserProfileExtension
    {
        private static IOnlineUserRepository onlineUserRepository = DIContainer.Resolve<IOnlineUserRepository>();

      
        /// <summary>
        /// 居住地是否存在
        /// </summary>
        public static string HomeAreaName(string HomeAreaCode)
        {
            if (!string.IsNullOrEmpty(HomeAreaCode))
            {
                return @Formatter.FormatArea(HomeAreaCode, false);
            }
            return string.Empty;
        }
        /// <summary>
        /// 判断用户是否在线
        /// </summary>
        public static bool IsOnline( long UserId,string UserName)
        {

            if (UserContext.CurrentUser != null && UserContext.CurrentUser.UserId == UserId)
                return true;
            else
            {
               return  onlineUserRepository.GetLoggedUsers().ContainsKey(UserName);
             }
        }

        /// <summary>
        /// 用户的第三人称
        /// </summary>
        /// <param name="userProfile">用户资料实体</param>
        /// <returns></returns>
        public static string ThirdPerson(this UserProfile userProfile)
        {
            if (userProfile == null)
            {
                return string.Empty;
            }

            if (UserContext.CurrentUser != null && UserContext.CurrentUser.UserId == userProfile.UserId)
            {
                return "我";
            }

            string resourceKey = "Common_";
            switch (userProfile.Gender)
            {
                case GenderType.FeMale:
                    resourceKey += "She";
                    break;
                case GenderType.Male:
                    resourceKey += "He";
                    break;
                default:
                    resourceKey += "Ta";
                    break;
            }

            return Tunynet.ResourceAccessor.GetString(resourceKey);
        }
    }
}