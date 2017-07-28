//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Common
{
    public  class AutoMapperConfiguration
    {
        /// <summary>
        /// 初始化autompper 配置
        /// </summary>
        public static void Initialize()
        {
            Mapper.Initialize(cfg => {
                cfg.AddProfile<SourceProfile>();
            });
        }
    }
}
