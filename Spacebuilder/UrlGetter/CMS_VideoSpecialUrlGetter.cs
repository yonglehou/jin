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
    /// <summary>
    /// 视频urlGetter
    /// </summary>
    public class CMS_VideoSpecialUrlGetter : ISpecialContentItemUrlGetter
    {

        /// <summary>
        /// 租户类型Id
        /// </summary>
        public string TenantTypeId
        {
            get { return TenantTypeIds.Instance().CMS_Video(); }
        }


        /// <summary>
        /// 获取被加特殊对象url
        /// </summary>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <returns></returns>
        public string GetSpecialContentItemDetailUrl(long commentedObjectId,  string tenantTypeId = null)
        {
            if (tenantTypeId == TenantTypeId)
            {
                return SiteUrls.Instance().CMSVideoDetail(commentedObjectId);
            }
            return string.Empty;
        }

       
    }
}