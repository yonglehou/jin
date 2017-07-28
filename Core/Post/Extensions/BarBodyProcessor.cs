//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Common;
using Tunynet.Utilities;
using System.Text.RegularExpressions;
using Tunynet;
using CodeKicker.BBCode;

namespace Tunynet.Post
{
    /// <summary>
    /// 贴子正文解析器
    /// </summary>
    public class BarBodyProcessor : IBodyProcessor
    {
        public string Process(string body, string tenantTypeId, long associateId, long userId)
        {
            //解析附件、视频、音频、@用户、表情
            //todo:需要封装附件、视频、音频、@用户、表情解析的辅助方法，参照表情解析完成；
            //性能优化采用敏感词的替换算法

            //解析at用户
            //AtUserService atUserService = new AtUserService(tenantTypeId);
            //body = atUserService.ResolveBodyForDetail(body, associateId, userId, AtUserTagGenerate);

            AttachmentService attachmentService = new AttachmentService(tenantTypeId);
            IEnumerable<Attachment> attachments = attachmentService.GetsByAssociateId(associateId);
            if (attachments != null && attachments.Count() > 0)
            {
                IList<BBTag> bbTags = new List<BBTag>();
                string htmlTemplate = "<div class=\"tn-annexinlaid\"><a href=\"javascript:;\" target=\"_blank\" menu=\"#attachement-artdialog-{4}\">{0}</a>（<em>{1}</em>{2}，<em>下载次数：{3}</em>）</div>";

                //解析文本中附件
                IEnumerable<Attachment> attachmentsFiles = attachments.Where(n => n.MediaType != MediaType.Image);
                foreach (var attachment in attachmentsFiles)
                {
                    bbTags.Add(AddBBTag(htmlTemplate, attachment));
                }
            }
            
            //body = new EmotionService().EmoticonTransforms(body);
            //body = new ParsedMediaService().ResolveBodyForHtmlDetail(body, ParsedMediaTagGenerate);

            return body;
        }

        #region private method

        /// <summary>
        /// 生成at用户标签
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="displayName">显示名</param>
        private string AtUserTagGenerate(string userName, string displayName)
        {
            //return string.Format("<a href=\"{1}\" target=\"_blank\" title=\"{0}\">@{0}</a> ", displayName, SiteUrls.Instance().SpaceHome(userName));
            return string.Empty;
        }

        /// <summary>
        /// 添加BBTag实体
        /// </summary>
        /// <param name="htmlTemplate">html模板</param>
        /// <param name="attachment">带替换附件</param>
        /// <returns></returns>
        private BBTag AddBBTag(string htmlTemplate, Attachment attachment)
        {

            BBAttribute bbAttribute = new BBAttribute("attachTemplate", "",
                                                      n =>
                                                      {
                                                          return string.Format(htmlTemplate,
                                                                               attachment.FriendlyFileName,
                                                                               attachment.FriendlyFileLength,
                                                                               attachment.Price > 0 ? "，<em>需要" + attachment.Price + "积分</em>" : "",
                                                                               attachment.DownloadCount,
                                                                               attachment.AttachmentId);
                                                      },
                                                      HtmlEncodingMode.UnsafeDontEncode);

            return new BBTag("attach:" + attachment.AttachmentId, "${attachTemplate}", "", false, BBTagClosingStyle.LeafElementWithoutContent, null, bbAttribute);
        }


        #endregion
    }
}