using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    public class AuditItemInRolesEditModel
    {
        //角色Id
        public long RoleId { get; set; }

        /// <summary>
        ///审核项目标识
        /// </summary>
        public string ItemKey { get; set; }

        /// <summary>
        ///严格程度
        /// </summary>
        public AuditStrictDegree StrictDegree { get; set; }
    }
}
