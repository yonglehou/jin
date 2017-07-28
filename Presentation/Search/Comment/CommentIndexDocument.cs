//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Lucene.Net.Documents;
using System.Collections.Generic;
using Tunynet.Utilities;

namespace Tunynet.Common
{
    /// <summary>
    /// 评论索引文档
    /// </summary>
    public class CommentIndexDocument
    {
        #region 索引字段

        public static readonly string CommentId = "Id";
        public static readonly string CommentedObjectId = "CommentedObjectId";
        public static readonly string TenantTypeId = "TenantTypeId";
        public static readonly string CommentType = "CommentType";
        public static readonly string Body = "Body";
        public static readonly string OwnerId = "OwnerId";
        public static readonly string UserId = "UserId";
        public static readonly string Author = "Author";
        public static readonly string DateCreated = "DateCreated";
        public static readonly string ApprovalStatus = "ApprovalStatus";

        #endregion

        /// <summary>
        /// comment转换成<see cref="Lucene.Net.Documents.Document"/>
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static Document Convert(Comment comment)
        {
            Document doc = new Document();
            doc.Add(new Field(CommentIndexDocument.CommentId, comment.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CommentIndexDocument.CommentedObjectId, comment.CommentedObjectId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CommentIndexDocument.TenantTypeId, comment.TenantTypeId, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CommentIndexDocument.CommentType, comment.CommentType == null ? "" : comment.CommentType, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CommentIndexDocument.OwnerId, comment.OwnerId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CommentIndexDocument.UserId, comment.UserId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CommentIndexDocument.Author, comment.Author, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(CommentIndexDocument.Body, HtmlUtility.StripHtml(comment.Body, true, false).ToLower(), Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field(CommentIndexDocument.DateCreated, DateTools.DateToString(comment.DateCreated, DateTools.Resolution.MINUTE), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(CommentIndexDocument.ApprovalStatus, ((int)comment.ApprovalStatus).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            return doc;
        }

        /// <summary>
        /// comments批量转换成<see cref="Lucene.Net.Documents.Document"/>
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        public static IEnumerable<Document> Convert(IEnumerable<Comment> comments)
        {
            List<Document> docs = new List<Document>();
            foreach (var contentItem in comments)
            {
                Document doc = Convert(contentItem);
                docs.Add(doc);
            }

            return docs;
        }
    }
}
