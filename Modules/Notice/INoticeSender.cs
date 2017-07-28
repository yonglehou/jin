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

namespace Tunynet.Common
{
   public  interface INoticeSender
    {
        /// <summary>
        /// 发送方式
        /// </summary>
        /// <returns></returns>
        int SendMode();

        /// <summary>
        /// 通知发送
        /// </summary>
        /// <param name="notices"></param>
        /// <returns></returns>
        bool Send(Notice notice);
        /// <summary>
        /// 通知发送
        /// </summary>
        /// <param name="notices"></param>
        /// <returns></returns>
        bool Send(IEnumerable<Notice> notice);
    }
}
