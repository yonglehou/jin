//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Tunynet.Caching;
using Tunynet;
using Tunynet.Repositories;
using Tunynet.Common;

namespace Tunynet.Spacebuilder
{
    public class ContentCategoryPortal : Tunynet.CMS.ContentCategory, IEntity
    {

        #region 需持久化属性
        /// <summary>
        /// 是否允许评论
        /// </summary>
        [Ignore]
        public bool IsComment
        {
            get
            {
                return GetExtendedProperty<bool>("IsComment");
            }
            set
            {
                SetExtendedProperty("IsComment", value);
            }
        }
        /// <summary>
        /// 继承父栏目设置
        /// </summary>
        [Ignore]
        public bool IsInherit
        {
            get
            {
                return GetExtendedProperty<bool>("IsInherit");
            }
            set
            {
                SetExtendedProperty("IsInherit", value);
            }
        }
        /// <summary>
        /// 必填标题图
        /// </summary>
        [Ignore]
        public bool IsListDisplay
        {
            get
            {
                return GetExtendedProperty<bool>("IsListDisplay");
            }
            set
            {
                SetExtendedProperty("IsListDisplay", value);
            }
        }
        
        /// <summary>
        /// 栏目管理员
        /// </summary>
        [Ignore]
        public string ContentCategoryAdmin
        {
            get
            {
                return GetExtendedProperty<string>("ContentCategoryAdmin");
            }
            set
            {
                SetExtendedProperty("ContentCategoryAdmin", value);
            }
        }

        #endregion
    }
}