using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum SelectUserStatus
    {
        /// <summary>
        /// 管制用户
        /// </summary>
        [Display(Name ="已管制")]
        IsModerated =1,

        /// <summary>
        /// 封禁用户
        /// </summary>
        [Display(Name ="已封禁")]
        IsBaned = 2,
    }

    public enum ModerateState
    {
        /// <summary>
        /// 管制
        /// </summary>
        [Display(Name ="管制")]
        Moderated=1,
        
        /// <summary>
        /// 非管制
        /// </summary>
        [Display(Name ="否")]
        NoModerated=2,

        /// <summary>
        /// 永久管制
        /// </summary>
        [Display(Name ="永久管制")]
        ForceModerated=3
    }
}
