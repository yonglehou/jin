//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tunynet.Imaging;
using Tunynet.Caching;
using PetaPoco;
using Tunynet.FileStore;
using System.Net;
using Tunynet.Common;

namespace Tunynet.Common
{

    /// <summary>
    /// 附件实体
    /// </summary>
    [TableName("tn_Attachments")]
    [PrimaryKey("AttachmentId", autoIncrement = true)]
    [CacheSetting(true, PropertyNamesOfArea = "AssociateId")]
    [Serializable]
    public class Attachment : SerializablePropertiesBase, IEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Attachment()
        {
            New();
        }

        /// <summary>
        /// 初始化属性默认值
        /// </summary>
        private void New()
        {
            this.UserDisplayName = string.Empty;
            this.DateCreated = DateTime.Now;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="contentType">指定contentType，会优先采用此contentType</param>
        public Attachment(System.Web.HttpPostedFileBase postedFile, string contentType = null)
        {
            New();//初始化属性默认值
            this.FileLength = postedFile.ContentLength;
            if (!string.IsNullOrEmpty(contentType))
                this.ContentType = contentType;
            else if (!string.IsNullOrEmpty(postedFile.ContentType))
                this.ContentType = postedFile.ContentType;
            else
                this.ContentType = string.Empty;

            if (!string.IsNullOrEmpty(this.ContentType))
            {
                this.ContentType = this.ContentType.Replace("pjpeg", "jpeg");
                this.MediaType = GetMediaType(this.ContentType);
            }
            else
            {
                this.ContentType = "unknown/unknown";
                this.MediaType = MediaType.Other;
            }

           
            this.FriendlyFileName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("\\") + 1);

            //自动生成用于存储的文件名称
            this.FileName = GenerateFileName(Path.GetExtension(this.FriendlyFileName));

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="contentType">指定contentType，会优先采用此contentType</param>
        public Attachment(System.Web.HttpPostedFile postedFile, string contentType = null)
        {
            New();//初始化属性默认值
            this.FileLength = postedFile.ContentLength;

            if (!string.IsNullOrEmpty(contentType))
                this.ContentType = contentType;
            else if (!string.IsNullOrEmpty(postedFile.ContentType))
                this.ContentType = postedFile.ContentType;
            else
                this.ContentType = string.Empty;

            if (!string.IsNullOrEmpty(this.ContentType))
            {
                this.ContentType = this.ContentType.Replace("pjpeg", "jpeg");
                this.MediaType = GetMediaType(this.ContentType);
            }
            else
            {
                this.ContentType = "unknown/unknown";
                this.MediaType = MediaType.Other;
            }

            if (Path.GetExtension(postedFile.FileName) == "")
            {
                switch (this.ContentType)
                {
                    case "image/jpeg":
                        this.FileName = postedFile.FileName + ".jpg";
                        break;
                    case "image/gif":
                        this.FileName = postedFile.FileName + ".gif";
                        break;
                    case "image/png":
                        this.FileName = postedFile.FileName + ".png";
                        break;
                    default:

                        break;
                }
            }
            else
            {
                this.FileName = postedFile.FileName;
            }

            this.FriendlyFileName = this.FileName.Substring(this.FileName.LastIndexOf("\\") + 1);

            //自动生成用于存储的文件名称
            this.FileName = GenerateFileName(Path.GetExtension(this.FriendlyFileName));

            //CheckImageInfo(postedFile.InputStream);
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="httpWebResponse"></param>
        /// <param name="friendlyFileName"></param>
        public Attachment(Stream stream, string contentType, string friendlyFileName)
        {
            New();//初始化属性默认值
            this.FileLength = stream.Length;
            this.ContentType = contentType;
            this.MediaType = GetMediaType(this.ContentType);
            this.FriendlyFileName = friendlyFileName;
            this.FileName = GenerateFileName(Path.GetExtension(this.FriendlyFileName));
            //CheckImageInfo(stream);
        }

        #region 需持久化属性

        /// <summary>
        ///Id
        /// </summary>
        public long AttachmentId { get; protected set; }

        /// <summary>
        ///附件关联Id（例如：博文Id、贴子Id）
        /// </summary>
        public long AssociateId { get; set; }

        /// <summary>
        ///拥有者Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        ///租户类型Id
        /// </summary>
        public string TenantTypeId { get; set; }

        /// <summary>
        ///附件上传人UserId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///附件上传人名称
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        ///实际存储文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///文件显示名称
        /// </summary>
        public string FriendlyFileName { get; set; }

        /// <summary>
        ///附件MIME类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        ///文件大小
        /// </summary>
        public long FileLength { get; set; }

        /// <summary>
        ///售价（积分）
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        ///是否已经转换成功（可以浏览）
        /// </summary>
        public bool BrowseIsReady { get; set; }

        /// <summary>
        ///附件描述
        /// </summary>
        public string Discription { get;  set; }

      

        /// <summary>
        ///是否在文章中的附件列表显示
        /// </summary>
        public bool IsShowInAttachmentList { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get; protected set; }

        /// <summary>
        /// 附件类型（<seealso cref="Tunynet.Common.MediaType"/>）
        /// </summary>
        public MediaType MediaType { get; set; }
        /// <summary>
        ///排序
        /// </summary>
        public long DisplayOrder { get; set; }
        #endregion
        #region 计数
        /// <summary>
        /// 下载计数
        /// </summary>
        [Ignore]
        public int DownloadCount
        {
            get
            {
                CountService countService = new CountService(TenantTypeIds.Instance().Attachment());
                return countService.Get(CountTypes.Instance().DownloadCount(), this.AttachmentId);
            }
        }
        #endregion

        #region 序列化属性
        /// <summary>
        ///附件显示位置
        /// </summary>
        [Ignore]
        public int Position
        {
            get { return GetExtendedProperty<int>("Position"); }
            set { SetExtendedProperty("Position", value); }
        }


        /// <summary>
        ///图片类型附件的高度（单位:px）
        /// </summary>
        [Ignore]
        public int Width
        {
            get { return GetExtendedProperty<int>("Width"); }
            set { SetExtendedProperty("Width", value); }
        }

        /// <summary>
        /// 图片类型附件的高度（单位:px）
        /// </summary>
        [Ignore]
        public int Height
        {
            get { return GetExtendedProperty<int>("Height"); }
            set { SetExtendedProperty("Height", value); }
        }
        /// <summary>
        /// 文档类型附件页码
        /// </summary>
        [Ignore]
        public int Page
        {
            get { return GetExtendedProperty<int>("Page"); }
            set { SetExtendedProperty("Page", value); }
        }

        #endregion

        #region Help Methods

        /// <summary>
        /// 友好的附件大小信息
        /// </summary>
        public string FriendlyFileLength
        {
            get
            {
                if (this.FileLength > 0)
                {
                    if (this.FileLength > 1024 * 1024)
                        return string.Format("{0:F2}M", (this.FileLength / (1024 * 1024F)));
                    else
                        return string.Format("{0:F2}K", (this.FileLength / 1024F));
                }
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 获取文件直连Url
        /// </summary>
        /// <returns></returns>
        public string GetDirectlyUrl(string Key = null)
        {
            var url = new AttachmentService(TenantTypeId).GetDirectlyUrl(this);
            if (!string.IsNullOrEmpty(Key))
            {
                if (Path.GetExtension(url).Contains(".gif"))
                    return url;
                url =  new ImageAccessor(TenantTypeId).GetDirectlyUrl(this, Key, false);
            }
            return url;
        }


        /// <summary>
        /// 获取附件原始文件名称
        /// </summary>
        /// <returns></returns>
        public string GetOriginalFileName()
        {
            return string.Format("{0}-{1}{2}", FileName, "original", Path.GetExtension(FileName));
        }

        /// <summary>
        /// 获取附件存储的相对路径
        /// </summary>
        public virtual string GetRelativePath()
        {
            TenantFileSettings tenantAttachmentSettings = TenantFileSettings.GetRegisteredSettings(this.TenantTypeId);
            if (tenantAttachmentSettings == null)
                return string.Empty;

            IStoreProvider storeProvider = DIContainer.ResolveNamed<IStoreProvider>(tenantAttachmentSettings.StoreProviderName);
            if (storeProvider == null)
                return string.Empty;

            string[] datePaths = new string[] { tenantAttachmentSettings.FileDirectory };

            if (tenantAttachmentSettings.AutoGenerateDirectoryByDate)
                datePaths = datePaths.Concat(this.DateCreated.ToString("yyyy-MM-dd").Split('-')).ToArray();

            return storeProvider.JoinDirectory(datePaths);
        }


        /// <summary>
        /// 生成随机文件名
        /// </summary>
        /// <returns></returns>
        private string GenerateFileName(string extension)
        {
            return DateTime.Now.Ticks.ToString() + extension;
        }

        /// <summary>
        /// 生成随机文件名
        /// </summary>
        /// <returns></returns>
        public string GenerateFileName()
        {
            return GenerateFileName(Path.GetExtension(this.FriendlyFileName));
        }

        /// <summary>
        /// 依据MIME获取MediaType
        /// </summary>
        /// <param name="contentType">附件MIME类型</param>
        /// <returns></returns>
        protected MediaType GetMediaType(string contentType)
        {
            if (this.ContentType == null)
                return MediaType.Other;

            this.ContentType = this.ContentType.ToLower();

            if (this.ContentType.StartsWith("image"))
                return MediaType.Image;
            else if (this.ContentType.IndexOf("x-shockwave-flash") != -1)
                return MediaType.Flash;
            else if (this.ContentType.StartsWith("video"))
                return MediaType.Video;
            else if (this.ContentType.StartsWith("audio"))
                return MediaType.Audio;
            else if (this.ContentType.IndexOf("application/msword") != -1)
                return MediaType.Document;
            else if (this.ContentType.IndexOf("application/msexcel") != -1 || this.ContentType.IndexOf("application/vnd.ms-excel") != -1)
                return MediaType.Excel;
            else if (this.ContentType.IndexOf("application/mspowerpoint") != -1 || this.ContentType.IndexOf("application/vnd.ms-powerpoint") != -1)
                return MediaType.PPT;
            else if (this.ContentType.IndexOf("-compressed") != -1)
                return MediaType.Compressed;
            else
                return MediaType.Other;
        }

        #endregion


        #region IEntity 成员

        object IEntity.EntityId { get { return AttachmentId; } }

        bool IEntity.IsDeletedInDatabase { get; set; }


        #endregion

    }
}
