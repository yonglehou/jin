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
using Tunynet.Caching;
using Tunynet.Events;
using Tunynet.Common.Repositories;
using System.IO;
using System.Drawing;

namespace Tunynet.Common
{
    /// <summary>
    /// 广告业务逻辑类
    /// </summary>
    public class AdvertisingService
    {
        #region 构造器

        public IAdvertisingRepository advertisingRepository;
        public IAdvertisingPositionRepository advertisingPositionRepository;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="advertisingRepository">广告仓储接口</param>
        /// <param name="advertisingPositionRepository">广告位仓储接口</param>
        public AdvertisingService(IAdvertisingRepository advertisingRepository, IAdvertisingPositionRepository advertisingPositionRepository)
        {
            this.advertisingRepository = advertisingRepository;
            this.advertisingPositionRepository = advertisingPositionRepository;
        }

        #endregion

        #region 广告

        /// <summary>
        /// 创建广告
        /// </summary>
        /// <param name="advertising">广告</param>
        /// <param name="positionIds">广告位Id集合</param>
        /// <returns></returns>
        public bool CreateAdvertising(Advertising advertising, IEnumerable<long> positionIds)
        {
            advertisingRepository.Insert(advertising);
            if (advertising.AdvertisingId > 0)
            {
                AddPositionsToAdvertising(advertising.AdvertisingId, positionIds);
                advertising.DisplayOrder = advertising.AdvertisingId;
                advertisingRepository.Update(advertising);

                EventBus<Advertising, CommonEventArgs>.Instance().OnAfter(advertising,new CommonEventArgs(EventOperationType.Instance().Create()));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 更新广告
        /// </summary>
        /// <param name="advertising">广告</param>
        /// <param name="positionIds">广告位Id集合</param>
        /// <returns></returns>
        public void UpdateAdvertising(Advertising advertising, IEnumerable<long> positionIds)
        {
            advertisingRepository.Update(advertising);
            ClearPositionsFromAdvertising(advertising.AdvertisingId);
            AddPositionsToAdvertising(advertising.AdvertisingId, positionIds);

            EventBus<Advertising, CommonEventArgs>.Instance().OnAfter(advertising, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        /// 删除广告
        /// </summary>
        /// <param name="advertisingId">广告Id</param>
        /// <returns></returns>
        public bool DeleteAdvertising(long advertisingId)
        {
            Advertising advertising = advertisingRepository.Get(advertisingId);
            int result = advertisingRepository.Delete(advertising);
            if (result > 0)
            {
                ClearPositionsFromAdvertising(advertisingId);
                //删除附件
                AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Advertising());
                attachmentService.DeletesByAssociateId(advertisingId);

                EventBus<Advertising, CommonEventArgs>.Instance().OnAfter(advertising, new CommonEventArgs(EventOperationType.Instance().Delete()));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 获取广告列表
        /// </summary>
        /// <param name="keyword">广告备注</param>
        /// <param name="positionId">广告位</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isExpired">是否过期</param>
        /// <param name="isEnable">是否启用</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        public PagingDataSet<Advertising> GetAdvertisingsForAdmin(string keyword, long? positionId, DateTime? startDate, DateTime? endDate, bool? isExpired, bool? isEnable, int pageSize = 20, int pageIndex = 1)
        {
            return advertisingRepository.GetAdvertisingsForAdmin( keyword, positionId, startDate, endDate, isExpired, isEnable, pageSize, pageIndex);
        }

       

        /// <summary>
        /// 设置广告是否启用
        /// </summary>
        /// <param name="advertisingId">广告Id</param>
        /// <param name="isEnable">是否启用</param>
        public void SetAdvertisingStatus(long advertisingId, bool isEnable)
        {
            Advertising advertising = advertisingRepository.Get(advertisingId);
            if (advertising.IsEnable != isEnable)
            {
                advertising.IsEnable = isEnable;
                advertisingRepository.Update(advertising);

                EventBus<Advertising, CommonEventArgs>.Instance().OnAfter(advertising, new CommonEventArgs(isEnable? EventOperationType.Instance().Approved(): EventOperationType.Instance().Disapproved()));
            }
        }

        /// <summary>
        /// 获取广告
        /// </summary>
        /// <param name="advertisingId">广告Id</param>
        /// <returns></returns>
        public Advertising GetAdvertising(long advertisingId)
        {
            return advertisingRepository.Get(advertisingId);
        }

        /// <summary>
        /// 获取广告统计数据
        /// </summary>
        /// <returns></returns>
        public long GetAdvertisingCount()
        {
            return advertisingRepository.GetAdvertisingCount();
        }

        /// <summary>
        /// 清除广告的所有广告位
        /// </summary>
        /// <param name="advertisingId">广告Id</param>
        private void ClearPositionsFromAdvertising(long advertisingId)
        {
            advertisingRepository.ClearPositionsFromAdvertising(advertisingId);
        }

        /// <summary>
        /// 为广告批量设置广告位
        /// </summary>
        /// <param name="advertisingId">广告Id</param>
        /// <param name="positionIds">广告位Id集合</param>
        private void AddPositionsToAdvertising(long advertisingId, IEnumerable<long> positionIds)
        {
            advertisingRepository.AddPositionsToAdvertising(advertisingId, positionIds);
        }

        /// <summary>
        /// 根据广告Id取所有的广告位
        /// </summary>
        /// <param name="advertisingId">广告Id</param>
        /// <returns></returns>
        public IEnumerable<AdvertisingPosition> GetPositionsByAdvertisingId(long advertisingId)
        {
            return advertisingRepository.GetPositionsByAdvertisingId(advertisingId);
        }
        #endregion

        #region 广告位

        /// <summary>
        /// 创建广告位
        /// </summary>
        /// <param name="position">广告位</param>
        /// <param name="stream">图片流</param>
        /// <returns></returns>
        public bool CreatePosition(AdvertisingPosition position)
        {
            advertisingPositionRepository.Insert(position);
            AdvertisingPosition advertisingPosition = advertisingPositionRepository.Get(position.PositionId);
            if (advertisingPosition != null)
            {
                EventBus<AdvertisingPosition, CommonEventArgs>.Instance().OnAfter(position, new CommonEventArgs(EventOperationType.Instance().Create()));
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 更新广告位
        /// </summary>
        /// <param name="position">广告位</param>
        /// <param name="stream">图片流</param>
        public void UpdatePosition(AdvertisingPosition position)
        {
            advertisingPositionRepository.Update(position);
            EventBus<AdvertisingPosition, CommonEventArgs>.Instance().OnAfter(position, new CommonEventArgs(EventOperationType.Instance().Update()));
        }

        /// <summary>
        /// 删除广告位
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public void DeletePosition(long positionId)
        {
            AdvertisingPosition position = advertisingPositionRepository.Get(positionId);
            advertisingPositionRepository.Delete(position);
            EventBus<AdvertisingPosition, CommonEventArgs>.Instance().OnAfter(position, new CommonEventArgs(EventOperationType.Instance().Delete()));
        }

        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <param name="height">高度</param>
        /// <param name="width">宽度</param>
        /// <param name="isEnable">是否启用</param>
        /// <returns></returns>
        public IEnumerable<AdvertisingPosition> GetPositionsForAdmin( int? height=null , int? width=null, bool? isEnable = null)
        {
            return advertisingPositionRepository.GetPositionsForAdmin( height, width, isEnable);
        }

        /// <summary>
        /// 根据广告位Id取所有的广告
        /// </summary>
        /// <param name="positionId">广告位Id</param>
        /// <param name="isEnable">是否启用（null：全部广告,true：仅启用广告,false：仅禁用广告）</param>
        /// <returns></returns>
        public IEnumerable<Advertising> GetAdvertisingsByPositionId(long positionId, bool? isEnable = true)
        {
            return advertisingPositionRepository.GetAdvertisingsByPositionId(positionId,isEnable);
        }
     

        /// <summary>
        /// 获取广告位
        /// </summary>
        /// <param name="positionId">广告位ID</param>
        /// <returns></returns>
        public AdvertisingPosition GetPosition(long positionId)
        {
            return advertisingPositionRepository.Get(positionId);
        }

        /// <summary>
        /// 获取所有的广告位尺寸
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllPositionSize()
        {
            List<string> positionSizes = new List<string>();

            IEnumerable<AdvertisingPosition> positions = advertisingPositionRepository.GetAll();
            foreach (var position in positions)
            {
                positionSizes.Add(position.Width.ToString()+"*"+position.Height.ToString());
            }

            return positionSizes.Distinct();
        }

        /// <summary>
        /// 获取广告位统计数据
        /// </summary>
        /// <returns></returns>
        public long GetPositionCount()
        {
            return advertisingPositionRepository.GetPositionCount();
        }


        /// <summary>
        /// 定期移除过期广告
        /// </summary>
        /// <returns></returns>
        public void DeleteExpiredAdvertisings()
        {
            //执行删除
            advertisingRepository.DeleteExpiredAdvertising();
        }

        #endregion


    }
}
