//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.CMS;
using Tunynet.Common;
using Tunynet.Post;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 贴子扩展
    /// </summary>
    public static class ThreadExtension
    {
        static CountService countService = new CountService(TenantTypeIds.Instance().ContentItem());
        static SpecialContentitemService specialContentitemService = DIContainer.Resolve<SpecialContentitemService>();


        /// <summary>
        /// 贴子是否特殊内容
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static bool isEssential(this Thread thread,int typeId =0 )
        {
            var specialContentTypeId = SpecialContentTypeIds.Instance().Essential();
            if (typeId > 0)
                return specialContentitemService.IsRecommend(thread.TenantTypeId, thread.ThreadId);
            return specialContentitemService.IsSpecial(thread.ThreadId, thread.TenantTypeId, specialContentTypeId);
        }

        /// <summary>
        /// 贴子是否被推荐
        /// </summary>
        public static bool IsSpecial(this Thread thread,int typeId)
        {
            var isSpecial = specialContentitemService.IsSpecial(thread.ThreadId,TenantTypeIds.Instance().Thread(), typeId);


            return isSpecial;

        }
        ///// <summary>
        ///// 是否有权限操作
        ///// </summary>
        ///// <returns></returns>
        //public static bool IsAuthorizer(this ContentItem operationType)
        //{
        //    var authorizer = new Tunynet.Spacebuilder.Authorizer();
        //    return authorizer.IsContentFoldersAdmin(operationType.ContentCategoryId);
        //}

        ///// <summary>
        ///// 资讯阅读数
        ///// </summary>
        ///// <param name="operationType"></param>
        ///// <returns></returns>
        //public static int ReadCount(this ContentItem operationType)
        //{
        //    return countService.Get(CountTypes.Instance().HitTimes(), operationType.ContentItemId);
        //}

        ///// <summary>
        ///// 资讯是否为新
        ///// </summary>
        ///// <param name="operationType"></param>
        ///// <returns></returns>
        //public static bool isNew(this ContentItem operationType)
        //{
        //    var published = Convert.ToDateTime(operationType.DatePublished.ToString("yyyy-MM-dd"));
        //    return Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).Subtract(published).Days < 3;
        //}
        ///// <summary>
        ///// 资讯是否读过
        ///// </summary>
        ///// <param name="operationType"></param>
        ///// <returns></returns>
        //public static bool isRead(this ContentItem operationType, long userId)
        //{
        //    var isReads = countService.GetOfUser(CountTypes.Instance().HitTimes(), operationType.ContentItemId, userId);
        //    return isReads > 0;
        //}

        ///// <summary>
        ///// 资讯是否置顶
        ///// </summary>
        ///// <param name="operationType"></param>
        ///// <returns></returns>
        //public static bool isSticky(this ContentItem operationType)
        //{
        //    return specialContentitemService.IsSpecial(operationType.ContentItemId, TenantTypeIds.Instance().ContentItem(), SpecialContentTypeIds.Instance().Stick());
        //}


        ///// <summary>
        ///// 获取资讯流转实例
        ///// </summary>
        ///// <param name="operationType"></param>
        ///// <returns></returns>
        //public static FlowInstance GetFlowInstance(this ContentItem operationType)
        //{
        //    var flowInstanceService = new FlowInstanceService();
        //    var flowInstance = flowInstanceService.GetFlowInstance(TenantTypeIds.Instance().ContentItem(), operationType.ContentItemId);
        //    if (flowInstance != null)
        //        return flowInstance;
        //    else
        //        return new FlowInstance();
        //    //var flowService = new FlowService();
        //    //if (flowInstance != null)
        //    //{
        //    //    title = flowService.GetFlowStep("CMSFlow", flowInstance.CurrentStepCode).StepName;
        //    //}
        //}
        ///// <summary>
        ///// 获取资讯流转实例
        ///// </summary>
        ///// <param name="operationType"></param>
        ///// <returns></returns>
        //public static string TitleName(int categoryId)
        //{
        //    if (CMSCategoryConfig.NWGK == categoryId)
        //        return "内务公开";
        //    if (CMSCategoryConfig.ZTZJ == categoryId)
        //        return "专题征集";
        //    if (CMSCategoryConfig.WTJD == categoryId)
        //        return "问题解答";
        //    if (CMSCategoryConfig.ZDJY == categoryId)
        //        return "主动建议";
        //    if (CMSCategoryConfig.TZGG == categoryId)
        //        return "通知公告";
        //    return "";

        //}
    }
}
