//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 列表业务逻辑
    /// </summary>
    public class ListService
    {
        private IRepository<ListEntity> repository;
        private IListItemRepository itemRepository;

        public ListService(IRepository<ListEntity> repository, IListItemRepository itemRepository)
        {
            this.repository = repository;
            this.itemRepository = itemRepository;
        }

        /// <summary>
        /// 创建列表
        /// </summary>
        /// <param name="listsEntity">列表管理实体</param>
        public void Create(ListEntity listEntity)
        {
            repository.Insert(listEntity);
        }

        /// <summary>
        /// 删除列表
        /// </summary>
        /// <param name="code">主键编码</param>
        public void Delete(string code)
        {
            itemRepository.DeleteItemsByListCode(code);
            repository.DeleteByEntityId(code);
        }

        /// <summary>
        /// 修改列表
        /// </summary>
        /// <param name="listEntity">列表管理实体</param>
        public void Update(ListEntity listEntity)
        {
            repository.Update(listEntity);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="code">主键编码</param>
        /// <returns>一条列表</returns>
        public ListEntity Get(string code)
        {
            return repository.Get(code);

        }

        /// <summary>
        /// 获取所有列表
        /// </summary>
        /// <returns>所有列表</returns>
        public IEnumerable<ListEntity> GetLists()
        {
            return repository.GetAll();
        }


    }
}
