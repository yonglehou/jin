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

namespace Tunynet.Common
{
    /// <summary>
    /// 积分纪录实体类
    /// </summary>
    [TableName("tn_PointRecords")]
    [PrimaryKey("RecordId", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class PointRecord : IEntity
    {
        //除空参数的构造函数以外，保留一个具有UserId、PointItemName、Description、ExperiencePoints、ReputationPoints、TradePoints参数的构造函数
        //缓存分区：UserId

        /// <summary>
        /// 无参构造器
        /// </summary>
        public PointRecord() { }
        /// <summary>
        /// 带参构造器
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pointItem"></param>
        /// <param name="description"></param>
        public PointRecord(long userId, PointItem pointItem, string description)
        {
            this.UserId = userId;
            this.PointItemName = pointItem.ItemName;
            this.Description = description;
            this.ExperiencePoints = pointItem.ExperiencePoints;
            this.ReputationPoints = pointItem.ReputationPoints;
            this.TradePoints = pointItem.TradePoints;
            this.TradePoints2 = pointItem.TradePoints2;
            this.TradePoints3 = pointItem.TradePoints3;
            this.TradePoints4 = pointItem.TradePoints4;

            DateCreated = DateTime.Now;
        }

        public PointRecord(long userId,long operatorUserId, string pointItemName, string description, int experiencePoints, int reputationPoints, int tradePoints)
        {
            this.UserId = userId;
            this.OperatorUserId = operatorUserId;
            this.PointItemName = pointItemName;
            this.Description = description;
            this.ExperiencePoints = experiencePoints;
            this.ReputationPoints = reputationPoints;
            this.TradePoints = tradePoints;
            DateCreated = DateTime.Now;
        }

        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static PointRecord New()
        {
            PointRecord pointRecord = new PointRecord()
            {
                PointItemName = string.Empty,
                Description = string.Empty,
                DateCreated = DateTime.Now

            };
            return pointRecord;
        }

        #region 需持久化属性

        /// <summary>
        ///RecordId
        /// </summary>
        public long RecordId { get; protected set; }

        /// <summary>
        ///用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///操作者用户Id
        /// </summary>
        public long OperatorUserId { get; set; }
        /// <summary>
        ///积分项目名称
        /// </summary>
        public string PointItemName { get; set; }

        /// <summary>
        ///积分描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///经验积分值
        /// </summary>
        public int ExperiencePoints { get; set; }

        /// <summary>
        ///威望积分值
        /// </summary>
        public int ReputationPoints { get; set; }

        /// <summary>
        ///交易积分值
        /// </summary>
        public int TradePoints { get; set; }

        /// <summary>
        ///交易积分值2
        /// </summary>
        public int TradePoints2 { get; set; }

        /// <summary>
        ///交易积分值3
        /// </summary>
        public int TradePoints3 { get; set; }

        /// <summary>
        ///交易积分值4
        /// </summary>
        public int TradePoints4 { get; set; }


        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime DateCreated { get; protected set; }

        #endregion

        #region 扩展方法
        public string UserDisplayName
        {
            get
            {
                UserService userService = DIContainer.Resolve<UserService>();
                var user = userService.GetUser(UserId);
                return user == null ? "" : user.DisplayName;
            }
        }

        public string OperatorUserDisplayName
        {
            get
            {
                UserService userService = DIContainer.Resolve<UserService>();
                var user = userService.GetUser(OperatorUserId);
                return user == null ? "系统" : user.DisplayName;
            }
        }
        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.RecordId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
