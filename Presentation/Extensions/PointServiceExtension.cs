using System;
using System.Web.Routing;

namespace Tunynet.Common
{
    /// <summary>
    /// 积分业务逻辑类扩展
    /// </summary>
    public static class PointServiceExtension
    {
        static CommentService commentService = DIContainer.Resolve<CommentService>();
        /// <summary>
        /// 资讯、贴子或评论通过审核后依据规则增减积分
        /// </summary>
        /// <param name="userId">增减积分的UserId</param>
        /// <param name="pointItemKey">积分项目标识</param>
        /// <param name="description">积分记录描述</param>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="itemId">资讯、贴子或评论Id</param>
        /// <param name="subject">资讯、贴子标题或评论内容</param>
        /// <param name="needPointMessage">是否需要积分提醒</param>
        public static void GenerateByRoles(this PointService pointService, long userId, long operatorUserId, string pointItemKey, string description, string tenantTypeId, long itemId, string subject, bool needPointMessage = false, bool isComment = false)
        {           
            if (isComment == false)
            {
                if (tenantTypeId == TenantTypeIds.Instance().Thread())
                    description = description + "：<a target=\"_blank\" class=\"a\" href=" + CachedUrlHelper.Action("ThreadDetail", "Post", null, new RouteValueDictionary { { "threadId", itemId } }) + ">" + subject + "</a>";
                if (tenantTypeId == TenantTypeIds.Instance().ContentItem())
                    description = description + "：<a target=\"_blank\" class=\"a\" href=" + SiteUrls.Instance().CMSDetail(itemId) + ">" + subject + "</a>";

            }
            else
            {

                var commentedObject = commentService.Get(itemId).GetCommentedObject();
                
                description = description + "：<a target=\"_blank\" class=\"a\" href=" + commentedObject.DetailUrl + ">" + commentedObject.Name + "</a>";

            }
            pointService.GenerateByRole(userId, operatorUserId, pointItemKey, description, needPointMessage);
        }
    }
}
