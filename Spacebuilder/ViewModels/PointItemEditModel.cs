using System;
using System.ComponentModel.DataAnnotations;

namespace Tunynet.Spacebuilder
{
    public class PointItemEditModel
    {

        public string ItemKey { get; set; }
        /// <summary>
        /// 积分规则名称
        /// </summary>
        [Display(Name = "积分规则名称")]
        [Required(ErrorMessage = "请输入积分规则名称")]
        public string ItemName { get; set; }

        /// <summary>
        /// 经验
        /// </summary>
        [Display(Name = "经验")]
        [Required(ErrorMessage = "请输入经验值")]
        [Range(-999, 999, ErrorMessage = "经验值必须在-999到999之间")]
        [RegularExpression("^(-)?[0-9]*$", ErrorMessage = "只能输入数字")]
        public int ExperiencePoints { get; set; }

        /// <summary>
        /// 金币
        /// </summary>
        [Display(Name = "金币")]
        [Required(ErrorMessage = "请输入金币数")]
        [Range(-999, 999,ErrorMessage ="金币数值必须在-999到999之间")]
        [RegularExpression("^(-)?[0-9]*$", ErrorMessage = "只能输入数字")]
        public int TradePoints { get; set; }
    }
}
