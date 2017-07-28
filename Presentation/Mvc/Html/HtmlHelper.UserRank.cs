//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Text;
using System.Web.Mvc;
using Tunynet.Utilities;
using Tunynet.Common;
using Tunynet;

namespace Tunynet.Common
{
    /// <summary>
    /// 扩展对用户等级的HtmlHelper使用方法
    /// </summary>
    public static class HtmlHelperUserRankExtensions
    {
        private static int scale = 3;

        /// <summary>
        /// 根据当前数字排列的位置获取对应等级数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static int GetRankNum(int num)
        {
            if (num <= 0)
                return 1;
            return GetRankNum(num - 1) * scale;
        }

        /// <summary>
        /// 生成用户等级图标
        /// </summary>
        public static MvcHtmlString UserRankIcon(this HtmlHelper htmlHelper, int rank)
        {
            TagBuilder spanBuilder;
            TagBuilder imgBuilder;
            StringBuilder builder = new StringBuilder();
            int remainder = rank;
           
            string[] ranksImages = new string[] {
            "~/img/star3.png",
            "~/img/star2.png",
            "~/img/star1.png"
            };

            for (int k = ranksImages.Length - 1; k >= 0; k--)
            {
                int resultCount = remainder / GetRankNum(k);
                remainder = remainder % GetRankNum(k);
                for (int i = 0; i < resultCount; i++)
                {
                    spanBuilder = new TagBuilder("li");
                    imgBuilder = new TagBuilder("img");
                    imgBuilder.MergeAttribute("src", WebUtility.ResolveUrl(ranksImages[k]));
                    spanBuilder.InnerHtml += imgBuilder.ToString();
                    builder.Append(spanBuilder.ToString());
                }
                if (remainder == 0)
                    break;
            }

            UserRank userRank = DIContainer.Resolve<UserRankService>().Get(rank);
            string rankName = "";
            if (userRank != null)
                rankName = userRank.RankName;
            spanBuilder = new TagBuilder("ul");
            spanBuilder.MergeAttribute("class", "list-inline star");
            if (!string.IsNullOrEmpty(rankName))
                spanBuilder.MergeAttribute("title", rankName);
            spanBuilder.InnerHtml = builder.ToString();
            builder = new StringBuilder(spanBuilder.ToString());
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}
