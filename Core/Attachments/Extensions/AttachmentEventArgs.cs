//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Tunynet;
using Tunynet.Common;
using Tunynet.Events;
using Tunynet.FileStore;
using Tunynet.Imaging;

namespace Tunynet.Common
{
    /// <summary>
    ///附件变化自定义事件
    /// </summary>
    public class AttachmentEventArgs : CommonEventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isMobile">是否手机</param>
        /// <param name="tenantTypeId"></param>
        /// <param name="eventOperationType"></param>
        /// 
        public AttachmentEventArgs(string eventOperationType, string tenantTypeId, bool isMobile = false)
            : base(eventOperationType)
        {
            this.isMobile = isMobile;
            this.TenantTypeId = tenantTypeId;
        }

        private bool isMobile;
        /// <summary>
        /// 是否手机
        /// </summary>
        public bool IsMobile
        {
            get { return isMobile; }
        }

        public string TenantTypeId { get; set; }

    }
}