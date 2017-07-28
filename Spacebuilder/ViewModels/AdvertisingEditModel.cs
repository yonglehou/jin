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
    public class AdvertisingEditModel
    {
        /// <summary>
        ///广告Id
        /// </summary>
        public long AdvertisingId { get; set; }

        /// <summary>
        ///广告名称
        /// </summary>
        [Display(Name ="广告名称")]
        [Required(ErrorMessage ="请填写广告名称")]
        public string Name { get; set; }

        /// <summary>
        ///呈现方式
        /// </summary>
        
        public AdvertisingType AdvertisingType { get; set; }

        /// <summary>
        ///广告内容
        /// </summary>
        [Display(Name = "广告内容")]
        [AllowHtml]
        public string Body { get; set; }


        /// <summary>
        ///图片附件Id
        /// </summary>
        [Required(ErrorMessage = "请上传图片")]
        public long ImageAttachmentId { get; set; }

        /// <summary>
        ///广告链接地址
        /// </summary>
        [Display(Name = "链接地址")]
        [Required(ErrorMessage = "请填写广告链接")]
        public string LinkUrl { get; set; }

        /// <summary>
        ///是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        public bool IsEnable { get; set; }

        /// <summary>
        ///是否新开窗口
        /// </summary>
        [Display(Name ="在新窗口打开")]
        public bool TargetBlank { get; set; }

        /// <summary>
        ///开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 位置列表
        /// </summary>
        public List<AdvertisingPositionEditModel> positionList { get; set; }
         
        /// <summary>
        /// 获取广告图Url
        /// </summary>
        /// <returns></returns>
        public string GetImageUrl()
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Advertising());
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
