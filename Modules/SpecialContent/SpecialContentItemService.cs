//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tunynet.Caching;
using Tunynet.Events;

namespace Tunynet.Common
{
    /// <summary>
    /// 推荐内容管理
    /// </summary>
    public class SpecialContentitemService
    {
        private ISpecialContentItemRepository contentItemRepositories;
        private ISpecialContentTypeRepository contentTypeRepository;
        private ICacheService cacheService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contentItemRepositories"></param>
        public SpecialContentitemService(ISpecialContentItemRepository contentItemRepositories, ISpecialContentTypeRepository contentTypeRepository, ICacheService cacheService)
        {
            this.contentItemRepositories = contentItemRepositories;
            this.contentTypeRepository = contentTypeRepository;
            this.cacheService = cacheService;
        }

        /// <summary>
        /// 创建推荐内容
        /// </summary>
        /// <param name="itemId">推荐内容Id</param>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="typeId">推荐类别</param>
        /// <param name="recommenderUserId">推荐人Id</param>
        /// <param name="expiredDate">推荐期限</param>
        /// <param name="featuredImageAttachmentId"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool Create(long itemId, string tenantTypeId, int typeId, long recommenderUserId, string itemName, long featuredImageAttachmentId = 0, DateTime? expiredDate = null, string link = null)
        {
            string cachekey = RecommendCachekey(tenantTypeId, itemId);
            cacheService.Remove(cachekey);
            SpecialContentItem specialContentitem = SpecialContentItem.New(itemId, tenantTypeId, typeId, recommenderUserId, itemName);
            var contentType = contentTypeRepository.Get(typeId);
            //如果截止时间有值 并且推荐类型允许截止时间 则赋值截止时间 否则没有截止时间
            if (contentType.RequireExpiredDate && expiredDate.HasValue) specialContentitem.ExpiredDate = expiredDate.Value;
            else specialContentitem.ExpiredDate = DateTime.Now.AddYears(20);
            //是否包含标题图
            if (contentType.RequireFeaturedImage && featuredImageAttachmentId != 0)
                specialContentitem.FeaturedImageAttachmentId = featuredImageAttachmentId;
            //是否是外链 或者站内链接
            if (link != null)
                specialContentitem.Link = link;
            
            var item = contentItemRepositories.GetItem(itemId, tenantTypeId, typeId);
            contentItemRepositories.Delete(item);

            contentItemRepositories.Insert(specialContentitem);
            if (specialContentitem.Id > 0)
            {
                specialContentitem.DisplayOrder = specialContentitem.Id;
                contentItemRepositories.Update(specialContentitem);
                EventBus<SpecialContentItem, CommonEventArgs>.Instance().OnAfter(specialContentitem,new CommonEventArgs(EventOperationType.Instance().Create()));
                return true;
            }
            else
                return false;

        }


        /// <summary>
        /// 调整顺序，用于交换两条记录的顺序
        /// </summary>
        ///<param name="firstId">第一个推荐内容Id</param>
        /// <param name="secondId">第二个推荐内容Id</param>
        public void ChangeOrder(long firstId, long secondId)
        {
            var firstItem = Get(firstId);
            var secondItem = Get(secondId);
            long firstDisplayOrder = firstItem.DisplayOrder;
            firstItem.DisplayOrder = secondItem.DisplayOrder;
            contentItemRepositories.Update(firstItem);

            secondItem.DisplayOrder = firstDisplayOrder;
            contentItemRepositories.Update(secondItem);

        }
        /// <summary>
        /// 更新推荐内容
        /// </summary>
        /// <param name="specialContentitem">推荐实体</param>
        /// <param name="typeIds">推荐类别的ID数组</param>
        public void Update(SpecialContentItem specialContentitem)
        {
            contentItemRepositories.Update(specialContentitem);
            EventBus<SpecialContentItem, CommonEventArgs>.Instance().OnAfter(specialContentitem, new CommonEventArgs(EventOperationType.Instance().Update()));
        }
        /// <summary>
        /// 获取某内容项下的所有推荐内容
        /// </summary>
        /// <param name="itemId">内容Id</param>
        /// <returns></returns>
        public IEnumerable<SpecialContentItem> GetItems(long itemId, string tenantTypeId)
        {
            return contentItemRepositories.GetItems(itemId, tenantTypeId);
        }

        /// <summary>
        /// 获取推荐内容的实体
        /// </summary>
        /// <param name="id">推荐内容的ID</param>
        /// <returns></returns>
        public SpecialContentItem Get(long id)
        {
            return contentItemRepositories.Get(id);
        }
        /// <summary>
        /// 判断是否是否特殊内容
        /// </summary>
        public bool IsSpecial(long itemId, string tenantTypeId, int typeId)
        {
                var item = contentItemRepositories.GetItem(itemId, tenantTypeId, typeId);
                if (item != null && item.ExpiredDate > DateTime.Now.AddDays(-1))
                return true;
                else
                return false;

        }

