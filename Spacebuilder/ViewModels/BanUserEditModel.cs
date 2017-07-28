using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Spacebuilder
{
    public class BanUserEditModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 封禁截止日期
        /// </summary>
        [Display(Name ="截止日期")]
        [Required(ErrorMessage ="请输入封禁截止日期")]
        public DateTime BanDeadline { get; set; }
        /// <summary>
        /// 封禁原因
        /// </summary>
        [Required(ErrorMessage ="请输入封禁原因")]
        [StringLength(64, ErrorMessage = "原因过长，应小于64个字符")]
        [Display(Name ="封禁原因")]
        public string BanReason { get; set; }
    }
}
