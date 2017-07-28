//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Repositories;
using Tunynet.Common.Configuration;
using Tunynet.Settings;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 用户等级的数据访问类
    /// </summary>
    public class UserRankRepository : Repository<UserRank>, IUserRankRepository
    {
        private ISettingsManager<PointSettings> pointSettingsManager = DIContainer.Resolve<ISettingsManager<PointSettings>>();


        /// <summary>
        /// 根据一定规则重置所有的用户等级
        /// </summary>
        public void ResetAllUser()
        {
            PointSettings pointSettings = pointSettingsManager.Get();

            var sql_Update = PetaPoco.Sql.Builder;
            sql_Update.Append("update tn_Users set Rank = (select max(Rank) from tn_UserRanks UR where UR.PointLower <= (ExperiencePoints * @0 ) or ((ExperiencePoints * @0 )<0 and UR.Rank = 1) )", pointSettings.ExperiencePointsCoefficient);
            CreateDAO().Execute(sql_Update);
        }

       

        /// <summary>
        /// 插入一条用户等级数据
        /// </summary>
        /// <param name="entity">用户等级</param>
        /// <returns>受影响条数</returns>
        public override void Insert(UserRank entity)
        {
            if (entity.Rank < 1)
                return;

            UserRank userRank = Get(entity.Rank);
            if (userRank != null)
                return;

            base.Insert(entity);
        }
    }
}
