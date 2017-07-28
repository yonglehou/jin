//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Tunynet.Caching;
using Tunynet.Repositories;
using PetaPoco;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    ///标签与内容项关联的仓储实现
    /// </summary>
    public class ItemInTagRepository : Repository<ItemInTag>, IItemInTagRepository
    {
        /// <summary>
        /// 为多个内容项添加相同标签
        /// </summary>
        /// <param name="itemIds">内容项Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="tagName">标签名</param>
        public int AddItemsToTag(IEnumerable<long> itemIds, string tenantTypeId,string tagName)
        {
            var dao = CreateDAO();

            IList<Sql> sqls = new List<Sql>();
            var sql = Sql.Builder;

            dao.OpenSharedConnection();
            //创建标签
            sql.From("tn_Tags")
               .Where("TenantTypeId = @0", tenantTypeId)
               .Where("TagName = @0", tagName);
            var tag = dao.FirstOrDefault<Tag>(sql);
            if (tag == null)
                sqls.Add(Sql.Builder.Append("insert into tn_Tags (TenantTypeId,TagName,DateCreated) values(@0,@1,@2)", tenantTypeId, tagName, DateTime.Now));
          

            int affectCount = 0, itemCount = 0;
            foreach (var itemId in itemIds)
            {
                if (itemId <= 0)
                    continue;

                //创建标签与内容项的关联
                sqls.Add(Sql.Builder.Append("insert into tn_ItemsInTags (TagName,ItemId,TenantTypeId)  values(@0,@1,@2)", tagName, itemId, tenantTypeId));
                         

                itemCount++;
            }
            //增加标签相关统计
            sqls.Add(Sql.Builder.Append("update tn_Tags Set ItemCount = ItemCount + @2 where TenantTypeId = @0 and TagName = @1", tenantTypeId, tagName, itemCount));
         

            //通过事务来控制多条语句执行时的一致性
            using (var transaction = dao.GetTransaction())
            {
                dao.Execute(sqls);
                transaction.Complete();
            }

            if (affectCount > 0)
            {
                foreach (var itemId in itemIds)
                {
                    if (itemId <= 0)
                        continue;

                    RealTimeCacheHelper.IncreaseAreaVersion("ItemId", itemId);
                }

            
            }

            dao.CloseSharedConnection();

            return affectCount;
        }

        /// <summary>
        /// 为内容项批量设置标签
        /// </summary>
        /// <param name="tagNames">标签名称集合</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="itemId">内容项Id</param>
        public int AddTagsToItem(string[] tagNames, string tenantTypeId,long itemId)
        {
            int affectCount = 0;

            var dao = CreateDAO();
            dao.OpenSharedConnection();

            foreach (string tagName in tagNames)
            {
                if (string.IsNullOrEmpty(tagName))
                    continue;

                IList<Sql> sqls = new List<Sql>();

                //创建标签
                var sql = Sql.Builder;
                sql.From("tn_Tags")
                   .Where("TenantTypeId = @0", tenantTypeId)
                   .Where("TagName = @0", tagName);
                var tag = dao.FirstOrDefault<Tag>(sql);
                if (tag == null)
                    sqls.Add(Sql.Builder.Append("insert into tn_Tags (TenantTypeId,TagName,Description,ImageAttachmentId,IsFeatured,ItemCount,DateCreated) values (@0,@1,@2,@3,@4,@5,@6)", tenantTypeId, tagName, string.Empty,0,0,0, DateTime.Now));
            
                sql = Sql.Builder;
                sql.From("tn_ItemsInTags")
                   .Where("TagName = @0", tagName)
                  .Where("TenantTypeId = @0", tenantTypeId)
                   .Where("ItemId = @0", itemId);

                var itemInTag = dao.FirstOrDefault<ItemInTag>(sql);
                if (itemInTag == null)
                {
                    //创建标签与内容项的关联
                    sqls.Add(Sql.Builder.Append("insert into tn_ItemsInTags (TagName,ItemId,tenantTypeId) values(@0,@1,@2)", tagName, itemId, tenantTypeId));
                                      
                    sqls.Add(Sql.Builder.Append("update tn_Tags Set ItemCount = ItemCount + 1 where TenantTypeId = @0 and TagName = @1", tenantTypeId, tagName));
                }

                //通过事务来控制多条语句执行时的一致性
                using (var transaction = dao.GetTransaction())
                {
                    affectCount = dao.Execute(sqls);
                    transaction.Complete();
                }

              
            }

            if (tagNames.Length > 0)
            {
             
                RealTimeCacheHelper.IncreaseAreaVersion("ItemId", itemId);
                foreach (var tagName in tagNames)
                {
                    if (string.IsNullOrEmpty(tagName))
                        continue;
                    RealTimeCacheHelper.IncreaseAreaVersion("TagName", tagName);
                }
            }

            dao.CloseSharedConnection();

            return affectCount;
        }

        /// <summary>
        /// 删除标签与成员的关系实体
        /// </summary>
        /// <param name="entity">待处理的实体</param>
        /// <returns></returns>
        public override int Delete(ItemInTag entity)
        {
            if (entity==null)
                return 0;

            var dao = CreateDAO();
            dao.OpenSharedConnection();

            int affectCount = base.Delete(entity);

            List<Sql> sqls = new List<Sql>();

           
                      
            sqls.Add(Sql.Builder.Append("update tn_Tags set ItemCount = ItemCount - 1")
                        .Where("ItemCount > 0 and TagName = @0 and TenantTypeId = @1", entity.TagName, entity.TenantTypeId));

            affectCount = dao.Execute(sqls);

            RealTimeCacheHelper.IncreaseAreaVersion("TagName", entity.TagName);
            RealTimeCacheHelper.IncreaseAreaVersion("ItemId", entity.ItemId);



            dao.CloseSharedConnection();

            return affectCount;
        }

    
        /// <summary>
        /// 清除内容项的所有标签
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        public int ClearTagsFromItem(long itemId, string tenantTypeId)
        {
            var dao = CreateDAO();
            dao.OpenSharedConnection();

            var sql = Sql.Builder;
            sql.Select("IT.TagName")
               .From("tn_ItemsInTags IT")
               .Where("IT.TenantTypeId = @0", tenantTypeId)
               .Where("IT.ItemId = @0", itemId);

            List<string> tagNames = dao.Fetch<string>(sql);

            sql = Sql.Builder;
            sql.Append("delete from tn_ItemsInTags where ItemId = @0 and TenantTypeId = @1", itemId, tenantTypeId);

            int affectCount = dao.Execute(sql);

            if (affectCount > 0)
            {
                List<Sql> sqls = new List<Sql>();
                foreach (string tagName in tagNames)
                {
                    sqls.Add(Sql.Builder.Append("update tn_Tags set ItemCount = ItemCount - 1")
                                        .Where("ItemCount > 0 and TagName = @0 and TenantTypeId = @1", tagName, tenantTypeId));

                    RealTimeCacheHelper.IncreaseAreaVersion("TagName", tagName);
                }

                dao.Execute(sqls);

                RealTimeCacheHelper.IncreaseAreaVersion("ItemId", itemId);
            }

            dao.CloseSharedConnection();

            return affectCount;
        }

        /// <summary>
        /// 获取标签的所有内容项集合
        /// </summary>
        /// <param name="tagName">标签名称</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>返回指定的内容项Id集合</returns>
        public IEnumerable<long> GetTagOfItemIds(string tagName, string tenantTypeId)
        {
            var dao = CreateDAO();
           
            //组装sql语句
            var sql = Sql.Builder;
           
                sql.Select("ItemId")
                   .From("tn_ItemsInTags");

                if (!string.IsNullOrEmpty(tenantTypeId))
                    sql.Where("TenantTypeId = @0", tenantTypeId);
                if (!string.IsNullOrEmpty(tagName))
                    sql.Where("TagName = @0", tagName);
          
            List<long> itemIds = null;
           
            if (itemIds == null)
            {
                itemIds = dao.FetchFirstColumn(sql).Cast<long>().ToList();
               
            }

            return itemIds;
        }

        /// <summary>
        /// 获取标签的内容项集合
        /// </summary>
        /// <param name="tagName">标签名称</param>
        /// <param name="tenantTypeId">租户id</param>
        /// <param name="pageSize">页数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagingDataSet<ItemInTag> GetItemIds(string tagName, string tenantTypeId, int pageSize, int pageIndex)
        {
            var dao = CreateDAO();

            //组装sql语句
            var sql = Sql.Builder;

            sql.Select("*")
               .From("tn_ItemsInTags");

            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);
            if (!string.IsNullOrEmpty(tagName))
                sql.Where("TagName = @0", tagName);
            return GetPagingEntities(pageSize, pageIndex, sql);

        }

        /// <summary>
        ///  获取多个标签的内容项集合
        /// </summary>
        /// <param name="tagNames">多个名称</param>
        /// <param name="tenantTypeId">租户id</param>
        /// <param name="pageSize">页数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public PagingDataSet<ItemInTag> GetItemIds(IEnumerable<string> tagNames, string tenantTypeId, int pageSize, int pageIndex)
        {
            var sql = Sql.Builder;
            //组装sql语句
            sql.Select("ItemId")
               .From("tn_ItemsInTags");

            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);
            if (tagNames != null && tagNames.Count() > 0)
                sql.Where("TagName IN (@tagNames)", new { tagNames = tagNames });

            return GetPagingEntities(pageSize, pageIndex, sql);

        }

        /// <summary>
        /// 获取内容项的所有标签
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <returns>返回内容项的标签Id集合,无返回时返回空集合</returns>
        public IEnumerable<long> GetTagIdsOfItem(long itemId, string tenantTypeId)
        {

            List<long> tagIds = new List<long>();

           
                var sql = Sql.Builder;
                sql.Select("T.TagId")
                   .From("tn_Tags T")
                   .InnerJoin("tn_ItemsInTags IT")
                   .On("IT.TagName = T.TagName")
                   .Where("IT.ItemId = @0 and IT.TenantTypeId = @1 and T.TenantTypeId =@1", itemId, tenantTypeId); ;

                tagIds = CreateDAO().FetchFirstColumn(sql).Cast<long>().ToList();
            return tagIds;
        }

        /// <summary>
        /// 获取内容项与标签关联Id集合
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <param name="tenantTypeId">租户类型</param>
        /// <returns>返回内容项的标签Id集合,无返回时返回空集合</returns>
        public IEnumerable<long> GetItemInTagIdsOfItem(long itemId, string tenantTypeId)
        {

            List<long> itemInTagIds = new List<long>();
            var sql = PetaPoco.Sql.Builder;
                sql.Select("IT.Id")
                   .From("tn_ItemsInTags IT")
                   .InnerJoin("tn_Tags T")
                   .On("IT.TagName = T.TagName")
                    .Where("IT.ItemId = @0", itemId); 
            if (!string.IsNullOrEmpty(tenantTypeId))
                    sql.Where("IT.TenantTypeId = @0 and T.TenantTypeId =@0", tenantTypeId);
            itemInTagIds = CreateDAO().FetchFirstColumn(sql).Cast<long>().ToList();
            return itemInTagIds;
        }

   
        /// <summary>
        /// 根据用户ID列表获取用户tag，本方法现用于用户搜索功能的索引生成
        /// </summary>
        /// <param name="userIds">用户ID列表</param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetTagNamesOfUsers(IEnumerable<long> userIds)
        {
            var sql = Sql.Builder;
            sql.Select("ItemId as UserId,TagName")
                   .From("tn_ItemsInTags")
                   .Where("ItemId in (@userIds)", new { userIds = userIds });
            return CreateDAO().Fetch<dynamic>(sql);
        }

        /// <summary>
        /// 根据用户ID列表获取ItemInTag的ID列表，本方法现用于用户搜索功能的索引生成
        /// </summary>
        /// <param name="userIds">用户ID列表</param>
        /// <returns>ItemInTag的ID列表</returns>
        public IEnumerable<long> GetEntityIdsByUserIds(IEnumerable<long> userIds)
        {
            if (userIds == null || userIds.Count() == 0)
                return new List<long>();
            var sql = Sql.Builder;
            sql.Select("Id")
                   .From("tn_ItemsInTags")
                   .Where("ItemId in (@userIds)", new { userIds = userIds });
            return CreateDAO().Fetch<long>(sql);
        }

        /// <summary>
        /// 根据Id获取
        /// </summary>
        /// <param name="itemId">成员Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="tagInOwnerId">标签与拥有者关联Id</param>
        /// <returns></returns>
        public Dictionary<string, long> GetTagNamesWithIdsOfItem(long itemId, string tenantTypeId)
        {

            Dictionary<string, long> tagNames_Ids = new Dictionary<string, long>();

         
                Sql sql = Sql.Builder;
                sql.Select("tn_Tags.*")
                   .From("tn_Tags")
                   .InnerJoin("tn_ItemsInTags IIT")
                   .On("tn_Tags.TagName = IIT.TagName")
                   .Where("IIT.ItemId = @0", itemId)
                   .Where("tn_Tags.TenantTypeId = @0", tenantTypeId);


                IEnumerable<Tag> results = CreateDAO().Fetch<Tag>(sql);

                if (results != null)
                {
                    tagNames_Ids = new Dictionary<string, long>();
                    foreach (var tag in results)
                    {
                        tagNames_Ids[tag.TagName] = tag.TagId;
                    }
                }

            return tagNames_Ids;
        }

    }
}