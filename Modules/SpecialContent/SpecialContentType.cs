//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tunynet.Common
{
    /// <summary>
    /// 推荐的类别
    /// </summary>
    [TableName("tn_SpecialContentTypes")]
    [PrimaryKey("TypeId", autoIncrement = false)]

    public class SpecialContentType : IEntity
    {
        public static SpecialContentType New()
        {
            SpecialContentType specialContentType = new SpecialContentType();
            specialContentType.TypeId = 0;
            specialContentType.Name = string.Empty;
            specialContentType.Description = string.Empty;
            specialContentType.RequireFeaturedImage = false;

            return specialContentType;

        }
        /// <summary>
        /// 类型ID（创建后不允许修改）
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 推荐类型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 推荐类型描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantTypeId { get; set; }
        /// <summary>
        /// 是否需要截止日期
        /// </summary>
        public bool RequireExpiredDate { get; set; }

        /// <summary>
        /// 是否包含标题图
        /// </summary>
        public bool RequireFeaturedImage { get; set; }

        /// <summary>
        /// 是否允许添加外链
        /// </summary>
        public bool AllowExternalLink { get; set; }
        /// <summary>
        /// 是否系统内置
        /// </summary>
        public bool IsBuiltIn { get; set; }

        /// <summary>
        /// 标题图说明
        /// </summary>
        public string FeaturedImageDescrption { get; set; }


        #region IEntity 成员
        object IEntity.EntityId { get { return this.TypeId; } }
        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
