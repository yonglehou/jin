using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Tunynet.Common
{
    public class SpecialContentTypeEditModel
    {
        /// <summary>
        /// 类型ID（创建后不允许修改）
        /// </summary>
        [Required(ErrorMessage = "此项为必填项")]
        [Display(Name = "类别Id")]
        [RegularExpression("^\\d{1,8}$", ErrorMessage = "请输入正确格式")]
        [Remote("CheckUniqueType", "ControlPanel",ErrorMessage = "类别Id已存在")]
        public int? TypeId { get; set; }

        /// <summary>
        /// 租户类型（所属）
        /// </summary>
        [Display(Name="所属")]
        public string TenantTypeId { get; set; }
        /// <summary>
        /// 内容模型 Key 集合(多个用英文逗号隔开)
        /// </summary>
        public string ContentModelKeys { get; set; }

        /// <summary>
        /// 推荐类型名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [Display(Name = "类别名称")]
        [StringLength(16,  ErrorMessage = "类别名称不能超过{1}位")]
        public string Name { get; set; }

        /// <summary>
        /// 推荐类型描述
        /// </summary>
        //[Required(ErrorMessage = "描述不能为空")]
        [Display(Name = "类别描述")]
        public string Description { get; set; }

        /// <summary>
        /// 是否有图标
        /// </summary>
        public bool HasIcon { get; set; }
        /// <summary>
        /// 是否允许添加外链
        /// </summary>
        [Display(Name = "允许添加外链")]
        public bool AllowExternalLink { get; set; }
        /// <summary>
        /// 菜单文字旁边的图标 url
        /// </summary>
        public string IconSrc { get; set; }

        /// <summary>
        /// 是否包含标题图
        /// </summary>
        [Display(Name = "需要标题图")]
        public bool RequireFeaturedImage { get; set; }

        /// <summary>
        /// 标题图说明
        /// </summary>
        [Display(Name = "标题图尺寸说明")]
        public string FeaturedImageDescrption { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        private DateTime dateCreated = DateTime.Now;

        /// <summary>
        /// 是否为新建类型
        /// </summary>
        public bool isNew { get; set; }

        /// <summary>
        /// 获取已经分裂好的数组
        /// </summary>
        public string[] ContentModelKeyArray
        {
            get { return Regex.Split(this.ContentModelKeys, ",", RegexOptions.IgnoreCase); }
        }

        public DateTime DateCreated
        {
            get
            {
                return dateCreated;
            }

            set
            {
                dateCreated = value;
            }
        }


    }
    
   

}
