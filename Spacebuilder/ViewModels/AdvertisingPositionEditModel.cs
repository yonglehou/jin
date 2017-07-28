using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    public class AdvertisingPositionEditModel
    {
        /// <summary>
        ///广告位Id
        /// </summary>
        [Display(Name ="广告位编码")]
        [Required(ErrorMessage ="请填写广告位编码")]
        //[RegularExpression("^[1-9][0-9]{1,7}$", ErrorMessage = "请输入正确格式编码")]
        public long PositionId { get; set; }

        /// <summary>
        ///描述
        /// </summary>
        [Display(Name ="描述")]
        [StringLength(128,ErrorMessage ="描述不能超过{1}个字")]
        public string Description { get; set; }

        /// <summary>
        ///示意图
        /// </summary>
        [Display(Name ="广告位图例")]
        public long ImageAttachmentId { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [Display(Name = "建议宽度")]
        [Required(ErrorMessage ="请填写建议宽度")]
        [RegularExpression("^\\d+", ErrorMessage = "请输入正确格式宽度")]
        public int Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        [Display(Name = "建议高度")]
        [Required(ErrorMessage = "请填写建议高度")]
        [RegularExpression("^\\d+", ErrorMessage = "请输入正确格式高度")]
        public int Height { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否为添加
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked { get; set; }

        public bool IsLocked { get; set; }
        /// <summary>
        /// 获取示意图Url
        /// </summary>
        /// <returns></returns>
        public string GetImageUrl()
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().AdvertisingPosition());
            Attachment attachment = attachmentService.Get(ImageAttachmentId);
            if (attachment != null)
            {
                string url = attachment.GetDirectlyUrl();
                return url;
            }
            else
            {
                return null;
            }
        }
    }
}
