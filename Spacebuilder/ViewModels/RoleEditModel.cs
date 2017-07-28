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
    public class RoleEditModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name ="角色ID")]
        [Required(ErrorMessage ="请输入角色ID")]
        [RegularExpression("^[1-9][0-9]*$",ErrorMessage ="请输入正确格式ID")]
        [Remote("CheckUniqueRoleId", "ControlPanel", ErrorMessage = "角色Id已存在")]
        public long RoleId { get; set; }

        /// <summary>
        /// 对外显示的名称
        /// </summary>
        [Display(Name ="对外显示的角色名称")]
        [StringLength(64,ErrorMessage ="角色名应小于64个字符")]
        [Required(ErrorMessage = "请输入对外显示角色名称")]
        [RegularExpression("^\\S+$", ErrorMessage ="角色名称中禁止包含空格")]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [Display(Name = "角色描述")]
        [StringLength(255, ErrorMessage = "角色描述应小于255个字符")]
        public string Description { get; set; }

        /// <summary>
        /// 是否允许关联到用户
        /// </summary>
        [Display(Name = "允许关联到用户")]
        public bool ConnectToUser { get; set; }

        /// <summary>
        /// 角色是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// 角色标识图附件Id
        /// </summary>
        [Display(Name = "角色标识图片")]
        public string RoleImageAttachmentId { get; set; }

        /// <summary>
        /// 角色标识图
        /// </summary>
        public string RoleAttachmentPath { get; set; }
		
		/// <summary>
		/// 是否内置
		/// </summary>
        public bool IsBuiltIn { get; set; }
      
        #region 拓展方法

        /// <summary>
        /// 获取角色图标
        /// </summary>
        /// <returns></returns>
        public string RoleImageUrl()
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Role());
            Attachment attachment = attachmentService.Get(long.Parse(RoleImageAttachmentId));
            if (attachment!=null)
            {
                string url = attachment.GetDirectlyUrl("Small");
                return url;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
