//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Linq;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 友情链接数据访问
    /// </summary>
    public class LinkRepository : Repository<LinkEntity>, ILinkRepository
    {
        /// <summary>
        /// 获取Owner友情链接
        /// </summary>
        /// <param name="ownerType">拥有者类型</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns></returns>
        public IEnumerable<LinkEntity> GetsOfOwner(int ownerType, long ownerId, int topNumber)
        {
            StringBuilder cacheKey = new StringBuilder();
            cacheKey.Append(RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "OwnerId", ownerId));
            cacheKey.AppendFormat("GetsOfOwner::ownerType-{0}", ownerType);

            IEnumerable<long> linkIds = cacheService.Get<IEnumerable<long>>(cacheKey.ToString());
            if (linkIds == null)
            {
                Sql sql = Sql.Builder;
                sql.Select("*")
                    .From("tn_Links")
                    .Where("ownerType=@0 and ownerId=@1", ownerType, ownerId)
                    .OrderBy("displayorder");

                linkIds = CreateDAO().FetchPrimaryKeys<LinkEntity>(sql).Cast<long>();
                cacheService.Set(cacheKey.ToString(), linkIds, CachingExpirationType.RelativelyStable);
            }

            return PopulateEntitiesByEntityIds(linkIds.Take(topNumber));
        }

        /// <summary>
        /// 获取站点友情链接(后台管理)
        /// </summary>
        /// <param name="categoryId">分页标识</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public IEnumerable<LinkEntity> GetsOfSiteForAdmin(long? categoryId)
        {
            Sql sql = Sql.Builder.Append("select * from tn_Links");
            if (categoryId.HasValue)
            {
                sql.InnerJoin("tn_ItemsInCategories")
                   .On("tn_Links.LinkId=tn_ItemsInCategories.ItemId")
                   .Where("categoryId=@0", categoryId.Value)
                   .OrderBy("displayorder");
            }
            else
            {
                sql.OrderBy("displayorder");
            }

            return GetPagingEntities(1000, 1, sql);
        }
    }
}
