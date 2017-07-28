//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Caching;
using Tunynet.Common.Repositories;
using Tunynet.Events;
using System.IO;
using System.Text.RegularExpressions;

namespace Tunynet.Common
{
    /// <summary>
    /// 标签业务逻辑类
    /// </summary>
    public class TagService<T> where T : Tag
    {
        #region private item
        private ITagRepository<T> tagRepository;
        private IItemInTagRepository itemInTagRepository;
        private IRelatedTagRepository relatedTagRepository;
        private ICacheService cacheService;
        #endregion

        private string tenantTypeId;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        public TagService(string tenantTypeId)
            : this(tenantTypeId, new TagRepository<T>(), new ItemInTagRepository(), new RelatedTagRepository(), DIContainer.Resolve<ICacheService>())
        {
        }

        /// <summary>
        /// 可设置repository的构造函数
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="tagRepository">标签Repository</param>
        /// <param name="itemInTagRepository">内容与标签关系Repository</param>
        /// <param name="tagInOwnerReposiory">标签与拥有者关系Repository</param>
        /// <param name="tagGroupRepository">标签分组Repository</param>
        /// <param name="tagInGroupRepository">标签与分组关系Repository</param>
        /// <param name="relatedTagRepository"></param>
        public TagService(string tenantTypeId, ITagRepository<T> tagRepository, IItemInTagRepository itemInTagRepository, IRelatedTagRepository relatedTagRepository, ICacheService cacheService)
        {
            this.tenantTypeId = tenantTypeId;
            this.tagRepository = tagRepository;
            this.itemInTagRepository = itemInTagRepository;
            this.relatedTagRepository = relatedTagRepository;
            this.cacheService = cacheService;
        }

        /// <summary>
        /// 用于标签分割的字符数组
        /// </summary>
        /// <remarks>
        /// 可以在添加标签时用户SplitCharacters中的字符做分割一次录入多个标签
        /// </remarks>
        public static readonly char[] SplitCharacters = new char[] { ',', ';', '，', '；', ' ' };

        /// <summary>
        /// Url特殊字符
        /// </summary>
        private static readonly char[] URLSpecialCharacters = new char[] { '%', '/', '?', '&', '*', '-', ':' };

        /// <summary>
        /// 标签云系数
        /// </summary>
        private static float[] siteTagLevelPartitions = new float[] { 0.0F, 0.01F, 0.04F, 0.09F, 0.16F, 0.25F, 0.36F, 0.49F, 0.64F, 0.81F };

