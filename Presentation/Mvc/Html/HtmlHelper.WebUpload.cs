//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Web.Mvc;
using System.Web.Routing;

namespace Tunynet.Common
{
    /// <summary>
    /// 封装Uploadify插件
    /// </summary>
    public static class HtmlHelperUploadifyExtensions
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="htmlHelper">被扩展对象</param>
        /// <param name="name">组件名称</param>
        /// <param name="tenantTypeId">租户类型标识</param>
        /// <param name="ownerId">拥有者标识</param>
        /// <param name="innerContent"></param>
        /// <param name="uploadUrl">文件上传路径</param>
        /// <param name="extensions">允许的文件类型</param>
        /// <param name="btnSelector">指定的上传按钮</param>
        /// <param name="callbacks"></param>
        /// <returns></returns>
        public static MvcHtmlString FileUploader(this HtmlHelper htmlHelper, string name, string tenantTypeId, long ownerId, string innerContent = "", string uploadUrl = "", long associateId = 0, string extensions = "", object callbacks = null, string btnSelector = "", bool showProgress = true, int maxCount = 0, bool multiple = true/*, int fileSizeLimit = 50, UploadFileOptions uploadFileOptions, ButtonOptions buttonOptions = null */)
        {
            if (string.IsNullOrEmpty(name) /* || string.IsNullOrEmpty(allowedFileExtensions) || fileSizeLimit <= 0|| uploadFileOptions == null*/)
            {
                throw new ExceptionFacade("参数不能为空");
            }

            bool hasIcon = !string.IsNullOrWhiteSpace(innerContent) && innerContent.StartsWith("<");
            uploadUrl = string.IsNullOrWhiteSpace(uploadUrl) ? CachedUrlHelper.Action("Uploads", "Common") : uploadUrl;

            TagBuilder builder = new TagBuilder(hasIcon ? "a" : "div");

            if (hasIcon)
            {
                builder.MergeAttribute("class", "btn btn-default");
            }

            builder.MergeAttribute("href", "javascript:;");
            builder.MergeAttribute("name", name);
            builder.MergeAttribute("id", "uploader-" + name);
            builder.MergeAttribute("data-plugin", "fileuploader");
            builder.MergeAttribute("data-uploadurl", uploadUrl);
            builder.MergeAttribute("data-tenanttypeid", tenantTypeId);
            builder.MergeAttribute("data-ownerid", ownerId.ToString());
            builder.MergeAttribute("data-extensions", extensions);
            builder.MergeAttribute("data-selector", btnSelector);
            builder.MergeAttribute("data-show-progress", showProgress.ToString().ToLower());
            builder.MergeAttribute("data-maxCount", maxCount.ToString());
            builder.MergeAttribute("data-associateid", associateId.ToString());
            builder.MergeAttribute("data-multiple", multiple.ToString().ToLower());
            builder.InnerHtml = innerContent;
            if (callbacks != null)
            {
                RouteValueDictionary callbackDict = HtmlHelper.AnonymousObjectToHtmlAttributes(callbacks);

                foreach (var callback in callbackDict)
                    builder.MergeAttribute("data-" + callback.Key.ToLower(), callback.Value.ToString());
            }

            //builder.SetInnerText(innerContent);

            return MvcHtmlString.Create(builder.ToString());
        }



        ///// <summary>
        ///// 封面图设置
        ///// </summary>
        ///// <param name="htmlHelper"></param>
        ///// <param name="name"></param>
        ///// <param name="tenantTypeId"></param>
        ///// <param name="ownerId"></param>
        ///// <param name="imageId"></param>
        ///// <returns></returns>
        //public static MvcHtmlString CoverImageUploder(this HtmlHelper htmlHelper, string name, string tenantTypeId, long ownerId, long imageId = 0)
        //{

        //    string innerHtml = "";
        //    TagBuilder trigger = new TagBuilder("a");


        //    trigger.MergeAttribute("href", "javascript:;");
        //    trigger.MergeAttribute("data-name", name);
        //    trigger.MergeAttribute("class", "jn-training-cover");
        //    trigger.MergeAttribute("data-plugin", "CoverImageUploder");
        //    trigger.MergeAttribute("data-uploadurl", CachedUrlHelper.Action("UploadPicture", "Portal", "", new RouteValueDictionary { { "TenantTypeId", tenantTypeId }, { "ownerId", ownerId }, { "timestamp", "System.DateTime.Now.Millisecond" } }));

        //    trigger.InnerHtml = "<i class=\"fa fa-picture-o\"></i>";

        //    TagBuilder img = new TagBuilder("img");
        //    img.MergeAttribute("id", "spanimg");

        //    if (imageId > 0)
        //    {
        //        Attachment iamge = DIContainer.Resolve<AttachmentService>().Get(imageId);
        //        if (iamge != null)
        //        {
        //            img.MergeAttribute("src", iamge.GetDirectlyUrl());
        //            img.MergeAttribute("style", "width:100px;height:100px;");
        //            trigger.MergeAttribute("style", "display:none;width:100px;height:100px;");
        //        }
        //    }
        //    else
        //    {
        //        img.MergeAttribute("style", "display:none;width:100px;height:100px;");
        //    }

        //    TagBuilder input = new TagBuilder("input");
        //    input.MergeAttribute("type", "hidden");
        //    input.MergeAttribute("name", name);
        //    input.MergeAttribute("value", imageId.ToString());


        //    innerHtml = trigger.ToString() + img.ToString() + input.ToString();

        //    return MvcHtmlString.Create(innerHtml);
        //}

        ///// <summary>
        ///// 整理允许上传的文件类型
        ///// </summary>
        ///// <param name="allowedFileExtensions">从配置文件提取的文件类型字符串</param>
        ///// <returns> 整理好的允许上传的文件类型</returns>
        //private static string FileTypeExts(string allowedFileExtensions)
        //{
        //    StringBuilder str = new StringBuilder();
        //    //默认取配置项里的设置
        //    string[] exts = allowedFileExtensions.Split(',');

        //    foreach (var ext in exts)
        //    {
        //        str.Append("*." + ext + ";");
        //    }
        //    return str.ToString().TrimEnd(';');
        //}
    }
}