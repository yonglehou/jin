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
using Tunynet.Common.Repositories;

namespace Tunynet.Post
{
    /// <summary>
    /// 贴吧
    /// </summary>
    [TableName("tn_Sections")]
    [PrimaryKey("SectionId", autoIncrement = true)]
    [CacheSetting(true)]
    [Serializable]
    public class Section : SerializablePropertiesBase, IEntity
    {
        /// <summary>
        /// 新建实体时使用
        /// </summary>
        //todo:需要检查成员初始化的类型是否正确
        public static Section New()
        {
            Section barSection = new Section()
            {
                Name = string.Empty,
                DateCreated = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 100

            };
            return barSection;
        }

        #region 需持久化属性

        /// <summary>
        ///SectionId
        /// </summary>
        public long SectionId { get; set; }

        /// <summary>
        ///贴吧租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        /// 拥有者Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        ///吧主用户Id（若是活动/群组，则对应活动/群组创建者Id）
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///贴吧名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///贴吧描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///标题图Id
        /// </summary>
        public long FeaturedImageAttachmentId { get; set; }


        /// <summary>
        ///是否启用
        /// </summary>
        public bool IsEnabled { get; set; }


        /// <summary>
        ///主题分类状态 0=禁用；1=启用（不强制）；2=启用（强制）
        /// </summary>
        public ThreadCategoryStatus ThreadCategorySettings { get; set; }


        /// <summary>
        ///排序序号
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.SectionId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 判断是否有Logo
        /// </summary>
        [Ignore]
        public bool HasLogoImage
        {
            get
            {
                return FeaturedImageAttachmentId > 0;
            }
        }


        /// <summary>
        /// 吧管理员列表
        /// </summary>
        [Ignore]
        public IEnumerable<User> SectionManagers
        {
            get
            {
                return DIContainer.Resolve<SectionService>().GetSectionManagers(this.SectionId);
            }
        }


        /// <summary>
        /// 吧主
        /// </summary>
        [Ignore]
        public User User
        {
            get
            {
                IUserService userService = DIContainer.Resolve<IUserService>();
                return userService.GetFullUser(this.UserId);
            }
        }


        /// <summary>
        /// 获取分类
        /// </summary>
        [Ignore]
        public Category Category
        {
            get
            {
                IEnumerable<Category> categories = DIContainer.Resolve<CategoryService>().GetCategoriesOfItem(SectionId, 0, this.TenantTypeId);
                return categories == null || categories.Count() == 0 ? null : categories.FirstOrDefault();
            }
        }

        /// <summary>
        /// 贴吧下所有贴子的分类
        /// </summary>
        [Ignore]
        public IEnumerable<Category> ThreadCategories
        {
            get { return new CategoryRepository().GetOwnerCategories(this.SectionId, TenantTypeIds.Instance().Thread()); }
        }


        #endregion


        #region 计数

        /// <summary>
        /// 主题贴数
        /// </summary>
        [Ignore]
        public int ThreadCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Section());
                return countService.Get(CountTypes.Instance().ThreadCount(), this.SectionId);
            }
        }

        /// <summary>
        /// 主题贴和回贴总数
        /// </summary>
        [Ignore]
        public int ThreadAndPostCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Section());
                return countService.Get(CountTypes.Instance().ThreadAndPostCount(), this.SectionId);
            }
        }

        /// <summary>
        /// 今日主题贴和回贴总数
        /// </summary>
        [Ignore]
        public int ToDayThreadAndPostCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Section());
                return countService.GetStageCount(CountTypes.Instance().ThreadAndPostCount(), 1, this.SectionId);
            }
        }


        #endregion
    }
}