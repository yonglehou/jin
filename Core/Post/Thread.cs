//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;
using Tunynet.Caching;
using Tunynet.Common;
using Tunynet;
using Tunynet.Utilities;

namespace Tunynet.Post
{
    /// <summary>
    /// 贴子
    /// </summary>
    [Serializable]
    [TableName("tn_Threads")]
    [PrimaryKey("ThreadId", autoIncrement = true)]
    [CacheSetting(true, PropertyNameOfBody = "Body")]
    public class Thread : SerializablePropertiesBase, IEntity, IAuditable
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>        
        public static Thread New()
        {
            Thread barThread = new Thread()
            {
                Author = string.Empty,
                Subject = string.Empty,
                IP = WebUtility.GetIP(),
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now
            };
            return barThread;
        }

        #region 需持久化属性

        /// <summary>
        ///ThreadId
        /// </summary>
        public long ThreadId { get; protected set; }

        /// <summary>
        ///所属贴吧Id
        /// </summary>
        public long SectionId { get; set; }

        /// <summary>
        ///所属贴吧租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        ///所属贴吧拥有者Id（例如：群组Id）
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        ///主题作者用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///主题作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///贴子标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///贴子内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///是否锁定
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        ///是否置顶
        /// </summary>
        public bool IsSticky { get; set; }

        /// <summary>
        ///审批状态
        /// </summary>
        public AuditStatus ApprovalStatus { get; set; }


        /// <summary>
        ///发贴人IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///最后更新日期（被回复时也需要更新时间）
        /// </summary>
        public DateTime LastModified { get; set; }

        #endregion

        #region IAuditable 实现

        /// <summary>
        /// 审核项Key 
        /// </summary>
        public string AuditItemKey
        {
            get { return AuditItemKeys.Instance().Post(); }
        }


        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.ThreadId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

        #region 序列化属性
        /// <summary>
        /// 是否匿名评论
        /// </summary>
        [Ignore]
        public bool IsAnonymous
        {
            get { return GetExtendedProperty<bool>("IsAnonymous"); }
            set { SetExtendedProperty("IsAnonymous", value); }
        }


        #endregion

        #region 扩展属性

        /// <summary>
        /// 下一主题ThreadId
        /// </summary>
        [Ignore]
        public long NextThreadId
        {
            get { return new ThreadRepository().GetNextThreadId(this); }

        }

        /// <summary>
        /// 上一主题ThreadId
        /// </summary>
        [Ignore]
        public long PrevThreadId
        {
            get { return new ThreadRepository().GetPrevThreadId(this); }
        }

        /// <summary>
        /// 下一主题
        /// </summary>
        [Ignore]
        public Thread NextThread
        {
            get
            {
                return new ThreadRepository().Get(NextThreadId);
            }
        }

        /// <summary>
        /// 上一主题
        /// </summary>
        [Ignore]
        public Thread PrevThread
        {
            get
            {
                return new ThreadRepository().Get(PrevThreadId);
            }
        }

        /// <summary>
        /// 所属贴吧
        /// </summary>
        [Ignore]
        public Section BarSection
        {
            get
            {
                return new SectionRepository().Get(this.SectionId);
            }
        }

        /// <summary>
        /// 贴子是否为新
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public bool isNew()
        {
            return DateTime.Now.Subtract(this.DateCreated).Days < 3;
        }

        /// <summary>
        /// 主题贴作者头像
        /// </summary>
        [Ignore]
        public string UserAvatar
        {
            get
            {
                return "";
                //return new UserRepository().GetUser(this.UserId).Avatar;
            }
        }


        /// <summary>
        /// 所属本贴子的
        /// </summary>
        [Ignore]
        public IEnumerable<Attachment> Attachments
        {
            get
            {
                IEnumerable<Attachment> attachments = new AttachmentService(TenantTypeIds.Instance().Thread()).GetsByAssociateId(this.ThreadId);
                if (attachments != null)
                    return attachments;
                return new List<Attachment>();
            }
        }

        /// <summary>
        /// 贴子所属分类
        /// </summary>
        public Category ThreadCategory
        {
            get
            {
                CategoryService categoryService = DIContainer.Resolve<CategoryService>();
                var category = categoryService.GetItems(this.ThreadId,this.TenantTypeId);
                if (category != null)
                {
                    return categoryService.Get(category.CategoryId);
                }
                return null;
            }
        }

        /// <summary>
        /// 最后回帖人ID
        /// </summary>
        [Ignore]     
        public long LastModifiedUserId
        {
            get {
                CommentService commentService = DIContainer.Resolve<CommentService>();
                var lastcomment=commentService.GetObjectComments(TenantTypeIds.Instance().Thread(),this.ThreadId,1,SortBy_Comment.DateCreatedDesc).FirstOrDefault();
                if (lastcomment!=null)
                {
                    return lastcomment.UserId;
                }
                return 0;
            }
        }

        #endregion

        #region 提交表单扩展

        /// <summary>
        /// Body Imgids
        /// </summary>
        [Ignore]
        public IEnumerable<long> BodyImageAttachmentIds { get; set; }

        #endregion

        #region 计数

        /// <summary>
        /// 评论数
        /// </summary>
        [Ignore]
        public int CommentCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Thread());
                return countService.Get(CountTypes.Instance().CommentCount(), this.ThreadId);
            }
        }
        /// <summary>
        /// 浏览数
        /// </summary>
        [Ignore]
        public int HitTimes
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Thread());
                return countService.Get(CountTypes.Instance().HitTimes(), this.ThreadId);
            }
        }

        /// <summary>
        /// 今日浏览数
        /// </summary>
        [Ignore]
        public int TodayHitTimes
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Thread());
                return countService.GetStageCount(CountTypes.Instance().HitTimes(), 1, this.ThreadId);
            }
        }

        /// <summary>
        /// 最近7天浏览数
        /// </summary>
        [Ignore]
        public int Last7DaysHitTimes
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Thread());
                return countService.GetStageCount(CountTypes.Instance().HitTimes(), 7, this.ThreadId);
            }
        }

        #endregion


        #region 方法

        /// <summary>
        /// 获取BarThread的Body
        /// </summary>
        /// <remarks>
        /// 由于使用独立的实体内容缓存，Body属性已经置为null
        /// </remarks>
        /// <returns></returns>
        public string GetBody()
        {
            return new ThreadRepository().GetBody(this.ThreadId);
        }

        /// <summary>
        /// 获取Thread的解析过的内容(在web呈现)
        /// </summary>
        public string GetResolvedBody()
        {
            return new ThreadRepository().GetResolvedBody(this.ThreadId);
        }

        /// <summary>
        /// 获取手机端BarThread的Body
        /// </summary>
        /// <returns>(返回的img添加对应的标识符，用于预览)</returns>
        public string GetMobileBody()
        {
            return new ThreadRepository().GetBody(this.ThreadId).Replace("/>", " data-preview-src=\"\" data-preview-group=\"1\" />").Replace("src=\"/img/Emotions", "src=\"../img/Emotions");
        }

        #endregion
    }
}