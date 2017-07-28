//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.FileStore;
using System.IO;
using Tunynet.Imaging;
using Tunynet.Events;
using Tunynet.Settings;
using Tunynet.Common;
using System.Drawing;

namespace Tunynet.Common
{

    /// <summary>
    /// 附件业务逻辑类
    /// </summary>
    /// <typeparam name="T">附件实体类</typeparam>
    public class AttachmentService<T> where T : Attachment
    {
        private IUserService userService = DIContainer.Resolve<IUserService>();
        private IAttachmentRepository<T> attachmentRepository = DIContainer.Resolve<IAttachmentRepository<T>>();
        private ISettingsManager<FileSettings> attachmentSettingsManager = DIContainer.Resolve<ISettingsManager<FileSettings>>();
        /// <summary>
        /// 文件存储Provider
        /// </summary>
        private IStoreProvider StoreProvider;
        /// <summary>
        /// 图片处理
        /// </summary>
        private ImageAccessor imageaccessor;
        /// <summary>
        /// 租户附件设置
        /// </summary>
        public TenantFileSettings TenantAttachmentSettings;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        public AttachmentService(string tenantTypeId)
        {
            this.imageaccessor = new ImageAccessor(tenantTypeId);
            this.TenantAttachmentSettings = TenantFileSettings.GetRegisteredSettings(tenantTypeId);
            if (this.TenantAttachmentSettings == null)
                throw new ExceptionFacade("没有注册租户附件设置");

            this.StoreProvider = DIContainer.ResolveNamed<IStoreProvider>(this.TenantAttachmentSettings.StoreProviderName);
        }


        #region Create & Delete

        /// <summary>
        /// 创建附件
        /// </summary>
        /// <param name="attachment">附件</param>
        /// <param name="contentStream">文件流</param>
        public void Create(T attachment, Stream contentStream)
        {
            if (contentStream == null)
            {
                return;
            }

            if (attachment.MediaType == MediaType.Image)
            {
                int width;
                int height;

                imageaccessor.Save(contentStream, attachment.GetRelativePath(), attachment.FileName, out width, out height);
                attachment.Width = width;
                attachment.Height = height;
            }
            else
            {
                StoreProvider.AddOrUpdateFile(attachment.GetRelativePath(), attachment.FileName, contentStream);
            }

            if (contentStream != null)
            {
                contentStream.Dispose();
            }

            EventBus<T>.Instance().OnBefore(attachment, new CommonEventArgs(EventOperationType.Instance().Create()));
            attachmentRepository.Insert(attachment);
            attachment.DisplayOrder = attachment.AttachmentId;
            attachmentRepository.Update(attachment);

            EventBus<T>.Instance().OnAfter(attachment, new CommonEventArgs(EventOperationType.Instance().Create()));
        }

        /// <summary>
        /// 附件重新命名（修改FriendlyFileName）
        /// </summary>
        /// <param name="attachmentId">附件Id</param>
        /// <param name="newFriendlyFileName">新附件名</param>
        public void RenameFriendlyFileName(long attachmentId, string newFriendlyFileName)
        {
            T attachment = Get(attachmentId);
            if (attachment != null)
            {
                attachment.FriendlyFileName = newFriendlyFileName;
                attachmentRepository.Update(attachment);
            }
        }

