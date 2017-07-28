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
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Tunynet.Utilities;

using Tunynet;

namespace Tunynet.Common
{
    /// <summary>
    /// 编辑贴子的EditModel
    /// </summary>
    public class ThreadEditModel
    {


        /// <summary>
        /// 贴子Id
        /// </summary>
        public long ThreadId { get; set; }


        /// <summary>
        /// 贴子标题
        /// </summary>
        [WaterMark(Content = "标题")]
        [Required(ErrorMessage = "请输入标题")]
        [StringLength(64, ErrorMessage = "不能超过64个字符")]
        [DataType(DataType.Text)]
        public string Subject { get; set; }


        /// <summary>
        /// 贴子的内容
        /// </summary>
        [Required(ErrorMessage = "请输入内容")]
        [AllowHtml]
        [DataType(DataType.Html)]
        public string Body { get; set; }

        /// <summary>
        /// 所属贴吧Id
        /// </summary>
        public long SectionId { get; set; }

        /// <summary>
        /// 贴子分类
        /// </summary>
        public long CategoryId { get; set; }

        #region 提交表单扩展

        /// <summary>
        /// Body Imgid
        /// </summary>
        public string BodyImageAttachmentId { get; set; }

        /// <summary>
        /// Body Imgids
        /// </summary>
        public IEnumerable<long> BodyImageAttachmentIds
        {
            get
            {
                if (string.IsNullOrEmpty(BodyImageAttachmentId))
                    return null;
                var bodyImageIds = new List<string>(BodyImageAttachmentId.Split(','));
                var bodyImageIdslong = bodyImageIds.Where(n => n.Length > 0);
                if (bodyImageIdslong.Count() > 0)
                    return bodyImageIdslong.Select(n => Convert.ToInt64(n)).ToList();
                return null;
            }
        }

        #endregion



    }
}