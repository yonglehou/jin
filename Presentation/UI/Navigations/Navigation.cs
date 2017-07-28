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
using Tunynet.Caching;
using Tunynet.Utilities;

namespace Tunynet.UI
{
    /// <summary>
    /// 导航实体
    /// </summary>
    [TableName("tn_Navigations")]
    [PrimaryKey("NavigationId", autoIncrement = true)]
    [CacheSetting(true, ExpirationPolicy = EntityCacheExpirationPolicies.Stable, PropertyNamesOfArea = "NavigationId")]
    [Serializable]
    public class Navigation : IEntity
    {
        #region 构造器

        /// <summary>
        /// 构造器
        /// </summary>
        public Navigation()
        {
        }

        #endregion 构造器

        #region 需持久化属性
        /// <summary>
        ///NavigationId
        /// </summary>
        public int NavigationId { get; set; }

        /// <summary>
        ///ParentNavigationId
        /// </summary>
        public int ParentNavigationId { get; set; }

        /// <summary>
        ///深度（从上到下以0开始）
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// 导航来源于栏目时的栏目 Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///导航类型 （1：来源于应用；2：来源于栏目）
        /// </summary>
        public NavigationTypes NavigationType { get; set; }

        /// <summary>
        ///导航文字
        /// </summary>
        public string NavigationText { get; set; }

        /// <summary>
        ///导航url， 如果是来源于应用,并且该字段为空,则根据UrlRouteName获取 
        /// </summary>
        public string NavigationUrl { get; set; }

        /// <summary>
        ///应用导航路由规则名称 将会根据该规则名称获取应用导航地址
        /// </summary>
        public string UrlRouteName { get; set; }

        /// <summary>
        ///路由数据名称(Url中包含的路由数据)
        /// </summary>
        public string RouteDataName { get; set; }

        /// <summary>
        ///是新开窗口还是在当前窗口（默认:_self）
        /// </summary>
        public string NavigationTarget { get; set; }

        /// <summary>
        ///排序序号
        /// </summary>
        public int DisplayOrder { get; set; }


        /// <summary>
        ///是否锁定 默认锁定：状态是flase
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        ///是否启用 默认启用：状态是true
        /// </summary>
        public bool IsEnabled { get; set; }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.NavigationId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion




        #region 扩展属性

        /// <summary>
        /// 子导航列表 //todo 待删除
        /// </summary>
        [Ignore]
        public IEnumerable<Navigation> Children
        {
            get
            {
                IEnumerable<Navigation> navigation = DIContainer.Resolve<NavigationService>().GetAll();
                return navigation.Where(n => n.ParentNavigationId == this.NavigationId);
            }
        }

        /// <summary>
        /// 子导航列表
        /// </summary>
        /// <param name="isAdmin">是否后台显示</param>
        /// <returns></returns>
        public IEnumerable<Navigation> Childrens(bool isAdmin = true)
        {
            IEnumerable<Navigation> navigation = DIContainer.Resolve<NavigationService>().GetAll();
            var navigations = navigation.Where(n => n.ParentNavigationId == this.NavigationId);
            if (navigations.Count() > 0)
                navigations = navigations.Where(n => n.IsEnabled);
            return navigations;
        }

        ///// <summary>
        ///// 添加子导航
        ///// </summary>
        //internal protected void AppendChild(Navigation childNavigation)
        //{
        //    if (children == null)
        //        children = new List<Navigation>();

        //    children.Add(childNavigation);
        //}



        ///// <summary>
        ///// Navigation比较（用于排序）
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public int CompareTo(object obj)
        //{
        //    Navigation other = obj as Navigation;
        //    if (other == null)
        //        return 1;

        //    if (this.Depth != other.Depth)
        //        return this.Depth.CompareTo(other.Depth);
        //    else
        //        return this.DisplayOrder.CompareTo(other.DisplayOrder);
        //}

        #endregion






    }
}
