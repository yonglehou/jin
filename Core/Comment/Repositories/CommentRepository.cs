//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Caching;
using Tunynet.Repositories;
using System.Configuration;
using PetaPoco;
using Tunynet.Settings;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// CommentRepository
    /// </summary>
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private ISettingsManager<SiteSettings> siteSettings = DIContainer.Resolve<ISettingsManager<SiteSettings>>();

        private int PageSize = 10;

        #region   Delete & Update


        /// <summary>
        /// 删除评论 
        /// </summary>
        /// <param name="id">评论Id</param>
        public int Delete(long id)
        {
            Comment comment = Get(id);

            //todo: by mazq, 20170325, @zhangzh 已改正
            var sql = Sql.Builder;
            sql.Append("Delete  FROM tn_Comments where  id = @0 or ParentIds like @1", id, "%" + id + ",%");
            var affectCount = CreateDAO().Execute(sql);
            #region 处理缓存
            if (affectCount > 0)
            {
                base.OnDeleted(comment);
            }
            #endregion

            return affectCount;
        }

        /// <summary>
        /// 删除被评论对象的所有评论
        /// </summary>
        /// <remarks>
        /// 供被评论对象删除时调用
        /// </remarks>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <returns></returns>
        public int DeleteCommentedObjectComments(long commentedObjectId, string tenantTypeId)
        {
            var sql = Sql.Builder;
            sql.Append("DELETE FROM tn_Comments ");
            sql.Where("CommentedObjectId = @0 and tenantTypeId =@1", commentedObjectId, tenantTypeId);
            int affectCount = CreateDAO().Execute(sql);

            #region 处理缓存

            if (affectCount > 0)
                RealTimeCacheHelper.IncreaseAreaVersion("CommentedObjectId", commentedObjectId);

            #endregion

            return affectCount;
        }

        /// <summary>
        ///  删除用户发布的评论
        /// </summary>
        /// <remarks>
        /// 供用户删除时处理用户相关信息时调用
        /// </remarks>
        /// <param name="userId">UserId</param>
        /// <param name="reserveCommnetsAsAnonymous">true=保留用户发布的评论，但是修改为匿名用户；false=直接删除评论</param>
        /// <returns></returns>
        public int DeleteUserComments(long userId, bool reserveCommnetsAsAnonymous)
        {
            var sql = Sql.Builder;

            if (reserveCommnetsAsAnonymous)
                sql.Append("UPDATE tn_Comments  SET UserId=0");
            else
                sql.Append("DELETE FROM tn_Comments ");

            sql.Where("UserId=@0", userId);
            int rows = CreateDAO().Execute(sql);

            #region 处理缓存
            if (rows > 0)
                RealTimeCacheHelper.IncreaseAreaVersion("UserId", userId);
            #endregion

            return rows;
        }

        /// <summary>
        /// 删除垃圾数据
        /// </summary>
        public void DeleteTrashDatas()
        {
            IEnumerable<TenantType> tenantTypes = new TenantTypeRepository().Gets(MultiTenantServiceKeys.Instance().Comment());

            List<Sql> sqls = new List<Sql>();
            sqls.Add(Sql.Builder.Append("delete from tn_Comments where not exists (select 1 from (select 1 as c from tn_Users,tn_Comments where tn_Comments.UserId = tn_Users.UserId) as a)"));

            foreach (var tenantType in tenantTypes)
            {
                Type type = Type.GetType(tenantType.ClassType);
                if (type == null)
                    continue;
                var pd = TableInfo.FromPoco(type);
                sqls.Add(Sql.Builder.Append("delete from tn_Comments")
                                    .Where("not exists (select 1 from (select 1 as c from tn_Comments," + pd.TableName + " where tn_Comments.CommentedObjectId = " + pd.PrimaryKey + ") as a) and tn_Comments.TenantTypeId = @0"
                                    , tenantType.TenantTypeId));
            }

            CreateDAO().Execute(sqls);
        }

        #endregion

        #region  Get

        /// <summary>
        /// 获取被评论对象的所有评论（用于删除被评论对象时的积分处理）
        /// </summary>
        /// <param name="commentedObjectId">被评论对象ID</param>
        /// <returns></returns>
        public IEnumerable<Comment> GetCommentedObjectComments(long commentedObjectId, string tenantTypeId)
        {
            var sql = Sql.Builder
                .Select("*")
                .From("tn_Comments")
                .Where("CommentedObjectId = @0", commentedObjectId)
                .Where("TenantTypeId = @0", tenantTypeId);

            IEnumerable<Comment> CommentedObjectComments = CreateDAO().Fetch<Comment>(sql);
            return CommentedObjectComments;
        }


        /// <summary>
        /// 获取顶级评论列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="sortBy">排序字段</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetRootComments(string tenantTypeId, long commentedObjectId, int pageIndex, SortBy_Comment sortBy)
        {
            var sql = Sql.Builder;

            sql.Where("ParentId = 0");

            if (!String.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);

            if (commentedObjectId > 0)
                sql.Where("CommentedObjectId = @0", commentedObjectId);

            //审核
            sql = AuditSqls(sql);
            switch (sortBy)
            {
                case SortBy_Comment.DateCreated:
                    sql.OrderBy("Id"); break;
                case SortBy_Comment.DateCreatedDesc:
                    sql.OrderBy("Id DESC"); break;
                default:
                    sql.OrderBy("Id"); break;
            }
            return GetPagingEntities(PageSize, pageIndex, sql);
        }

        /// <summary>
        /// 获取对象的所有评论列表
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="commentedObjectId">被评论对象Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="sortBy">排序字段</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetObjectComments(string tenantTypeId, long commentedObjectId, int pageIndex, SortBy_Comment sortBy, bool? IsPrivate, long? userId, long? parentId)
        {
            var sql = Sql.Builder;

            if (!String.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId = @0", tenantTypeId);

            if (commentedObjectId > 0)
                sql.Where("CommentedObjectId = @0", commentedObjectId);
            if (IsPrivate.HasValue)
                sql.Where("IsPrivate = @0 ", IsPrivate.Value);
            if (userId.HasValue)
                sql.Where(" UserId=@0 ", userId.Value);
            if (parentId.HasValue)
                sql.Where(" ParentId=@0 ", parentId.Value);
            //审核
            sql = AuditSqls(sql);
            switch (sortBy)
            {
                case SortBy_Comment.DateCreated:
                    sql.OrderBy("Id ASC"); break;
                case SortBy_Comment.DateCreatedDesc:
                    sql.OrderBy("Id DESC"); break;
                default:
                    sql.OrderBy("Id ASC"); break;
            }
            return GetPagingEntities(PageSize, pageIndex, sql);

        }

        /// <summary>
        /// 根据贴吧获取评论
        /// </summary>
        /// <param name="sectionId">贴吧Id</param>
        /// <param name="auditStatus">审核状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetSectionComments(long sectionId, AuditStatus? auditStatus, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            var sql = Sql.Builder;
            if (sectionId > 0)
            {
                sql.Select("tn_Comments.*")
                    .From("tn_Comments")
                    .InnerJoin("tn_Threads").On("tn_Comments.CommentedObjectId=tn_Threads.ThreadId")
                    .Where("tn_Threads.SectionId=@0", sectionId)
                    .Where("tn_Comments.TenantTypeId=@0", TenantTypeIds.Instance().Thread());
            }
            
            if (startDate.HasValue)
                sql.Where("tn_Comments.DateCreated >= @0", startDate);
                
            if (endDate.HasValue)
                sql.Where("tn_Comments.DateCreated < @0", endDate.Value.AddDays(1));

            //审核
            if (auditStatus.HasValue)
            {
                sql.Where("tn_Comments.ApprovalStatus=@0", auditStatus.Value);
            }

            sql.OrderBy("tn_Comments.Id DESC");

            PagingDataSet<Comment> pds = GetPagingEntities(pageSize, pageIndex, sql);
            return pds;
        }

        /// <summary>
        /// 获取子级评论列表
        /// </summary>
        /// <param name="parentId">父评论Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">当前个数</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="IncludeCommentDescendants">是否包含所有后代评论</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetChildren(long parentId, int pageIndex, int pageSize, SortBy_Comment sortBy, bool IncludeCommentDescendants)
        {
            if (parentId == 0)
                return null;
            var sql = Sql.Builder;
            if (IncludeCommentDescendants)
                sql.Where("ParentIds like @0", "%," + parentId + "%");
            else
                sql.Where("ParentId = @0", parentId);

            //审核
            sql = AuditSqls(sql);
            switch (sortBy)
            {
                case SortBy_Comment.DateCreated:
                    sql.OrderBy("Id ASC"); break;
                case SortBy_Comment.DateCreatedDesc:
                    sql.OrderBy("Id DESC"); break;
                default:
                    sql.OrderBy("Id ASC"); break;
            }
            return GetPagingEntities(pageSize, pageIndex, sql);

        }

        /// <summary>
        /// 获取拥有者的评论
        /// </summary>
        /// <param name="ownerId">评论拥有者Id</param>
        /// <param name="userId">评论发布人UserId</param>
        /// <param name="tenantTypeId">租户类型Id（如果为null，则获取该拥有者所有评论）</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetUserComments(long? ownerId, long? userId, string tenantTypeId, DateTime? startDate, DateTime? endDate, int pageIndex)
        {
            PagingDataSet<Comment> pds = null;
            var sql = Sql.Builder;
            if (ownerId.HasValue && ownerId > 0)
            {
                sql.Where("OwnerId = @0", ownerId);
                sql.Where(" OwnerId != UserId  ");
            }
            if (userId.HasValue && userId > 0)
                sql.Where("UserId = @0", userId);

            if (startDate.HasValue)
                sql.Where(" DateCreated >= @0", startDate);

            if (endDate.HasValue)
                sql.Where(" DateCreated < @0", endDate.Value.AddDays(1));


            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId=@0", tenantTypeId);
            //审核
            sql = AuditSqls(sql);
            sql.OrderBy("Id  DESC");

            if (startDate.HasValue || endDate.HasValue)
            {
                pds = GetPagingEntities(PageSize, pageIndex, sql);
            }
            else
            {
                pds = GetPagingEntities(PageSize, pageIndex, sql);

            }

            return pds;
        }

        /// <summary>
        /// 获取前topNumber条评论
        /// </summary>
        /// <param name="ownerId">评论拥有者Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="topNumber">获取的评论数量</param>
        /// <param name="sortBy">排序字段</param>
        /// <returns></returns>
        public IEnumerable<Comment> GetTopComments(long ownerId, string tenantTypeId, int topNumber, SortBy_Comment sortBy)
        {
            var sql = Sql.Builder.Where("TenantTypeId = @0 ", tenantTypeId);

            if (ownerId > 0)
                sql.Where("OwnerId = @0", ownerId);
            //审核
            sql = AuditSqls(sql);
            switch (sortBy)
            {
                case SortBy_Comment.DateCreated:
                    sql.OrderBy("Id ASC"); break;
                case SortBy_Comment.DateCreatedDesc:
                    sql.OrderBy("Id DESC"); break;
                default:
                    sql.OrderBy("Id ASC"); break;
            }
            return GetTopEntities(topNumber, sql);

        }

        /// <summary>
        /// 查询用户评论
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        ///   <param name="publiclyAuditStatus">审核状态</param>
        /// <param name="userId">评论发布人UserId</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetComments(string tenantTypeId, PubliclyAuditStatus? publiclyAuditStatus, long? userId, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            var sql = Sql.Builder;
            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId=@0", tenantTypeId);

            if (userId.HasValue && userId > 0)
                sql.Where(" UserId = @0", userId);

            if (startDate.HasValue)
                sql.Where(" DateCreated >= @0", startDate);

            //todo: by mazq, 20170325, @zhangzh 逻辑不对，该文件还有其他类似问题 @mazq 该文件已改正
            if (endDate.HasValue)
                sql.Where(" DateCreated < @0", endDate.Value.AddDays(1));
            //审核
            if (publiclyAuditStatus.HasValue)
            {
                sql.Where("ApprovalStatus=@0", publiclyAuditStatus.Value);
            }

            sql.OrderBy("Id  DESC");

            PagingDataSet<Comment> pds = GetPagingEntities(pageSize, pageIndex, sql);
            return pds;
        }

        /// <summary>
        /// 查询用户评论
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="userId">评论发布人UserId</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetUserComments(string tenantTypeId, long? userId, long? parentId, DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            var sql = Sql.Builder;
            if (!string.IsNullOrEmpty(tenantTypeId))
                sql.Where("TenantTypeId=@0", tenantTypeId);

            if (userId.HasValue && userId > 0)
                sql.Where(" UserId = @0", userId);
            if (parentId.HasValue)
                sql.Where(" parentId = @0", parentId);
            if (startDate.HasValue)
                sql.Where(" DateCreated >= @0", startDate);

            if (endDate.HasValue)
                sql.Where(" DateCreated < @0", endDate.Value.AddDays(1));
            //审核
            sql = AuditSqls(sql);
            sql.OrderBy("Id  DESC");


            PagingDataSet<Comment> pds = GetPagingEntities(pageSize, pageIndex, sql);
            return pds;
        }

        /// <summary>
        /// 获取解析后的内容
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns></returns>
        public string GetResolvedBody(long id)
        {
            Comment comment = Get(id);
            if (comment == null)
                return string.Empty;

            string cacheKey = string.Format("CommentResolvedBody{0}::{1}", RealTimeCacheHelper.GetEntityVersion(id), id);
            string resolveBody = cacheService.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(resolveBody))
            {
                resolveBody = comment.Body;
                ICommentBodyProcessor commentBodyProcessor = DIContainer.Resolve<ICommentBodyProcessor>();
                resolveBody = commentBodyProcessor.Process(comment.Body, TenantTypeIds.Instance().Comment(), comment.Id, comment.UserId);
                cacheService.Set(cacheKey, resolveBody, CachingExpirationType.SingleObject);
            }

            return resolveBody;
        }


        #endregion



        /// <summary>
        /// 评论计数获取(后台用)
        /// </summary>
        /// <param name="approvalStatus">审核状态</param>
        /// <param name="is24Hours">是否24小时之内</param>
        /// <returns></returns>
        public int GetCommentCount(AuditStatus? approvalStatus, bool is24Hours)
        {
            Sql sql = Sql.Builder;
            sql.Select(" count(tn_Comments.Id )").From("tn_Comments");
            if (approvalStatus.HasValue)
                sql.Where("tn_Comments.ApprovalStatus=@0", (int)approvalStatus.Value);
            if (is24Hours)
                sql.Where("tn_Comments.DateCreated>@0", DateTime.Now.AddHours(-24));
            return CreateDAO().SingleOrDefault<int>(sql);
        }




        /// <summary>
        /// 审核语句组装
        /// </summary>
        /// <param name="wheresql">wheresql</param>
        /// <returns></returns>
        private Sql AuditSqls(Sql wheresql)
        {
            var setting = siteSettings.Get();
            if (setting.AuditStatus == PubliclyAuditStatus.Success)
                wheresql.Where("ApprovalStatus=@0", setting.AuditStatus);
            else if (setting.AuditStatus == PubliclyAuditStatus.Again)
                wheresql.Where("ApprovalStatus>@0 ", PubliclyAuditStatus.Pending);
            else if (setting.AuditStatus == PubliclyAuditStatus.Pending)
                wheresql.Where("ApprovalStatus>@0 ", PubliclyAuditStatus.Fail);
            return wheresql;

        }
    }
}
