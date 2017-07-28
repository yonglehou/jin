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
    ///评价汇总实体
    /// </summary>
    [TableName("tn_ReviewSummaries")]
    [PrimaryKey("Id",autoIncrement =true)]
    [CacheSetting(true)]
    [Serializable]
    public class ReviewSummary : IEntity
    {
       

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
        /// 被评论对象
        /// </summary>
        public long ReviewedObjectId { get; set; }

        /// <summary>
        /// 拥有者Id
        /// </summary>
        public long OwnerId { get; set; }
  
      
        /// <summary>
        /// 星级评价总评分
        /// </summary>
        public int RateSum { get; set; }
        /// <summary>
        /// 星级评价人数
        /// </summary>
        public int RateCount { get; set; }
        /// <summary>
        /// 好评数
        /// </summary>
        public int PositiveReivewCount { get; set; }

        /// <summary>
        /// 中评数
        /// </summary>
        public int ModerateReivewCount { get; set; }
        /// <summary>
        /// 差评数
        /// </summary>
        public int NegativeReivewCount { get; set; }


        #endregion

        #region 扩展属性

     
        /// <summary>
        /// 星级评价平均分
        /// </summary>
        [Ignore]
        public int RateAverage
        {
            get
            {
                return this.RateSum / this.RateCount;
            }
        }

        #endregion




        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion IEntity 成员
    }
}
