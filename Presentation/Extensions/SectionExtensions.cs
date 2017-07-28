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
using Tunynet.Common;
using Tunynet.Post;

namespace Tunynet.Post
{
    public static class SectionExtensions
    {
        /// <summary>
        /// 获取关注贴吧的用户数
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static int GetFavoritedUserCount(this Section section)
        {
            FavoriteService favoriteService = new FavoriteService(TenantTypeIds.Instance().Bar());
            int count = favoriteService.GetFavoritedUserCount(section.SectionId);
            return count;
        }

        /// <summary>
        /// 判断是否是推荐贴吧
        /// </summary>
        /// <param name="section"></param>
        /// <param name="secionId"></param>
        /// <returns></returns>
        public static bool IsSpecial(this Section section, long secionId)
        {
            var specialContentItemService = DIContainer.Resolve<SpecialContentitemService>();
            return specialContentItemService.IsSpecial(secionId, TenantTypeIds.Instance().Bar(), SpecialContentTypeIds.Instance().Special());
        }

        /// <summary>
        /// 当前贴吧是否被当前用户关注
        /// </summary>
        /// <returns></returns>
        public static bool IsSectionFavoriteByCurrentUser(this Section section,IUser user)
        {
            if (user == null)
            {
                return false;
            }
            FavoriteService favoriteService = new FavoriteService(TenantTypeIds.Instance().Bar());

            return favoriteService.IsFavorited(section.SectionId, user.UserId);
        }

        /// <summary>
        /// 当前贴吧是否被当前用户关注(手机端)
        /// </summary>
        /// <param name="section"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsSectionFavoriteByCurrentUser(this Section section, long userId)
        {
            FavoriteService favoriteService = new FavoriteService(TenantTypeIds.Instance().Bar());

            return favoriteService.IsFavorited(section.SectionId, userId);
        }

        /// <summary>
        /// 获取贴吧logo路径
        /// </summary>
        /// <param name="attachmentid">附件ID</param>
        /// <returns></returns>
        public static string GetSectionLogo(this Section section, long attachmentId)
        {
            AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().Bar());
            return attachmentService.Get(attachmentId).GetRelativePath();
        }

    }
}
