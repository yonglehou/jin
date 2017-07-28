//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Lucene.Net.Documents;
using System.Collections.Generic;
using Tunynet.Common;
using Tunynet.Utilities;

namespace Tunynet.CMS
{
    /// <summary>
    /// 资讯索引文档
    /// </summary>
    public class CmsIndexDocument
    {
        #region 索引字段

        public static readonly string ContentItemId = "Id";
        public static readonly string ContentCategoryId = "ContentCategoryId";
        public static readonly string Subject = "Subject";
        public static readonly string Body = "Body";
        public static readonly string Summary = "Summary";
        public static readonly string Tags = "Tags";
        public static readonly string UserId = "UserId";
        public static readonly string DatePublished = "DatePublished";
        public static readonly string ApprovalStatus = "ApprovalStatus";
        public static readonly string PropertyValues = "PropertyValues";

        #endregion

        /// <summary>
        /// contentItem转换成<see cref="Lucene.Net.Documents.Document"/>
        /// </summary>
        /// <param name="contentItem">资讯实体</param>
        /// <returns>Lucene.Net.Documents.Document</returns>
        public static Document Convert(ContentItem contentItem)
        {
            Document doc = new Document();

            //索引资讯基本信息
            doc.Add(new Field(CmsIndexDocument.ContentItemId, contentItem.ContentItemId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CmsIndexDocument.Summary, contentItem.Summary ?? "", Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field(CmsIndexDocument.Subject, contentItem.Subject.ToLower(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(CmsIndexDocument.Body, HtmlUtility.StripHtml(contentItem.Body, true, false).ToLower(), Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field(CmsIndexDocument.UserId, contentItem.UserId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CmsIndexDocument.ApprovalStatus, ((int)contentItem.ApprovalStatus).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CmsIndexDocument.DatePublished, DateTools.DateToString(contentItem.DatePublished, DateTools.Resolution.MINUTE), Field.Store.YES, Field.Index.NOT_ANALYZED));

            var tags = new TagService(TenantTypeIds.Instance().ContentItem()).GetTopTagsOfItem(contentItem.ContentItemId, 100);

            foreach (var tag in tags)
            {
                doc.Add(new Field(CmsIndexDocument.Tags, tag.TagName.ToLower(), Field.Store.YES, Field.Index.ANALYZED));
            }

            return doc;
        }

        /// <summary>
        /// contentItem批量转换成<see cref="Lucene.Net.Documents.Document"/>
        /// </summary>
        /// <param name="contentItems">资讯实体</param>
        /// <returns>Lucene.Net.Documents.Document</returns>
        public static IEnumerable<Document> Convert(IEnumerable<ContentItem> contentItems)
        {
            List<Document> docs = new List<Document>();
            foreach (ContentItem contentItem in contentItems)
            {
                Document doc = Convert(contentItem);
                docs.Add(doc);
            }

            return docs;
        }
    }
}
