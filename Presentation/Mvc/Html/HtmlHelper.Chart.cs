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
    public static class HtmlHelperChartExtensions
    {

        /// <summary>
        /// 图表
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="style">样式，默认为0  0:复合型 1:饼图 2:柱形图</param>
        /// <returns></returns>
        public static MvcHtmlString Chart(this HtmlHelper htmlHelper, string model = "", int style = 0)
        {

            TagBuilder container = new TagBuilder("div");
            container.AddCssClass("jn-exam-option");

            container.MergeAttribute("data-plugin", "chart");
            if (style == 0)
            {
                container.MergeAttribute("data-mode", "charts");
            }
            else if(style == 1)
            {
                container.MergeAttribute("data-mode", "chartpie");
            }
            else if (style == 2)
            {
                container.MergeAttribute("data-mode", "chartbar");
            }
            container.MergeAttribute("data-model", model);

            return new MvcHtmlString(container.ToString());
        }

    }
}
