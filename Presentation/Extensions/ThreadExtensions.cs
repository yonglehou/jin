using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunynet.Common;

namespace Tunynet.Post
{
    public static class ThreadExtensions
    {
        /// <summary>
        /// 当前贴子是否被当前用户收藏
        /// </summary>
        /// <returns></returns>
        public static bool IsThreadFavoriteByCurrentUser(this Thread thread)
        {
            FavoriteService favoriteService = new FavoriteService(TenantTypeIds.Instance().Thread());
            if (UserContext.CurrentUser!=null)
            {
                return favoriteService.IsFavorited(thread.ThreadId, UserContext.CurrentUser.UserId);
            }

            return false;
            

        }
    }
}
