//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.CMS;
using Tunynet.Common;
using Tunynet.Post;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 关注用户扩展
    /// </summary>
    public static class FollowEntityExtension
    {

        /// <summary>
        /// 关注用户实体
        /// </summary>
        /// <param name="followEntity"></param>
        /// <returns></returns>
        public static IUser followuser(this FollowEntity followEntity)
        {
            UserService userService = DIContainer.Resolve<UserService>();
            return userService.GetUser(followEntity.FollowedUserId);
        }

    }
}
