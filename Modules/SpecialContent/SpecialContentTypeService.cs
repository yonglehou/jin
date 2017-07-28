//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Events;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 推荐类别管理
    /// </summary>
    public class SpecialContentTypeService
    {
        private ISpecialContentItemRepository iContentItemRepositories;
        private ISpecialContentTypeRepository iContentTypesRepository;
      
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="iContentItemRepositories"></param>
        /// <param name="iContentTypesRepository"></param>
        public SpecialContentTypeService(ISpecialContentItemRepository iContentItemRepositories, ISpecialContentTypeRepository iContentTypesRepository)
        {
            this.iContentItemRepositories = iContentItemRepositories;
            this.iContentTypesRepository = iContentTypesRepository;
        }

        /// <summary>
        /// 创建推荐类别
        /// </summary>
        /// <param name="specialContentType">推荐类别的实体</param>
        /// <returns>成功为true 失败为false</returns>
        public bool Create(SpecialContentType specialContentType)
        { 
           var specialType = iContentTypesRepository.Get(specialContentType.TypeId);
            if (specialType==null)
            {
                iContentTypesRepository.Insert(specialContentType);
                //记录日志
                EventBus<SpecialContentType, CommonEventArgs>.Instance().OnAfter(specialContentType,new CommonEventArgs(EventOperationType.Instance().Create()));
                return true;
            }
             return false;
        }
        /// <summary>
        /// 更新推荐类别
        /// </summary>
        /// <param name="specialContentType">推荐类别的实体</param>
        public void Update(SpecialContentType specialContentType)
        {
            iContentTypesRepository.Update(specialContentType);
            //记录日志
            EventBus<SpecialContentType, CommonEventArgs>.Instance().OnAfter(specialContentType, new CommonEventArgs(EventOperationType.Instance().Update()));

        }
        /// <summary>
        /// 删除推荐类别
        /// </summary>
        /// <param name="TypeId">推荐类型的ID</param>
        /// <returns>成功为true 失败为false</returns>
        public int Delete(int typeId)
        {
            //记录日志
            EventBus<SpecialContentType, CommonEventArgs>.Instance().OnAfter(Get(typeId), new CommonEventArgs(EventOperationType.Instance().Delete()));
            //删除推荐类别
            return iContentTypesRepository.DeleteByEntityId(typeId);
      
        }

        /// <summary>
        /// 根据推荐类别的ID获取推荐类别
        /// </summary>
        /// <param name="TypeId"></param>
        /// <returns></returns>
        public SpecialContentType Get(int typeId)
        {
            return iContentTypesRepository.Get(typeId);
        }

        /// <summary>
        /// 获取所有推荐类别
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SpecialContentType> GetAll()
        {
            return iContentTypesRepository.GetAll();
        }

        /// <summary>
        /// 获取适合租户类型的推荐类别
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SpecialContentType> GetTypesByTenantType(string tenantTypeId)
        {
            return iContentTypesRepository.GetTypesByTenantType(tenantTypeId);
        }
    }
}
