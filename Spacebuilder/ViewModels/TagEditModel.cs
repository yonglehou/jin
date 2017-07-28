using System;
using System.ComponentModel.DataAnnotations;

namespace Tunynet.Spacebuilder
{
    public class TagEditModel
    {
        /// <summary>
        /// 标签Id
        /// </summary>
        public long TagId { get; set; }
        /// <summary>
        /// 租户类型
        /// </summary>
        public string TenantTypeId { get; set; }
        /// <summary>
        /// 内容项目数
        /// </summary>
        public int ItemCount { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        [Display(Name = "标签名称")]
        [Required(ErrorMessage = "请输入标签名称")]
        [StringLength(60,ErrorMessage ="标签名称长度不能超过60个字符")]
        public string TagName { get; set; }
        /// <summary>
        /// 标题图Id
        /// </summary>
        [Display(Name = "标题图")]
        public long ImageAttachmentId { get; set; }
        /// <summary>
        /// 是否为特色标签
        /// </summary>
        [Display(Name = "特色标签")]
        public bool IsFeatured { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "标签描述")]
        public string Description { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
