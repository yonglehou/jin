using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Tunynet.Common.Configuration;
using Tunynet.Common;
using Tunynet.Imaging;

namespace Tunynet.Spacebuilder
{
    public class SiteSettingEditModel
    {
        #region 站点设置

        //站点名称
        [Display(Name = "站点名称")]
        [Required(ErrorMessage ="请输入一个站点名称")]
        public string SiteName { get; set; }

        //站点描述
        [Display(Name = "站点宣传语")]
        [AllowHtml]
        public string SiteDescription { get; set; }

        /// <summary>
        /// 版权声明
        /// </summary>
        [Display(Name ="版权声明")]
        [AllowHtml]
        public string Copyright { get; set; }

        //备案信息
        [Display(Name = "备案信息")]
        [AllowHtml]
        public string BeiAnScript { get; set; }

        //站点统计脚本
        [Display(Name = "站点统计脚本")]
        [AllowHtml]
        public string StatScript { get; set; }

        //页脚链接
        [Display(Name = "页脚链接")]
        [AllowHtml]
        public string Links { get; set; }

        //是否允许匿名访问
        [Display(Name = "匿名访问")]
        public bool EnableAnonymousBrowse { get; set; }

        //站点风格设置
        [Display(Name = "风格设置")]
        public SiteStyleType SiteStyle { get; set; }

        //Meta Keywords
        [Display(Name = "Meta Keywords")]
        public string SearchMetaKeyWords { get; set; }

        //Meta Description
        [Display(Name = "Meta Description")]
        public string SearchMetaDescription { get; set; }

        #endregion

        #region 用户设置

        //注册方式
        [Display(Name = "注册方式")]
        public RegisterType RegisterType { get; set; }

        //是否启用邮箱注册
        public bool isEmail { get; set; }

        //是否启用手机注册
        public bool isMobile { get; set; }

        //注册时禁止使用的昵称
        [Display(Name = "注册时禁止使用的昵称")]
        public string DisallowedUserNames { get; set; }

        //朋友邀请码有效期
        [Display(Name = "朋友邀请码有效期")]
        [Required(ErrorMessage ="请输入邀请码有效期")]
        [RegularExpression("^\\+?[1-9][0-9]*$",ErrorMessage ="请输入一个大于等于1的整数")]
        public int InvitationCodeTimeLiness { get; set; }

        //是否启用用户管制
        [Display(Name = "用户管制")]
        public bool AutomaticModerated { get; set; }

        //解除管制所需经验值
        [Display(Name = "解除管制所需经验值")]
        [Required(ErrorMessage = "请输入解除管制所需经验值")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "请输入一个非负整数")]
        public int NoModeratedUserPoint { get; set; }

        #endregion

        #region 水印设置

        //水印类型
        [Display(Name = "水印类型")]
        public WatermarkType WatermarkType { get; set; }

        //水印位置
        [Display(Name = "水印位置")]
        public AnchorLocation WatermarkLocation { get; set; }

        //水印文字
        [Display(Name = "水印文字")]
        public string WatermarkText { get; set; }

        //水印图片名称
        [Display(Name = "水印图片名称")]
        public string WatermarkImageName { get; set; }

        //水印最小宽度
        [Display(Name = "宽")]
        [Required(ErrorMessage ="请输入水印最小宽度")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "请输入一个非负整数")]
        public int WatermarkMinWidth { get; set; }

        //水印最小高度
        [Display(Name = "高")]
        [Required(ErrorMessage = "请输入水印最小高度")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "请输入一个非负整数")]
        public int WatermarkMinHeight { get; set; }

        #endregion

    }
}
