//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Common.Repositories;
using Tunynet.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Tunynet.Common;

namespace Tunynet.Common
{
 
    public class CommentEditModel 
    {
    
        /// <summary>
        ///Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///所有父级评论Id集合
        /// </summary>
        public string ParentIds { get; set; }

        /// <summary>
        ///父评论Id（一级ParentId等于0）
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        ///被评论对象guId
        /// </summary>
        public long CommentedObjectId { get; set; }

        /// <summary>
        ///租户类型Id（4位ApplicationId+2位顺序号）
        /// </summary>
        public string TenantTypeId { get; set; }

      

        /// <summary>
        ///评论人UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///评论人名称
        /// </summary>
        public string Author { get; set; }


        /// <summary>
        ///评论内容
        /// </summary>
        [Display(Name = "提问")]
        [Required(ErrorMessage = "内容不能为空")]
        [StringLength(1000, ErrorMessage = "不能超过1000个字符")]
        [AllowHtml]
        public string Body { get; set; }


        /// <summary>
        ///是否匿名评论
        /// </summary>
        [Display(Name = "匿名提问")]
        public bool IsAnonymous { get; set; }
        /// <summary>
        /// 是否悄悄话
        /// </summary>
        [Display(Name = "公开问题")]
        public bool IsPrivate { get; set; }

        /// <summary>
        ///评论人IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get;  set; }
        /// <summary>
        /// 流转用户
        /// </summary>
        public string[] UserGuid { get; set; }
        /// <summary>
        /// 流转用户
        /// </summary>
        public string[] UserGuidl { get; set; }

        /// <summary>
        ///是否同意
        /// </summary>
        public string IsorNotConsent { get; set; }

        /// <summary>
        ///意见
        /// </summary>
        public string Comment { get; set; }
       
        /// <summary>
        /// 附件列表
        /// </summary>
        [Ignore]
        public IEnumerable<Attachment> Attachments
        {
            get
            {
                AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Comment());
                return attachmentService.GetsByAssociateId(this.Id);

            }
        }

    }
}