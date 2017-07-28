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
using Tunynet.Common;
using Tunynet.Utilities;
using Tunynet.Caching;
using System.Collections.Concurrent;
using Tunynet.Events;

namespace Tunynet.UI
{
    /// <summary>
    /// 导航业务逻辑
    /// </summary>
    public class NavigationService
    {

        private IRepository<Navigation> repository;
        private ICacheService cacheService;

        #region 构造函数
        /// <summary>
        /// 可设置repository的构造函数
        /// </summary>
        public NavigationService(IRepository<Navigation> repository, ICacheService cacheService)
        {
            this.repository = repository;
            this.cacheService = cacheService;
        }

        #endregion

        #region Get&& Gets

        /// <summary>
        /// 获取所有导航
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Navigation> GetAll()
        {
            IEnumerable<Navigation> navigations = repository.GetAll();
            navigations = navigations.OrderBy(n => n.Depth).ThenBy(n => n.DisplayOrder);
            return navigations;
        }

        /// <summary>
        /// 获取Navigation
        /// </summary>        
        /// <param name="navigationId">导航Id</param>
        /// <returns>返回navigationId对应的初始化导航实体</returns>
        public Navigation Get(int navigationId)
        {
            Navigation navigation = repository.Get(navigationId);
            if (navigation == null)
                return null;
            IEnumerable<Navigation> navigations = repository.GetAll();
            return navigations.FirstOrDefault(n => n.NavigationId == navigationId);
        }

        /// <summary>
        /// 获取顶级栏目
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Navigation> GetRootNavigation()
        {
            return GetAll().Where(x => x.ParentNavigationId == 0);
        }
        /// <summary>
        /// 以缩进排序方式获取所有导航
        /// </summary>
        public IEnumerable<Navigation> GetIndentedAllNavigation()
        {
            IEnumerable<Navigation> rootNavigation = GetRootNavigation();
            List<Navigation> organizedNavigation = new List<Navigation>();
            foreach (var folder in rootNavigation)
            {
                organizedNavigation.Add(folder);
                NavigationForIndented(folder, organizedNavigation);
            }

            return organizedNavigation;
        }
        /// <summary>
        /// 把导航组织成缩进格式
        /// </summary>
        private static void NavigationForIndented(Navigation parentNavigation, List<Navigation> organizedNavigation)
        {
            if (parentNavigation.Children.Count() > 0)
            {
                foreach (Navigation child in parentNavigation.Children)
                {
                    organizedNavigation.Add(child);
                    NavigationForIndented(child, organizedNavigation);
                }
            }
        }

        /// <summary>
        /// 获取当前导航路径的NavigationId（按照Depth 从低到高）
        /// </summary>
        /// <param name="currentNavigationId">当前导航Id</param>
        /// <returns>返回当前导航路径的NavigationId集合</returns>
        public IEnumerable<int> GetCurrentNavigationPathIds(int currentNavigationId)
        {
            IEnumerable<Navigation> navigationsOfPresentAreaOwner = repository.GetAll();
            //对于动态导航有可能会与静态导航的NavigationId重复
            IEnumerable<Navigation> currentNavigations = navigationsOfPresentAreaOwner.Where(n => n.NavigationId == currentNavigationId);
            Navigation currentNavigation;
            if (currentNavigations.Count() > 0)
                currentNavigation = currentNavigations.First();
            else
                return Enumerable.Empty<int>();

            List<int> selectedNavigationIds = new List<int>();
            if (currentNavigation.Depth != 0 || currentNavigation.ParentNavigationId != 0)
            {
                List<Navigation> allAscendants = new List<Navigation>();
                GetRecursiveParents(navigationsOfPresentAreaOwner, currentNavigation, ref allAscendants);

                //按照Depth从低到高排序
                allAscendants.Reverse();
                selectedNavigationIds.AddRange(allAscendants.Select(n => n.NavigationId));
            }
            selectedNavigationIds.Add(currentNavigation.NavigationId);

            return selectedNavigationIds;
        }

        /// <summary>
        /// 递归获取Navigation所有祖先
        /// </summary>
        /// <param name="allNavigations"></param>
        /// <param name="childNavigation"></param>
        /// <param name="allAscendants"></param>
        private void GetRecursiveParents(IEnumerable<Navigation> allNavigations, Navigation childNavigation, ref List<Navigation> allAscendants)
        {
            if (childNavigation.Depth != 0 || childNavigation.ParentNavigationId != 0)
            {
                childNavigation = Get(childNavigation.ParentNavigationId);
                allAscendants.Add(childNavigation);
                GetRecursiveParents(allNavigations, childNavigation, ref allAscendants);
            }
        }


        /// <summary>
        /// 获取Navigation的后代
        /// </summary>
        /// <param name="parentNavigation">父Navigation</param>
        /// <returns>返回parentInitialNavigation的所有后代</returns>
        public IEnumerable<Navigation> GetNavigationOfDescendants(Navigation parentNavigation)
        {
            List<Navigation> descendants = new List<Navigation>();
            IEnumerable<Navigation> navigationsOfPresentArea = repository.GetAll();
            RecursiveGetChildrens(navigationsOfPresentArea, parentNavigation, ref descendants);

            return descendants;
        }

