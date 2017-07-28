//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Common
{

    /// <summary>
    /// 通知的实体类
    /// </summary>
    [TableName("tn_Notices")]
    [PrimaryKey("Id", autoIncrement = true)]
    [Serializable]
    public class Notice : SerializablePropertiesBase, IEntity
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static Notice New()
        {
            Notice notice = new Notice();
            notice.Body = string.Empty;
            notice.NoticeTypeKey = string.Empty;
            notice.ReceiverId = 0;
            notice.LeadingActorUserId = 0;
            notice.LeadingActor = string.Empty;
            notice.RelativeObjectName = string.Empty;
            notice.RelativeObjectId = 0;
            notice.RelativeObjectUrl = string.Empty;
            notice.Status = 0;
            notice.DateCreated = DateTime.Now;
            notice.LastSendDate = DateTime.Now;
            notice.Times = 0;


            return notice;
        }

        #region 需持久化属性
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 通知类型 Key
        /// </summary>
        public string NoticeTypeKey { get; set; }
        /// <summary>
        /// 通知接收人
        /// </summary>
        public long ReceiverId { get; set; }
        /// <summary>
        /// 主角 UserId
        /// </summary>
        public long LeadingActorUserId { get; set; }
        /// <summary>
        /// 主角
        /// </summary>
        public string LeadingActor { get; set; }
        /// <summary>
        /// 相关项对象名称
        /// </summary>
        public string RelativeObjectName { get; set; }
        /// <summary>
        /// 相关项对象 Id
        /// </summary>
        public long RelativeObjectId { get; set; }
        /// <summary>
        /// 相关项对象链接地址
        /// </summary>
        public string RelativeObjectUrl { get; set; }
        /// <summary>
        /// 触发通知的对象Id
        /// </summary>
        public long ObjectId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 处理状态   0=Unhandled:未处理;1=Readed  知道了;  2=Accepted 接受；3=Refused 拒绝；
        /// </summary>
        public NoticeStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// 上次发送时间
        /// </summary>
        public DateTime LastSendDate { get; set; }
        /// <summary>
        /// 通知发送次数
        /// </summary>
        public int Times { get; set; }

        #endregion

        #region 拓展属性

        /// <summary>
        /// 获取解析过的内容
        /// </summary>
        [Ignore]
        public string ResolvedBody
        {
            get
            {
                //if (!string.IsNullOrEmpty(Body))
                //    return Body;
                return NoticeBuilder.Instance().Resolve(this);
            }
        }
        /// <summary>
        /// 获取手机模板解析内容
        /// </summary>
        [Ignore]
        public string MobileResolvedBody
        {
            get
            {
                //if (!string.IsNullOrEmpty(Body))
                //    return Body;
                return NoticeBuilder.Instance().MobileResolve(this);
            }
        }

        /// <summary>
        /// 个推内容的title
        /// </summary>
        [Ignore]
        public string GetuiTitle
        {
            get
            {
                var title = string.Empty;

                switch (this.NoticeTypeKey)
                {
                    case "NewThreadReply":
                    case "NewCommentReply":
                    case "NewArticleReply":
                    case "NewImageReply":
                    case "NewVideoReply":
                    case "NewCMSReply":
                    case "NewAskAnswerComment":
                    case "NewAskQuestionComment":
                    case "NewAskComment":
                        title = "您有一条新回复";
                        break;
                    case "ThreadApproved":
                    case "CMSArticleApproved":
                    case "ThreadDisapproved":
                    case "CMSArticleDisapproved":
                        title = "您有一条新提醒";
                        break;
                    case "FollowUser":
                        title = "您有一条关注信息";
                        break;
                    case "NewAnswer":
                        title = "您的问题有新的回答";
                        break;
                    case "SetBestAnswer":
                        title = "您的回答被设为了最佳回答";
                        break;
                    default:
                        break;
                }

                return title;
            }
        }

        /// <summary>
        /// 个推内容的Text
        /// </summary>
        [Ignore]
        public string GetuiText
        {
            get
            {
                var text = string.Empty;

                switch (this.NoticeTypeKey)
                {
                    case "NewThreadReply":
                    case "NewCommentReply":
                    case "NewArticleReply":
                    case "NewImageReply":
                    case "NewVideoReply":
                    case "NewCMSReply":
                    case "NewAskAnswerComment":
                    case "NewAskQuestionComment":
                    case "NewAskComment":
                        text = string.Format("{0}在'{1}'中回复了您", LeadingActor, RelativeObjectName);
                        break;
                    case "ThreadApproved":
                        text = string.Format("您的帖子'{0}'通过了审核", RelativeObjectName);
                        break;
                    case "CMSArticleApproved":
                        text = string.Format("您的文章'{0}'通过了审核", RelativeObjectName);
                        break;
                    case "ThreadDisapproved":
                        text = string.Format("您的帖子'{0}'没有通过审核", RelativeObjectName);
                        break;
                    case "CMSArticleDisapproved":
                        text = string.Format("您的文章'{0}'没有通过审核", RelativeObjectName);
                        break;
                    case "FollowUser":
                        text = string.Format("{0}关注了您", LeadingActor);
                        break;
                    case "NewAnswer":
                        text = string.Format("{0}在'{1}'中回答了您", LeadingActor, RelativeObjectName);
                        break;
                    case "SetBestAnswer":
                        text = string.Format("您在'{0}'的回答被设为了最佳回答", RelativeObjectName);
                        break;
                    default:
                        break;
                }

                return text;
            }
        }

        /// <summary>
        /// 主角空间链接
        /// </summary>
        [Ignore]
        public string LeadingActorUrl
        {
            get
            {
                return GetExtendedProperty<string>("LeadingActorUrl");
            }
            set
            {
                SetExtendedProperty("LeadingActorUrl", value);
            }
        }
        #endregion

        #region IEntity 成员
        object IEntity.EntityId { get { return this.Id; } }
        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

    }
}
