//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Caching;
using Tunynet.Repositories;

namespace Tunynet.Common
{

    public class SpecialContentItemRepository : Repository<SpecialContentItem>, ISpecialContentItemRepository
    {

        /// <summary>
        /// 获取某推荐类型下的前几条
        /// </summary>
        /// <param name="topNumber">前几条</param>
        /// <param name="typeId">推荐类型的ID</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="isDisplayOrderDesc">是否倒序</param>
        /// <returns></returns>
        public IEnumerable<SpecialContentItem> GetTops(int topNumber, int typeId,string tenantTypeId,bool isDisplayOrderDesc)
        {
            Sql sql = Sql.Builder.Where("tn_SpecialContentItems.TypeId=@0", typeId);
            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId=@0", tenantTypeId);
            if (isDisplayOrderDesc)
                sql .OrderBy("tn_SpecialContentItems.DisplayOrder desc");
            else
                sql.OrderBy("tn_SpecialContentItems.DisplayOrder ");
            return GetTopEntities(topNumber, sql);
        }
        /// <summary>
        /// 获取分页下的推荐类型的的所有推荐内容
        /// </summary>
        /// <param name="typeId">推荐类型的ID</param>
        /// <param name="TenantId">推荐内容租户Id</param>
        /// <param name="pageSize">数量</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagingDataSet<SpecialContentItem> Gets(int typeId,string tenantTypeId, int pageSize, int pageIndex)
        {
            Sql sql = Sql.Builder;
            if (typeId!=0)
            {
                sql.Where("TypeId=@0",typeId);
            }
            if (!string.IsNullOrEmpty(tenantTypeId))
            {
                sql.Where("TenantTypeId=@0", tenantTypeId);
            }
            sql.OrderBy("tn_SpecialContentItems.DisplayOrder ");
            return GetPagingEntities(pageSize, pageIndex, sql);
        }


        /// <summary>
        /// 根据租户ID和推荐类别去获取所有的内容
        /// </summary>
        /// <param name="typeId">推荐类型的ID</param>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="isDisplayOrderDesc">是否倒序</param>
        /// <returns></returns>
        public IEnumerable<long> GetItemIds(string tenantTypeId, int typeId, bool isDisplayOrderDesc)
        {

            IEnumerable<long> itemIds = null;
            StringBuilder cacheKey = new StringBuilder();
            if (typeId>0)
            {
              
                cacheKey.Append(RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "TypeId", typeId));
                cacheKey.AppendFormat(":tenantTypeId-{0}", tenantTypeId);
                itemIds= cacheService.Get<IEnumerable<long>>(cacheKey.ToString());
            }
            if (itemIds == null)
            {
                dynamic model = new ExpandoObject();
                model.ItemId = string.Empty;
                model.DisplayOrder = string.Empty;
                Sql sql = Sql.Builder.Select("tn_SpecialContentItems.ItemId as ItemId,tn_SpecialContentItems.DisplayOrder as DisplayOrder").From("tn_SpecialContentItems").Where(" tn_SpecialContentItems.tenantTypeId=@0 and tn_SpecialContentItems.ExpiredDate>@1", tenantTypeId, DateTime.Now.AddDays(-1));
                if (typeId>0)
                    sql.Where("tn_SpecialContentItems.TypeId=@0 ", typeId);
                if (isDisplayOrderDesc)
                    sql.OrderBy("tn_SpecialContentItems.DisplayOrder desc ");
                else
                sql.OrderBy("tn_SpecialContentItems.DisplayOrder ");
                var  models= CreateDAO().Fetch<dynamic>(sql);
                itemIds = models.Select(n => (long)n.ItemId);
                cacheService.Set(cacheKey.ToString(), itemIds, CachingExpirationType.UsualSingleObject);
            }
            return itemIds;
        }



        /// <summary>
        /// 获取特殊内容
        /// </summary>
        public SpecialContentItem GetItem(long itemId, string tenantTypeId, int typeId)
        {
            Sql sql = Sql.Builder.Where("itemId =@0 and  tenantTypeId =@1 and typeId = @2 ", itemId, tenantTypeId, typeId);
            return   CreateDAO().SingleOrDefault<SpecialContentItem>(sql);

        }


        /// <summary>
        /// 获取某内容项下的所有推荐内容
        /// </summary>
        /// <param name="itemId">内容Id</param>
        /// <returns>返回所有推荐内容</returns>
        public IEnumerable<SpecialContentItem> GetItems(long itemId, string tenantTypeId)
        {
            Sql sql = Sql.Builder;
            sql.Where("ItemId =@0 and TenantTypeId =@1", itemId, tenantTypeId);
            return CreateDAO().Fetch<SpecialContentItem>(sql);
        }

        /// <summary>
        /// 根据内容删除推荐（用于内容删除）
        /// </summary>
        public void DeleteSpecialContentItem(long itemId, string tenantTypeId)
        {

            //删除附件
            AttachmentService attachmentServicecourse = new AttachmentService(TenantTypeIds.Instance().Recommend());
            var items = GetItems(itemId, tenantTypeId);
            foreach (var item in items)
            {
                attachmentServicecourse.DeletesByAssociateId(item.Id);
            }
            Sql sql = Sql.Builder.Select("*").From("tn_SpecialContentItems").Where("ItemId = @0 and TenantTypeId = @1 ", itemId, tenantTypeId);
            var specialContentItem = CreateDAO().SingleOrDefault<SpecialContentItem>(sql);
            Delete(specialContentItem);
        }
        /// <summary>
        /// 取消特殊推荐
        /// </summary>
        public void UnStick(long itemId, string tenantTypeId, int typeId)
        {
            Sql sql = Sql.Builder.Append("select  * from tn_SpecialContentItems  where ItemId = @0 and TenantTypeId = @1 and typeId =@2 ", itemId, tenantTypeId, typeId);
            var specialContentItem = CreateDAO().SingleOrDefault<SpecialContentItem>(sql);

            int affectedRecords = base.Delete(specialContentItem);
        }



        /// <summary>
        /// 定期移除过期的推荐内容
        /// </summary>
        public void DeleteExpiredRecommendItems()
        {
            Sql sql = Sql.Builder.Where(" ExpiredDate < @0", DateTime.Now);
            var specialContentItems = CreateDAO().Fetch<SpecialContentItem>(sql);
            //删除附件
            AttachmentService attachmentServicecourse = new AttachmentService(TenantTypeIds.Instance().Recommend());
            foreach (var item in specialContentItems)
            {
                attachmentServicecourse.DeletesByAssociateId(item.Id);
            }
            sql = Sql.Builder.Append("delete from tn_SpecialContentItems where ExpiredDate < @0", DateTime.Now);
            CreateDAO().Execute(sql);
        }


    }
}
