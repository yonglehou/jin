//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Tunynet.Common.Configuration;
using Tunynet.FileStore;
using Tunynet.Imaging;
using Tunynet.Logging;
using Tunynet.Settings;

namespace Tunynet.Common
{
    /// <summary>
    /// 图片管理
    /// </summary>
    public class ImageAccessor
    {
        public TenantFileSettings TenantFileSettings { get; private set; }


        ISettingsManager<ImageSettings> ImageSettingsManager = DIContainer.Resolve<ISettingsManager<ImageSettings>>();
        //Image文件扩展名
        private static readonly string ImageFileExtension = "jpg";
        /// <summary>
        /// 文件存储Provider
        /// </summary>
        public IStoreProvider StoreProvider = DIContainer.Resolve<IStoreProvider>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        public ImageAccessor(string tenantTypeId)
        {
            this.TenantFileSettings = TenantFileSettings.GetRegisteredSettings(tenantTypeId);
            if (this.TenantFileSettings == null)
                throw new ExceptionFacade("没有注册租户附件设置");
            this.StoreProvider = DIContainer.ResolveNamed<IStoreProvider>(this.TenantFileSettings.StoreProviderName);
        }
        #region 获取不同尺寸大小

        /// <summary>
        /// 上传Image
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="associateId"></param>
        /// <param name="associateDateCreated"></param>
        /// <returns>上传文件的相对路径（包含文件名）</returns>
        public string Save(Stream stream, object associateId, DateTime associateDateCreated)
        {
            string relativeFileName = string.Empty;
            if (stream != null)
            {
                ImageSettings ImageSettings = ImageSettingsManager.Get();

                //检查是否需要缩放原图
                Image image = Image.FromStream(stream);
                if (image.Height > this.TenantFileSettings.MaxImageHeight || image.Width > this.TenantFileSettings.MaxImageWidth)
                {
                    stream = ImageProcessor.Resize(stream, this.TenantFileSettings.MaxImageWidth, this.TenantFileSettings.MaxImageHeight, ImageSettings.ResizeMethod);
                }
                string relativePath = Get(associateDateCreated, associateId);
                string fileName = GetImageFileName(associateId);
                relativeFileName = "\\ uploads" + relativePath + "\\" + fileName;

                StoreProvider.AddOrUpdateFile(relativePath, fileName, stream);
                stream.Dispose();

                //根据不同租户类型的设置生成不同尺寸的图片，用于图片直连访问
                if (this.TenantFileSettings.ThumbnailTypes != null && this.TenantFileSettings.ThumbnailTypes.Count > 0)
                {
                    foreach (var imageSizeTypes in this.TenantFileSettings.ThumbnailTypes)
                    {
                        string sizedFileName = GetSizeImageName(fileName, imageSizeTypes.Value.Key, imageSizeTypes.Value.Value);
                        StoreProvider.DeleteFile(relativePath, sizedFileName);
                        IStoreFile file = GetResizedImage(associateDateCreated, associateId, imageSizeTypes.Key, false);
                    }
                }
            }

            return relativeFileName;
        }

        /// <summary>
        /// 删除Image
        /// </summary>
        /// <param name="associateDateCreated">Image创建时间</param>
        /// <param name="associateId"></param>
        public void Delete(DateTime associateDateCreated, object associateId)
        {
            //删除文件系统的Image
            StoreProvider.DeleteFolder(Get(associateDateCreated, associateId));
        }
        /// <summary>
        /// 获取不同尺寸的Image
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="imageSizeTypeKey"></param>
        /// <returns></returns>
        public string GetDirectlyUrl(Attachment attachment, string imageSizeTypeKey, bool autogeneration)
        {
          
            var pair = TenantFileSettings.ThumbnailTypes[imageSizeTypeKey];
            string sizedFileName = GetSizeImageName(attachment.FileName, pair.Key, pair.Value);
             return StoreProvider.GetDirectlyUrl(attachment.GetRelativePath(), sizedFileName);
          
        }

        /// <summary>
        /// 获取直连URL
        /// </summary>
        /// <param name="associateDateCreated">创建时间</param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public string GetDirectlyUrl(DateTime associateDateCreated, object associateId)
        {
            string filename = GetImageFileName(associateId);
            return StoreProvider.GetDirectlyUrl(Get(associateDateCreated, associateId), filename);
        }

        /// <summary>
        /// 获取Image
        /// </summary>
        /// <param name="associateId">关联Id</param>
        /// <returns></returns>
        public IStoreFile GetImage(DateTime associateDateCreated, object associateId)
        {
            return StoreProvider.GetFile(Get(associateDateCreated, associateId), GetImageFileName(associateId));
        }