        #region Tags

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="tag">待创建的标签</param>
        /// <param name="logoStream">标题图文件流</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public bool Create(T tag)
        {
            //创建数据前，触发相关事件
            EventBus<T>.Instance().OnBefore(tag, new CommonEventArgs(EventOperationType.Instance().Create()));
            tagRepository.Insert(tag);
            //if (tag.TagId > 0)
            //{
            //    tagRepository.Update(tag);
            //若创建成功，触发创建后相关事件
            EventBus<T>.Instance().OnAfter(tag, new CommonEventArgs(EventOperationType.Instance().Create()));
            return true;
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="tag">待创建的标签</param>
        /// <param name="logoStream">标题图文件流</param>
        /// <returns></returns>
        public void Update(T tag)
        {
            //若更新据前，触发相关事件
            EventBus<T>.Instance().OnBefore(tag, new CommonEventArgs(EventOperationType.Instance().Update()));
            tagRepository.Update(tag);
            //若更新成功，触发创建后相关事件
            EventBus<T>.Instance().OnAfter(tag, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagId">标签Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool Delete(long tagId)
        {
            T tag = tagRepository.Get(tagId);

            int affectCount = 0;

            if (tag != null)
            {
                //删除数据前，触发相关事件
                EventBus<T>.Instance().OnBefore(tag, new CommonEventArgs(EventOperationType.Instance().Delete()));
                affectCount = tagRepository.Delete(tag);
                //若删除成功，触发删除后相关事件
                EventBus<T>.Instance().OnAfter(tag, new CommonEventArgs(EventOperationType.Instance().Delete()));

                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取Tag
        /// </summary>
        /// <param name="tagId">标签Id</param>
        public T Get(long tagId)
        {
            return tagRepository.Get(tagId);
        }

        /// <summary>
        /// 获取标签实体
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <returns></returns>
        public T Get(string tagName)
        {
            return tagRepository.Get(tagName, tenantTypeId);
        }

        /// <summary>
        /// 获取前N个标签
        /// </summary>
        ///<param name="topNumber">前N条数据</param>
        ///<param name="sortBy">标签排序字段</param>
        /// <returns>{Key:标签实体,Value:标签级别}</returns>
        public Dictionary<T, int> GetTopTags(int topNumber, SortBy_Tag? sortBy)
        {
            StringBuilder cacheKey = new StringBuilder();
            cacheKey.Append(EntityData.ForType(typeof(T)).RealTimeCacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion, "TenantTypeId", tenantTypeId));
            cacheKey.AppendFormat("TagCloud-TenantTypeId:{0}", tenantTypeId);
            cacheKey.AppendFormat("TagCloud-TopNumber:{0}", topNumber);

            Dictionary<T, int> tagCloud = cacheService.Get<Dictionary<T, int>>(cacheKey.ToString());

            if (tagCloud == null)
            {
                tagCloud = new Dictionary<T, int>();
                List<T> tags = tagRepository.GetTopTags(tenantTypeId, topNumber, null, sortBy).ToList();

                float x = 0;
                foreach (T tag in tags)
                {
                    x = (float)1 / (tag.ItemCount == 0 ? 1 : tag.ItemCount);

                    for (int j = 0; j < 9; j++)
                    {
                        if (x >= siteTagLevelPartitions[j] && x < siteTagLevelPartitions[j + 1])
                        {
                            tagCloud[tag] = 9 - j;
                            break;
                        }
                    }

                    if (x >= siteTagLevelPartitions[9])
                        tagCloud[tag] = 0;
                }

                tagCloud = tagCloud.OrderBy(n => n.Key.TagName).ToDictionary(k => k.Key, v => v.Value);
                cacheService.Set(cacheKey.ToString(), tagCloud, CachingExpirationType.ObjectCollection);
            }
            return tagCloud;
        }

        /// <summary>
        /// 获取前N个标签
        /// </summary>
        ///<param name="topNumber">获取数据的条数</param>
        ///<param name="isFeatured">是否为特色标签</param>
        ///<param name="sortBy">标签排序字段</param>
        public IEnumerable<T> GetTopTags(int topNumber, bool? isFeatured, SortBy_Tag? sortBy)
        {

            return tagRepository.GetTopTags(tenantTypeId, topNumber, isFeatured, sortBy);
        }

        /// <summary>
        /// 获取前N个标签名
        /// </summary>
        /// <remarks>用于智能提示</remarks>
        ///<param name="keyword">标签名称关键字</param>
        ///<param name="topNumber">前N条数据</param>
        public IEnumerable<string> GetTopTagNames(string keyword, int topNumber)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;

            IEnumerable<T> tags = GetTopTags(1000, null, SortBy_Tag.ItemCountDesc);
            IEnumerable<string> tagNames = null;

            if (tags != null)
            {
                tagNames = tags.Select(n => n.TagName).Where(n => n.Contains(keyword.Trim())).Take(topNumber);
            }

            return tagNames;
        }

        /// <summary>
        ///分页检索标签
        /// </summary>
        ///<param name="query">查询条件</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        public PagingDataSet<T> GetTags(TagQuery query, int pageIndex, int pageSize)
        {
            return tagRepository.GetTags(query, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据标签Id列表组装标签实体
        /// </summary>
        /// <param name="tagIds">标签Id集合</param>
        /// <returns></returns>
        public IEnumerable<T> GetTags(IEnumerable<long> tagIds)
        {
            return tagRepository.PopulateEntitiesByEntityIds(tagIds);
        }

        #endregion Tags

        #region ItemInTag

        /// <summary>
        /// 为多个内容项添加相同标签
        /// </summary>
        /// <param name="itemIds">内容项Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="tagName">标签名</param>
        public void AddItemsToTag(IEnumerable<long> itemIds, string tagName)
        {
            string name = string.Concat(tagName.Split(URLSpecialCharacters)).Replace("[_]", " ");
            EventBus<long, TagEventArgs>.Instance().OnBatchBefore(itemIds, new TagEventArgs(EventOperationType.Instance().Create(), tenantTypeId, name));
            itemInTagRepository.AddItemsToTag(itemIds, tenantTypeId, name);
            EventBus<long, TagEventArgs>.Instance().OnBatchAfter(itemIds, new TagEventArgs(EventOperationType.Instance().Create(), tenantTypeId, name));
        }

        /// <summary>
        /// 为内容项批量设置标签
        /// </summary>
        /// <param name="tagName">标签名称</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="itemId">内容项Id</param>
        public void AddTagToItem(string tagName, long itemId)
        {
            string name = string.Concat(tagName.Split(URLSpecialCharacters)).Replace("[_]", " ");
            EventBus<string, TagEventArgs>.Instance().OnBefore(name, new TagEventArgs(EventOperationType.Instance().Create(), tenantTypeId, itemId));
            itemInTagRepository.AddTagsToItem(new string[] { name }, tenantTypeId, itemId);
            EventBus<string, TagEventArgs>.Instance().OnAfter(name, new TagEventArgs(EventOperationType.Instance().Create(), tenantTypeId, itemId));
        }

        /// <summary>
        /// 为内容项批量设置标签
        /// </summary>
        /// <remarks>标签中如果要包含空格需要用""引起来</remarks>
        /// <param name="tagString">待处理的标签字符串</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="itemId">内容项Id</param>
        public void AddTagsToItem(string tagString, long itemId)
        {
            //拆分标签数组
            string[] tagNames = SplitTagString(tagString);
            if (tagNames != null)
            {
                for (int i = 0; i < tagNames.Length; i++)
                {
                    //把空格的占位符替换回来
                    tagNames[i] = string.Concat(tagNames[i].Split(URLSpecialCharacters)).Replace("[_]", " ");
                }
            }

            EventBus<string, TagEventArgs>.Instance().OnBatchBefore(tagNames, new TagEventArgs(EventOperationType.Instance().Create(), tenantTypeId, itemId));
            //添加标签关联记录
            itemInTagRepository.AddTagsToItem(tagNames, tenantTypeId, itemId);
            EventBus<string, TagEventArgs>.Instance().OnBatchAfter(tagNames, new TagEventArgs(EventOperationType.Instance().Create(), tenantTypeId, itemId));
        }

        /// <summary>
        /// 为内容项批量设置标签
        /// </summary>
        /// <remarks>标签中如果要包含空格需要用""引起来</remarks>
        /// <param name="tagNames">待添加的标签集合</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="itemId">内容项Id</param>
        public void AddTagsToItem(string[] tagNames, long itemId)
        {
            for (int i = 0; i < tagNames.Length; i++)
            {
                //把空格的占位符替换回来
                tagNames[i] = string.Concat(tagNames[i].Split(URLSpecialCharacters)).Replace("[_]", " ");
            }
            EventBus<string, TagEventArgs>.Instance().OnBatchBefore(tagNames, new TagEventArgs(EventOperationType.Instance().Create(), tenantTypeId, itemId));
            itemInTagRepository.AddTagsToItem(tagNames, tenantTypeId, itemId);
            EventBus<string, TagEventArgs>.Instance().OnBatchAfter(tagNames, new TagEventArgs(EventOperationType.Instance().Create(), tenantTypeId, itemId));
        }

        /// <summary>
        /// 删除标签与内容项的关联
        /// </summary>
        /// <param name="itemInTagId">内容项与标签关联Id</param>
        public void DeleteTagFromItem(long itemInTagId)
        {
            ItemInTag itemInTag = itemInTagRepository.Get(itemInTagId);
            EventBus<ItemInTag>.Instance().OnBefore(itemInTag, new CommonEventArgs(EventOperationType.Instance().Delete()));
            //TagInOwner tagInOwner = tagInOwnerReposiory.Get(itemInTag.TagInOwnerId);
            //if (tagInOwner != null && tagInOwner.ItemCount <= 1)
            //{
            //    tagInOwnerReposiory.Delete(tagInOwner);
            //}
            itemInTagRepository.Delete(itemInTag);

            EventBus<ItemInTag>.Instance().OnAfter(itemInTag, new CommonEventArgs(EventOperationType.Instance().Delete()));
        }

        /// <summary>
        /// 清除内容项的所有标签
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <param name="ownerId">拥有者Id</param>
        public void ClearTagsFromItem(long itemId)
        {
            EventBus<long, TagEventArgs>.Instance().OnBefore(itemId, new TagEventArgs(EventOperationType.Instance().Delete(), tenantTypeId));
            itemInTagRepository.ClearTagsFromItem(itemId, tenantTypeId);
            EventBus<long, TagEventArgs>.Instance().OnAfter(itemId, new TagEventArgs(EventOperationType.Instance().Delete(), tenantTypeId));
        }

        /// <summary>
        /// 获取标签的所有内容项集合
        /// </summary>
        /// <param name="tagName">标签名称</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>返回指定的内容项Id集合</returns>
        //public IEnumerable<long> GetTagOfItemIds(string tagName)
        //{
        //    return itemInTagRepository.GetTagOfItemIds(tagName, tenantTypeId);
        //}

        ///// <summary>
        ///// 获取标签的内容项集合
        ///// </summary>
        ///// <param name="tagName">标签名称</param>
        ///// <param name="tenantTypeId">租户id</param>
        ///// <param name="pageSize">页数</param>
        ///// <param name="pageIndex">页码</param>
        ///// <returns></returns>
        public PagingDataSet<ItemInTag> GetItemIds(string tagName, int pageSize, int pageIndex)
        {
            return itemInTagRepository.GetItemIds(tagName, tenantTypeId, pageSize, pageIndex);
        }

        ///// <summary>
        /////  获取多个标签的内容项集合
        ///// </summary>
        ///// <param name="tagNames">多个名称</param>
        ///// <param name="tenantTypeId">租户id</param>
        ///// <param name="pageSize">页数</param>
        ///// <param name="pageIndex">页码</param>
        ///// <returns></returns>
        //public PagingDataSet<ItemInTag> GetItemIds(IEnumerable<string> tagNames,  int pageSize, int pageIndex)
        //{
        //    return itemInTagRepository.GetItemIds(tagNames, tenantTypeId, pageSize, pageIndex);
        //}

        /// <summary>
        /// 获取内容项的所有标签
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <returns>返回内容项的标签集合</returns>
        public IEnumerable<ItemInTag> attiGetItemInTagsOfItem(long itemId)
        {
            IEnumerable<long> tagIds = itemInTagRepository.GetItemInTagIdsOfItem(itemId, tenantTypeId);
            return itemInTagRepository.PopulateEntitiesByEntityIds(tagIds);
        }

        /// <summary>
        /// 获取内容项的前N个标签标签
        /// </summary>
        /// <param name="itemId">内容项Id</param>
        /// <param name="topNumber">前N条记录</param>
        /// <returns>返回内容项的标签集合</returns>
        public IEnumerable<T> GetTopTagsOfItem(long itemId, int topNumber)
        {
            IEnumerable<long> ids = itemInTagRepository.GetTagIdsOfItem(itemId, tenantTypeId);
            if (ids != null && ids.Count() > topNumber)
                ids = ids.Take(topNumber);

            return tagRepository.PopulateEntitiesByEntityIds(ids);
        }


        /// <summary>
        /// 根据用户ID列表获取ItemInTag的ID列表，本方法现用于用户搜索功能的索引生成
        /// </summary>
        /// <param name="userIds">用户ID列表</param>
        /// <returns>ItemInTag的ID列表</returns>
        public IEnumerable<long> GetItemInTagIdsByItemIds(IEnumerable<long> userIds)
        {
            return itemInTagRepository.GetEntityIdsByUserIds(userIds);
        }

        /// <summary>
        /// 根据Id列表获取ItemInTag的实体列表
        /// </summary>
        /// <param name="entityIds">ItemInTag的Id列表</param>
        /// <returns>ItemInTag的实体列表</returns>
        public IEnumerable<ItemInTag> GetItemInTags(IEnumerable<long> entityIds)
        {
            return itemInTagRepository.PopulateEntitiesByEntityIds<long>(entityIds);
        }

        /// <summary>
        /// 根据Id获取
        /// </summary>
        /// <param name="itemId">成员Id</param>
        /// <param name="tagInOwnerId">标签与拥有者关联Id</param>
        /// <returns></returns>
        public Dictionary<string, long> GetTagNamesWithIdsOfItem(long itemId)
        {
            return itemInTagRepository.GetTagNamesWithIdsOfItem(itemId, tenantTypeId);
        }

        #endregion ItemInTag

        #region RelatedTag

        /// <summary>
        /// 添加相关标签
        /// </summary>
        /// <param name="tagString">待处理的标签字符串</param>
        /// <param name="tagId">标签Id</param>
        public bool AddRelatedTagsToTag(string tagString, long tagId)
        {
            //拆分标签数组
            string[] tagNames = SplitTagString(tagString);
            for (int i = 0; i < tagNames.Length; i++)
            {
                //把空格的占位符替换回来
                tagNames[i] = string.Concat(tagNames[i].Split(URLSpecialCharacters)).Replace("[_]", " ");
            }
            return relatedTagRepository.AddRelatedTagsToTag(tagNames, tenantTypeId, tagId) > 0;
        }

        /// <summary>
        /// 清除拥有者的所有标签
        /// </summary>
        /// <param name="tagId">被关联的标签Id</param>
        public void ClearRelatedTagsFromTag(long tagId)
        {
            relatedTagRepository.ClearRelatedTagsFromTag(tagId);
        }

        /// <summary>
        /// 清除关联的标签
        /// </summary>
        /// <remarks>会删除双向的关联关系</remarks>
        /// <param name="relatedTagId">关联的标签Id</param>
        /// <param name="tagId">被关联的标签Id</param>
        public void DeleteRelatedTagFromTag(long relatedTagId, long tagId)
        {
            relatedTagRepository.DeleteRelatedTagFromTag(relatedTagId, tagId);
        }

        /// <summary>
        /// 获取相关标签
        /// </summary>
        /// <param name="tagId">被关联的标签Id</param>
        public IEnumerable<T> GetRelatedTags(long tagId)
        {
            IEnumerable<long> relatedTagIds = relatedTagRepository.GetRelatedTagIds(tagId);
            IEnumerable<T> relatedTags = tagRepository.PopulateEntitiesByEntityIds(relatedTagIds);

            return relatedTags;
        }
       /// <summary>
       /// 标签内容数减1（删除资讯时使用）
       /// </summary>
       /// <param name="tagName"></param>
        public void SetItemCount(string tagName)
        {
             tagRepository.SetItemCount(tagName);
        }
        #endregion RelatedTag


        #region 标签解析

        /// <summary>
        /// 解析内容用于创建话题
        /// </summary>
        /// <param name="body">待解析的内容</param>
        /// <param name="ownerId">标签拥有者Id</param>
        /// <param name="associateId">关联项Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        public void ResolveBodyForEdit(string body, long ownerId, long associateId, string tenantTypeId)
        {
            if (!body.Contains("#") || string.IsNullOrEmpty(body))
                return;

            Regex rg = new Regex(@"(?<=(?<!\&)(\#)(?!\d\;))[^\#@]*(?=(?<!\&)(\#)(?![0-9]+\;))", RegexOptions.Multiline | RegexOptions.Singleline);
            Match m = rg.Match(body);

            if (!m.Success)
                return;

            IList<string> tagNames = new List<string>();
            int i = 0, index = -1;

            while (m != null)
            {
                if (i % 2 == 1)
                {
                    m = m.NextMatch();
                    i++;
                    continue;
                }

                if (index == m.Index)
                    break;

                index = m.Index;

                if (!string.IsNullOrEmpty(m.Value) && !tagNames.Contains(m.Value))
                    tagNames.Add(m.Value);
                else
                    continue;

                m = m.NextMatch();
                i++;
            }

            if (tagNames.Count > 0)
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Tag());
                AddTagsToItem(tagNames.ToArray(), associateId);

                Dictionary<string, long> tagNamesWithIds = GetTagNamesWithIdsOfItem(associateId);
                if (tagNamesWithIds != null)
                {
                    foreach (KeyValuePair<string, long> pair in tagNamesWithIds)
                    {
                        countService.ChangeCount(CountTypes.Instance().ItemCounts(), pair.Value, ownerId, 1);
                    }
                }
            }
        }

        /// <summary>
        /// 解析内容中的AtUser用户展示展示
        /// </summary>
        /// <param name="body">待解析的内容</param>
        /// <param name="associateId">关联项Id</param>
        /// <param name="ownerId">标签拥有者Id</param>
        /// <param name="TagGenerate">用户生成对应标签的方法</param>
        public string ResolveBodyForDetail(string body, long associateId, long ownerId, Func<KeyValuePair<string, long>, long, string> TagGenerate)
        {
            if (string.IsNullOrEmpty(body) || !body.Contains("#") || ownerId <= 0)
                return body;

            Dictionary<string, long> tagNamesWithIds = itemInTagRepository.GetTagNamesWithIdsOfItem(associateId, tenantTypeId);

            if (tagNamesWithIds != null)
            {
                foreach (var item in tagNamesWithIds)
                {
                    body = body.Replace("#" + item.Key + "#", TagGenerate(item, ownerId));
                }
            }
            return body;
        }

        #endregion

        #region helper method

        /// <summary>
        /// 分割tagString的到标签名集合
        /// </summary>
        /// <remarks>保留引号中标签名的空格</remarks>
        /// <param name="tagString">标签名字符串</param>
        /// <returns></returns>
        private string[] SplitTagString(string tagString)
        {
            if (string.IsNullOrEmpty(tagString))
                return null;

            int count = tagString.Count(s => s == '\"');

            if (count > 1)
            {
                string[] tagArray = tagString.Split('\"');

                for (int i = (tagString.StartsWith("\"") ? 0 : 1); i < (tagString.EndsWith("\"") ? tagArray.Length : tagArray.Length - 1); i++)
                {
                    if (tagArray.Length < i + 1)
                        break;

                    if (i % 2 == 0)
                        continue;

                    tagArray[i] = tagArray[i].Replace(" ", "[_]");
                }

                tagString = String.Concat(tagArray);
            }

            return tagString.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);
        }


        #endregion helper method
    }
}
