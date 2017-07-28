//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Web.Mvc;

namespace Tunynet.Common
{
    /// <summary>
    /// 分页按钮控件
    /// </summary>
    public static class HtmlHelperPaginationExtensions
    {
        /// <summary>
        /// 呈现普通分页按钮
        /// </summary>
        /// <param name="html">被扩展的HtmlHelper</param>
        /// <param name="pagingDataSet">数据源</param>
        /// <param name="paginationMode">分页按钮显示模式</param>
        /// <returns></returns>
        public static MvcHtmlString PagingButton(this HtmlHelper html, IPagingDataSet pagingDataSet, PaginationMode paginationMode = PaginationMode.NumericNextPrevious)
        {
            return PagingButton(html, pagingDataSet, null, null, paginationMode);
        }

        /// <summary>
        /// 呈现异步分页按钮
        /// </summary>
        /// <param name="paginationMode">分页按钮显示模式</param>
        /// <param name="html">被扩展的HtmlHelper</param>
        /// <param name="pagingDataSet">数据集</param>
        /// <param name="updateTargetId">异步分页时，被更新的目标元素Id</param>
        /// <param name="numericPagingButtonCount">数字分页按钮显示个数</param>
        /// <returns>分页按钮html代码</returns>
        public static MvcHtmlString AjaxPagingButton(this HtmlHelper html, IPagingDataSet pagingDataSet, string updateTargetId, string ajaxLoadUrl = null, PaginationMode paginationMode = PaginationMode.NumericNextPrevious)
        {
            return PagingButton(html, pagingDataSet, updateTargetId, ajaxLoadUrl, paginationMode);
        }

        /// <summary>
        /// 呈现分页按钮
        /// </summary>
        /// <param name="html">被扩展的HtmlHelper</param>
        /// <param name="pagingDataSet">数据集</param>
        /// <param name="updateTargetId">异步分页时，被更新的目标元素Id</param>
        /// <param name="paginationMode">分页按钮显示模式</param>
        /// <param name="numericPagingButtonCount">数字分页按钮显示个数</param>
        /// <param name="enableAjax">是否使用ajax分页</param>
        /// <returns>分页按钮html代码</returns>
        private static MvcHtmlString PagingButton(this HtmlHelper html, IPagingDataSet pagingDataSet, string targetId, string ajaxLoadUrl = null, PaginationMode paginationMode = PaginationMode.NumericNextPrevious)
        {
            if (pagingDataSet.TotalRecords == 0 || pagingDataSet.PageSize == 0)
                return MvcHtmlString.Empty;

            //计算总页数
            int totalPages = (int)(pagingDataSet.TotalRecords / (long)pagingDataSet.PageSize);
            if ((pagingDataSet.TotalRecords % pagingDataSet.PageSize) > 0)
                totalPages++;

            //未超过一页时不显示分页按钮
            if (totalPages <= 1)
                return MvcHtmlString.Empty;

            //对pageIndex进行修正
            if ((pagingDataSet.PageIndex < 1) || (pagingDataSet.PageIndex > totalPages))
                pagingDataSet.PageIndex = 1;

            TagBuilder container = new TagBuilder("div");

            container.MergeAttribute("data-plugin", "page");
            container.MergeAttribute("data-mode", paginationMode.ToString());
            if (!string.IsNullOrWhiteSpace(ajaxLoadUrl))
            {
                container.MergeAttribute("data-ajaxloadurl", ajaxLoadUrl);
            }
            if (!string.IsNullOrWhiteSpace(targetId))
            {
                container.MergeAttribute("data-targetid", targetId);
            }
            container.MergeAttribute("data-sum", pagingDataSet.TotalRecords.ToString());
            container.MergeAttribute("data-size", pagingDataSet.PageSize.ToString());
            container.MergeAttribute("data-pageindex", pagingDataSet.PageIndex.ToString());


            return MvcHtmlString.Create(container.ToString());
        }


