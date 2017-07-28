//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Tunynet;
using Tunynet.Caching;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户资料
    /// </summary>
    [TableName("spb_UserProfiles")]
    [PrimaryKey("UserId", autoIncrement = false)]
    //[CacheSetting(true, PropertyNamesOfArea = "UserId")]
    [CacheSetting(true, ExpirationPolicy = EntityCacheExpirationPolicies.Usual)]
    [Serializable]
    public class UserProfile : SerializablePropertiesBase, IEntity
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        public static UserProfile New(long userId)
        {
            UserProfile userProfile = new UserProfile()
            {
                Birthday = DateTime.Now,
                LunarBirthday = DateTime.Now,
                BirthdayType = BirthdayType.Birthday,
                QQ = string.Empty,
                CardID = string.Empty,
                Introduction = string.Empty,
                CardType = CertificateType.Residentcard,
                NowAreaCode = string.Empty,
                Integrity = 0,
                Gender = GenderType.NotSet,
                UserId = userId
            };
            return userProfile;
        }

        #region 需持久化属性

        /// <summary>
        ///UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///性别1=男,2=女,0=未设置
        /// </summary>
        public GenderType Gender { get; set; }

        /// <summary>
        ///生日类型1=公历,2=阴历
        /// </summary>
        public BirthdayType BirthdayType { get; set; }

        /// <summary>
        ///公历生日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        ///阴历生日
        /// </summary>
        public DateTime LunarBirthday { get; set; }
        
        /// <summary>
        ///所在地
        /// </summary>
        public string NowAreaCode { get; set; }
     
        /// <summary>
        ///QQ
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        ///证件类型
        /// </summary>
        public CertificateType CardType { get; set; }

        /// <summary>
        ///证件号码
        /// </summary>
        public string CardID { get; set; }

        /// <summary>
        ///自我介绍
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 资料完整度（0至100）
        /// </summary>
        public int Integrity { get; set; }

        #endregion 需持久化属性


        #region 序列化属性

        ///// <summary>
        ///// 下次登录是否需要向导(true为不需要向导)
        ///// </summary>
        //[Ignore]
        //public bool IsNeedGuide
        //{
        //    get { return GetExtendedProperty<bool>("IsNeedGuide"); }
        //    set { SetExtendedProperty("IsNeedGuide", value); }
        //}

        ///// <summary>
        ///// 是否上传过头像
        ///// </summary>
        //[Ignore]
        //public bool IsUploadedAvatar
        //{
        //    get { return GetExtendedProperty<bool>("IsUploadedAvatar", false); }
        //    set { SetExtendedProperty("IsUploadedAvatar", value); }
        //}


        #endregion

        #region 扩展属性

      

        /// <summary>
        /// 是都存在IM
        /// </summary>
        [Ignore]
        public bool HasIM
        {
            get
            {
                return !string.IsNullOrEmpty(QQ);
            }
        }

        /// <summary>
        /// 检查所在地是否存在
        /// </summary>
        [Ignore]
        public bool HasNowAreaCode { get { return !string.IsNullOrEmpty(NowAreaCode); } }


        /// <summary>
        /// 自我介绍是否存在
        /// </summary>
        [Ignore]
        public bool HasIntroduction { get { return !string.IsNullOrEmpty(Introduction); } }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.UserId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion IEntity 成员
    }


}