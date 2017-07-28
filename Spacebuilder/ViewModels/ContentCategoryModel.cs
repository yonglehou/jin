//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Tunynet.CMS;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    public class ContentCategoryModel
    {
        /// <summary>
        /// CategoryId
        /// </summary>
        public int CategoryId { get; set; }

        private string categoryname = string.Empty;
        /// <summary>
        /// 栏目名称
        /// </summary>
        [Display(Name = "栏目名称")]
        [Required(ErrorMessage = "请输入栏目名称")]
        [StringLength(100, ErrorMessage = "栏目名称过长")]
        public string CategoryName
        {
            get { return categoryname; }
            set { categoryname = value; }
        }

        private string description = string.Empty;
        /// <summary>
        /// 栏目描述
        /// </summary>
        [Display(Name = "栏目描述")]
        [StringLength(500, ErrorMessage = "栏目描述过长")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int? parentId = 0;
        /// <summary>
        /// ParentId
        /// </summary>
        public int? ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }

        /// <summary>
        /// 父栏目名称
        /// </summary>
        [Display(Name = "父级栏目")]
        public string ParentName
        {
            get
            {
                if (this.ParentId != null && this.ParentId.Value > 0)
                {
                    var parentFolder = DIContainer.Resolve<ContentCategoryService>().Get(this.ParentId.Value);
                    if (parentFolder != null)
                        return parentFolder.CategoryName;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        public bool IsEnabled { get; set; }


        private List<string> contentmodelkeys = new List<string>();
        /// <summary>
        /// 内容模型Id
        /// </summary>
        [Display(Name = "内容模型")]
        [Required(ErrorMessage = "请选择模型")]
        public List<string> ContentModelKeys
        {
            get { return contentmodelkeys; }
            set { contentmodelkeys = value; }
        }

        /// <summary>
        ///所有父级CatetoryId
        /// </summary>
        public string ParentIdList { get; set; }

        /// <summary>
        ///子栏目数目
        /// </summary>
        public int ChildCount { get; set; }

        /// <summary>
        ///深度(从0开始)
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        ///内容计数
        /// </summary>
        public int ContentCount { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool IsComment { get; set; }

        /// <summary>
        /// 在栏目列表显示
        /// </summary>
        public bool IsListDisplay { get; set; }

        /// <summary>
        /// 是否继承父栏目
        /// </summary>
        public bool IsInherit { get; set; }

        /// <summary>
        /// 栏目管理员
        /// </summary>
        [Display(Name = "栏目管理员")]
        public List<string> ContentCategoryAdmin { get; set; }

        /// <summary>
        ///排列顺序，默认和CategoryId一致
        /// </summary>
        public int DisplayOrder { get; set; }


        /// <summary>
        ///转换指定字段为数据模型
        /// </summary>
        /// <returns></returns>
        public ContentCategoryPortal AsContentCategory()
        {
            ContentCategoryPortal contentCategoryPortal = new ContentCategoryPortal();
            ContentCategory contentcategory = null;
            if (CategoryId > 0)
            {
                contentcategory = DIContainer.Resolve<ContentCategoryService>().Get(CategoryId);
                contentcategory.MapTo(contentCategoryPortal);
            }
            else
            {
                contentCategoryPortal = new ContentCategoryPortal();               
            }
            if (contentcategory == null || contentcategory.ParentIdList == null)
                contentCategoryPortal.ParentIdList = "";
            if (ParentId > 0)
            {
                contentCategoryPortal.IsInherit = IsInherit;
                contentCategoryPortal.ParentId = ParentId.Value;
            }

            contentCategoryPortal.ContentModelKeys = string.Join(",", ContentModelKeys.ToArray());
            contentCategoryPortal.DateCreated = DateTime.Now;
            contentCategoryPortal.CategoryName = CategoryName == null ? "" : CategoryName;
            contentCategoryPortal.IsEnabled = IsEnabled;
            contentCategoryPortal.Description = Description == null ? "" : Description;
            contentCategoryPortal.IsComment = IsComment;
            contentCategoryPortal.IsListDisplay = IsListDisplay;

            return contentCategoryPortal;

        }
    }

    public static class ContentCategoryModelExtensions
    {
        ///// <summary>
        ///// 静态构造
        ///// </summary>
        //static ContentCategoryModelExtensions()
        //{
        //    //创建映射配置
        //    Mapper.CreateMap<ContentCategory, ContentCategoryModel>();
        //    Mapper.CreateMap<ContentCategoryModel, ContentCategory>();
        //}

        /// <summary>
        /// 转换为数据模型
        /// </summary>
        /// <returns></returns>
        public static ContentCategory AsContentCategory(this ContentCategoryModel source)
        {
            return Mapper.Map<ContentCategory>(source);
        }


    }

}

