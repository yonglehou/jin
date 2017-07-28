
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
using Tunynet.Caching;
using PetaPoco;
using Tunynet.Utilities;

namespace Tunynet.Common
{
    /// <summary>
    /// Application仓储
    /// </summary>
    public class AttachmentRepository<T> : Repository<T>, IAttachmentRepository<T> where T : Attachment
    {
        private int PageSize = 30;

        /// <summary>
        /// 删除AssociateId相关的附件
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id（例如：博文Id、贴子Id）</param>
        public void DeletesByAssociateId(string tenantTypeId, long associateId)
        {
            var sql = Sql.Builder;
            sql.Append("DELETE  FROM tn_Attachments").Where("TenantTypeId=@0 and AssociateId=@1", tenantTypeId, associateId);

            int affectCount = CreateDAO().Execute(sql);
            if (affectCount > 0)
            {
                //更新缓存
                RealTimeCacheHelper.IncreaseAreaVersion("AssociateId", associateId);
                IEnumerable<object> attachmentIds = GetIdsByAssociateId(tenantTypeId, associateId);
                foreach (var id in attachmentIds.Cast<long>())
                {
                    RealTimeCacheHelper.IncreaseEntityCacheVersion(id);
                }
            }
        }

        /// <summary>
        /// 删除UserId相关的附件
        /// </summary>
        /// <param name="userId">上传者Id</param>
        public void DeletesByUserId(string tenantTypeId, long userId)
        {
            var sql = Sql.Builder;
            sql.Append("DELETE  FROM tn_Attachments").Where("TenantTypeId=@0 and UserId=@1", tenantTypeId, userId);

            int affectCount = CreateDAO().Execute(sql);
            if (affectCount > 0)
            {
                //更新缓存
                IEnumerable<object> attachmentIds = GetIdsByUserId(tenantTypeId, userId);
                foreach (var id in attachmentIds.Cast<long>())
                {
                    RealTimeCacheHelper.IncreaseEntityCacheVersion(id);
                }
            }
        }

        /// <summary>
        /// 删除OwnerId相关的附件
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        public void DeletesByOwnerId(string tenantTypeId, long ownerId)
        {
            var sql = Sql.Builder;
            sql.Append("DELETE  FROM tn_Attachments ").Where("TenantTypeId=@0 and OwnerId=@1", tenantTypeId, ownerId);

            int affectCount = CreateDAO().Execute(sql);

            if (affectCount > 0)
            {
                //获取被影响的Id集合
                IEnumerable<object> attachmentIds = GetIdsByOwnerId(ownerId, tenantTypeId);
                foreach (var id in attachmentIds.Cast<long>())
                {
                    RealTimeCacheHelper.IncreaseEntityCacheVersion(id);
                }
            }
        }

        /// <summary>
        /// 依据UserId获取附件列表（用于UserId与附件一对多关系）
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="userId">附件关联Id</param>
        /// <returns>附件列表</returns>
        public IEnumerable<T> GetsByUserId(string tenantTypeId, long userId)
        {
            IEnumerable<object> attachmentIds = GetIdsByUserId(tenantTypeId, userId);
            return PopulateEntitiesByEntityIds(attachmentIds);
        }

        /// <summary>
        /// 依据UserId获取附件Id列表（用于UserId与附件一对多关系）
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="userId">附件关联Id</param>
        /// <returns>附件Id列表</returns>
        private IEnumerable<object> GetIdsByUserId(string tenantTypeId, long userId)
        {
            var sql = Sql.Builder;
            sql.Select("AttachmentId")
               .From("tn_Attachments")
               .Where("TenantTypeId=@0", tenantTypeId)
               .Where("UserId=@0", userId);

            List<object> attachmentIds = CreateDAO().FetchFirstColumn(sql).ToList();

            return attachmentIds;
        }

        /// <summary>
        /// 依据AssociateId获取附件列表（用于AssociateId与附件一对多关系）
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id</param>
        /// <returns>附件列表</returns>
        public IEnumerable<T> GetsByAssociateId(string tenantTypeId, long associateId)
        {
            IEnumerable<object> attachmentIds = GetIdsByAssociateId(tenantTypeId, associateId);
            return PopulateEntitiesByEntityIds(attachmentIds);
        }

        /// <summary>
        /// 依据AssociateId获取附件Id列表（用于AssociateId与附件一对多关系）
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="associateId">附件关联Id</param>
        /// <returns>附件Id列表</returns>
        private IEnumerable<object> GetIdsByAssociateId(string tenantTypeId, long associateId)
        {
          
                var sql = Sql.Builder;
                sql.Select("AttachmentId")
                   .From("tn_Attachments")
                   .Where("TenantTypeId=@0", tenantTypeId)
                   .Where("AssociateId=@0", associateId)
                   .OrderBy("AttachmentId");
             var   attachmentIds = CreateDAO().FetchFirstColumn(sql).ToList();

           
            return attachmentIds;
        }

        /// <summary>
        /// 获取拥有者的所有附件或者拥有者一种租户类型的附件
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>附件列表</returns>
        public IEnumerable<T> Gets(long ownerId, string tenantTypeId)
        {
            IEnumerable<object> attachmentIds = GetIdsByOwnerId(ownerId, tenantTypeId);
            return PopulateEntitiesByEntityIds(attachmentIds);
        }

        /// <summary>
        /// 获取拥有者的所有附件或者拥有者一种租户类型的附件
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>附件Id列表</returns>
        private IEnumerable<object> GetIdsByOwnerId(long ownerId, string tenantTypeId)
        {
            var sql = Sql.Builder;
            sql.Select("AttachmentId")
               .From("tn_Attachments")
               .Where("OwnerId=@0", ownerId);
            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);

