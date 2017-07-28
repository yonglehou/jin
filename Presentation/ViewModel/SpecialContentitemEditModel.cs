using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Common
{
    public class SpecialContentitemEditModel
    {
        /// <summary>
        /// 推荐内容Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 内容实体ID
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// 租户类型ID
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        /// 该推荐内容的推荐类型
        /// </summary>
        [Display(Name = "推荐类别")]
        [Required(ErrorMessage = "请选择类别")]
        public int TypeId { get; set; }

        /// <summary>
        /// 推荐标题（默认为内容名称或标题，允许推荐人修改）
        /// </summary>
        [Display(Name = "标题")]
        [Required(ErrorMessage = "请填写标题")]
        public string ItemName { get; set; }

        /// <summary>
        /// 推荐标题图附件Id
        /// </summary>
        [Display(Name = "标题图")]
        [Required(ErrorMessage = "请添加标题图")]
        public long FeaturedImageAttachmentId { get; set; }

        /// <summary>
        /// 推荐人 DisplayName
        /// </summary>
        public string Recommender { get; set; }

        /// <summary>
        /// 推荐内容是否为外链
        /// </summary>
        public bool IsLink { get; set; }

        /// <summary>
        /// 外链地址
        /// </summary>
        [Required]
        [Display(Name = "链接")]
        [RegularExpression("((http|ftp|https)://)(([a-zA-Z0-9\\._-]+\\.[a-zA-Z]{2,6})|([0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\\&%_\\./-~-]*)?", ErrorMessage = "请输入正确格式链接")]
        public string Link { get; set; }

        /// <summary>
        /// 推荐人用户 Id
        /// </summary>
        public long RecommenderUserId { get; set; }

        /// <summary>
        /// 推荐日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 推荐期限
        /// </summary>
        public DateTime ExpiredDate { get; set; }

        #region 拓展方法

        /// <summary>
        /// 获取标题图url
        /// </summary>
        /// <param name="tenantId">租户类型id</param>
        /// <returns></returns>
        public string GetFeaturedImageUrl()
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Recommend());
            if (FeaturedImageAttachmentId!=0)
            {
                Attachment attachment= attachmentService.Get(FeaturedImageAttachmentId);
                if (attachment!=null)
                {
                    return attachmentService.GetDirectlyUrl(attachment);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

    }

}
