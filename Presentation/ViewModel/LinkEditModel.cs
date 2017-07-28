using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.CMS;

namespace Tunynet.Common
{
    public class LinkEditModel
    {
        /// <summary>
        /// 链接主键标识
        /// </summary>
        public long LinkId { get; set; }

        /// <summary>
        ///栏目Id
        /// </summary>
        [Required(ErrorMessage ="请选择链接类别")]
        [Display(Name = "链接类别")]
        public long CategoryId { get; set; }

        /// <summary>
        ///内容模型Id
        /// </summary>
        public int ContentModelId { get; set; }

        /// <summary>
        ///链接名称
        /// </summary>
        [Display(Name ="链接名称")]
        [Required(ErrorMessage = "请输入链接名称")]
        [StringLength(30, ErrorMessage = "最多可输入{1}个字")]
        public string LinkName { get; set; }

        /// <summary>
        ///Logo图Id
        /// </summary>
        [Display(Name = "链接图片")]
        public long ImageAttachmentId { get; set; }

        /// <summary>
        ///Logo图文件（带部分路径）
        /// </summary>
        [RegularExpression(@"^(https?):\/\/([A-z0-9]+[_\-]?[A-z0-9]*\.)*[A-z0-9]+\-?[A-z0-9]+\.[A-z]{2,}(\/.*)*\/?", ErrorMessage = "输入的地址有误")]
        public string FeaturedImage { get; set; }

        /// <summary>
        ///发布者UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///发布者DisplayName
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///链接说明
        /// </summary>
        [Display(Name ="链接说明")]
        [StringLength(200, ErrorMessage = "最多可输入{1}个字")]
        public string Description { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        [Display(Name ="是否启用")]
        public bool IsEnabled { get; set; }

        /// <summary>
        ///发布时间
        /// </summary>
        public DateTime DatePublished { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///最后更新时间
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        ///链接地址
        /// </summary>
        [Display(Name = "链接地址")]
        [Required(ErrorMessage = "请输入链接地址")]
        [RegularExpression(@"^(https?):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?$", ErrorMessage = "输入的地址有误")]
        public string LinkUrl { get; set; } = "http://";

        /// <summary>
        ///排序，默认与主键相同
        /// </summary>
        public long DisplayOrder { get; set; }

        

     

    }


}
