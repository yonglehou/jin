//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Repositories;
using System;
using System.Text;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 用户收藏数据访问类
    /// </summary>
    public class FavoriteRepository : Repository<FavoriteEntity>, IFavoriteRepository
    {

        private int pageSize = 20;

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="objectId">被收藏对象Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns>true-收藏成功,false-收藏失败</returns>
        public bool Favorite(long objectId, long userId, string tenantTypeId)
        {
            if (IsFavorited(objectId, userId, tenantTypeId))
                return false;

            FavoriteEntity favorite = FavoriteEntity.New();
            favorite.UserId = userId;
            favorite.ObjectId = objectId;
            favorite.TenantTypeId = tenantTypeId;
            CreateDAO().Insert(favorite);

            if (favorite.Id > 0)
            {
                string cacheKey = GetCacheKey_AllFavorites(userId, tenantTypeId);
                bool isDatabase = false;
                IList<long> objectIds = GetAllObjectIds(userId, tenantTypeId, out isDatabase).ToList();
                //如果是从缓存取出来的则更新缓存
                if (objectIds != null&& !isDatabase)
                {
                    objectIds.Add(objectId);
                    cacheService.Set(cacheKey, objectIds, CachingExpirationType.UsualObjectCollection);
                }
                cacheKey = GetCacheKey_AllUserIdOfObject(objectId, tenantTypeId);
                IList<long> userIds = GetUserIdsOfObject(objectId, tenantTypeId, out isDatabase).ToList();
                //如果是从缓存取出来的则更新缓存
                if (userIds != null && !isDatabase)
                {
                    objectIds.Add(objectId);
                    cacheService.Set(cacheKey, userIds, CachingExpirationType.UsualObjectCollection);
                }
                int count = GetFavoritedUserCount(objectId, tenantTypeId, out isDatabase);
                //如果不是直接访问数据库则更新缓存
                if (!isDatabase)
                {
                    count++;
                    cacheKey = GetCacheKey_FavoritedUserCount(objectId, tenantTypeId);
                    cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="objectId">被收藏对象Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns>true-取消成功,false-取消失败</returns>
        public bool CancelFavorited(long objectId, long userId, string tenantTypeId)
        {
            if (IsFavorited(objectId, userId, tenantTypeId))
            {
                Sql sql = Sql.Builder;
                sql.Append("delete from tn_Favorites where UserId = @0 and ObjectId = @1 and TenantTypeId = @2", userId, objectId, tenantTypeId);
                int affectCount = CreateDAO().Execute(sql);
                if (affectCount > 0)
                {
                    RealTimeCacheHelper.IncreaseAreaVersion("UserId", userId);
                    RealTimeCacheHelper.IncreaseAreaVersion("ObjectId", objectId);
                    string cacheKey = GetCacheKey_AllFavorites(userId, tenantTypeId);
                    bool isDatabase = false;
                    IList<long> objectIds = GetAllObjectIds(userId, tenantTypeId,out isDatabase).ToList();
                    //如果从数据库取出来则移除这个数据
                    if (objectIds != null && objectIds.Contains(objectId)&& !isDatabase)
                    {
                        objectIds.Remove(objectId);
                        cacheService.Set(cacheKey, objectIds, CachingExpirationType.UsualObjectCollection);
                    }

                    cacheKey = GetCacheKey_AllUserIdOfObject(objectId, tenantTypeId);
                    IList<long> userIds = GetUserIdsOfObject(objectId, tenantTypeId, out isDatabase).ToList();
                    //如果是从缓存取出来的则更新缓存
                    if (userIds != null && userIds.Contains(userId) &&!isDatabase)
                    {
                        userIds.Remove(userId);
                        cacheService.Set(cacheKey, userIds, CachingExpirationType.UsualObjectCollection);
                    }
                  
                    int count = GetFavoritedUserCount(objectId, tenantTypeId, out isDatabase);
                    //如果不是直接访问数据库则更新缓存
                    if (!isDatabase)
                    {
                        count--;
                        cacheKey = GetCacheKey_FavoritedUserCount(objectId, tenantTypeId);
                        cacheService.Set(cacheKey, count, CachingExpirationType.SingleObject);
                    }
                   

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 清除某个实体的所有订阅
        /// </summary>
        /// <param name="objectId">实体ID</param>
        /// <returns></returns>
        public bool CleanSubscribesFromObject(long objectId)
        {
            Sql sql = Sql.Builder;
            sql.Append("delete from tn_Favorites where ObjectId = @0", objectId);
            int affectCount = CreateDAO().Execute(sql);
            if (affectCount > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 判断是否收藏
        /// </summary>
        /// <param name="objectId">被收藏对象Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns>true-已收藏，false-未收藏</returns>
        public bool IsFavorited(long objectId, long userId, string tenantTypeId)
        {
            bool isDatabase;
            IEnumerable<long> objectIds = GetAllObjectIds(userId, tenantTypeId,out isDatabase);
            return objectIds != null && objectIds.Contains(objectId);
        }

        /// <summary>
        /// 获取部分收藏对象Id分页数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示的内容数</param>
        ///<returns></returns>
        public PagingDataSet<FavoriteEntity> GetPagingPartObjectIds(long userId, IEnumerable<string> tenantTypeIds, int pageIndex, int? pageSize = null)
        {
            if (!pageSize.HasValue)
            {
                pageSize = this.pageSize;
            }
            Sql sql = Sql.Builder;
            sql.Select("*")
               .From("tn_Favorites")
               .Where("UserId = @0", userId)
               .Where("TenantTypeId in (@tenantTypeIds)", new { tenantTypeIds = tenantTypeIds })
               .OrderBy("Id desc");

            return GetPagingEntities(pageSize.Value, pageIndex, sql);



        }
        /// <summary>
        /// 获取部分收藏对象Id分页数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示的内容数</param>
        ///<returns></returns>
        public PagingDataSet<FavoriteEntity> GetPagingPartObjectIds(long userId, string tenantTypeId, int pageIndex, int? pageSize = null)
        {
            if (!pageSize.HasValue)
            {
                pageSize = this.pageSize;
            }
            Sql sql = Sql.Builder;
            sql.Select("tn_Favorites.Id")
               .From("tn_Favorites")
               .Where("UserId = @0", userId)
               .Where("TenantTypeId =@0", tenantTypeId)
               .OrderBy("Id desc");
            return GetPagingEntities(pageSize.Value, pageIndex, sql);
        }
        /// <summary>
        /// 获取收藏对象Id分页数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示的内容数</param>
        ///<returns></returns>
        public PagingDataSet<long> GetPagingObjectIds(long userId, string tenantTypeId, int pageIndex, int? pageSize = null)
        {
            //PagingEntityIdCollection peic = null;

            Sql sql = Sql.Builder;
            sql.Select("ObjectId")
               .From("tn_Favorites")
               .Where("UserId = @0", userId)
               .Where("TenantTypeId = @0", tenantTypeId)
               .OrderBy("Id desc");

            var dao = CreateDAO();

            if (!pageSize.HasValue)
            {
                pageSize = this.pageSize;
            }

            IEnumerable<object> followedUserIds = null;
            long totalRecords;
            followedUserIds = dao.FetchPagingPrimaryKeys(pageSize.Value, pageIndex, "ObjectId", sql, out totalRecords);

            PagingDataSet<long> pds;
            if (followedUserIds != null)
            {
                pds = new PagingDataSet<long>(followedUserIds.Cast<long>());
                pds.PageSize = pageSize.Value;
                pds.PageIndex = pageIndex;
                pds.TotalRecords = totalRecords;
                return pds;
            }
            else
            {
                pds = new PagingDataSet<long>(new List<long>());
            }
            return pds;

            //if (pageIndex < CacheablePageCount)
            //{
            //    string cacheKey = GetCacheKey_PaingObjectIds(userId, tenantTypeId);
            //peic = cacheService.Get<PagingEntityIdCollection>(cacheKey);
            //if (peic == null)
            //{
            //peic = dao.FetchPagingPrimaryKeys(int.MaxValue, pageSize.Value * CacheablePageCount, 1, "ObjectId", sql);
            //peic.IsContainsMultiplePages = true;
            //    cacheService.Add(cacheKey, peic, CachingExpirationType.ObjectCollection);
            //}
            //}
            //else
            //{
            //    peic = dao.FetchPagingPrimaryKeys(int.MaxValue, pageSize.Value, pageIndex, "ObjectId", sql);
            //}

            //if (peic != null)
            //{
            //    PagingDataSet<long> pds = new PagingDataSet<long>(peic.GetPagingEntityIds(pageSize.Value, pageIndex).Cast<long>());
            //    pds.PageSize = pageSize.Value;
            //    pds.PageIndex = pageIndex;
            //    pds.TotalRecords = peic.TotalRecords;
            //    return pds;
            //}
        }

        /// <summary>
        /// 获取前N个收藏对象Id
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">要获取Id的个数</param>
        ///<returns></returns>
        public IEnumerable<long> GetTopObjectIds(long userId, string tenantTypeId, int topNumber)
        {
            bool isDatabase;
            IEnumerable<long> objectIds = GetAllObjectIds(userId, tenantTypeId,out isDatabase);
            if (objectIds != null)
            {
                return objectIds.Take(topNumber);
            }
            else
            {
                return new List<long>();
            }
        }

        /// <summary>
        /// 获取全部收藏对象Id
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="isDatabase">是读取数据库</param>
        ///<returns></returns>
        public IEnumerable<long> GetAllObjectIds(long userId, string tenantTypeId, out bool isDatabase)
        {
            string cacheKey = GetCacheKey_AllFavorites(userId, tenantTypeId);
            IList<long> objectIds = cacheService.Get<IList<long>>(cacheKey);
            isDatabase = false;
            if (objectIds == null)
            {
                isDatabase = true;
                Sql sql = Sql.Builder;
                sql.Select("ObjectId")
                   .From("tn_Favorites")
                   .Where("UserId = @0", userId)
                   .Where("TenantTypeId = @0", tenantTypeId)
                   .OrderBy("Id desc");

                objectIds = CreateDAO().Fetch<long>(sql);
                cacheService.Set(cacheKey, objectIds, CachingExpirationType.UsualObjectCollection);
            }

            return objectIds;
        }

        /// <summary>
        /// 根据收藏对象获取UserId
        /// </summary>
        /// <param name="objectId">收藏对象Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="isDatabase">是读取数据库</param>
        /// <returns></returns>
        public IEnumerable<long> GetUserIdsOfObject(long objectId, string tenantTypeId, out bool isDatabase)
        {
            string cacheKey = GetCacheKey_AllUserIdOfObject(objectId, tenantTypeId);
            IEnumerable<long> userIds = cacheService.Get<IEnumerable<long>>(cacheKey);
            isDatabase = false;
            if (userIds == null)
            {
                isDatabase = true;
                Sql sql = Sql.Builder;
                sql.Select("UserId")
                   .From("tn_Favorites")
                   .Where("ObjectId = @0", objectId)
                   .Where("TenantTypeId = @0", tenantTypeId)
                   .OrderBy("UserId desc");

                userIds = CreateDAO().FetchFirstColumn(sql).Cast<long>();
                cacheService.Set(cacheKey, userIds, CachingExpirationType.ObjectCollection);
            }

            return userIds;
        }

        /// <summary>
        /// 根据收藏对象获取收藏了此对象的前N个用户Id集合
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">要获取记录数</param>
        /// <returns></returns>
        public IEnumerable<long> GetTopUserIdsOfObject(long objectId, string tenantTypeId, int topNumber)
        {
            bool isDatabase;
            IEnumerable<long> userIds = GetUserIdsOfObject(objectId, tenantTypeId,out isDatabase);
            return userIds != null ? userIds.Take(topNumber) : userIds;
        }

        /// <summary>
        /// 根据收藏对象获取收藏了此对象的用户Id分页集合
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagingDataSet<long> GetPagingUserIdsOfObject(long objectId, string tenantTypeId, int pageIndex)
        {
            var dao = CreateDAO();
            //PagingEntityIdCollection peic = null;

            Sql sql = Sql.Builder;
            sql.Select("UserId")
               .From("tn_Favorites")
               .Where("ObjectId = @0", objectId)
               .Where("TenantTypeId = @0", tenantTypeId)
               .OrderBy("UserId desc");
            IEnumerable<object> followedUserIds = null;
            long totalRecords;

            followedUserIds = dao.FetchPagingPrimaryKeys(pageSize, pageIndex, "UserId", sql, out totalRecords);
            if (followedUserIds != null)
            {
                PagingDataSet<long> pds = new PagingDataSet<long>(followedUserIds.Cast<long>());
                pds.PageSize = pageSize;
                pds.PageIndex = pageIndex;
                pds.TotalRecords = totalRecords;
                return pds;
            }
            //if (pageIndex < CacheablePageCount)
            //{
            //    //string cacheKey = RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "ObjectId", objectId) + string.Format("PagingUserIdsOfObjectId:tenantTypeId-{0}", tenantTypeId);

            //    peic = cacheService.Get<PagingEntityIdCollection>(cacheKey);
            //    if (peic == null)
            //    {
            //        peic = dao.FetchPagingPrimaryKeys(int.MaxValue, pageSize * CacheablePageCount, 1, "UserId", sql);
            //        peic.IsContainsMultiplePages = true;
            //        cacheService.Add(cacheKey, peic, CachingExpirationType.ObjectCollection);
            //    }
            //}
            //else
            //{
            //peic = dao.FetchPagingPrimaryKeys(int.MaxValue, pageSize, pageIndex, "UserId", sql);
            //}

            //if (peic != null)
            //{
            //    PagingDataSet<long> pds = new PagingDataSet<long>(peic.GetPagingEntityIds(pageSize, pageIndex).Cast<long>());
            //    pds.PageSize = pageSize;
            //    pds.PageIndex = pageIndex;
            //    pds.TotalRecords = peic.TotalRecords;
            //    return pds;
            //}

            return null;
        }

        /// <summary>
        /// 根据收藏对象获取同样收藏此对象的我的关注用户
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <param name="userId">当前用户的userId</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public IEnumerable<long> GetFollowedUserIdsOfObject(long objectId, long userId, string tenantTypeId)
        {
            string cacheKey = GetCacheKey_AllFollowedUserIdsOfObject(objectId, userId, tenantTypeId);
            IEnumerable<long> userIds = cacheService.Get<IList<long>>(cacheKey);

            if (userIds == null)
            {
                Sql sql = Sql.Builder;
                sql.Select("distinct FollowedUserId")
                   .From("tn_Follows")
                   .InnerJoin("tn_Favorites Fav")
                   .On("tn_Follows.FollowedUserId = Fav.UserId")
                   .Where("Fav.ObjectId = @0", objectId)
                   .Where("Fav.TenantTypeId = @0", tenantTypeId)
                   .Where("tn_Follows.UserId = @0", userId);

                userIds = CreateDAO().Fetch<long>(sql);
                cacheService.Set(cacheKey, userIds, CachingExpirationType.ObjectCollection);
            }

            return userIds;
        }

        /// <summary>
        /// 获取被收藏数
        /// </summary>
        /// <param name="objectId">收藏对象Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public int GetFavoritedUserCount(long objectId, string tenantTypeId,out  bool isDatabase)
        {
            string cacheKey = GetCacheKey_FavoritedUserCount(objectId, tenantTypeId);
            int count;
            isDatabase = false;
            if (!cacheService.TryGetValue<int>(cacheKey, out count))
            {
                 isDatabase = true;
                Sql sql = Sql.Builder;
                sql.Select("Count(*)")
                   .From("tn_Favorites")
                   .Where("ObjectId = @0", objectId)
                   .Where("TenantTypeId = @0", tenantTypeId);

                count = CreateDAO().SingleOrDefault<int>(sql);

                cacheService.Set(cacheKey, count, CachingExpirationType.ObjectCollection);
            }
            return count;
        }

        /// <summary>
        /// 删除垃圾数据
        /// </summary>
        /// <param name="serviceKey">服务标识</param>
        public void DeleteTrashDatas(string serviceKey)
        {
            IEnumerable<TenantType> tenantTypes = new TenantTypeRepository().Gets(serviceKey);

            List<Sql> sqls = new List<Sql>();
            //2017-5-17 在MySql中 不能先select出同一表中的某些值，再update 或delete 这个表(在同一语句中) 已改正
            sqls.Add(Sql.Builder.Append("delete from tn_Favorites where not exists (select 1 from (select 1 as c from tn_Users,tn_Favorites where tn_Users.UserId = tn_Favorites.UserId) as a)"));

            foreach (var tenantType in tenantTypes)
            {
                Type type = Type.GetType(tenantType.ClassType);
                if (type == null)
                    continue;

                var pd = TableInfo.FromPoco(type);
                sqls.Add(Sql.Builder.Append("delete from tn_Favorites")
                                    .Where("not exists (select 1 from (select 1 as c from tn_Favorites," + pd.TableName + " where tn_Favorites.ObjectId = " + pd.PrimaryKey + ") as a) and tn_Favorites.TenantTypeId = @0"
                                    , tenantType.TenantTypeId));
            }

            CreateDAO().Execute(sqls);
        }

        #region Helper Method

        private static string GetCacheKey_FavoritedUserCount(long objectId, string tenantTypeId)
        {
            return string.Format("FavoritedCount::UserId:{0}-TenaantTypeId:{1}", objectId, tenantTypeId);
        }

        /// <summary>
        /// 获取全部收藏CacheKey
        /// </summary>
        /// <param name="userId">收藏用户Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public string GetCacheKey_AllFavorites(long userId, string tenantTypeId)
        {
            return string.Format("AllFavorites:UserId-{0}:TenantTypeId-{1}", userId, tenantTypeId);
        }

        ///// <summary>
        ///// 获取全部收藏CacheKey
        ///// </summary>
        ///// <param name="userId">收藏用户Id</param>
        ///// <param name="tenantTypeId">租户类型Id</param>
        ///// <returns></returns>
        //public string GetCacheKey_PaingObjectIds(long userId, string tenantTypeId)
        //{
        //    string cacheKeyPrefix = RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "UserId", userId);
        //    return cacheKeyPrefix + string.Format("PaingFavoriteObjectIds:TenantTypeId-{0}", tenantTypeId);
        //}

        /// <summary>
        /// 获取收藏对象全部收藏用户
        /// </summary>
        /// <param name="objectId">收藏对象Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public string GetCacheKey_AllUserIdOfObject(long objectId, string tenantTypeId)
        {
            return string.Format("AllFavorites:ObjectId-{0}:TenantTypeId-{1}", objectId, tenantTypeId);
        }

        /// <summary>
        /// 根据收藏对象获取同样收藏此对象的我的关注用户
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <param name="userId">当前用户的userId</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public string GetCacheKey_AllFollowedUserIdsOfObject(long objectId, long userId, string tenantTypeId)
        {
            return string.Format("AllFollowedUserIdsOfFavorite:ObjectId-{0}:UserId-{1}:TenantTypeId-{2}", objectId, userId, tenantTypeId);
        }

        #endregion Helper Method

    }
}
