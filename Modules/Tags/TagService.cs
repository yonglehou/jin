//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

namespace Tunynet.Common
{
    /// <summary>
    /// 标签业务逻辑类
    /// </summary>
    public class TagService : TagService<Tag>
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        public TagService(string tenantTypeId)
            : base(tenantTypeId)
        { }
    }
}