        /// <summary>
        /// 附件重新调整售价（修改Price）
        /// </summary>
        /// <param name="attachmentId">附件Id</param>
        /// <param name="price">新售价</param>
        public void UpdatePrice(long attachmentId, int price)
        {
            T attachment = Get(attachmentId);
            if (attachment != null)
            {
                attachment.Price = price;
                attachmentRepository.Update(attachment);
            }
        }
        /// <summary>
        /// 附件更新
        /// </summary>
        /// <param name="attachmentId">附件Id</param>
        /// <param name="price">新售价</param>
        public void Update(T attachment)
        {
            attachmentRepository.Update(attachment);
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="attachmentId">附件Id</param>
        public void Delete(long attachmentId)
        {
            T attachment = Get(attachmentId);
            if (attachment != null)
            {
                Delete(attachment);
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="attachment">附件</param>
        public void Delete(T attachment)
        {
            DeleteStoredFile(attachment);

            EventBus<T>.Instance().OnBefore(attachment, new CommonEventArgs(EventOperationType.Instance().Delete()));

            attachmentRepository.Delete(attachment);

            EventBus<T>.Instance().OnAfter(attachment, new CommonEventArgs(EventOperationType.Instance().Delete()));
        }

        /// <summary>
        /// 删除AssociateId相关的附件
        /// </summary>
        /// <param name="associateId">附件关联Id（例如：博文Id、贴子Id）</param>
        public virtual void DeletesByAssociateId(long associateId)
        {
            IEnumerable<T> attachments = GetsByAssociateId(associateId);
            foreach (var attachment in attachments)
            {
                DeleteStoredFile(attachment);
            }
            string tenantTypeId = this.TenantAttachmentSettings.TenantTypeId;
            attachmentRepository.DeletesByAssociateId(tenantTypeId, associateId);
        }

        /// <summary>
        /// 删除OwnerId相关的附件
        /// </summary>
        /// <param name="ownerId">拥有者Id</param>
        public virtual void DeletesByOwnerId(long ownerId)
        {
            IEnumerable<T> attachments = GetsByOwnerId(ownerId);
            foreach (var attachment in attachments)
            {
                DeleteStoredFile(attachment);
            }
            string tenantTypeId = this.TenantAttachmentSettings.TenantTypeId;
            attachmentRepository.DeletesByOwnerId(tenantTypeId, ownerId);
        }

        /// <summary>
        /// 删除UserId相关的附件
        /// </summary>
        /// <param name="userId">上传者Id</param>
        public virtual void DeletesByUserId(long userId)
        {
            IEnumerable<T> attachments = GetsByUserId(userId);
            foreach (var attachment in attachments)
            {
                DeleteStoredFile(attachment);
            }
            string tenantTypeId = this.TenantAttachmentSettings.TenantTypeId;
            attachmentRepository.DeletesByUserId(tenantTypeId, userId);
        }

        /// <summary>
        /// 删除文件系统中的文件
        /// </summary>
        /// <param name="attachment">附件</param>
        protected void DeleteStoredFile(T attachment)
        {

            //如果属于图片附件，则还需删除生成的图片缩略图及附件原图
            if (attachment.MediaType == MediaType.Image)
            {
                //删除所有图(包括原始图、缩略图)
                StoreProvider.DeleteFiles(attachment.GetRelativePath(), attachment.FileName);
            }
            else
            {
                StoreProvider.DeleteFile(attachment.GetRelativePath(), attachment.FileName);
            }


        }

        /// <summary>
        /// 为指定用户生成指定附件的拷贝
        /// </summary>
        /// <param name="attachmentId">指定附件的id</param>
        /// <param name="currentUser">当前操作用户</param>
        /// <param name="ownerId">附件OwnerId</param>
        /// <param name="associateId">附件关联Id</param>        
        /// <returns>新附件实体</returns>
        public T CloneForUser(long attachmentId, IUser currentUser, string tenantTypeId, long? ownerId = null, long? associateId = null)
        {
            return CloneForUser(attachmentRepository.Get(attachmentId), currentUser, tenantTypeId, ownerId, associateId);
        }
        
        /// <summary>
        /// 为指定用户生成指定附件的拷贝
        /// </summary>
        /// <param name="attachmentId">指定附件的id</param>
        /// <param name="currentUser">当前操作用户</param>
        /// <param name="ownerId">附件OwnerId</param>
        /// <param name="associateId">附件关联Id</param>    
        /// <returns>新附件实体</returns>
        public T CloneForUser(T attachment, IUser currentUser, string tenantTypeId, long? ownerId = null, long? associateId = null)
        {
            //复制数据库记录
            T newAttachment = (T)new Attachment();
            newAttachment.AssociateId = associateId.HasValue ? associateId.Value : attachment.AssociateId;
            newAttachment.TenantTypeId = string.IsNullOrEmpty(tenantTypeId) ? attachment.TenantTypeId : tenantTypeId;
            newAttachment.ContentType = attachment.ContentType;
            newAttachment.FileLength = attachment.FileLength;
            newAttachment.FriendlyFileName = attachment.FriendlyFileName;
            newAttachment.Height = attachment.Height;
            newAttachment.MediaType = attachment.MediaType;
            newAttachment.OwnerId = ownerId ?? 0;
            newAttachment.UserDisplayName = currentUser.DisplayName;
            newAttachment.UserId = currentUser.UserId;
            newAttachment.Width = attachment.Width;
            newAttachment.FileName = attachment.FileName;
            EventBus<T>.Instance().OnBefore(newAttachment, new CommonEventArgs(EventOperationType.Instance().Create()));
            attachmentRepository.Insert(newAttachment);
            EventBus<T>.Instance().OnAfter(newAttachment, new CommonEventArgs(EventOperationType.Instance().Create()));

            TenantFileSettings tenantAttachmentSettings = TenantFileSettings.GetRegisteredSettings(tenantTypeId);
            if (tenantAttachmentSettings == null)
                return null;
            IStoreProvider storeProvider = DIContainer.ResolveNamed<IStoreProvider>(tenantAttachmentSettings.StoreProviderName);
            if (storeProvider == null)
                return null;

            string[] datePaths = new string[] { tenantAttachmentSettings.FileDirectory };

            if (tenantAttachmentSettings.AutoGenerateDirectoryByDate)
                datePaths = datePaths.Concat(DateTime.Now.ToString("yyyy-MM-dd").Split('-')).ToArray();

            var relativePath = storeProvider.JoinDirectory(datePaths);

            //复制文件
            Stream stream = null;
            try
            {
                IStoreFile file = StoreProvider.GetFile(attachment.GetRelativePath(), attachment.FileName);
                using (stream = file.OpenReadStream())
                {
                    if (attachment.MediaType == MediaType.Image)
                    {
                        //重新把流文件 写入到 MemoryStream中 进行重新裁剪并且上传
                        MemoryStream msStream = new MemoryStream();
                        byte[] inData = new byte[4096];
                        int bytesRead = stream.Read(inData, 0, inData.Length);
                        while (bytesRead > 0)
                        {
                            msStream.Write(inData, 0, bytesRead);
                            bytesRead = stream.Read(inData, 0, inData.Length);
                        }
                        int width;
                        int height;
                        imageaccessor.Save(msStream, relativePath, attachment.FileName, out width, out height);
                        msStream.Dispose();
                    }
                    else
                    {
                        StoreProvider.AddOrUpdateFile(relativePath, attachment.FileName, stream);
                    }
                }
            }
            catch
            {
            }

            return newAttachment;
        }

        #endregion


        #region Get & Gets

        /// <summary>
        /// 依据attachmentId获取附件
        /// </summary>
        /// <param name="attachmentId">附件Id</param>
        public T Get(long attachmentId)
        {
            return attachmentRepository.Get(attachmentId);
        }

        /// <summary>
        /// 依据AssociateId获取单个附件（用于AssociateId与附件一对一关系）
        /// </summary>
        /// <param name="associateId">附件关联Id</param>
        /// <returns>附件</returns>
        public T GetByAssociateId(long associateId)
        {
            IEnumerable<T> attachments = GetsByAssociateId(associateId);
            if (attachments == null)
                return null;

            //by weanglei, 附件按排序字段获取第一个附件
            return attachments.OrderByDescending(n => n.DisplayOrder).FirstOrDefault<T>();
        }

        /// <summary>
        /// 依据AssociateId获取附件列表（用于AssociateId与附件一对多关系）
        /// </summary>
        /// <param name="associateId">附件关联Id</param>
        /// <returns>附件列表</returns>
        public IEnumerable<T> GetsByAssociateId(long associateId)
        {
            string tenantTypeId = this.TenantAttachmentSettings.TenantTypeId;
            return attachmentRepository.GetsByAssociateId(tenantTypeId, associateId);
        }

        /// <summary>
        /// 依据userId获取附件列表（用于userId与附件一对多关系）
        /// </summary>
        /// <param name="userId">附件上传人Id</param>
        /// <returns>附件列表</returns>
        public IEnumerable<T> GetsByUserId(long userId)
        {
            string tenantTypeId = this.TenantAttachmentSettings.TenantTypeId;
            return attachmentRepository.GetsByUserId(tenantTypeId, userId);
        }

        /// <summary>
        /// 获取拥有者的所有附件或者拥有者一种租户类型的附件
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <returns>附件列表</returns>
        public IEnumerable<T> GetsByOwnerId(long ownerId, string tenantTypeId = null)
        {
            return attachmentRepository.Gets(ownerId, tenantTypeId);
        }

        /// <summary>
        /// 搜索附件并分页显示
        /// </summary>
        /// <param name="tenantTypeId">附件租户类型</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns>附件分页列表</returns>
        public PagingDataSet<T> Gets(string tenantTypeId, string keyword, int pageIndex)
        {
            return attachmentRepository.Gets(tenantTypeId, keyword, pageIndex);
        }


        /// <summary>
        /// 获取直连URL
        /// </summary>
        /// <param name="attachment">附件</param>
        /// <returns>返回可以http直连该附件的url</returns>
        public string GetDirectlyUrl(T attachment)
        {
            return StoreProvider.GetDirectlyUrl(attachment.GetRelativePath(), attachment.FileName);
        }

        #endregion


        #region 临时附件

        /// <summary>
        /// 获取拥有者一种租户类型的临时附件
        /// </summary>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        public IEnumerable<T> GetTemporaryAttachments(long ownerId, string tenantTypeId)
        {
            return attachmentRepository.GetTemporaryAttachments(ownerId, tenantTypeId);
        }

        /// <summary>
        /// 删除拥有者的临时附件
        /// </summary>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="tenantTypeId">租户类型Id</param>
        public void DeleteTemporaryAttachments(long ownerId, string tenantTypeId)
        {
            IEnumerable<T> attachments = GetTemporaryAttachments(ownerId, tenantTypeId);

            if (attachments.Count() == 0)
                return;

            foreach (var attachment in attachments)
            {
                Delete(attachment.AttachmentId);
            }

        }

        /// <summary>
        /// 删除过期的垃圾临时附件
        /// </summary>
        public void DeleteTrashTemporaryAttachments()
        {
            if (attachmentSettingsManager == null)
                return;

            FileSettings attachmentSettings = attachmentSettingsManager.Get();
            int temporaryAttachmentStorageDay = attachmentSettings.TemporaryAttachmentStorageDay;
            if (temporaryAttachmentStorageDay < 1)
                temporaryAttachmentStorageDay = 1;

            IEnumerable<T> attachments = attachmentRepository.GetTrashTemporaryAttachments(temporaryAttachmentStorageDay);
            if (attachments.Count() == 0)
                return;

            foreach (var attachment in attachments)
            {
                DeleteStoredFile(attachment);
            }
            attachmentRepository.DeleteTrashTemporaryAttachments(temporaryAttachmentStorageDay);
        }

        //todo: by mazq, 2017-03-25, @zhangzh 调用的地方不对 @mazq 已全部改到事件处理当中
        /// <summary>
        /// 把临时附件转成正常附件
        /// </summary>
        /// <param name="tenantTypeId">租户类型Id</param>
        /// <param name="ownerId">拥有者Id</param>
        /// <param name="associateId">附件关联Id</param>
        /// <param name="attachmentIds">待转换的附件Id</param>
        public void ToggleTemporaryAttachments(long ownerId, string tenantTypeId, long associateId, IEnumerable<long> attachmentIds = null)
        {
            attachmentRepository.ToggleTemporaryAttachments(ownerId, tenantTypeId, associateId, attachmentIds);

            //将多余的临时附件删除
            DeleteTemporaryAttachments(ownerId, tenantTypeId);
        }

      

        #endregion

    }
}
