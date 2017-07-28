using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
     public class UserInRoleEditModel
    {
        /// <summary>
        /// 用户角色
        /// </summary>
        public Role UserRole{ get; set; }

        /// <summary>
        /// 是否属于角色
        /// </summary>
        public bool IsInRole { get; set; }
    }
}
