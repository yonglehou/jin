//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

namespace Tunynet.Common
{
    /// <summary>
    /// 扩展用户业务逻辑
    /// </summary>
    public static class UserProfileExtension
    {
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