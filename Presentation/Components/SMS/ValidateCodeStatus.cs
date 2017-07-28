using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Common
{
    /// <summary>
    /// 验证码验证状态
    /// </summary>
    public enum ValidateCodeStatus
    {
        Passed,
        /// <summary>
        /// 为空
        /// </summary>
        Empty,
        /// <summary>
        /// 超时
        /// </summary>
        Overtime,
        /// <summary>
        /// 验证码输入错误
        /// </summary>
        WrongInput,
       
        /// <summary>
        /// 验证码已失效
        /// </summary>
        Failure

    }

}
