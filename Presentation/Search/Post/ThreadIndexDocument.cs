//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Lucene.Net.Documents;
using System.Collections.Generic;
using Tunynet.Utilities;

namespace Tunynet.Post
{
    /// <summary>
    /// 贴子索引文档
    /// </summary>
    public class ThreadIndexDocument
    {
        #region 索引字段

        public static readonly string ThreadId = "Id";
        public static readonly string SectionId = "SectionId";
        public static readonly string TenantTypeId = "TenantTypeId";
        public static readonly string UserId = "UserId";
        public static readonly string OwnerId = "OwnerId";
        public static readonly string Subject = "Subject";
        public static readonly string Body = "Body";
        public static readonly string DateCreated = "DateCreated";
        public static readonly string ApprovalStatus = "ApprovalStatus";

        #endregion


        /// <summary>
        /// Thread转换成<see cref="Lucene.Net.Documents.Document"/>
        /// </summary>
        /// <param name="thread">贴子实体</param>
        /// <returns></returns>
        public static Document Convert(Thread thread)
        {
            Document doc = new Document();

            //索引发帖基本信息
            doc.Add(new Field(ThreadIndexDocument.ThreadId, thread.ThreadId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(ThreadIndexDocument.SectionId, thread.SectionId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(ThreadIndexDocument.TenantTypeId, thread.TenantTypeId, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(ThreadIndexDocument.UserId, thread.UserId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(ThreadIndexDocument.OwnerId, thread.OwnerId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(ThreadIndexDocument.Subject, thread.Subject, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(ThreadIndexDocument.Body, HtmlUtility.StripHtml(thread.GetBody(), true, false).ToLower(), Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field(ThreadIndexDocument.DateCreated, DateTools.DateToString(thread.DateCreated, DateTools.Resolution.DAY), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(ThreadIndexDocument.ApprovalStatus, ((int)thread.ApprovalStatus).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            return doc;
        }

        /// <summary>
        /// Thread批量转换成<see cref="Lucene.Net.Documents.Document"/>
        /// </summary>
        /// <param name="threads">发帖实体</param>
        /// <returns>Lucene.Net.Documents.Document</returns>
        public static IEnumerable<Document> Convert(IEnumerable<Thread> threads)
        {
            List<Document> docs = new List<Document>();
            foreach (var thread in threads)
            {
                Document doc = Convert(thread);
                docs.Add(doc);
            }

            return docs;
        }
    }
}
