//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq;

namespace Tunynet.Common
{
    /// <summary>
    /// 日期时间选择器
    /// </summary>
    /// <remarks> 
    /// <list type="number">
    /// <item>http://jqueryui.com/demos/datepicker/</item>
    /// <item>http://trentrichardson.com/examples/timepicker/</item>
    /// </list>    
    /// <para>如果需要更多option设置，可以通过设置AdditionalParameters属性来实现</para>
    /// <para>依赖文件：</para>
    /// <list type="number">
    /// <item>jquery-ui.js</item>
    /// <item>jquery-ui-timepicker-addon.js（V0.9.7）</item>
    /// <item>jquery.ui.datepicker-zh-CN.js</item>
    /// </list>
    /// </remarks>
    public class DateTimePicker
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <remarks>在构造器中为属性赋默认值</remarks>
        public DateTimePicker()
        {
            this.ChangeMonth = true;
            this.ChangeYear = true;
            this.DateFormat = "YYYY-MM-DD";
            this.TimeFormat = "HH:mm";
            this.StartDate = "";
            this.EndDate = "";
            this.EnabledTime = false;
            this.ShowDropdowns = false;
            this.Onlytime = false;
            this.EnabledDate = true;
            this.ErrorMessage = "";
        }
        /// <summary>
        /// 是否只显示时间
        /// </summary>
        /// <value>false</value>
        public bool Onlytime { get; set; }

        /// <summary>
        /// 年月下拉选择
        /// </summary>
        /// <value>false</value>
        public bool ShowDropdowns { get; set; }

        /// <summary>
        /// 是否显示时间
        /// </summary>
        /// <value>false</value>
        public bool EnabledTime { get; set; }

        public bool EnabledDate { get; set; }


        public string ErrorMessage { get; set; }

        /// <summary>
        /// 默认日期
        /// </summary>        
        public DateTime? CurrentValue { get; set; }

        /// <summary>
        /// 日期格式
        /// </summary>
        /// <example>'yyyy-MM-dd' </example>
        /// <value>"yyyy-MM-dd"</value>
        public string DateFormat { get; set; }


        /// <summary>
        /// 时间格式
        /// </summary>
        /// <example>"HH:mm"</example>
        /// <value>"HH:mm"</value>
        public string TimeFormat { get; set; }

        /// <summary>
        /// 最小可选时间
        /// </summary>
        /// <value>"0"</value>
        public string StartDate { get; set; }

        /// <summary>
        /// 最大可选时间
        /// </summary>
        /// <value>"+10Y"</value>
        public string EndDate { get; set; }


        /// <summary>
        /// 是否可改变月份
        /// </summary>
        /// <value>true</value>
        public bool ChangeMonth { get; set; }
        /// <summary>
        /// 是否可改变月份
        /// </summary>
        /// <value>true</value>
        public bool ChangeYear { get; set; }

        /// <summary>
        /// 呈现日期选择器前的Javascript回调函数<br/>
        /// 原型为 function(input, inst)
        /// </summary>
        public string BeforeShowCallBack { get; set; }

        /// <summary>
        /// 选中事件的Javascript回调函数<br/>
        /// 原型为 fn(dateText,inst)
        /// </summary>
        public string OnSelectCallBack { get; set; }

        /// <summary>
        /// 设置关闭事件的Javascript回调函数<br/>
        /// 原型为 fn(dateText,inst)
        /// </summary>
        public string OnCloseCallBack { get; set; }

        /// <summary>
        /// datepicker或timepicker插件中提供的其他option选项
        /// </summary>
        private Dictionary<string, object> AdditionalOptions { get; set; }

        /// <summary>
        /// 文本框的html属性集合
        /// </summary>
        private Dictionary<string, object> HtmlAttributes { get; set; }


        /// <summary>
        /// 默认时间值
        /// </summary>
        private string InitialDate
        {
            get
            {
                if (CurrentValue == null || CurrentValue == DateTime.MinValue)
                    return string.Empty;

                return EnabledTime ? CurrentValue.Value.ToString(this.DateFormat + " HH:mm") : CurrentValue.Value.ToString(this.DateFormat);
            }
        }