        /// <summary>
        /// 根据租户ID和推荐类别去获取所有的内容
        /// </summary>
        /// <param name="typeId">推荐类型的ID</param>
        /// <param name="tenantTypeId">租户Id</param>
        ///  <param name="isDisplayOrderDesc">是否倒序</param>
        /// <returns></returns>
        public IEnumerable<long> GetItemIds(string tenantTypeId, int typeId, bool isDisplayOrderDesc = true)
        {
            return contentItemRepositories.GetItemIds(tenantTypeId, typeId , isDisplayOrderDesc);
        }
        /// <summary>
        /// 获取前几条根据推荐类型的推荐内容
        /// </summary>
        /// <param name="topNumber"></param>
        /// <param name="typeId"></param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="isDisplayOrderDesc">是否倒序</param>
        /// <returns></returns>
        public IEnumerable<SpecialContentItem> GetTops(int topNumber, int typeId, string tenantTypeId, bool isDisplayOrderDesc=false)
        {
            return contentItemRepositories.GetTops(topNumber, typeId, tenantTypeId, isDisplayOrderDesc);
        }
       
        /// <summary>
        /// <summary>
        /// 获取分页下的推荐类型的的所有推荐内容
        /// </summary>
        /// <param name="typeId">推荐类型的ID</param>
        /// <param name="tenantTypeId">推荐内容租户Id</param>
        /// <param name="pageSize">数量</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagingDataSet<SpecialContentItem> Gets(int typeId, string tenantTypeId, int pageSize, int pageIndex)
        {
            return contentItemRepositories.Gets(typeId, tenantTypeId, pageSize, pageIndex);
        }

        /// <summary>
        /// 上传标题图
        /// </summary>
        /// <param name="id">推荐内容的ID</param>
        /// <param name="stream"></param>
        public string UploadFeaturedImage(long id, Stream stream)
        {
            string featuredImage = string.Empty;
            if (stream != null)
            {
                SpecialContentItem specialContentItem = contentItemRepositories.Get(id);
                ImageAccessor imageAccessor = new ImageAccessor(TenantTypeIds.Instance().Recommend());
                featuredImage = imageAccessor.Save(stream, id, specialContentItem.DateCreated);
            }
            return featuredImage;
        }

        /// <summary>
        /// 删除标题图
        /// </summary>
        /// <param name="itemId">推荐内容的ID</param>
        public void DeleteFeaturedImage(long itemId)
        {
            SpecialContentItem specialContentitem = contentItemRepositories.Get(itemId);
            ImageAccessor imageAccessor = new ImageAccessor(TenantTypeIds.Instance().Recommend());
            imageAccessor.Delete(specialContentitem.DateCreated, itemId);
            specialContentitem.FeaturedImageAttachmentId = 0;
            contentItemRepositories.Update(specialContentitem);
        }
        /// <summary>
        /// 取消特殊推荐
        /// </summary>
        public void UnStick(long itemId, string tenantTypeId, int typeId)
        {
            
            //记录日志
            var specialContentitem=GetItems(itemId, tenantTypeId).Where(n=>n.TypeId == typeId).FirstOrDefault();
            EventBus<SpecialContentItem, CommonEventArgs>.Instance().OnAfter(specialContentitem, new CommonEventArgs(EventOperationType.Instance().Delete()));

            contentItemRepositories.UnStick(itemId, tenantTypeId, typeId);
            string cachekey = RecommendCachekey(tenantTypeId, itemId);
            cacheService.Remove(cachekey);
        }

        /// <summary>
        /// 根据内容删除推荐（用于内容删除）
        /// <param name="itemId">推荐内容Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// </summary>
        public void DeleteSpecialContentItem(long itemId, string tenantTypeId)
        {
            
            contentItemRepositories.DeleteSpecialContentItem(itemId, tenantTypeId);
            string cachekey = RecommendCachekey(tenantTypeId, itemId);
            cacheService.Remove(cachekey);
        }


        /// <summary>
        /// 定期移除过期的推荐内容
        /// </summary>
        public void DeleteExpiredRecommendItems()
        {
            //执行删除
            contentItemRepositories.DeleteExpiredRecommendItems();
          
        }

        /// <summary>
        /// 是否是推荐内容(排除精华)
        /// </summary>
        /// <param name="itemId">推荐内容Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public bool IsRecommend(string tenantTypeId, long itemId)
        {
            string cachekey = RecommendCachekey(tenantTypeId, itemId);
            bool? result;
            cacheService.TryGetValue<bool?>(cachekey, out result);
            if (result==null)
            {
            var contentItems = contentItemRepositories.GetItems(itemId, tenantTypeId).Where(n=>n.TypeId!=SpecialContentTypeIds.Instance().Essential());
            if (contentItems.Count() > 0)
                result= true;
            else
                result= false;

            cacheService.Set(cachekey, result, CachingExpirationType.UsualSingleObject);
            }

            return result.Value;
        }
        /// <summary>
        /// 缓存key
        /// </summary>
        public string RecommendCachekey (string tenantTypeId, long itemId)
        {
           
            return $"RecommendCachekey:tenantTypeId-{tenantTypeId}itemId-{itemId}";
        }
    }
}
