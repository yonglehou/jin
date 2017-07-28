//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;
using System.Text;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 积分项目Repository
    /// </summary>
    public class PointItemRepository : Repository<PointItem>, IPointItemRepository
    {

        /// <summary>
        /// 更新积分项目
        /// </summary>
        /// <param name="entity">待更新的积分项目</param>
        public override void Update(PointItem entity)
        {
            //注意：ItemId、ApplicationId、ItemName、DisplayOrder不允许修改
            var sql = Sql.Builder;
            sql.Append("Update tn_PointItems set ExperiencePoints = @0, ReputationPoints = @1, TradePoints = @2, TradePoints2 = @3, TradePoints3 = @4, TradePoints4 = @5, Description = @6 where ItemKey = @7", entity.ExperiencePoints, entity.ReputationPoints, entity.TradePoints, entity.TradePoints2, entity.TradePoints3, entity.TradePoints4, entity.Description, entity.ItemKey);
            CreateDAO().Execute(sql);
         

            base.OnUpdated(entity);
        }

        /// <summary>
        /// 获取积分项目集合
        /// </summary>
        /// <param name="applicationId">应用Id</param>
        /// <returns>如果无满足条件的积分项目返回空集合</returns>
        public List<PointItem> GetPointItems(string tenantTypeId = "")
        {           
            List<string> itemKeys = new List<string>();
           
            var sql = Sql.Builder;
            sql.Select("*")
               .From("tn_PointItems");
            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);
            sql.OrderBy("DisplayOrder");
         
            return CreateDAO().Fetch<PointItem>(sql);
        }
    }
}