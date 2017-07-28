//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tunynet.CMS;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    public class ContentItemCommentUrlGetter : ICommentUrlGetter
    {

        private Authorizer authorizer;
        private IUserService userService;
        public ContentItemCommentUrlGetter(Authorizer authorizer, IUserService userService)
        {
            this.authorizer = authorizer;
            this.userService = userService;
        }

        /// <summary>
        /// 租户类型Id
        /// </summary>
        public string TenantTypeId
        {
            get { return TenantTypeIds.Instance().ContentItem(); }
        }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsManager(long userId)
        {
            var user = userService.GetUser(userId);
            var result = authorizer.IsCategoryManager(this.TenantTypeId, user,null);
            return result;
        }
        /// <summary>
        /// 获取被评论对象
        /// </summary>
        /// <param name="commentedObjectId"></param>
        /// <returns></returns>
        public CommentedObject GetCommentedObject(long commentedObjectId)
        {
            var contentItem = new ContentItemRepository().Get(commentedObjectId);
            if (contentItem != null)
            {
                CommentedObject commentedObject = new CommentedObject();
                commentedObject.DetailUrl = SiteUrls.Instance().CMSDetail(commentedObjectId);
                commentedObject.Name = contentItem.Subject;
                commentedObject.Author = contentItem.Author;
                commentedObject.UserId = contentItem.UserId;
                commentedObject.contentModelName = contentItem.ContentModel.ModelName;
                return commentedObject;
            }
            return null;
        }
    }
}