        /// <summary>
        /// 默认时间值
        /// </summary>
        private string InitialTime
        {
            get
            {
                if (CurrentValue == null || CurrentValue == DateTime.MinValue)
                    return string.Empty;

                return CurrentValue.Value.ToString(this.DateFormat + " HH:mm");
            }
        }

        #region 方法

        /// <summary>
        /// datepicker或timepicker插件中提供的其他option选项
        /// </summary>
        /// <param name="optionName">选项名</param>
        /// <param name="optionValue">选项值</param>
        public DateTimePicker MergeAdditionalOption(string optionName, object optionValue)
        {
            if (this.AdditionalOptions == null)
                this.AdditionalOptions = new Dictionary<string, object>();
            this.AdditionalOptions[optionName] = optionValue;
            return this;
        }

        /// <summary>
        /// 添加html属性
        /// </summary>
        /// <param name="attributeName">属性名</param>
        /// <param name="attributeValue">属性值</param>
        /// <remarks>如果存在，则覆盖</remarks>
        public DateTimePicker MergeHtmlAttribute(string attributeName, object attributeValue)
        {
            if (this.HtmlAttributes == null)
                this.HtmlAttributes = new Dictionary<string, object>();
            this.HtmlAttributes[attributeName] = attributeValue;
            return this;
        }

        #endregion

        /// <summary>
        /// 转为Html属性集合
        /// </summary>
        public IDictionary<string, object> ToHtmlAttributes()
        {
            var result = new Dictionary<string, object>();
            if (HtmlAttributes != null)
                result = new Dictionary<string, object>(HtmlAttributes);

            var data = new Dictionary<string, object>();

            result.TryAdd("data-initialdate", InitialDate);
            result.TryAdd("data-showdropdowns", ShowDropdowns);
            result.TryAdd("data-onlytime", Onlytime);
            result.TryAdd("data-date-format", DateFormat);
            if (!String.IsNullOrWhiteSpace(StartDate) && !result.ContainsKey("minDate"))
            {
                //判断是否为字符串类型的整数
                int startDateInt = 0;
                if (int.TryParse(StartDate, out startDateInt))
                    result["data-startdate"] = startDateInt;
                else
                    result["data-startdate"] = StartDate;
            }
            if (!String.IsNullOrWhiteSpace(EndDate) && !data.ContainsKey("maxDate"))
            {
                //判断是否为字符串类型的整数
                int endDateInt = 0;
                if (int.TryParse(EndDate, out endDateInt))
                    result["data-enddate"] = endDateInt;
                else
                    result["data-enddate"] = EndDate;
            }

            string clientTimeFormat = TimeFormat;
            if (!EnabledDate)
            {
                result["data-enableddate"] = EnabledDate;
                result["data-date-format"] = clientTimeFormat;
            }
            else if (EnabledTime)
            {
                result["data-enabledtime"] = EnabledTime;

                if (result.ContainsKey("data-date-format"))
                {
                    result["data-date-format"] = result["data-date-format"] + " " + clientTimeFormat;
                }
                else
                {
                    result["data-date-format"] = clientTimeFormat;
                }
            }
            result["data-time-picker"] = EnabledTime ? "true" : "false";
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                result["data-msg-required"] = ErrorMessage;
                result["data-rule-required"] = "true";
            }


