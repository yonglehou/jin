//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using AutoMapper.Mappers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tunynet.UI;
using System.Linq;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Tunynet.Common
{

    /// <summary>
    /// 扩展对js控件的HtmlHelper输出方法
    /// </summary>
    public static class HtmlHelperControlExtensions
    {
        #region DateTimePicker

        /// <summary>
        /// 输出日期选择器
        /// </summary>
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="name">name属性</param>
        /// <param name="selectedDate">选中日期</param>
        /// <param name="minDate">最小允许选择日期</param>
        /// <param name="maxDate">最大允许选择日期</param>
        public static MvcHtmlString DatePicker(this HtmlHelper htmlHelper, string name, DateTime? selectedDate = null, string startDate = "", string endDate = "", bool isDaterangepicker = false)
        {
            return htmlHelper.DateTimePicker(name, new DateTimePicker() { EnabledTime = false, StartDate = startDate, EndDate = endDate, CurrentValue = selectedDate }, isDaterangepicker);
        }

        /// <summary>
        /// 输出日期选择器
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="expression">选取model中属性的lamda表达式</param>
        /// <param name="dateTimePicker">日期时间选择器控件</param>
        public static MvcHtmlString DatePickerFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime>> expression, string minDate = "0", string maxDate = "+10Y", bool isDaterangepicker = false)
        {

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var dateTimePicker = new DateTimePicker() { EnabledTime = false, StartDate = minDate, EndDate = maxDate, CurrentValue = (DateTime)metadata.Model };

            PropertyInfo propertyInfo = metadata.ContainerType.GetProperty(metadata.PropertyName);
            RequiredAttribute requiredAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(RequiredAttribute)) as RequiredAttribute;

            if (requiredAttribute != null && !string.IsNullOrWhiteSpace(requiredAttribute.ErrorMessage))
            {
                dateTimePicker.ErrorMessage = requiredAttribute.ErrorMessage;
            }
            return htmlHelper.DateTimePicker(ExpressionHelper.GetExpressionText(expression), dateTimePicker, isDaterangepicker);
        }

        /// <summary>
        /// 输出日期时间选择器
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="name">name属性</param>
        /// <param name="dateTimePicker">日期时间选择器控件</param> 
        public static MvcHtmlString TimePicker(this HtmlHelper htmlHelper, string name, DateTime? value)
        {
            var dateTimePicker = new DateTimePicker() { EnabledDate = false, CurrentValue = value };
            return dateTimePicker.Render(htmlHelper, name);
        }

        /// <summary>
        /// 输出日期时间选择器
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="expression">选取model中属性的lamda表达式</param>
        /// <param name="dateTimePicker">日期时间选择器控件</param>
        public static MvcHtmlString DateTimePickerFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime>> expression, DateTimePicker dateTimePicker = null, bool isDaterangepicker = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            if (dateTimePicker == null)
                dateTimePicker = new DateTimePicker();
            if (metadata.Model != null)
            {
                dateTimePicker.CurrentValue = (DateTime)metadata.Model;
                dateTimePicker.StartDate = ((DateTime)metadata.Model).ToString("yyyy-MM-dd HH:mm");
            }

            PropertyInfo propertyInfo = metadata.ContainerType.GetProperty(metadata.PropertyName);
            RequiredAttribute requiredAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(RequiredAttribute)) as RequiredAttribute;

            if (requiredAttribute != null && !string.IsNullOrWhiteSpace(requiredAttribute.ErrorMessage))
            {
                dateTimePicker.ErrorMessage = requiredAttribute.ErrorMessage;
            }

            return htmlHelper.DateTimePicker(ExpressionHelper.GetExpressionText(expression), dateTimePicker, isDaterangepicker);

        }

        /// <summary>
        /// 输出日期时间选择器
        /// </summary>
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="name">name属性</param>
        /// <param name="selectedDate">选中日期时间</param>
        /// <param name="minDate">最小可选时间</param>
        /// <param name="maxDate">最大可选时间</param>
        /// <remarks>
        /// <para>最小可选时间和最大可选时间的赋值说明如下：</para>
        /// </remarks>
        /// <include file='../Controls/DateTimePicker.xml' path='doc/members/member[@name="P:Tunynet.Mvc.DateTimePicker.MinDate"]/remarks'/>
        public static MvcHtmlString DateTimePicker(this HtmlHelper htmlHelper, string name, DateTime? selectedDate = null, string minDate = "", string maxDate = "", bool isDaterangepicker = false)
        {
            return htmlHelper.DateTimePicker(name, new DateTimePicker() { EnabledTime = true, StartDate = minDate, EndDate = maxDate, CurrentValue = selectedDate }, isDaterangepicker);
        }

        /// <summary>
        /// 输出日期时间选择器
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="name">name属性</param>
        /// <param name="dateTimePicker">日期时间选择器控件</param> 
        public static MvcHtmlString DateTimePicker(this HtmlHelper htmlHelper, string name, DateTimePicker dateTimePicker, bool isDaterangepicker = false)
        {
            if (dateTimePicker == null)
                dateTimePicker = new DateTimePicker();

            //var d = htmlHelper.GetUnobtrusiveValidationAttributes(name);
            //var c = htmlHelper.GetUnobtrusiveValidationAttributes("StartDate", null);
            if (isDaterangepicker)
            {
                return dateTimePicker.RenderDaterangepicker(htmlHelper, name);
            }
            return dateTimePicker.Render(htmlHelper, name);

        }


        #endregion

        #region ajaxform

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="options">异步提交表单选项</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxForm(this HtmlHelper htmlHelper, AjaxFormOptions options)
        {
            return FormHelper(htmlHelper, null /* formAction */ , FormMethod.Post, options, new RouteValueDictionary());
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, AjaxFormOptions options)
        {
            return BeginAjaxForm(htmlHelper, actionName, controllerName, null /* values */, method, options, null /* htmlAttributes */);
        }
        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, AjaxFormOptions options)
        {
            return BeginAjaxForm(htmlHelper, actionName, controllerName, routeValues, method, options, null /* htmlAttributes */);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <param name="htmlAttributes">表单html属性集合</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, AjaxFormOptions options, object htmlAttributes)
        {
            RouteValueDictionary newValues = new RouteValueDictionary(routeValues);
            RouteValueDictionary newAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return BeginAjaxForm(htmlHelper, actionName, controllerName, newValues, method, options, newAttributes);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, AjaxFormOptions options)
        {
            return BeginAjaxForm(htmlHelper, actionName, controllerName, routeValues, method, options, null /* htmlAttributes */);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <param name="htmlAttributes">表单html属性集合</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, AjaxFormOptions options, IDictionary<string, object> htmlAttributes)
        {
            // get target URL
            string formAction = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues ?? new RouteValueDictionary(), htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true /* includeImplicitMvcValues */);
            return FormHelper(htmlHelper, formAction, method, options, htmlAttributes);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="routeName"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method, AjaxFormOptions options)
        {
            return BeginAjaxRouteForm(htmlHelper, routeName, null /* routeValues */, method, options, null /* htmlAttributes */);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="routeName"></param>
        /// <param name="routeValues"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues, FormMethod method, AjaxFormOptions options)
        {
            return BeginAjaxRouteForm(htmlHelper, routeName, (object)routeValues, method, options, null /* htmlAttributes */);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="routeName"></param>
        /// <param name="routeValues"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <param name="htmlAttributes">表单html属性集合</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues, FormMethod method, AjaxFormOptions options, object htmlAttributes)
        {
            RouteValueDictionary newAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return BeginAjaxRouteForm(htmlHelper, routeName, new RouteValueDictionary(routeValues), method, options, newAttributes);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="routeName"></param>
        /// <param name="routeValues"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxRouteForm(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues, FormMethod method, AjaxFormOptions options)
        {
            return BeginAjaxRouteForm(htmlHelper, routeName, routeValues, method, options, null /* htmlAttributes */);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="routeName"></param>
        /// <param name="routeValues"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <param name="htmlAttributes">表单html属性集合</param>
        /// <returns>MvcForm</returns>
        public static MvcForm BeginAjaxRouteForm(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues, FormMethod method, AjaxFormOptions options, IDictionary<string, object> htmlAttributes)
        {
            string formAction = UrlHelper.GenerateUrl(routeName, null /* actionName */, null /* controllerName */, routeValues ?? new RouteValueDictionary(), htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, false /* includeImplicitMvcValues */);
            return FormHelper(htmlHelper, formAction, method, options, htmlAttributes);
        }

        /// <summary>
        /// 输出AjaxForm表单
        /// </summary> 
        /// <param name="htmlHelper">被扩展的htmlHelper实例</param>
        /// <param name="formAction"></param>
        /// <param name="method">表单请求方式</param>
        /// <param name="options">异步提交表单选项</param>
        /// <param name="htmlAttributes">表单html属性集合</param>
        /// <returns>MvcForm</returns>
        private static MvcForm FormHelper(this HtmlHelper htmlHelper, string formAction, FormMethod method, AjaxFormOptions options, IDictionary<string, object> htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("form");
            builder.MergeAttributes(htmlAttributes);
            if (!string.IsNullOrEmpty(formAction))
                builder.MergeAttribute("action", formAction);
            builder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            builder.MergeAttributes(options.ToHtmlAttributes());
            htmlHelper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.StartTag) + htmlHelper.AntiForgeryToken());
            MvcForm theForm = new MvcForm(htmlHelper.ViewContext);
            return theForm;
        }

        #endregion ajaxform

        /// <summary>
        /// 用户选择器
        /// </summary>
        /// <param name="htmlHelper">被扩展的HtmlHelper对象</param>
        /// <param name="name">组件名称</param>
        /// <param name="itemNumber">搜索时可选择数量</param>
        /// <param name="selectionAdminUserIds">内置用户</param>
        /// <param name="selectedValues"></param>
        /// <returns></returns>
        public static MvcHtmlString UserSelector(this HtmlHelper htmlHelper, string name, IEnumerable<long> selectionUserIds = null, IEnumerable<string> selectionOrgIds = null, int selectionNum = 0, int searchResultNum = 0, string parentId = "", bool require = false, string placeHolder = "", bool cacheData = true, UserOrOrgSelectorMode mode = UserOrOrgSelectorMode.User, string sourceUrl = "", string innerText = "", bool openTree = false, bool validation = true, IEnumerable<long> selectionAdminUserIds = null)
        {
            var userService = DIContainer.Resolve<UserService>();
            var infos = userService.GetTopUsers(100, SortBy_User.DateCreated);

            string[] names = name.IndexOf(",") > 0 ? name.Split(',') : new string[] { name };

            string selectedStr = "", valuesStr = "<li class=\"selection-vals\">";
            if (selectionAdminUserIds != null)
            {
                foreach (var val in selectionAdminUserIds)
                {
                    var info = infos.FirstOrDefault(n => n.UserId == val);
                    if (info != null)
                    {
                        selectedStr += "<li class=\"tn-choice-item bg-info\" data-value=\"" + info.UserId + "\"><span class=\"tb-text\">" + info.DisplayName + "</span><a href=\"javascript:; \"></a></li>";
                        valuesStr += "<input type=\"hidden\" name=\"" + names[0] + "\" value=\"" + info.UserId + "\" />";
                    }
                }
            }
            if (selectionUserIds != null)
            {
                foreach (var val in selectionUserIds)
                {
                    var info = infos.FirstOrDefault(n => n.UserId == val);
                    if (info != null)
                    {
                        selectedStr += "<li class=\"tn-choice-item bg-info\" data-value=\"" + info.UserId + "\"><span class=\"tb-text\">" + info.DisplayName + "</span><a href=\"javascript:; \"><i class=\"fa fa-close\"></i></a></li>";
                        valuesStr += "<input type=\"hidden\" name=\"" + names[0] + "\" value=\"" + info.UserId + "\" />";
                    }
                }
            }

            if (mode > UserOrOrgSelectorMode.User && selectionOrgIds != null)
            {
                foreach (var selectItem in selectionOrgIds)
                {

                    if (selectItem == null)
                        continue;
                    var info = userService.GetUser(selectItem);
                    if (info != null)
                    {
                        selectedStr += "<li class=\"tn-choice-item bg-success\" data-value=\"" + info.UserId + "\"><span class=\"tb-text\">" + info.DisplayName + "</span><a href=\"javascript:; \"><i class=\"fa fa-close\"></i></a></li>";
                        valuesStr += "<input type=\"hidden\" name=\"" + names[names.Length - 1] + "\" value=\"" + info.UserId + "\" />";
                    }
                }
            }
            valuesStr += "</li>";


            TagBuilder container = new TagBuilder("ul");

            container.MergeAttribute("class", "tn-chosen-choices clearfix");
            container.MergeAttribute("data-plugin", "UserOrOrgSelector");
            container.MergeAttribute("data-name", name);
            container.MergeAttribute("data-searchresultnum", searchResultNum.ToString());
            container.MergeAttribute("data-selectionnum", selectionNum.ToString());
            container.MergeAttribute("data-parentid", parentId.ToString());
            container.MergeAttribute("data-require", require.ToString());
            container.MergeAttribute("data-mode", mode.ToString().ToLower());
            container.MergeAttribute("data-cachedata", (cacheData ? 1 : 0).ToString());
            container.MergeAttribute("data-sourceurl", sourceUrl);
            container.MergeAttribute("data-innertext", innerText);
            container.MergeAttribute("data-opentree", (openTree ? 1 : 0).ToString());
            container.MergeAttribute("data-validation", validation.ToString());



            if (string.IsNullOrWhiteSpace(placeHolder))
            {
                placeHolder = "搜索用户、部门";
                switch (mode)
                {
                    case UserOrOrgSelectorMode.User:
                        placeHolder = "搜索用户";
                        break;
                    case UserOrOrgSelectorMode.Organization:
                        placeHolder = "搜索部门";
                        break;

                }
            }
            container.MergeAttribute("data-placeHolder", placeHolder);

            container.InnerHtml = selectedStr + valuesStr;

            return new MvcHtmlString(container.ToString());
        }






        ///// <summary>
        ///// 用户选择器
        ///// </summary>
        ///// <param name="htmlHelper">被扩展的HtmlHelper对象</param>
        ///// <param name="name">组件名称</param>
        ///// <param name="itemNumber">搜索时可选择数量</param>
        ///// <param name="selectedValues"></param>
        ///// <returns></returns>
        //public static MvcHtmlString UserOrOrgSelector(this HtmlHelper htmlHelper, string name, IEnumerable<string> selectionUserIds = null, IEnumerable<string> selectionOrgIds = null, int selectionNum = 0, int searchResultNum = 0, string parentId = "", bool require = false, string placeHolder = "", bool cacheData = true, UserOrOrgSelectorMode mode = UserOrOrgSelectorMode.User, string sourceUrl = "", string innerText = "", bool openTree = false, bool validation = true)
        //{
        //    string adminDepartmentId = "";
        //    var userService = DIContainer.Resolve<UserService>();
        //    List<User> infos = userService.GetTopUsers(10000);

        //    string[] names = name.IndexOf(",") > 0 ? name.Split(',') : new string[] { name };

        //    string selectedStr = "", valuesStr = "<li class=\"selection-vals\">";
        //    if (selectionUserIds != null)
        //    {
        //        foreach (var val in selectionUserIds)
        //        {
        //            if (val == null)
        //            {
        //                continue;
        //            }
        //            var info = infos.FirstOrDefault(n => n.ID.ToLower() == val.ToLower());
        //            if (info != null)
        //            {
        //                selectedStr += "<li class=\"tn-choice-item bg-info\" data-value=\"" + info.ID + "\"><span class=\"tb-text\">" + info.Name + "</span><a href=\"javascript:; \"><i class=\"fa fa-close\"></i></a></li>";
        //                valuesStr += "<input type=\"hidden\" name=\"" + names[0] + "\" value=\"" + info.ID + "\" />";
        //            }
        //        }
        //    }

        //    if (mode > UserOrOrgSelectorMode.User && selectionOrgIds != null)
        //    {
        //        foreach (var selectItem in selectionOrgIds)
        //        {

        //            if (selectItem == null)
        //                continue;
        //            var info = GetDepartment(selectItem);
        //            if (info != null)
        //            {
        //                selectedStr += "<li class=\"tn-choice-item bg-success\" data-value=\"" + info.ID + "\"><span class=\"tb-text\">" + info.Name + "</span><a href=\"javascript:; \"><i class=\"fa fa-close\"></i></a></li>";
        //                valuesStr += "<input type=\"hidden\" name=\"" + names[names.Length - 1] + "\" value=\"" + info.ID + "\" />";
        //            }
        //        }
        //    }
        //    valuesStr += "</li>";


        //    TagBuilder container = new TagBuilder("ul");

        //    container.MergeAttribute("class", "tn-chosen-choices clearfix");
        //    container.MergeAttribute("data-plugin", "UserOrOrgSelector");
        //    container.MergeAttribute("data-name", name);
        //    container.MergeAttribute("data-searchresultnum", searchResultNum.ToString());
        //    container.MergeAttribute("data-selectionnum", selectionNum.ToString());
        //    container.MergeAttribute("data-parentid", parentId.ToString());
        //    container.MergeAttribute("data-require", require.ToString());
        //    container.MergeAttribute("data-mode", mode.ToString().ToLower());
        //    container.MergeAttribute("data-cachedata", (cacheData ? 1 : 0).ToString());
        //    container.MergeAttribute("data-sourceurl", sourceUrl);
        //    container.MergeAttribute("data-innertext", innerText);
        //    container.MergeAttribute("data-opentree", (openTree ? 1 : 0).ToString());
        //    container.MergeAttribute("data-validation", validation.ToString());



        //    if (string.IsNullOrWhiteSpace(placeHolder))
        //    {
        //        placeHolder = "搜索用户、部门";
        //        switch (mode)
        //        {
        //            case UserOrOrgSelectorMode.User:
        //                placeHolder = "搜索用户";
        //                break;
        //            case UserOrOrgSelectorMode.Organization:
        //                placeHolder = "搜索部门";
        //                break;

        //        }
        //    }
        //    container.MergeAttribute("data-placeHolder", placeHolder);

        //    container.InnerHtml = selectedStr + valuesStr;

        //    return new MvcHtmlString(container.ToString());
        //}


        ///// <summary>
        ///// 部门选择器
        ///// </summary>
        ///// <param name="htmlHelper"></param>
        ///// <param name="name"></param>
        ///// <param name="itemNumber">可选择的数量</param>
        ///// <returns></returns>
        //public static MvcHtmlString DepartmentSelector(this HtmlHelper htmlHelper, string name, int itemNumber = 0, List<string> selectedValues = null)
        //{
        //    var ogu_service = OGU_DIContainer.Resolve<IOguPermissionServiceWrapper>();
        //    List<OguBaseInfo> infos = ogu_service.GetChildren(SchemaType.Organizations, SearchLevel.SubTree, departmentID);

        //    string selectedStr = "";
        //    if (selectedValues != null)
        //    {
        //        foreach (var selectItem in selectedValues)
        //        {

        //            if (selectItem == null)
        //                continue;
        //            var info = GetDepartment(selectItem);
        //            if (info != null)
        //            {
        //                selectedStr += "<li class=\"tn-choice-item bg-info\" name=\"" + info.ID + "\"><span class=\"tb-text\">" + info.Name + "</span><a href=\"javascript:; \"><i class=\"fa fa-close\"></i></a></li>";
        //            }
        //        }
        //    }
        //    //class=\"tn-chosen-container\"
        //    //string tagstring = "<div><ul id = \"selectdevals\" class=\"tn-chosen-choices clearfix\"><li class=\"tn-choice-add\"><a id = \"ztreeselect\" href=\"javascript:;\">点击选择部门</a></li></ul></div><div data-mode=\"ztree\" data=\"ztree\"" + "data-name=\"" + name + "\"" + (itemNumber > 0 ? "data-itemnumber=\"" + itemNumber + "\"" : "") + "><div id=\"deSelected\"></div></div>";
        //    string tagstring = "<div><ul id = \"selectdevals\" class=\"tn-chosen-choices clearfix\">" + selectedStr + "<li class=\"tn-choice-add\"><a id = \"adddeselect\" href=\"javascript:;\">点击选择部门</a></li></ul></div><div data=\"departmentSeleCtorUS\" data-mode=\"departmentSeleCtorUS\"" + "data-name=\"" + name + "\"" + (itemNumber > 0 ? "data-itemnumber=\"" + itemNumber + "\"" : "") + "data-url=" + CachedUrlHelper.Action("GetAllLikeDepartment", "Administrator") + "><div id=\"deSelected\"></div></div>";
        //    return new MvcHtmlString(tagstring);
        //}
    }


    /// <summary>
    /// 用户组织选择器模式
    /// </summary>
    public enum UserOrOrgSelectorMode
    {
        //用户选择模式
        User,
        //组织选择模式
        Organization,
        //部门及用户混合选择模式
        All

    }
}
