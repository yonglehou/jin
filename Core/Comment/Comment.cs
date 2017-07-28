//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Common.Repositories;
using Tunynet.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tunynet.Common
{
    [TableName("tn_Comments")]
    [PrimaryKey("Id", autoIncrement = true)]
    [CacheSetting(true, PropertyNamesOfArea = "CommentedObjectId,UserId,OwnerId,ParentId")]
    [Serializable]
    public class Comment : SerializablePropertiesBase, IEntity, IAuditable
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static Comment New()
        {
            Comment comment = new Comment()
            {
                Author = string.Empty,
                Subject = string.Empty,
                IP = WebUtility.GetIP(),
                DateCreated = DateTime.Now
            };
            return comment;
        }

        #region 需持久化属性

        /// <summary>
        ///Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        ///所有父级评论Id集合
        /// </summary>
        public string ParentIds { get; set; }

        /// <summary>
        ///父评论Id（一级ParentId等于0）
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        ///被评论对象Id
        /// </summary>
        public long CommentedObjectId { get; set; }

        /// <summary>
        ///租户类型Id（4位ApplicationId+2位顺序号）
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        ///拥有者Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        ///评论人UserId
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 评论类别
        /// </summary>
        public string CommentType { get; set; }
       /// <summary>
       /// 子回复个数
       /// </summary>
        public int ChildrenCount { get; set; }
        /// <summary>
        ///评论人名称
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///评论内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///审核状态
        /// </summary>
        public AuditStatus ApprovalStatus { get; set; }

        /// <summary>
        ///是否匿名评论
        /// </summary>
        public bool IsAnonymous { get; set; }
        /// <summary>
        /// 是否悄悄话
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        ///评论人IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get;  set; }

        /// <summary>
        /// 获取父级数组扩展
        /// </summary>
        /// <returns></returns>
        public string[] GetParentIds()
        {
            if (!string.IsNullOrEmpty(this.ParentIds))
            {
                return Regex.Split(this.ParentIds, ",", RegexOptions.IgnoreCase);
            }
            else
                return null;
           
        }
        /// <summary>
        /// 联系方式
        /// </summary>
        [Ignore]
        public string Contact
        {
            get { return GetExtendedProperty<string>("Contact", string.Empty); }
            set { SetExtendedProperty("Contact", value); }
        }

        #endregion


        /// <summary>
        /// 审核项标识
        /// </summary>
        [Ignore]
        public string AuditItemKey
        {
            get
            {
                if (this.TenantTypeId == TenantTypeIds.Instance().Thread())
                    return AuditItemKeys.Instance().Post();
                else
                    return AuditItemKeys.Instance().Comment();

            }
        }
        /// <summary>
        /// 获取评论所引用的评论
        /// </summary>
        /// <param name="Isstretch">是否展开隐藏</param>
        /// <returns></returns>
        public List<Comment> GetParentComments(bool Isstretch)
        {
            return DIContainer.Resolve<CommentService>().GetParentComments(this.Id, Isstretch);
        }

        /// <summary>
        /// 获取当前评论的父级评论
        /// </summary>
        /// <returns></returns>
        public Comment GetParentComment()
        {
            if (ParentId!=0)
            {
                return DIContainer.Resolve<CommentService>().Get(ParentId);
            }
            else
            {
                return null;
            }
            
        }
        /// <summary>
        /// 获取评论下的第一级评论
        /// </summary>
        /// <param name="Isstretch">是否展开隐藏</param>
        /// <returns></returns>
        public PagingDataSet<Comment> GetChildComments()
        {
            return new CommentRepository().GetUserComments(TenantTypeIds.Instance().ContentItem(),null, this.Id,null,null,20,1);
        }

       

       

        /// <summary>
        /// 获取被评论对象的Url
        /// </summary>
        /// <returns></returns>
        public string GetCommentedObjectUrl()
        {
            ICommentUrlGetter urlGetter = CommentUrlGetterFactory.Get(this.TenantTypeId);
            if (urlGetter != null)
                return urlGetter.GetCommentedObject(this.CommentedObjectId).DetailUrl;
            else
                return string.Empty;
        }

        /// <summary>
        /// 获取被评论对象
        /// </summary>
        /// <returns></returns>
        public CommentedObject GetCommentedObject()
        {
            ICommentUrlGetter urlGetter = CommentUrlGetterFactory.Get(this.TenantTypeId);
            if (urlGetter != null)
                return urlGetter.GetCommentedObject(this.CommentedObjectId);
            else
                return null;
        }

        /// <summary>
        /// 获取活动发贴的解析过的内容（活动发贴详情页）
        /// </summary>
        public string GetResolvedBody()
        {
            return new CommentRepository().GetResolvedBody(this.Id);
        }

        /// <summary>
        /// 浏览数
        /// </summary>
        [Ignore]
        public int HitTimes
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Comment());
                return countService.Get(CountTypes.Instance().HitTimes(), this.Id);
            }
        }

        /// <summary>
        /// 评论数
        /// </summary>
        [Ignore]
        public int CommentCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Comment());
                return countService.Get(CountTypes.Instance().CommentCount(), this.Id);
            }
        }

        #region IEntity 成员

        object IEntity.EntityId { get { return this.Id; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

    }
}