        /// <summary>
        /// 递归获取Navigation所有子Navigation
        /// </summary>
        private void RecursiveGetChildrens(IEnumerable<Navigation> allNavigations, Navigation parentNavigation, ref List<Navigation> allDescendants)
        {
            IEnumerable<Navigation> children = allNavigations.Where(n => n.ParentNavigationId == parentNavigation.NavigationId);
            if (children.Count() > 0)
            {
                allDescendants.AddRange(children);
                foreach (var child in children)
                {
                    RecursiveGetChildrens(allNavigations, child, ref allDescendants);
                }
            }
        }

        #endregion

        #region Insert Update Delete
        /// <summary>
        /// 添加导航
        /// </summary>
        /// <param name="navigation">导航</param>
        /// <exception cref="ArgumentNullException">Navigation为空时</exception>
        /// <exception cref="ArgumentException">Navigation已经存在时</exception>
        /// <exception cref="ApplicationException">Navigation的ParentNavigationId大于0但是相应的Navigation不存在时</exception>
        /// <returns>返回创建的Navigation的Id</returns>
        public void Create(Navigation navigation)
        {
            if (navigation == null)
                throw new ArgumentNullException("Navigation不能为null");

            if (repository.Exists(navigation.NavigationId))
                throw new ArgumentException("NavigationId不允许重复");

            if (navigation.ParentNavigationId > 0)
            {
                Navigation parentNavigation = Get(navigation.ParentNavigationId);
                if (parentNavigation == null)
                    throw new ApplicationException(string.Format("Navigation的父级 {0} 不存在", navigation.ParentNavigationId));

                navigation.Depth = parentNavigation.Depth + 1;
            }
            else
            {
                navigation.Depth = 0;
            }

            EventBus<Navigation>.Instance().OnBefore(navigation, new CommonEventArgs(EventOperationType.Instance().Create()));
            repository.Insert(navigation);
            //设置navigation.DisplayOrder,默认和NavigationId相等
            navigation.DisplayOrder = navigation.NavigationId;
            repository.Update(navigation);

            EventBus<Navigation>.Instance().OnAfter(navigation, new CommonEventArgs(EventOperationType.Instance().Create()));
            EntityData.ForType(typeof(Navigation)).RealTimeCacheHelper.IncreaseAreaVersion("NavigationId", navigation.NavigationId);
        }

        /// <summary>
        /// 更新初始化导航
        /// </summary>
        /// <param name="initialNavigation">初始化导航</param>
        public void Update(Navigation navigation)
        {
            if (navigation == null)
                return;

            EventBus<Navigation>.Instance().OnBefore(navigation, new CommonEventArgs(EventOperationType.Instance().Update()));
            repository.Update(navigation);
            EventBus<Navigation>.Instance().OnAfter(navigation, new CommonEventArgs(EventOperationType.Instance().Update()));

            EntityData.ForType(typeof(Navigation)).RealTimeCacheHelper.IncreaseAreaVersion("NavigationId", navigation.NavigationId);
        }

        /// <summary>
        /// 删除导航
        /// </summary>
        /// <param name="navigationId">导航Id</param>
        /// <exception cref="ArgumentNullException">Navigation为空时</exception>
        /// <exception cref="ApplicationException">Navigation被锁定时</exception>
        public void Delete(int navigationId)
        {
            Navigation navigation = Get(navigationId);
            if (navigation == null)
                throw new ArgumentNullException(string.Format("NavigationId为{0}的Navigation不存在", navigationId));

            if (navigation.IsLocked == true)
                throw new ApplicationException("锁定状态的Navigation不允许删除");

            List<Navigation> navigationsForDeleteList = new List<Navigation>();

            IEnumerable<Navigation> descendants = GetNavigationOfDescendants(navigation);
            if (descendants.Count() > 0)
                navigationsForDeleteList.AddRange(descendants);

            navigationsForDeleteList.Add(navigation);
            foreach (var navigationForDelete in navigationsForDeleteList)
            {
                repository.Delete(navigationForDelete);
            }
            EventBus<Navigation>.Instance().OnAfter(navigation, new CommonEventArgs(EventOperationType.Instance().Delete()));
            EntityData.ForType(typeof(Navigation)).RealTimeCacheHelper.IncreaseAreaVersion("NavigationId", navigation.NavigationId);
        }

        #endregion

        #region 上移下移

        /// <summary>
        /// 交换导航DisplayOrder实现上移下移
        /// </summary>
        /// <param name="fromNavigationId">起始位置导航Id</param>
        /// <param name="toNavigationId">目标位置导航Id</param>
        public void ChangeNavigationDisplayOrder(int fromNavigationId, int toNavigationId)
        {

            var fromNavigation = Get(fromNavigationId);
            var toNavigation = Get(toNavigationId);
            int midNavigationOrder = fromNavigation.DisplayOrder;

            fromNavigation.DisplayOrder = toNavigation.DisplayOrder;
            Update(fromNavigation);

            toNavigation.DisplayOrder = midNavigationOrder;
            Update(toNavigation);
        }


        #endregion

    }
}