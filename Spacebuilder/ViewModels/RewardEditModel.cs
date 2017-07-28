using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Spacebuilder
{
    public class RewardEditModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 经验值
        /// </summary>
        [Required(ErrorMessage ="请输入经验值")]
        [RegularExpression("^-?\\d+$", ErrorMessage = "请输入整数")]
        [Display(Name ="经验")]
        public int ExperiencePoints { get; set; }
        /// <summary>
        /// 金币（交易积分）
        /// </summary>
        [Required(ErrorMessage = "请输入金币值")]
        [RegularExpression("^-?\\d+$",ErrorMessage ="请输入整数")]
        [Display(Name = "金币")]
        public int TradePoints { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        [Required(ErrorMessage = "请输入奖惩原因")]
        [StringLength(512,ErrorMessage = "原因过长，应小于512个字符")]
        [Display(Name = "原因")]
        public string Reason { get; set; }
    }
}
