//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tunynet.Common
{
    /// <summary>
    /// 计数队列项
    /// </summary>
    [Serializable]
    public class CountQueueItem
    {

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="countType">计数类型</param>
        /// <param name="objectId">计数对象Id</param>
        /// <param name="ownerId">拥有者</param>
        /// <param name="TenantTypeId">租户类型Id</param>
        /// <param name="statisticsCount">当天计数</param>
        public CountQueueItem(string countType, long objectId, long ownerId, int statisticsCount,string TenantTypeId)
        {
            this.CountType = countType;
            this.ObjectId = objectId;
            this.OwnerId = ownerId;
            this.StatisticsCount = statisticsCount;
            this.TenantTypeId = TenantTypeId;
        }

        /// <summary>
        /// 计数对象Id
        /// </summary>
        public long ObjectId { get; set; }

        /// <summary>
        /// 计数类型
        /// </summary>
        public string CountType { get; set; }

        /// <summary>
        ///租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }
        /// <summary>
        /// 拥有者Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// 计数
        /// </summary>
        public int StatisticsCount { get; set; }

        /// <summary>
        /// 转为计数实体
        /// </summary>
        /// <returns></returns>
        public CountEntity AsCountEntity()
        {
            CountEntity entity = new CountEntity();
            entity.CountType = this.CountType;
            entity.ObjectId = this.ObjectId;
            entity.OwnerId = this.OwnerId;
            entity.TenantTypeId = this.TenantTypeId;
            entity.StatisticsCount = this.StatisticsCount;
            return entity;
        }

        /// <summary>
        /// 转为每日计数实体
        /// </summary>
        /// <returns></returns>
        public CountPerDayEntity AsCountPerDayEntity()
        {
            CountPerDayEntity entity = new CountPerDayEntity();
            entity.CountType = this.CountType;
            entity.ObjectId = this.ObjectId;
            entity.OwnerId = this.OwnerId;
            entity.TenantTypeId = this.TenantTypeId;
            entity.StatisticsCount = this.StatisticsCount;
            entity.ReferenceYear = DateTime.Now.Year;
            entity.ReferenceMonth = DateTime.Now.Month;
            entity.ReferenceDay = DateTime.Now.Day;
            return entity;
        }

    }              
}