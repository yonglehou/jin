//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 附件
    /// </summary>
    public class AttachmentEditModel
    {

        /// <summary>
        ///Id
        /// </summary>
        public long AttachmentId { get; set; }

        /// <summary>
        ///附件描述
        /// </summary>
        [Required(ErrorMessage = "请输入描述")]
        [StringLength(2000, ErrorMessage = "最多可输入{1}个字")]
        public string Discription { get; set; }

    }
}
