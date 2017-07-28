//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;
using Tunynet.Caching;

namespace Tunynet.Common
{
    /// <summary>
    /// 评价实体
    /// </summary>
    [TableName("tn_Reviews")]
    [PrimaryKey("Id",autoIncrement =true)]
    [CacheSetting(true)]
    [Serializable]
    public class Review : IEntity
    {
        public static Review New()
        {
            Review review = new Review
            {
                Author = string.Empty,
                Body = string.Empty,
                TenantTypeId = string.Empty,
                ReviewRank = Review_Type.Positive,
                IP = string.Empty,
                DateCreated = DateTime.Now
            };
            return review;
        }

        #region 需持久化属性
        /// <summary>
        /// 自增长
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 被评价对象ID
        /// </summary>
        public long ReviewedObjectId { get; set; }
        /// <summary>
        /// 拥有者Id
        /// </summary>
        public long OwnerId { get; set; }
        /// <summary>
        ///评价人UserId
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 评论人名称
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 评价 内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 星级评价评分
        /// </summary>
        public int RateNumber { get; set; }
       
        /// <summary>
        /// 好中差评
        /// </summary>
        public Review_Type ReviewRank { get; set; }
		
        /// <summary>
        /// 是否匿名
        /// </summary>
        public int IsAnonymous { get; set; }
        /// <summary>
        /// 评论人Ip
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        #endregion


        #region 扩展属性
        /// <summary>
        /// 获取标题图列表url
        /// </summary>
        /// <returns></returns>
        public List<string> GetImageUrl(string key)
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Review());
            List<string> urls = new List<string>();
            var imageurl = string.Empty;
            var attachments = attachmentService.GetsByAssociateId(this.Id);
            foreach (var item in attachments)
            {
                urls.Add(item.GetDirectlyUrl(key));
            }
            return urls;
        }

        /// <summary>
        /// 获取子集评论实体
        /// </summary>
        [Ignore]
        public Review GetChildrenReview
        {
            get
            {
                return new ReviewRepository().GetChildrenReview(this.Id);
            }
        }


        #endregion


        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion IEntity 成员
    }
}
