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
    public interface IBodyProcessor
    {
        /// <summary>
        /// 解析内容
        /// </summary>
        /// <param name="body">被解析的正文</param>        
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">关联Id（例如：日志Id）</param>
        /// <param name="userId">作者Id</param>
        /// <returns>处理后的文本内容</returns>
        string Process(string body, string tenantTypeId, long associateId, long userId);
    }
}
