//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Tunynet.Utilities;
using System.Text;

namespace Tunynet.Common
{
    /// <summary>
    /// 封装对HtmlHelper的扩展方法
    /// </summary>
    public static class HtmlHelperExtensions
    {   
        #region 下拉列表

        /// <summary>
        /// 联动下拉列表
        /// </summary>
        /// <param name="htmlHelper">被扩展的HtmlHelper实例</param>
        /// <param name="expression">获取数据集合</param>
        /// <param name="level">显示多少级</param>
        /// <param name="defaultValue">TProperty类型的默认值（如string默认值为"")</param>
        /// <param name="rootItems">获取根级列表数据</param>
        /// <param name="getParentID">获取列表项的ParentID方法</param>
        /// <param name="getChildItems">获取子级列表数据集合方法</param>
        /// <param name="getChildSelectDataUrl">获取子级列表数据的远程地址</param>
        /// <returns>html代码</returns>
        public static MvcHtmlString LinkageDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
                                                                                                                  TProperty defaultValue,
                                                                                                                  int level,
                                                                                                                  Dictionary<TProperty, string> rootItems,
                                                                                                                  Func<TProperty, TProperty> getParentID,
                                                                                                                  Func<TProperty, Dictionary<TProperty, string>> getChildItems,
                                                                                                                  string getChildSelectDataUrl,
                                                                                                                  string optionLabel = "请选择")
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            return LinkageDropDownList(htmlHelper,
                                 ExpressionHelper.GetExpressionText(expression),
                                (TProperty)metadata.Model,
                                defaultValue,
                                 level,
                                rootItems,
                                getParentID,
                                getChildItems,
                                getChildSelectDataUrl,
                                optionLabel);
        }


        /// <summary>
        /// 联动下拉列表
        /// </summary>
        /// <param name="htmlHelper">被扩展的HtmlHelper实例</param>
        /// <param name="level">显示多少级</param>
        /// <param name="defaultValue">TProperty类型的默认值（如string默认值为"")</param>
        /// <param name="name">下拉列表表单项名</param>
        /// <param name="selectedValue">当前选中值</param>
        /// <param name="rootItems">获取根级列表数据</param>
        /// <param name="getParentId">获取列表项的ParentID方法</param>
        /// <param name="getChildItems">获取子级列表数据集合方法</param>
        /// <param name="getChildSelectDataUrl">获取子级列表数据的远程地址</param>
        /// <returns>html代码</returns>
        public static MvcHtmlString LinkageDropDownList<TProperty>(this HtmlHelper htmlHelper, string name,
                                                                                               TProperty selectedValue,
                                                                                                TProperty defaultValue,
                                                                                                int level,
                                                                                               Dictionary<TProperty, string> rootItems,
                                                                                               Func<TProperty, TProperty> getParentId,
                                                                                               Func<TProperty, Dictionary<TProperty, string>> getChildItems,
                                                                                               string getChildSelectDataUrl,
                                                                                               string optionLabel = "请选择")
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            //select data init
            Stack<Dictionary<TProperty, string>> stack = new Stack<Dictionary<TProperty, string>>();

            //如果有选中的值，则查找其所在列表前面的所有列表
            IList<TProperty> selectedValues = new List<TProperty>();
            if (selectedValue != null && !selectedValue.Equals(defaultValue))
            {
                TProperty itemId = selectedValue;
                TProperty parentItemId = getParentId(itemId);
                while (!itemId.Equals(defaultValue) && !parentItemId.Equals(defaultValue))
                {
                    stack.Push(getChildItems(parentItemId));
                    selectedValues.Add(itemId);
                    itemId = parentItemId;
                    parentItemId = getParentId(itemId);
                }
                if (rootItems.Count() > 0)
                {
                    TProperty rootId = getParentId(rootItems.First().Key);
                    if (!itemId.Equals(rootId))
                    {
                        stack.Push(rootItems);
                        selectedValues.Add(itemId);
                    }
                }
            }
            else
            {
                TProperty rootItemID = rootItems.Select(n => n.Key).FirstOrDefault();
                stack.Push(rootItems);
            }

            //生成标签
            TagBuilder containerBuilder = new TagBuilder("span");
            containerBuilder.MergeAttribute("plugin", "linkageDropDownList");
            containerBuilder.MergeAttribute("class", "form-inline");
            var data = new Dictionary<string, object>();
            data.TryAdd("GetChildSelectDataUrl", getChildSelectDataUrl);
            data.TryAdd("ControlName", name);
            data.TryAdd("Level", level);
            data.TryAdd("OptionLabel", optionLabel);
            data.TryAdd("DefaultValue", defaultValue.ToString());
            containerBuilder.MergeAttribute("data", Json.Encode(data));
            int currentIndex = 0;
            while (stack.Count > 0)
            {
                Dictionary<TProperty, string> dictionary = stack.Pop();
                IEnumerable<SelectListItem> selectList = dictionary.Select(n => new SelectListItem() { Selected = selectedValues.Contains(n.Key), Text = n.Value, Value = n.Key.ToString() });
                containerBuilder.InnerHtml += "\r\n" + htmlHelper.DropDownList(string.Format("{0}_{1}", name, currentIndex), selectList,
                                optionLabel, new { @class = "tn-dropdownlist form-control cms-floder-list" });
                currentIndex++;
            }
            containerBuilder.InnerHtml += "\r\n" + htmlHelper.Hidden(name);
            return MvcHtmlString.Create(containerBuilder.ToString());
        }
        #endregion
        
        /// <summary>
        /// 为RouteValueDictionary扩展添加class方法
        /// </summary>
        /// <param name="htmlAttributes">html属性集合</param>
        /// <param name="cssClass">样式名</param>
        /// <returns>RouteValueDictionary</returns>
        private static RouteValueDictionary AddCssClass(this RouteValueDictionary htmlAttributes, string cssClass)
        {
            if (htmlAttributes == null)
                htmlAttributes = new RouteValueDictionary();
            if (htmlAttributes.Any(n => n.Key.ToLower() == "class"))
                htmlAttributes["class"] += " " + cssClass;
            else
                htmlAttributes["class"] = cssClass;

            return htmlAttributes;
        }

        /// <summary>
        /// 获取水印文字并为标签添加水印标注
        /// </summary>
        /// <typeparam name="TModel">模型的类型</typeparam>
        /// <typeparam name="TProperty">模型中对应输出的属性</typeparam>
        /// <param name="expression">获取模型中的对应的属性</param>
        /// <param name="htmlAttributes">html属性集合</param>
        private static void GetWaterMark<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression, RouteValueDictionary htmlAttributes)
        {
            //获取水印
            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberExpression = (MemberExpression)expression.Body;
                if (memberExpression.Member is PropertyInfo)
                {
                    Type model = typeof(TModel);
                    string propertyName = memberExpression.Member.Name;

                    if (!string.IsNullOrEmpty(propertyName))
                    {
                        PropertyInfo property = model.GetProperties().FirstOrDefault(p => p.Name == propertyName);

                        if (property != null)
                        {
                            Attribute attr = (Attribute)property.GetCustomAttributes(false).FirstOrDefault(a => a is WaterMarkAttribute);
                            object val = attr != null ? attr.GetType().GetProperty("Content").GetValue(attr, null) : null;

                            if (val != null)
                            {
                                htmlAttributes.Add("watermark", val.ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}
