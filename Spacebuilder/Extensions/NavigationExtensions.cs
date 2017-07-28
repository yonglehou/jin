//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Linq;
using System.Web.Routing;
using Tunynet;
using Tunynet.Common;

using Tunynet.UI;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// Navigation扩展
    /// </summary>
    public static class NavigationExtensions
    {
        /// <summary>
        /// 获取导航Url
        /// </summary>
        /// <param name="navigation">被扩展的navigation</param>
        /// <param name="routeValueDictionary">路由数据集合</param>
        /// <returns></returns>
        public static string GetUrl(this Navigation navigation, RouteValueDictionary routeValueDictionary = null)
        {
            if (!string.IsNullOrEmpty(navigation.UrlRouteName))
            {
                if (!string.IsNullOrEmpty(navigation.RouteDataName) && routeValueDictionary != null)
                {
                    string[] routeNames = navigation.RouteDataName.Split(',');
                    return CachedUrlHelper.RouteUrl(navigation.UrlRouteName, new RouteValueDictionary(routeValueDictionary.Where(n => routeNames.Contains(n.Key)).ToDictionary(n => n.Key, n => n.Value)));
                }
                return CachedUrlHelper.RouteUrl(navigation.UrlRouteName);
            }
            if (navigation.NavigationUrl != null && !string.IsNullOrEmpty(navigation.NavigationUrl.Trim()))
            {
                return navigation.NavigationUrl;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取导航Url
        /// </summary>
        /// <param name="navigation">被扩展的navigation</param>
        /// <param name="spaceKey">空间标识</param>
        /// <param name="routeValueDictionary">路由数据集合</param>
        /// <returns></returns>
        public static string GetUrl(this Navigation navigation, string spaceKey, RouteValueDictionary routeValueDictionary = null)
        {
            if (!string.IsNullOrEmpty(navigation.UrlRouteName))
            {
                RouteValueDictionary routeDatas = null;
                if (!string.IsNullOrEmpty(navigation.RouteDataName) && routeValueDictionary != null)
                {
                    string[] routeNames = navigation.RouteDataName.Split(',');
                    routeDatas = new RouteValueDictionary(routeValueDictionary.Where(n => routeNames.Contains(n.Key)).ToDictionary(n => n.Key, n => n.Value));
                    routeDatas.AddOrReplace("spaceKey", spaceKey);
                }
                else
                    routeDatas = new RouteValueDictionary() { { "spaceKey", spaceKey } };

                return CachedUrlHelper.RouteUrl(navigation.UrlRouteName, routeDatas);
            }
            if (navigation.NavigationUrl != null && !string.IsNullOrEmpty(navigation.NavigationUrl.Trim()))
            {
                return navigation.NavigationUrl;
            }
            return string.Empty;
        }

    }
}
