//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 贴吧创建
    /// </summary>
    public class SectionEditModel
    {

        /// <summary>
        /// 贴吧Id
        /// </summary>
        public long SectionId { get; set; }

        /// <summary>
        ///吧主用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///贴吧名称
        /// </summary>
        [Display(Name = "贴吧名称")]
        [Required(ErrorMessage = "请输入贴吧名称")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "贴吧名称至少2位,并且不能超过32位字符")]
        public string Name { get; set; }

        /// <summary>
        /// 一句话描述
        /// </summary>
        [Display(Name = "一句话描述")]
        public string Description { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 贴吧管理员集合
        /// </summary>
        [Display(Name = "贴吧管理员")]
        public List<string> SectionManagers { get; set; }

        /// <summary>
        /// 吧主
        /// </summary>
        [Display(Name = "吧主")]
        public List<string> SectionOwner{ get; set; }

        /// <summary>
        /// 贴子类别ID
        /// </summary>
        public string ThreadCategoryId { get; set; }

        /// <summary>
        /// 贴子类别名称
        /// </summary>
        public string ThreadCategoryName { get; set; }

        /// <summary>
        /// 贴子类别名称的拼接字符串
        /// </summary>
        public string ThreadCategoryNames { get; set; }

        /// <summary>
        /// 贴子类别ID的拼接字符串
        /// </summary>
        public string ThreadCategoryIds { get; set; }

        /// <summary>
        /// 贴子类别集合
        /// </summary>
        public List<Category> ThreadCategories { get; set; }

        /// <summary>
        /// 是否启用贴吧
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 判断是否有Logo
        /// </summary>
        public bool HasLogoImage
        {
            get
            {
                return FeaturedImageAttachmentId > 0;
            }
        }

        /// <summary>
        /// 贴吧logoId
        /// </summary>
        public long FeaturedImageAttachmentId { get; set; }

        /// <summary>
        /// 是否启用贴子分类
        /// </summary>
        public bool EnabledThreadCategory { get; set; }
    }
}
