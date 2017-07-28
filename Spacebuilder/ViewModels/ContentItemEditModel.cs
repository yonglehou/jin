//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Tunynet.Utilities;
using Tunynet.Common;
using Tunynet.CMS;
using System.Linq;

namespace Tunynet.Spacebuilder
{
    [Serializable]
    public class ContentItemEditModel
    {
        /// <summary>
        ///扩展属性
        /// </summary>
        [AllowHtml]
        public IDictionary<string, object> AdditionalProperties { get; set; }

        /// <summary>
        ///主键标识
        /// </summary>
        public int ContentItemId { get; set; }

        /// <summary>
        ///栏目Id
        /// </summary>
        [Required(ErrorMessage = "请选择栏目")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "请选择栏目")]
        public int CategoryId { get; set; }

        /// <summary>
        ///内容模型Id
        /// </summary>
        public int ContentModelId { get; set; }

        /// <summary>
        ///标题允许录入及修改
        /// </summary>
        [Required(ErrorMessage = "请输入标题")]
        [StringLength(200, ErrorMessage = "最多可以输入{1}个字符")]
        [RegularExpression("^[^\\s'][^']*$", ErrorMessage = "不能以空格开头且不能输入单引号")]
        public string Subject { get; set; }
        /// <summary>
        ///内容
        /// </summary>
        [Required(ErrorMessage = "请输入内容")]
        [AllowHtml]
        public string Body { get; set; }
        /// <summary>
        ///标题图Id
        /// </summary>
        public long FeaturedImageAttachmentId { get; set; }

        /// <summary>
        ///标题图文件（带部分路径）
        /// </summary>
        public string FeaturedImage { get; set; }

        /// <summary>
        ///发布者UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///发布者DisplayName
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///摘要
        /// </summary>
        [StringLength(200, ErrorMessage = "最多可以输入{1}个字符")]
        public string Summary { get; set; }

        /// <summary>
        ///是否锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        ///排序
        /// </summary>
        public long DisplayOrder { get; set; }

        /// <summary>
        ///IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///发布时间
        /// </summary>
        //[Required(ErrorMessage = "请填写发布时间")]
        public DateTime DatePublished { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///资讯审核状态
        /// </summary>
        public int AudiStatus { get; set; }

        /// <summary>
        ///资讯作者Id
        /// </summary>
        public string CMSAuthorIds { get; set; }

        /// <summary>
        ///资讯作者
        /// </summary>
        public string CMSAuthors { get; set; }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool IsComment { get; set; }

        /// <summary>
        /// 是否允许附件下载
        /// </summary>
        public string IsAttachment { get; set; }

        /// <summary>
        /// 是否允许内容末尾显示附件列表
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 是否指定人员
        /// </summary>
        public bool IsSpecifyUser { get; set; }

        /// <summary>
        /// 领导Guid
        /// </summary>
        public List<string> LeaderGuids { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 信息来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 文章地址
        /// </summary>
        public string NewsUrl { get; set; }
        /// <summary>
        /// 流转用户
        /// </summary>
        public string[] UserGuid { get; set; }
        /// <summary>
        /// 流转用户
        /// </summary>
        public string[] UserGuidl { get; set; }

        /// <summary>
        ///是否同意
        /// </summary>
        public string IsorNotConsent { get; set; }

        /// <summary>
        ///意见
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        ///是否匿名
        /// </summary>
        public bool IsAnonymous { get; set; }
        /// <summary>
        /// 视频地址
        /// </summary>
        [RegularExpression("^(https?)://.*", ErrorMessage = "输入的地址有误")]
        public string VideoUrl { get; set; }

        #region 提交表单扩展

        /// <summary>
        /// Body Imgid
        /// </summary>
        public string BodyImageAttachmentId { get; set; }

        /// <summary>
        /// Body Imgids
        /// </summary>
        public IEnumerable<long> BodyImageAttachmentIds
        {
            get
            {
                if (string.IsNullOrEmpty(BodyImageAttachmentId))
                    return null;
                var bodyImageIds = new List<string>(BodyImageAttachmentId.Split(','));
                var bodyImageIdslong = bodyImageIds.Where(n => n.Length > 0);
                if (bodyImageIdslong.Count() > 0)
                {
                    bodyImageIdslong = bodyImageIdslong.Where(n => n != "undefined");
                    if (bodyImageIdslong.Count() > 0)
                        return bodyImageIdslong.Select(n => Convert.ToInt64(n)).ToList();
                }
                return null;
            }
        }

        #endregion


        #region 
        /// <summary>
        /// 是否为草稿
        /// </summary>
        public bool IsDraft { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public List<string> tagvalue { get; set; }

        /// <summary>
        /// 置顶
        /// </summary>
        public bool IsSticky { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        public string VideoUrls
        {
            get
            {
                string body = string.Empty;
                if (AdditionalProperties.ContainsKey("VideoUrl"))
                    return AdditionalProperties["VideoUrl"].ToString(); ;
                return body;
            }
        }


        #endregion


        /// <summary>
        ///转换指定字段为数据模型
        /// </summary>
        /// <returns></returns>
        public ContentItem AsContentItem(HttpRequestBase Request)
        {
            ContentItem contentItem = null;
            var _currentUser = UserContext.CurrentUser;
            if (ContentItemId > 0)
            {
                contentItem = DIContainer.Resolve<ContentItemService>().Get(ContentItemId);
            }
            else
            {
                contentItem = ContentItem.New();
                contentItem.Author = _currentUser.DisplayName;
                contentItem.UserId = _currentUser.UserId;
            }
            if (CategoryId > 0)
            {
                var folder = DIContainer.Resolve<ContentCategoryService>().Get(CategoryId);
                if (folder != null)
                {
                    contentItem.ContentCategoryId = CategoryId;
                    if (AdditionalProperties == null)
                        AdditionalProperties = new Dictionary<string, object>();

                    var contentTypeColumnDefinitions =
                        DIContainer.Resolve<ContentModelService>().GetColumnsByContentModelId((int)ContentModelId);
                    foreach (var item in contentTypeColumnDefinitions)
                    {
                        object value;
                        switch (item.DataType)
                        {
                            case "int":
                            case "long":
                            case "float":
                                value = Request.Form.Get<long>(item.FieldName, 0);
                                break;
                            case "datetime":
                                value = Request.Form.Get(item.FieldName, DateTime.MinValue);
                                break;

                            case "bool":
                                value = Request.Form.Get(item.FieldName, false);
                                break;
                            default:
                                value = Request.Form.Get(item.FieldName, "");
                                break;
                        }
                        if (AdditionalProperties.ContainsKey(item.FieldName))
                            AdditionalProperties[item.FieldName] = value;
                        else
                            AdditionalProperties.Add(item.FieldName, value);
                    }
                }
            }
            contentItem.AdditionalProperties = AdditionalProperties;

            contentItem.ContentModelId = ContentModelId;
            contentItem.IsLocked = false;


            contentItem.FeaturedImageAttachmentId = FeaturedImageAttachmentId;
            contentItem.Subject = Subject;
            contentItem.Body = Body == null ? string.Empty : Body;
            if (DatePublished.CompareTo(DateTime.MinValue) > 0)
                contentItem.DatePublished = DatePublished;

            //摘要
            contentItem.Summary = Summary ?? string.Empty;           

            contentItem.IsComment = IsComment;
            contentItem.IsSticky = IsSticky;
            contentItem.IsVisible = IsVisible;
            contentItem.BodyImageAttachmentIds = BodyImageAttachmentIds;

            return contentItem;
        }

    }


}