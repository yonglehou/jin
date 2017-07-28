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
    /// 租户类型Id
    /// </summary>
    public class TenantTypeIds
    {
        #region Instance

        private static volatile TenantTypeIds _instance = null;
        private static readonly object lockObject = new object();

        public static TenantTypeIds Instance() {
            if (_instance == null) {
                lock (lockObject) {
                    if (_instance == null) {
                        _instance = new TenantTypeIds();
                    }
                }
            }
            return _instance;
        }

        private TenantTypeIds() { }

        #endregion Instance

        #region 内置服务
      

        /// <summary>
        /// 用户
        /// </summary>
        public string User() {
            return "000001";
        }
        /// <summary>
        /// 角色
        /// </summary>
        /// <returns></returns>
        public string Role()
        {
            return "000002";
        }
        
        /// <summary>
        /// 分类
        /// </summary>
        /// <returns></returns>
        public string Categorie()
        {
            return "000021";
        }
        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        public string Comment()
        {
            return "000031";
        }
        /// <summary>
        /// 标签
        /// </summary>
        /// <returns></returns>
        public string Tag()
        {
            return "000041";
        }
        /// <summary>
        /// 附件
        /// </summary>
        /// <returns></returns>
        public string Attachment()
        {
            return "000051";
        }
        /// <summary>
        /// 推荐
        /// </summary>
        /// <returns></returns>
        public string Recommend()
        {
            return "000061";
        }
      
        /// <summary>
        /// 友情链接
        /// </summary>
        /// <returns></returns>
        public string Link()
        {
            return "000071";
        }

        /// <summary>
        /// 广告
        /// </summary>
        /// <returns></returns>
        public string Advertising()
        {
            return "000081";
        }
        /// <summary>
        /// 广告位
        /// </summary>
        /// <returns></returns>
        public string AdvertisingPosition()
        {
            return "000082";
        }

      
       
        /// <summary>
        /// 评价
        /// </summary>
        /// <returns></returns>
        public string Review()
        {
            return "000101";
        }
        /// <summary>
        /// 积分
        /// </summary>
        /// <returns></returns>
        public string Point()
        {
            return "000111";
        }

        /// <summary>
        /// 权限
        /// </summary>
        /// <returns></returns>
        public string Permission()
        {
            return "000121";
        }

        /// <summary>
        /// 导航
        /// </summary>
        /// <returns></returns>
        public string Navigation()
        {
            return "000131";
        }

        #endregion 内置服务

        #region 内置应用

        /// <summary>
        /// 板块
        /// </summary>
        /// <returns></returns>
        public string Section()
        {
            return "100001";
        }

        /// <summary>
        /// 贴子
        /// </summary>
        /// <returns></returns>
        public string Thread()
        {
            return "100002";
        }

        /// <summary>
        /// 贴吧
        /// </summary>
        /// <returns></returns>
        public string Bar()
        {
            return "100003";
        }
     

        /// <summary>
        /// 资讯
        /// </summary>
        /// <returns></returns>
        public string ContentItem()
        {
            return "100011";
        }
        /// <summary>
        /// 资讯栏目
        /// </summary>
        /// <returns></returns>
        public string CategoryManagers()
        {
            return "100012";
        }
        /// <summary>
        /// 文章
        /// </summary>
        /// <returns></returns>
        public string CMS_Article()
        {
            return "100013";
        }

        /// <summary>
        /// 组图
        /// </summary>
        /// <returns></returns>
        public string CMS_Image()
        {
            return "100014";
        }

        /// <summary>
        /// 视频
        /// </summary>
        /// <returns></returns>
        public string CMS_Video()
        {
            return "100015";
        }
       
        #endregion 内置应用

    }
}
