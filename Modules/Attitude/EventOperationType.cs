//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Tunynet.Events;

namespace Tunynet.Attitude
{
    public static class EventOperationTypeExtension
    {
        /// <summary>
        /// 顶
        /// </summary>
        public static string Support(this EventOperationType eventOperationType)
        {
            return "Support";
        }

        /// <summary>
        /// 踩
        /// </summary>
        public static string Oppose(this EventOperationType eventOperationType)
        {
            return "Oppose";
        }
    }
}
