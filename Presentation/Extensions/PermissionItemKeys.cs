//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


namespace Tunynet.Common
{
    /// <summary>
    /// 权限项标识扩展类
    /// </summary>
    public static class PermissionItemKeysExtension
    {
        /// <summary>
        /// 内容推荐管理、评论、附件、分类、标签、链接、广告
        /// </summary>
        /// <param name="pik"><see cref="PermissionItemKeys"/></param>
        /// <returns></returns>
        public static string GlobalContent(this PermissionItemKeys pik)
        {
            return "GlobalContent";
        }

        /// <summary>
        /// 资讯管理、栏目管理、可推荐资讯
        /// </summary>
        /// <param name="pik"><see cref="PermissionItemKeys"/></param>
        /// <returns></returns>
        public static string CMS(this PermissionItemKeys pik)
        {
            return "CMS";
        }

        /// <summary>
        /// 贴子管理、贴吧管理、可推荐贴子和贴吧
        /// </summary>
        /// <param name="pik"><see cref="PermissionItemKeys"/></param>
        /// <returns></returns>
        public static string Post(this PermissionItemKeys pik)
        {
            return "Post";
        }

        /// <summary>
        /// 站点设置、导航管理、积分规则、重建索引、清除缓存、任务管理、重启站点
        /// </summary>
        /// <param name="pik"><see cref="PermissionItemKeys"/></param>
        /// <returns></returns>
        public static string SiteManage(this PermissionItemKeys pik)
        {
            return "SiteManage";
        }

        /// <summary>
        /// 用户、角色、等级管理，权限管理、第三方登录设置、操作日志浏览及清除、积分记录浏览
        /// </summary>
        /// <param name="pik"><see cref="PermissionItemKeys"/></param>
        /// <returns></returns>
        public static string User(this PermissionItemKeys pik)
        {
            return "User";
        }
       


    }
}