        /// <summary>
        /// 呈现异步分页按钮
        /// </summary>
        /// <param name="paginationMode">分页按钮显示模式</param>
        /// <param name="html">被扩展的HtmlHelper</param>
        /// <param name="pagingDataSet">数据集</param>
        /// <param name="updateTargetId">异步分页时，被更新的目标元素Id</param>
        /// <param name="numericPagingButtonCount">数字分页按钮显示个数</param>
        /// <returns>分页按钮html代码</returns>
        public static MvcHtmlString AjaxPagingButtonSearch(this HtmlHelper html, int pageIndex, int pageSize, long totalRecords, string updateTargetId, string ajaxLoadUrl = null, PaginationMode paginationMode = PaginationMode.NumericNextPrevious)
        {
            return PagingButtonSearch(html, pageIndex, pageSize, totalRecords, updateTargetId, ajaxLoadUrl, paginationMode);
        }
        /// <summary>
        /// 呈现分页按钮
        /// </summary>
        /// <param name="html">被扩展的HtmlHelper</param>
        /// <param name="pagingDataSet">数据集</param>
        /// <param name="updateTargetId">异步分页时，被更新的目标元素Id</param>
        /// <param name="paginationMode">分页按钮显示模式</param>
        /// <param name="numericPagingButtonCount">数字分页按钮显示个数</param>
        /// <param name="enableAjax">是否使用ajax分页</param>
        /// <returns>分页按钮html代码</returns>
        private static MvcHtmlString PagingButtonSearch(this HtmlHelper html, int pageIndex, int pageSize, long totalRecords, string targetId, string ajaxLoadUrl = null, PaginationMode paginationMode = PaginationMode.NumericNextPrevious)
        {
            if (totalRecords < 1)
                totalRecords = 1;
            //未超过一页时不显示分页按钮
            if (totalRecords <= pageSize)
                return MvcHtmlString.Empty;
            if (pageIndex < 1)
                pageIndex = 1;
            if (pageSize < 1)
                pageSize = 1;
            TagBuilder container = new TagBuilder("div");
            container.MergeAttribute("data-plugin", "page");
            container.MergeAttribute("data-mode", paginationMode.ToString());
            if (!string.IsNullOrWhiteSpace(ajaxLoadUrl))
            {
                container.MergeAttribute("data-ajaxloadurl", ajaxLoadUrl);
            }
            if (!string.IsNullOrWhiteSpace(targetId))
            {
                container.MergeAttribute("data-targetid", targetId);
            }
            container.MergeAttribute("data-sum", totalRecords.ToString());
            container.MergeAttribute("data-size", pageSize.ToString());
            container.MergeAttribute("data-pageindex", pageIndex.ToString());
            return MvcHtmlString.Create(container.ToString());
        }

        ///// <summary>
        ///// 构建分页按钮的链接
        ///// </summary>
        ///// <param name="htmlHelper">被扩展的HtmlHelper</param>
        ///// <param name="pageIndex">当前页码</param>
        ///// <returns>分页按钮的url字符串</returns>
        //public static string GetPagingNavigateUrl(this HtmlHelper htmlHelper, int pageIndex, string currentUrl = null)
        //{
        //    object pageIndexObj = null;
        //    if (htmlHelper.ViewContext.RouteData.Values.TryGetValue("pageIndex", out pageIndexObj))
        //    {
        //        htmlHelper.ViewContext.RouteData.Values["pageIndex"] = pageIndex;

        //        return UrlHelper.GenerateUrl(null, null, null, htmlHelper.ViewContext.RouteData.Values, RouteTable.Routes, htmlHelper.ViewContext.RequestContext, true);
        //    }

        //    if (string.IsNullOrEmpty(currentUrl))
        //        currentUrl = HttpUtility.HtmlEncode(htmlHelper.ViewContext.HttpContext.Request.RawUrl);

        //    if (currentUrl.IndexOf("?") == -1)
        //    {
        //        return currentUrl + string.Format("?pageIndex={0}", pageIndex);
        //    }
        //    else
        //    {
        //        if (currentUrl.IndexOf("pageIndex=", StringComparison.InvariantCultureIgnoreCase) == -1)
        //            return currentUrl + string.Format("&pageIndex={0}", pageIndex);
        //        else
        //            return Regex.Replace(currentUrl, @"pageIndex=(\d+\.?\d*|\.\d+)", "pageIndex=" + pageIndex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //    }
        //}

        ///// <summary>
        ///// 生成带Href的链接
        ///// </summary>
        //private static string BuildLink(string linkText, string url, string cssClassName = "tn-page-number")
        //{
        //    return string.Format("<li><a href=\"{0}\" {1}>{2}</a></li>", url, string.IsNullOrEmpty(cssClassName) ? string.Empty : string.Format("class=\"{0}\"", cssClassName), linkText);
        //}
    }


    /// <summary>
    /// 分页按钮显示模式
    /// </summary>
    public enum PaginationMode
    {
        /// <summary>
        /// 上一页/下一页 模式
        /// </summary>
        NextPrevious,

        /// <summary>
        /// 首页/末页/上一页/下一页 模式
        /// </summary>
        //NextPreviousFirstLast,

        /// <summary>
        /// 上一页/下一页 + 数字 模式，例如： 上一页 1 2 3 4 5 下一页
        /// </summary>
        NumericNextPrevious,

        /// <summary>
        /// 加载更多
        /// </summary>
        NextLoadMore
    }
}
