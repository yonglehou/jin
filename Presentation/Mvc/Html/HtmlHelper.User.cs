//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Tunynet.Common;
using Tunynet.Utilities;
using System.Web.Routing;
using Tunynet;
using Tunynet.Settings;

namespace Tunynet.Common
{
    /// <summary>
    /// 扩展对User的HtmlHelper输出方法
    /// </summary>
    public static class HtmlHelperUserExtensions
    {

        #region 显示用户头像

        /// <summary>
        /// 显示用户头像
        /// </summary>
        /// <param name="userId">userID</param>
        /// <param name="enableNavigate">是否允许链接到用户空间</param>
        /// <param name="navigateTarget">头衔图片链接的Target</param>
        /// <param name="avatarSizeType">头像尺寸类别</param>
        /// <param name="htmlAttributes">html属性，例如：new RouteValueDictionary{{"Class","editor"},{"width","90%"}}</param>
        public static MvcHtmlString ShowUserAvatar(this HtmlHelper htmlHelper, long userId, AvatarSizeType avatarSizeType = AvatarSizeType.Small, bool enableNavigate = true, HyperLinkTarget navigateTarget = HyperLinkTarget._blank, bool enableClientCaching = true, RouteValueDictionary htmlAttributes = null, bool isShowUserCard = true, bool isShowTitle = false)
        {
            IUser user = DIContainer.Resolve<UserService>().GetUser(userId);
            return ShowUserAvatar(htmlHelper, user, avatarSizeType, enableNavigate, navigateTarget, enableClientCaching, htmlAttributes, isShowUserCard, isShowTitle);
        }

        /// <summary>
        /// 显示用户头像
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="enableNavigate">是否允许链接到用户空间</param>
        /// <param name="navigateTarget">头衔图片链接的Target</param>
        /// <param name="avatarSizeType">头像尺寸类别</param>
        /// <param name="enableClientCaching">是否允许在客户端缓存</param>
        /// <param name="htmlAttributes">html属性，例如：new RouteValueDictionary{{"Class","editor"},{"width","90%"}}</param>
        public static MvcHtmlString ShowUserAvatar(this HtmlHelper htmlHelper, IUser user, AvatarSizeType avatarSizeType = AvatarSizeType.Small, bool enableNavigate = true, HyperLinkTarget navigateTarget = HyperLinkTarget._blank, bool enableClientCaching = true, RouteValueDictionary htmlAttributes = null, bool isShowUserCard = true, bool isShowTitle = false)
        {
            return ShowUserAvatar(htmlHelper, user, enableNavigate ? SiteUrls.Instance().SpaceHome(user == null ? 0 : user.UserId) : string.Empty, avatarSizeType, navigateTarget, enableClientCaching, htmlAttributes, isShowUserCard, isShowTitle);
        }

        /// <summary>
        /// 显示头像
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="avatarSizeType">头像尺寸类别</param>
        /// <param name="link">链接到用户空间的地址</param>
        /// <param name="navigateTarget">链接类型</param>
        /// <param name="enableClientCaching">是否允许在客户端缓存</param>
        /// <param name="htmlAttributes">html属性，例如：new RouteValueDictionary{{"Class","editor"},{"width","90%"}}</param>
        /// <returns></returns>
        public static MvcHtmlString ShowUserAvatar(this HtmlHelper htmlHelper, IUser user, string link, AvatarSizeType avatarSizeType = AvatarSizeType.Small, HyperLinkTarget navigateTarget = HyperLinkTarget._blank, bool enableClientCaching = true, RouteValueDictionary htmlAttributes = null, bool isShowUserCard = true, bool isShowTitle = false)
        {

            string avatarUrl = SiteUrls.Instance().UserAvatarUrl(user, avatarSizeType, enableClientCaching);

            TagBuilder img = new TagBuilder("img");
            if (htmlAttributes != null)
                img.MergeAttributes(htmlAttributes);


            //@todo wanglei 待优化
            switch (avatarSizeType)
            {
                case AvatarSizeType.Big:
                    img.MergeAttribute("width", "120px");
                    img.MergeAttribute("height", "120px");
                    break;
                case AvatarSizeType.Medium:
                    img.MergeAttribute("width", "90px");
                    img.MergeAttribute("height", "90px");
                    break;
                case AvatarSizeType.Small:
                    img.MergeAttribute("width", "50px");
                    img.MergeAttribute("height", "50px");
                    break;
                case AvatarSizeType.Micro:
                    img.MergeAttribute("width", "30px");
                    img.MergeAttribute("height", "30px");
                    break;
                default:
                    break;
            }

            img.MergeAttribute("class", "img-circle");
            img.MergeAttribute("src", avatarUrl);
            if (user != null)
            {
                img.MergeAttribute("alt", user.DisplayName);
                if (isShowTitle)
                {
                    img.MergeAttribute("title", user.DisplayName);
                }
            }

            ISettingsManager<UserProfileSettings> userProfileSettingsManager = DIContainer.Resolve<ISettingsManager<UserProfileSettings>>();
            UserProfileSettings userProfileSettings = userProfileSettingsManager.Get();


            if (!string.IsNullOrEmpty(link) && user != null)
            {
                TagBuilder a = new TagBuilder("a");
                a.MergeAttribute("href", link);

                if (navigateTarget != HyperLinkTarget._self)
                    a.MergeAttribute("target", navigateTarget.ToString());

                if (isShowUserCard)
                {
                    a.MergeAttribute("rel", "hovercard");
                    a.MergeAttribute("title", "loading...");
                    a.MergeAttribute("data-url", SiteUrls.Instance()._UserCard(user.UserId));
                    a.MergeAttribute("class", "tn-avatar");
                }

                a.InnerHtml = img.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(a.ToString());
            }
            else
            {
                return new MvcHtmlString(img.ToString(TagRenderMode.SelfClosing));
            }
        }
        #endregion
    }

    /// <summary>
    /// 超级链接Target
    /// </summary>
    public enum HyperLinkTarget
    {
        /// <summary>
        /// 将内容呈现在一个没有框架的新窗口中
        /// </summary>
        _blank,

        /// <summary>
        /// 将内容呈现在含焦点的框架中
        /// </summary>
        _self,

        /// <summary>
        /// 将内容呈现在上一个框架集父级中
        /// </summary>
        _parent,

        /// <summary>
        /// 将内容呈现在没有框架的全窗口中
        /// </summary>
        _top

    }
}
