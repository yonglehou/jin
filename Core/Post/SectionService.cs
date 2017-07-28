//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tunynet;
using Tunynet.Common;
using System.Drawing;
using System.IO;
using Tunynet.Events;
using Tunynet.FileStore;
using Tunynet.Imaging;

namespace Tunynet.Post
{
    /// <summary>
    /// 贴吧业务逻辑类
    /// </summary>
    public class SectionService
    {
        private ISectionRepository barSectionRepository ;
        private IUserService userService;
        private CategoryService categoryService;
        private CategoryManagerService categoryManagerService;
        /// 构造函数
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="barSectionRepository"></param>
        public SectionService(ISectionRepository barSectionRepository, IUserService userService, CategoryService categoryService,
            CategoryManagerService categoryManagerService)
        {
            this.barSectionRepository = barSectionRepository;
            this.userService = userService;
            this.categoryService = categoryService;
            this.categoryManagerService = categoryManagerService;
        }

        #region 维护贴吧

        /// <summary>
        /// 创建贴吧
        /// </summary>
        /// <param name="section">贴吧</param>
        /// <param name="userId">当前操作人</param>
        /// <param name="managerIds">管理员用户Id</param>
        /// <param name="logoFile">贴吧标识图</param>
        /// <returns>是否创建成功</returns>
        public bool Create(Section section, IEnumerable<long> managerIds)
        {
            EventBus<Section>.Instance().OnBefore(section, new CommonEventArgs(EventOperationType.Instance().Create()));
            barSectionRepository.Insert(section);
            if (section.SectionId > 0)
            {

                if (managerIds != null && managerIds.Count() > 0)
                {
                    List<long> mangagerIds_list = managerIds.ToList();
                    mangagerIds_list.Remove(section.UserId);
                    managerIds = mangagerIds_list;
                    categoryManagerService.UpdateManagerIds(TenantTypeIds.Instance().Section(), section.SectionId, managerIds);
                }
                barSectionRepository.Update(section);
                EventBus<Section>.Instance().OnAfter(section, new CommonEventArgs(EventOperationType.Instance().Create()));
                //记录操作日志
                EventBus<Section, CommonEventArgs>.Instance().OnAfter(section, new CommonEventArgs(EventOperationType.Instance().Create()));
            }
            return section.SectionId > 0;
        }

