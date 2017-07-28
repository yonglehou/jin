//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tunynet.Common;
using Tunynet.Post;

namespace Tunynet.Spacebuilder
{
    public class ThreadCommentUrlGetter : ICommentUrlGetter
    {

        private Authorizer authorizer;
        private IUserService userService;
        public ThreadCommentUrlGetter (Authorizer authorizer, IUserService userService)
        {
            this.authorizer = authorizer;
            this.userService = userService;
        }
        /// <summary>
        /// 租户类型Id
        /// </summary>
        public string TenantTypeId
        {
            get { return TenantTypeIds.Instance().Thread(); }
        }
        /// <summary>
        /// 是否帖子管理员
        /// </summary>
        public bool IsManager(long userId)
        {
            var user =  userService.GetUser(userId);
            var result = authorizer.IsPostManager(user);
            return result;
        }

        /// <summary>
        /// 获取被评论对象
        /// </summary>
        /// <param name="commentedObjectId"></param>
        /// <returns></returns>
        public CommentedObject GetCommentedObject(long commentedObjectId)
        {
            var thread = new ThreadRepository().Get(commentedObjectId);
            if (thread != null)
            {
                CommentedObject commentedObject = new CommentedObject();
                commentedObject.DetailUrl = SiteUrls.Instance().ThreadDetail(commentedObjectId);
                commentedObject.Name = thread.Subject;
                commentedObject.Author = thread.Author;
                commentedObject.UserId = thread.UserId;
                commentedObject.contentModelName = null;
                return commentedObject;
            }
            return null;
        }
    }
}