            return result;
        }

        /// <summary>
        /// 呈现控件
        /// </summary>
        public virtual MvcHtmlString Render(HtmlHelper htmlHelper, string name)
        {
            //组装容器             

            TagBuilder builder = new TagBuilder("input");


            builder.MergeAttribute("class", "date form-control datepickertimes");
            builder.MergeAttribute("id", name);
            builder.MergeAttribute("name", name);
            builder.MergeAttribute("type", "text");


            IDictionary<string, object> attrs = this.ToHtmlAttributes();
            if (attrs != null)
            {
                foreach (var attr in attrs)
                {
                    builder.MergeAttribute(attr.Key, attr.Value.ToString());
                }
            }

            return new MvcHtmlString(builder.ToString());
        }

        /// <summary>
        /// 呈现控件
        /// </summary>
        public virtual MvcHtmlString RenderDaterangepicker(HtmlHelper htmlHelper, string name)
        {
            //组装容器             

            TagBuilder builder = new TagBuilder("input");

            builder.MergeAttribute("class", "date form-control datepickertime");
            builder.MergeAttribute("id", name);
            builder.MergeAttribute("name", name);
            builder.MergeAttribute("type", "text");
            builder.MergeAttribute("style", "width:233px");


            IDictionary<string, object> attrs = this.ToHtmlAttributes();
            if (attrs != null)
            {
                foreach (var attr in attrs)
                {
                    builder.MergeAttribute(attr.Key, attr.Value.ToString());
                }
            }

            return new MvcHtmlString(builder.ToString());
        }

        ///// <summary>
        ///// 转换为客户端可用的日期格式
        ///// </summary>
        ///// <param name="dateFormat">服务器端的日期格式</param>
        ///// <returns>客户端的日期格式</returns>
        //private static string ConvertToClientDateFormat(string dateFormat)
        //{
        //    //译码表
        //    //客户端编码|服务器端编码|含义
        //    //d --------|d-----------|天，不足两位不会在前面补零，比如：1
        //    //dd--------|dd----------|天，不足两位需要在前面补零，比如：01
        //    //D---------|ddd---------|一周的第几天，简写形式，比如：周一
        //    //DD--------|dddd--------|一周的第几天，全写形式，比如：星期一
        //    //m---------|M-----------|月，不足两位不会在前面补零，比如：1
        //    //mm--------|MM----------|月，不足两位需要在前面补零，比如：01
        //    //M---------|MMM---------|一年的第几个月，简写形式，比如：十二
        //    //MM--------|MMMM--------|一年的第几个月，全写形式，比如：十二月
        //    //y---------|yy----------|天，不足两位不会在前面补零，比如：1
        //    //yy--------|yyyy--------|天，不足两位需要在前面补零，比如：01
        //    string result = dateFormat;
        //    //替换天
        //    if (result.Contains("dddd"))
        //        result = result.Replace("dddd", "DD");
        //    else if (result.Contains("ddd"))
        //        result = result.Replace("ddd", "D");
        //    //替换月
        //    if (result.Contains("MMMM"))
        //        result = result.Replace("MMMM", "MM");
        //    else if (result.Contains("MMM"))
        //        result = result.Replace("MMM", "M");
        //    else if (result.Contains("MM"))
        //        result = result.Replace("MM", "mm");
        //    else if (result.Contains("M"))
        //        result = result.Replace("M", "m");
        //    //替换年
        //    if (result.Contains("yyyy"))
        //        result = result.Replace("yyyy", "yy");
        //    else if (result.Contains("yy"))
        //        result = result.Replace("yy", "y");
        //    return result;
        //}

        ///// <summary>
        ///// 转换为客户端可用的时间格式
        ///// </summary>
        ///// <param name="timeFormat">服务器端的时间格式</param>
        ///// <param name="ampm">输出参数，是否为12小时制</param>
        ///// <returns>客户端的时间格式</returns>
        //private static string ConvertToClientTimeFormat(string timeFormat, out bool ampm)
        //{
        //    //译码表
        //    //客户端编码|服务器端编码|含义
        //    //h --------|h-----------|时，不足两位不会在前面补零，比如：1
        //    //hh--------|hh----------|时，不足两位需要在前面补零，比如：01

        //    //m --------|m-----------|分，不足两位不会在前面补零，比如：1
        //    //mm--------|mm----------|分，不足两位需要在前面补零，比如：01

        //    //s---------|s-----------|秒，不足两位不会在前面补零，比如：1
        //    //ss--------|ss----------|秒，不足两位需要在前面补零，比如：01

        //    //t---------|t-----------|12小时制的简写形式，比如：上
        //    //tt--------|tt----------|12小时制的全写形式，比如：上午

        //    ampm = true;
        //    if (timeFormat.Contains("H"))
        //        ampm = false;

        //    string result = timeFormat;
        //    //替换时
        //    if (result.Contains("HH"))
        //        result = result.Replace("HH", "hh");
        //    else if (result.Contains("H"))
        //        result = result.Replace("H", "h");
        //    return result;
        //}

    }
}
