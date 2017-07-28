//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Tunynet.Repositories;

namespace Tunynet.CMS
{
    public class ContentModelService
    {
        private IRepository<ContentModel> contentModelRepository;
        private IRepository<ContentModelAdditionalFields> contentModelAdditionalRepository;

        /// <summary>
        /// 构造器
        /// </summary>
        public ContentModelService(IRepository<ContentModel> contentModelRepository, IRepository<ContentModelAdditionalFields> contentModelAdditionalRepository)
        {
            this.contentModelRepository = contentModelRepository;
            this.contentModelAdditionalRepository = contentModelAdditionalRepository;
        }

        /// <summary>
        /// 获取内容模型
        /// </summary>
        /// <param name="contentTypeId"></param>
        /// <returns></returns>
        public ContentModel Get(long modelId)
        {
            return contentModelRepository.Get(modelId);
        }

        /// <summary>
        /// 获取所有内容模型
        /// </summary>
        /// <param name="contentTypeId"></param>
        /// <returns></returns>
        public IEnumerable<ContentModel> GetAll()
        {
            return contentModelRepository.GetAll();
        }

        /// <summary>
        /// 获取内容模型的所有字段
        /// </summary>
        /// <param name="contentTypeId"></param>
        /// <returns></returns>
        public IEnumerable<ContentModelAdditionalFields> GetColumnsByContentModelId(int modelId)
        {
            return contentModelAdditionalRepository.GetAll().Where(n=>n.ModelId == modelId).ToList();
        }

        /// <summary>
        /// 通过ContentModelKey来获取ContentModel
        /// </summary>
        /// <param name="ContentModelKey">内容模型的Key</param>
        /// <returns></returns>
        public ContentModel GetContentModelsByContentModeKey(string contentModelKey)
        {
           return contentModelRepository.GetAll().FirstOrDefault(n => n.ModelKey == contentModelKey);
        }

    }
}
