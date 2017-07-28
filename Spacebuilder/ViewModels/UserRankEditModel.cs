using System;
using System.ComponentModel.DataAnnotations;


namespace Tunynet.Spacebuilder
{
    public class UserRankEditModel
    {
        /// <summary>
        /// 判断是否新建时使用
        /// </summary>
        public int OldRank { get; set; }

        /// <summary>
        /// 等级值
        /// </summary>
        [Display(Name = "等级值")]
        [Required(ErrorMessage = "请输入等级值")]
        [Range(1, 999, ErrorMessage = "等级值必须在1到999之间")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "只能输入数字")]
        public int Rank { get; set; }

        /// <summary>
        /// 等级名称
        /// </summary>
        [Display(Name = "等级名称")]
        [Required(ErrorMessage = "请输入等级名称")]
        public string RankName { get; set; }
        /// <summary>
        /// 经验下限
        /// </summary>
        [Display(Name = "经验下限")]
        [Required(ErrorMessage = "请输入经验下限")]
        [Range(0, 99999999, ErrorMessage = "经验值必须在0到99999999之间")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "只能输入数字")]
        public int PointLower { get; set; }
    }
}