        /// <summary>
        /// 获取不同尺寸的Image
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="imageSizeTypeKey"></param>
        /// <returns></returns>
        public IStoreFile GetResizedImage(DateTime associateDateCreated, object associateId, string imageSizeTypeKey, bool autogeneration)
        {
            IStoreFile ImageFile = null;
            if (TenantFileSettings.ThumbnailTypes == null || !TenantFileSettings.ThumbnailTypes.ContainsKey(imageSizeTypeKey))
                ImageFile = GetImage(associateDateCreated, associateId);
            else
            {
                var pair = TenantFileSettings.ThumbnailTypes[imageSizeTypeKey];
                ImageFile = GetResizedImage(Get(associateDateCreated, associateId), GetImageFileName(associateId), pair.Key, pair.Value, autogeneration);
            }
            return ImageFile;
        }
        /// <summary>
        /// 获取Image存储的相对路径
        /// </summary>
        /// <param name="associateDateCreated">图片创建时间</param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        private string Get(DateTime associateDateCreated, object associateId)
        {
            if (TenantFileSettings == null)
                return string.Empty;

            IStoreProvider storeProvider = DIContainer.ResolveNamed<IStoreProvider>(TenantFileSettings.StoreProviderName);
            if (storeProvider == null)
                return string.Empty;

            string[] datePaths = new string[] { TenantFileSettings.FileDirectory };

            if (TenantFileSettings.AutoGenerateDirectoryByDate)
                datePaths = datePaths.Concat((associateDateCreated.ToString("yyyy-MM-dd") + '-' + associateId.ToString()).Split('-')).ToArray();

            return storeProvider.JoinDirectory(datePaths);
        }

        /// <summary>
        /// 获取Image文件名称
        /// </summary>
        /// <param name="associateId">associateId</param>
        public string GetImageFileName(object associateId)
        {
            return string.Format("{0}.{1}", associateId, ImageFileExtension);
        }

        /// <summary>
        /// 获取不同尺寸大小的图片
        /// </summary>
        /// <param name="fileRelativePath">文件的相对路径</param>
        /// <param name="filename">文件名称</param>
        /// <param name="size">图片尺寸</param>
        /// <param name="resizeMethod">图像缩放方式</param>
        /// <param name="autogeneration">是否自动生成</param>
        /// <returns>若原图不存在，则会返回null，否则会返回缩放后的图片</returns>
        private IStoreFile GetResizedImage(string fileRelativePath, string filename, Size size, ResizeMethod resizeMethod, bool autogeneration)
        {

            string relativePath = fileRelativePath;

            if (filename.ToLower().EndsWith(".gif"))
            {
                return StoreProvider.GetFile(relativePath, filename); ;
            }

            string sizedFileName = GetSizeImageName(filename, size, resizeMethod);
            IStoreFile file = StoreProvider.GetFile(relativePath, sizedFileName);

            if (file == null)
            {
                IStoreFile originalFile = StoreProvider.GetFile(relativePath, filename);
                if (originalFile == null)
                {
                    return null;
                }

                using (Stream originalStream = originalFile.OpenReadStream())
                {
                    if (originalStream != null)
                    {
                        using (Stream resizedStream = ImageProcessor.Resize(originalStream, size.Width, size.Height, resizeMethod))
                        {
                            file = StoreProvider.AddOrUpdateFile(relativePath, sizedFileName, resizedStream);
                        }
                    }
                }
            }

            return file;
        }


        /// <summary>
        /// 获取图片附件的宽度和高度
        /// </summary>
        private void CheckImageInfo(Stream stream, out int width, out int height)
        {
            ImageMetadata.Check(stream, out width, out height);

        }

        /// <summary>
        /// 获取各种尺寸图片的名称
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="size">图片尺寸</param>
        /// <param name="resizeMethod">图片缩放方式</param>
        public string GetSizeImageName(string filename, Size size, ResizeMethod resizeMethod)
        {
            string resizedFileName = string.Format("{0}-{1}-{2}x{3}{4}", filename, resizeMethod != ResizeMethod.KeepAspectRatio ? resizeMethod.ToString() : string.Empty, size.Width, size.Height, Path.GetExtension(filename));

            return resizedFileName;
        }
        /// <summary>
        /// 获取附件原始文件名称
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <returns></returns>
        private string GetOriginalFileName(string FileName)
        {
            return string.Format("{0}-{1}{2}", FileName, "original", Path.GetExtension(FileName));
        }


