//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tunynet.Common
{
    /// <summary>
    /// 扩展对Html编辑器的HtmlHelper输出方法
    /// </summary>
    public static class HtmlHelperHtmlEditorExtensions
    {
        /// <summary>
        /// 输出Html编辑器
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="name">编辑器name属性</param>
        /// <param name="value">编辑器内容</param>
        /// <param name="options">异步提交表单选项</param>
        /// <returns>MvcForm</returns>
        public static MvcHtmlString HtmlEditor(this HtmlHelper htmlHelper, string name, string tenantTypeId, long associateId = 0, string value = null, Dictionary<string, object> htmlAttributes = null, string types = "")
        {
            TagBuilder builder = new TagBuilder("span");
            Dictionary<string, object> htmlAttrs = new Dictionary<string, object>();
            if (htmlAttributes != null)
                htmlAttrs = new Dictionary<string, object>(htmlAttributes);
            var data = new Dictionary<string, object>();
            data.Add("tenantTypeId", tenantTypeId);
            data.Add("associateId", associateId);

            //todo:libsh,需要后期处理
            //data.Add("ownerId", UserContext.CurrentUser.UserId);

            if (string.IsNullOrEmpty(tenantTypeId))
            {
                htmlAttrs.Add("tenant", 0);
            }
            else
            {
                htmlAttrs.Add("tenant", 1);
            }
            htmlAttrs.Add("types", types);
            htmlAttrs.Add("data", JsonConvert.SerializeObject(data));
            htmlAttrs.Add("plugin", "ueditor");
            builder.InnerHtml = htmlHelper.TextArea(name, value ?? string.Empty, htmlAttrs).ToString();
            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// 利用ViewModel输出Html编辑器
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="expression">获取ViewModel中的对应的属性</param>
        /// <param name="options">编辑器设置选项</param>
        /// <param name="types">自定义添加- "map"、"insertcode"、"map,insertcode"</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString HtmlEditorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string tenantTypeId, long associateId = 0, Dictionary<string, object> htmlAttributes = null, string types = "")
        {
            TagBuilder builder = new TagBuilder("span");
            Dictionary<string, object> htmlAttrs = new Dictionary<string, object>();
            if (htmlAttributes != null)
                htmlAttrs = new Dictionary<string, object>(htmlAttributes);
            var data = new Dictionary<string, object>();
            data.Add("tenantTypeId", tenantTypeId);
            data.Add("associateId", associateId);

            //todo:libsh,需要后期处理
            //data.Add("ownerId", UserContext.CurrentUser.UserId);

            if (string.IsNullOrEmpty(tenantTypeId))
            {
                htmlAttrs.Add("tenant", 0);
            }
            else
            {
                htmlAttrs.Add("tenant", 1);
            }
            htmlAttrs.Add("types", types);
            htmlAttrs.Add("data", JsonConvert.SerializeObject(data));
            htmlAttrs.Add("plugin", "ueditor");
            builder.InnerHtml = htmlHelper.TextAreaFor(expression, htmlAttrs).ToString();
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}
