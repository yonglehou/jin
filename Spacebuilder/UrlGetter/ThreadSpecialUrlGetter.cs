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
    public class ThreadSpecialUrlGetter : ISpecialContentItemUrlGetter
    {

        /// <summary>
        /// 租户类型Id
        /// </summary>
        public string TenantTypeId
        {
            get { return TenantTypeIds.Instance().Thread(); }
        }
        /// <summary>
        /// 获取被加特殊对象url
        /// </summary>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <returns></returns>
        public string GetSpecialContentItemDetailUrl(long commentedObjectId, string tenantTypeId = null)
        {
            if (tenantTypeId == TenantTypeId)
            {
                return SiteUrls.Instance().ThreadDetail(commentedObjectId);
            }
            return string.Empty;
        }


    }
}