            List<object> attachmentIds = CreateDAO().FetchFirstColumn(sql).ToList();

            return attachmentIds;
        }

        /// <summary>
        /// 搜索附件并分页显示
        /// </summary>
        /// <param name="tenantTypeId">附件租户类型</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        public PagingDataSet<T> Gets(string tenantTypeId, string keyword, int pageIndex)
        {
            var sql = Sql.Builder;

            if (!String.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);
            if (!String.IsNullOrEmpty(keyword))
                sql.Where("FriendlyFileName like @0", "%" + StringUtility.StripSQLInjection(keyword) + "%");

            sql.OrderBy("AttachmentId  ASC");

            return GetPagingEntities(PageSize, pageIndex, sql);

        }

        /// <summary>
        /// 获取拥有者一种租户类型的临时附件
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        public IEnumerable<T> GetTemporaryAttachments(long ownerId, string tenantTypeId)
        {
            List<object> attachmentIds;
            var sql = Sql.Builder;
            sql.Select("AttachmentId")
               .From("tn_Attachments")
               .Where("OwnerId=@0", ownerId)
               .Where("AssociateId=0");

            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);
            sql.OrderBy("AttachmentId");

            attachmentIds = CreateDAO().FetchFirstColumn(sql).ToList();

            return PopulateEntitiesByEntityIds(attachmentIds);
        }

        /// <summary>
        /// 获取需删除的垃圾临时附件
        /// </summary>
        /// <param name="beforeDays">多少天之前的附件</param>
        public IEnumerable<T> GetTrashTemporaryAttachments(int beforeDays)
        {
            var sql = Sql.Builder;

            sql.Select("AttachmentId")
               .From("tn_Attachments")
               .Where("AssociateId=0")
               .Where("DateCreated <= @0 ", DateTime.Now.AddDays(-beforeDays));
            IEnumerable<object> attachmentIds = CreateDAO().FetchFirstColumn(sql);

            return PopulateEntitiesByEntityIds(attachmentIds);
        }

        /// <summary>
        /// 删除垃圾临时附件
        /// </summary>
        /// <param name="beforeDays">多少天之前的附件</param>
        public void DeleteTrashTemporaryAttachments(int beforeDays)
        {
            var sql = Sql.Builder;
            sql.Append("DELETE  FROM tn_Attachments ")
            .Where("AssociateId=0")
            .Where("DateCreated <= @0 ", DateTime.Now.AddDays(-beforeDays));
            CreateDAO().Execute(sql);
        }

     
        /// <summary>
        /// 把临时附件转成正常附件
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="associateId">附件关联Id</param>
        /// <param name="attachmentIds">待转换的附件Id</param>
        public void ToggleTemporaryAttachments(long ownerId, string tenantTypeId, long associateId, IEnumerable<long> attachmentIds)
        {
            var sql = Sql.Builder;
            sql.Append("UPDATE tn_Attachments Set AssociateId=@0", associateId)
            .Where("TenantTypeId = @0", tenantTypeId)
            .Where("OwnerId = @0", ownerId);


            if (attachmentIds != null && attachmentIds.Count() > 0)
            {
                sql.Where("AttachmentId in (@attachmentIds)", new { attachmentIds = attachmentIds });
            }
            else
            {
                sql.Where("AssociateId=0");
            }

            CreateDAO().Execute(sql);
            RealTimeCacheHelper.IncreaseAreaVersion("AssociateId", associateId);
        }


        /// <summary>
        /// 删除垃圾数据
        /// </summary>
        /// <param name="serviceKey">服务标识</param>
        public void DeleteTrashDatas()
        {
            IEnumerable<TenantType> tenantTypes = new TenantTypeRepository().Gets(MultiTenantServiceKeys.Instance().Attachment());
              //2017-5-17 在MySql中 不能先select出同一表中的某些值，再update 或delete 这个表(在同一语句中) 已改正
            List<Sql> sqls = new List<Sql>();
            sqls.Add(Sql.Builder.Append("delete from tn_AttachmentAccessRecords where not exists (select 1 from (select 1 as c from  tn_Users,tn_AttachmentAccessRecords where tn_Users.UserId = tn_AttachmentAccessRecords.UserId) as a)"));


            sqls.Add(Sql.Builder.Append("delete from tn_AttachmentAccessRecords where not exists (select 1 from (select 1 as c from  tn_attachments,tn_AttachmentAccessRecords where tn_attachments.AttachmentId = tn_AttachmentAccessRecords.AttachmentId) as a)"));

            sqls.Add(Sql.Builder.Append("update tn_Attachments set tn_Attachments.AssociateId = 0,tn_Attachments.UserId = 0,tn_Attachments.OwnerId = 0")
                                .Where("not exists (select 1 from (select 1 as c from tn_Users,tn_Attachments where tn_Attachments.UserId = tn_Users.UserId) as a)"));

         

            foreach (var tenantType in tenantTypes)
            {
                Type type = Type.GetType(tenantType.ClassType);
                if (type == null)
                    continue;

                var pd = TableInfo.FromPoco(type);
                sqls.Add(Sql.Builder.Append("update tn_Attachments set tn_Attachments.AssociateId = 0,tn_Attachments.UserId = 0,tn_Attachments.OwnerId = 0")
                                    .Where("not exists (select 1 from (select 1 as c from tn_Attachments," + pd.TableName + " where tn_Attachments.AssociateId = " + pd.PrimaryKey + ") as a) and tn_Attachments.TenantTypeId = @0"
                                    , tenantType.TenantTypeId));

            }

            CreateDAO().Execute(sqls);
        }
    }
}
