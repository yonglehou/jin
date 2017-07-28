//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tunynet.Common
{
    /// <summary>
    /// 多租户服务标识
    /// </summary>
    public class MultiTenantServiceKeys
    {
        #region Instance

        private static volatile MultiTenantServiceKeys _instance = null;
        private static readonly object lockObject = new object();

        public static MultiTenantServiceKeys Instance()
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new MultiTenantServiceKeys();
                    }
                }
            }
            return _instance;
        }

        private MultiTenantServiceKeys()
        { }

        #endregion Instance

        /// <summary>
        /// 计数
        /// </summary>
        /// <returns></returns>
        public string Count()
        {
            return "Count";
        }

        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        public string Comment()
        {
            return "Comment";
        }
      

        /// <summary>
        /// 标签
        /// </summary>
        /// <returns></returns>
        public string Tag()
        {
            return "Tag";
        }

        /// <summary>
        ///  推荐
        /// </summary>
        /// <returns></returns>
        public string Recommend()
        {
            return "Recommend";
        }

        /// <summary>
        /// 顶踩
        /// </summary>
        /// <returns></returns>
        public string Attitude()
        {
            return "Attitude";
        }
        /// <summary>
        /// 附件
        /// </summary>
        /// <returns></returns>
        public string Attachment()
        {
            return "Attachment";
        }
        /// <summary>
        /// AtUser
        /// </summary>
        /// <returns></returns>
        public string AtUser()
        {
            return "AtUser";
        }

        

        /// <summary>
        /// 收藏
        /// </summary>
        /// <returns></returns>
        public string Favorites()
        {
            return "Favorites";
        }
        /// <summary>
        /// 栏目管理员
        /// </summary>
        /// <returns></returns>
        public string CategoryManager()
        {
            return "CategoryManager";
        }
        /// <summary>
        /// 分类
        /// </summary>
        /// <returns></returns>
        public string Category()
        {
            return "Category";
        }
        /// <summary>
        /// 操作日志
        /// </summary>
        /// <returns></returns>
        public string OperationLog()
        {
            return "OperationLog";
        }
    }

}