        /// <summary>
        /// 更新贴吧
        /// </summary>
        /// <param name="section">贴吧</param>
        /// <param name="userId">当前操作人</param>
        /// <param name="managerIds">管理员用户Id</param>
        /// <param name="sectionedFile">贴吧标识图</param>
        public void Update(Section section, IEnumerable<long> managerIds)
        {
            EventBus<Section>.Instance().OnBefore(section, new CommonEventArgs(EventOperationType.Instance().Update()));

            barSectionRepository.Update(section);

            if (managerIds != null && managerIds.Count() > 0)
            {
                List<long> mangagerIds_list = managerIds.ToList();
                mangagerIds_list.Remove(section.UserId);
                managerIds = mangagerIds_list;
            }
            categoryManagerService.UpdateManagerIds(TenantTypeIds.Instance().Section(), section.SectionId, managerIds);

            EventBus<Section>.Instance().OnAfter(section, new CommonEventArgs(EventOperationType.Instance().Update()));
            //记录操作日志
            EventBus<Section, CommonEventArgs>.Instance().OnAfter(section, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        /// 删除贴吧
        /// </summary>
        /// <param name="sectionId">贴吧Id</param>
        public void Delete(long sectionId)
        {
            Section section = barSectionRepository.Get(sectionId);
            if (section == null)
                return;
            EventBus<Section>.Instance().OnBefore(section, new CommonEventArgs(EventOperationType.Instance().Delete()));
            var barThreadService = DIContainer.Resolve<ThreadService>();
            //贴子
            barThreadService.DeletesBySectionId(sectionId);

            //贴吧分类
            categoryService.ClearCategoriesFromItem(sectionId, null, section.TenantTypeId);

            //贴子分类
            var categories = categoryService.GetRootCategoriesOfOwner(TenantTypeIds.Instance().Thread(), sectionId);
            foreach (var category in categories)
            {
                categoryService.Delete(category.CategoryId);
            }
            ////贴吧标签
            //TagService tagService = new TagService(TenantTypeIds.Instance().BarThread());
            //tagService.ClearTagsFromOwner(sectionId);
          
            ////删除推荐记录
            //RecommendService recommendService = new RecommendService();
            //recommendService.Delete(sectionId, TenantTypeIds.Instance().BarSection());

            int affectCount = barSectionRepository.Delete(section);
            if (affectCount > 0)
            {
                EventBus<Section>.Instance().OnAfter(section, new CommonEventArgs(EventOperationType.Instance().Delete()));
                //记录操作日志
                EventBus<Section, CommonEventArgs>.Instance().OnAfter(section, new CommonEventArgs(EventOperationType.Instance().Delete()));
                //EventBus<BarSection, AuditEventArgs>.Instance().OnAfter(section, new AuditEventArgs(null, section.AuditStatus));
            }
        }


        //todo,by mazq,20170401,@zhangzh 这个方法有用吗？@mazq 删除用户的创建 的贴吧 暂时无用 可以一起删除
       

        #endregion

        #region 获取贴吧
        /// <summary>
        /// 获取单个贴吧实体
        /// </summary>
        /// <param name="sectionId">贴吧Id</param>
        /// <returns>贴吧</returns>
        public Section Get(long sectionId)
        {
            return barSectionRepository.Get(sectionId);
        }

        //todo,by mazq,20170401,@zhangzh 这个方法应该去掉 @mazq 已去掉 GetSectionBySectionName
      
        //1、版主应该改成管理员，需求、原型图、本方法命名都应该修改；
        //2、贴吧管理员与贴吧的关注者是什么关系，贴吧管理员允许取消对贴吧的关注吗？

        /// <summary>
        /// 是否为吧主
        /// </summary>
        /// <param name="userId">被验证用户Id</param>
        /// <param name="sectionId">贴吧Id</param>
        /// <returns></returns>
        public bool IsSectionOwner(long userId, long sectionId)
        {
            Section barSection = Get(sectionId);
            if (barSection == null)
                return false;
            return barSection.UserId == userId;
        }





        /// <summary>
        /// 获取贴吧的管理员列表
        /// </summary>
        /// <param name="sectionId">贴吧Id</param>
        /// <returns>贴吧</returns>
        public IEnumerable<User> GetSectionManagers(long sectionId)
        {
            bool isReference = false;
            return userService.GetFullUsers(categoryManagerService.GetCategoryManagerIds(TenantTypeIds.Instance().Section(), sectionId, out isReference));
        }

        /// <summary>
        /// 依据OwnerId获取单个贴吧（用于OwnerId与贴吧一对一关系）
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>贴吧</returns>
        public Section GetByOwnerId(string tenantTypeId, long ownerId)
        {
            return barSectionRepository.GetByOwnerId(tenantTypeId, ownerId);
        }

        /// <summary>
        /// 获取拥有者的贴吧列表
        /// </summary>
        /// <param name="userId">吧主UserId</param>
        /// <returns>贴吧列表</returns>
        public IEnumerable<Section> GetsByUserId(string tenantTypeId, long userId)
        {
            return barSectionRepository.GetsByUserId(tenantTypeId, userId);
        }

        /// <summary>
        /// 获取贴吧的排行数据（仅显示审核通过的贴吧）
        /// </summary>
        /// <param name="topNumber">前多少条</param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="sortBy">贴吧排序依据</param>
        /// <returns></returns>
        public IEnumerable<Section> GetTops(int topNumber, long? categoryId, SortBy_BarSection sortBy = SortBy_BarSection.DateCreated_Desc)
        {
            //只获取启用状态的贴吧
            //缓存周期：相对稳定，不用维护即时性
            return barSectionRepository.GetTops(TenantTypeIds.Instance().Section(), topNumber, categoryId, sortBy);
        }

        /// <summary>
        /// 获取贴吧列表
        /// </summary>
        /// <remarks>在频道贴吧分类页使用</remarks>
        /// <param name="nameKeyword"></param>
        /// <param name="sortBy">贴吧排序依据</param>
        /// <param name="categoryId">贴吧分类Id</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴吧列表</returns>
        public PagingDataSet<Section> Gets(string nameKeyword, long? categoryId, SortBy_BarSection sortBy, int pageIndex)
        {            
            //需要获取categoryId所有后代的类别下的BarSection
            //按排序序号、创建时间倒序排序
            //缓存周期：相对稳定，需维护即时性
            return barSectionRepository.Gets(TenantTypeIds.Instance().Section(), nameKeyword, categoryId, sortBy, pageIndex);
        }

        /// <summary>
        /// 贴吧管理时查询贴吧分页集合
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="query">贴吧查询条件</param>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>贴吧分页集合</returns>
        public PagingDataSet<Section> Gets(string tenantTypeId, SectionQuery query, int pageSize, int pageIndex)
        {
            //缓存周期：对象集合，需要维护即时性
            //使用用户选择器设置query.UserId参数
            return barSectionRepository.Gets(tenantTypeId, query, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据贴吧Id集合组装贴吧实体集合
        /// </summary>
        /// <param name="sectionIds">贴吧Id集合</param>
        /// <returns>贴吧实体集合</returns>
        public IEnumerable<Section> GetBarsections(IEnumerable<long> sectionIds)
        {
            return barSectionRepository.PopulateEntitiesByEntityIds<long>(sectionIds);
        }
        #endregion



    }
}