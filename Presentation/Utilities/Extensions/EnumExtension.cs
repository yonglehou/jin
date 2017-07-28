//<TunynetCopyright>
//--------------------------------------------------------------
//<copyright>拓宇网络科技有限公司 2005-2016</copyright>
//<version>V0.5</verion>
//<createdate>2016-03-15</createdate>
//<author>libsh</author>
//<email>libsh@tunynet.com</email>
//<log date="2016-03-15" version="0.5">创建</log>
//--------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.Helpers;
using System.Reflection;

namespace Tunynet.Common
{
    /// <summary>
    /// 为枚举类型进行扩展
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举类型的displayName标注
        /// </summary>
        /// <param name="em">被扩展对象</param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum em)
        {
            FieldInfo fi = em.GetType().GetField(em.ToString());
            var attribute = fi.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                .FirstOrDefault();

            if (attribute != null)
                return attribute.Name;

            return em.ToString();
        }


    }
}
