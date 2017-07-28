//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Web.Mvc;

namespace Tunynet.Common
{

    /// <summary>
    /// 扩展对js控件的HtmlHelper输出方法
    /// </summary>
    public static class HtmlHelperTagExtensions
    {

        /// <summary>
        /// 标签选择器
        /// </summary>
        /// <param name="htmlHelper">被扩展的HtmlHelper对象</param>
        /// <param name="name">组件名称</param>
        /// <param name="selectionNum">搜索时可选择数量</param>
        /// <param name="value">默认值</param>
        /// <param name="limit">标签个数</param>
        /// <param name="sourceUrl">搜索地址</param>
        /// <param name="valuelength">标签长度</param>
        /// <param name="showAutocompleteOnFocus">是否支持搜索与自动完成</param>
        /// <param name="tenantTypeId">租户类型</param>
        /// <param name="validation">是否需要验证</param>
        /// <returns></returns>
        public static MvcHtmlString Tag(this HtmlHelper htmlHelper, string name, int selectionNum = 0, string value = "", int limit = 5, string sourceUrl = "", int valuelength = 60, bool showAutocompleteOnFocus = true, string tenantTypeId = "", bool isEntry = true, bool validation = true)
        {

            TagBuilder builder = new TagBuilder("input");

            builder.MergeAttribute("class", "tn-chosen-choices clearfix");
            builder.MergeAttribute("data-plugin", "SelectTag");
            builder.MergeAttribute("data-name", name);
            builder.MergeAttribute("data-selectionnum", selectionNum.ToString());
            builder.MergeAttribute("value", value);
            builder.MergeAttribute("data-limit", limit.ToString());
            builder.MergeAttribute("data-sourceurl", sourceUrl);
            builder.MergeAttribute("data-valuelength", valuelength.ToString());
            builder.MergeAttribute("data-showautocompleteonfocus", showAutocompleteOnFocus.ToString());
            builder.MergeAttribute("data-isEntry", isEntry.ToString());
            builder.MergeAttribute("data-validation", validation.ToString());
            builder.MergeAttribute("data-tenantTypeId", tenantTypeId);
            return new MvcHtmlString(builder.ToString());
        }
    }
}
