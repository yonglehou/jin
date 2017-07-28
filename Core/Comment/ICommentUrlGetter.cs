//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tunynet.Common
{
    /// <summary>
    /// 评论相关Url获取器
    /// </summary>
    public interface ICommentUrlGetter
    {
        /// <summary>
        /// 租户类型Id
        /// </summary>
        string TenantTypeId { get; }
        /// <summary>
        /// 是否此模块的管理员 (用于 权限通过)
        /// </summary>
        bool IsManager(long userId);
        /// <summary>
        /// 获取被评论的对象实体
        /// </summary>
        /// <param name="commentedObjectId"></param>
        /// <returns></returns>
        CommentedObject GetCommentedObject(long commentedObjectId);

      

    }


    public class CommentedObject
    {
        public string DetailUrl { get; set; }

        public string Name { get; set; }

        public long UserId { get; set; }

        public string Author { get; set; }

        public string contentModelName { get; set; }
    }
}