        /// <summary>
        /// 图像处理
        /// </summary>
        /// <param name="contentStream"></param>
        /// <param name="relativepath">相对路径</param>
        /// <param name="filename">附件名称</param>
        public void Save(Stream contentStream, string relativepath, string filename, out int width, out int height)
        {
            contentStream.Position = 0;
            byte[] bytes = StreamToBytes(contentStream);

            CheckImageInfo(contentStream, out width, out height);
            //检查是否需要缩放原图
            if (TenantFileSettings.MaxImageWidth > 0 || TenantFileSettings.MaxImageHeight > 0)
            {
                int maxWidth = TenantFileSettings.MaxImageWidth > 0 ? TenantFileSettings.MaxImageWidth : width;
                int maxHeight = TenantFileSettings.MaxImageHeight > 0 ? TenantFileSettings.MaxImageHeight : height;

                if (width > maxWidth || height > maxHeight)
                {
                    Stream resizedStream = ImageProcessor.Resize(contentStream, maxWidth, maxHeight, ResizeMethod.KeepAspectRatio);
                    if (resizedStream != contentStream)
                    {
                        contentStream.Dispose();
                    }
                    contentStream = resizedStream;
                }
            }           

            ImageSettings attachmentSettings = ImageSettingsManager.Get();

            Image image = Image.FromStream(contentStream);
            bool isGif = ImageProcessor.IsGIFAnimation(image);
            //检查是否需要打水印
            if (!isGif && TenantFileSettings.EnableWatermark && attachmentSettings.WatermarkSettings.WatermarkType != WatermarkType.None && image.Width >= attachmentSettings.WatermarkSettings.WatermarkMinWidth && image.Height >= attachmentSettings.WatermarkSettings.WatermarkMinHeight)
            {
                ImageProcessor imageProcessor = new ImageProcessor();
                if (attachmentSettings.WatermarkSettings.WatermarkType == WatermarkType.Text)
                {
                    TextWatermarkFilter watermarkFilter = new TextWatermarkFilter(attachmentSettings.WatermarkSettings.WatermarkText, attachmentSettings.WatermarkSettings.WatermarkLocation, attachmentSettings.WatermarkSettings.WatermarkOpacity);
                    imageProcessor.Filters.Add(watermarkFilter);
                }
                else if (attachmentSettings.WatermarkSettings.WatermarkType == WatermarkType.Image)
                {
                    ImageWatermarkFilter watermarkFilter = new ImageWatermarkFilter(attachmentSettings.WatermarkSettings.WatermarkImagePhysicalPath, attachmentSettings.WatermarkSettings.WatermarkLocation, attachmentSettings.WatermarkSettings.WatermarkOpacity);
                    imageProcessor.Filters.Add(watermarkFilter);
                }

                contentStream = new MemoryStream(bytes);
                //给原始尺寸的图添加水印
                using (Stream watermarkImageStream = imageProcessor.Process(contentStream))
                {
                    StoreProvider.AddOrUpdateFile(relativepath, filename, watermarkImageStream);
                }

                //根据设置生成不同尺寸的图片，并添加水印
                if (TenantFileSettings.ThumbnailTypes != null && TenantFileSettings.ThumbnailTypes.Count > 0)
                {
                    foreach (var imageSizeType in TenantFileSettings.ThumbnailTypes)
                    {
                        contentStream = new MemoryStream(bytes);
                        Stream resizedStream = ImageProcessor.Resize(contentStream, imageSizeType.Value.Key.Width, imageSizeType.Value.Key.Height, imageSizeType.Value.Value);
                        image = Image.FromStream(resizedStream);
                        if (image.Width >= attachmentSettings.WatermarkSettings.WatermarkMinWidth && image.Height >= attachmentSettings.WatermarkSettings.WatermarkMinHeight)
                        {
                            using (Stream watermarkImageStream = imageProcessor.Process(resizedStream))
                            {
                                StoreProvider.AddOrUpdateFile(relativepath, GetSizeImageName(filename, imageSizeType.Value.Key, imageSizeType.Value.Value), watermarkImageStream);
                            }
                        }
                        else
                        {
                            StoreProvider.AddOrUpdateFile(relativepath, GetSizeImageName(filename, imageSizeType.Value.Key, imageSizeType.Value.Value), resizedStream);
                        }

                        if (resizedStream != contentStream)
                        {
                            resizedStream.Dispose();
                        }
                    }
                }
                contentStream = new MemoryStream(bytes);
                //如果需要添加水印，则除水印图片以外，还需要保留原图
                StoreProvider.AddOrUpdateFile(relativepath, GetOriginalFileName(filename), contentStream);
            }
            else
            {             
                if (!isGif)
                {
                    //根据设置生成不同尺寸的图片
                    if (TenantFileSettings.ThumbnailTypes != null && TenantFileSettings.ThumbnailTypes.Count > 0)
                    {
                        foreach (var imageSizeType in TenantFileSettings.ThumbnailTypes)
                        {
                            contentStream = new MemoryStream(bytes);
                            Stream resizedStream = ImageProcessor.Resize(contentStream, imageSizeType.Value.Key.Width, imageSizeType.Value.Key.Height, imageSizeType.Value.Value);
                            StoreProvider.AddOrUpdateFile(relativepath, GetSizeImageName(filename, imageSizeType.Value.Key, imageSizeType.Value.Value), resizedStream);
                            if (resizedStream != contentStream)
                            {
                                resizedStream.Dispose();
                            }
                        }
                    }
                }
                contentStream = new MemoryStream(bytes);
                StoreProvider.AddOrUpdateFile(relativepath, filename, contentStream);
            }
        }
        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        #endregion
    }
}
