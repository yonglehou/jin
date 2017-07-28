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
using Tunynet.Utilities;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 积分记录Repository
    /// </summary>
    public class PointRecordRepository : Repository<PointRecord>, IPointRecordRepository
    {

        //PointRecord实体、列表 使用正常的缓存策略
        /// <summary>
        ///  清理积分记录
        /// </summary>
        /// <param name="beforeDays">清理beforeDays天以前的积分记录</param>
        /// <param name="cleanSystemPointRecords">是否也删除系统积分记录</param>
        public void CleanPointRecords(int beforeDays, bool cleanSystemPointRecords)
        {
            var sql = Sql.Builder;
            sql.Append("Delete from tn_PointRecords")
                .Where("DateCreated < @0 ", DateTime.Now.AddDays(-beforeDays));
            if (!cleanSystemPointRecords)
                sql.Where("UserId <> @0", 0);
            CreateDAO().Execute(sql);
        }

        /// <summary>
        /// 查询用户积分记录
        /// </summary>
        /// <param name="userId">用户Id<remarks>系统积分的UserId=0</remarks></param>
        /// <param name="isIncome">是不是收入的积分</param>
        /// <param name="pointItemName">积分项目名称</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public PagingDataSet<PointRecord> GetPointRecords(long? userId, bool? isIncome, string pointItemName, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
           
            var sql = Sql.Builder;
            sql.Select("*")
            .From("tn_PointRecords");
            if (pointItemName != null)
                sql.Where("PointItemName like @0", StringUtility.StripSQLInjection(pointItemName) + "%");
            if (isIncome.HasValue)
                sql.Where("IsIncome = @0", isIncome);
            if (userId.HasValue)
                sql.Where("UserId = @0", userId);
            if (startDate.HasValue)
                sql.Where("DateCreated >= @0", startDate);
            if (endDate.HasValue)
                sql.Where("DateCreated < @0", endDate.Value.AddDays(1));

            sql.OrderBy("DateCreated desc");
            return GetPagingEntities(pageSize, pageIndex, sql);
        }

        /// <summary>
        /// 获取用户userId今日获得的交易积分数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetTotalDayStorePoint(long userId)
        {
            //todo:by mazq,20170405,@zhangzh mysql支持isnull吗？ //@mazq 已改正
            Sql sql = Sql.Builder;
            sql.Append("select sum(TradePoints) from tn_PointRecords where UserId = @0 and DateCreated >= @1 and DateCreated < @2", userId, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            int? pointCount = CreateDAO().SingleOrDefault<int?>(sql);

            return pointCount ?? 0;
        }
    }


}