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
using Tunynet;
using Tunynet.Common;
using Tunynet.Common.Configuration;
using Tunynet.Events;
using Tunynet.FileStore;
using Tunynet.Imaging;
using Tunynet.Utilities;
using Tunynet.Settings;

namespace Tunynet.Common
{
    /// <summary>
    /// 扩展用户业务逻辑
    /// </summary>
    public static class UserServiceExtension
    {


        #region 头像处理

        //Avatar文件扩展名
        private static readonly string AvatarFileExtension = "jpg";

        /// <summary>
        ///  Avatar存储目录名
        /// </summary>
        public static readonly string AvatarDirectory = "Avatars";

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="postedFile">上传的二进制头像文件</param>
        public static void UploadOriginalAvatar(this IUserService userService, long userId, Stream postedFile)
        {
            if (postedFile == null)
                return;
            Image image = Image.FromStream(postedFile);

            UserProfileSettings userProfileSettings = DIContainer.Resolve<ISettingsManager<UserProfileSettings>>().Get();

            postedFile = ImageProcessor.Resize(postedFile, userProfileSettings.OriginalAvatarWidth, userProfileSettings.OriginalAvatarHeight, ResizeMethod.Crop);

            string relativePath = GetAvatarRelativePath(userId);

            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            IUserRepository userRepository = userService.GetUserRepository();
            var user = userRepository.Get(userId);
            storeProvider.AddOrUpdateFile(relativePath, GetAvatarFileName(userId, AvatarSizeType.Original), postedFile);
            postedFile.Dispose();
            //1、如果原图超过一定尺寸（可以配置宽高像素值）则原图保留前先缩小（原图如果太大，裁剪时不方便操作）再保存
        }

        /// <summary>
        /// 上传封面图
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="postedFile">上传的二进制头像文件</param>
        public static void UploadOriginalCover(this IUserService userService, long userId, Stream postedFile)
        {
            var postedFiles = postedFile;
            if (postedFile == null)
                return;
            Image image = Image.FromStream(postedFile);

            UserProfileSettings userProfileSettings = DIContainer.Resolve<ISettingsManager<UserProfileSettings>>().Get();
            string relativePath = GetAvatarRelativePath(userId);


            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            IUserRepository userRepository = userService.GetUserRepository();
            var user = userRepository.Get(userId);
            userRepository.UpdateCover(user, 1);

            postedFile = ImageProcessor.Resize(postedFile, userProfileSettings.CoverWidth, userProfileSettings.CoverHeight, ResizeMethod.Crop);
            storeProvider.AddOrUpdateFile(relativePath, GetCoverFileName(userId), postedFile);
            postedFile.Dispose();


            //裁剪手机端图片
            postedFiles = ImageProcessor.Resize(postedFiles, userProfileSettings.BigCoverWidth, userProfileSettings.BigCoverHeight, ResizeMethod.Crop);
            storeProvider.AddOrUpdateFile(relativePath, GetCoverFileName(userId), postedFiles);
            postedFiles.Dispose();
            //1、如果原图超过一定尺寸（可以配置宽高像素值）则原图保留前先缩小（原图如果太大，裁剪时不方便操作）再保存
        }

        /// <summary>
        /// 根据用户自己选择的尺寸及位置进行头像裁剪
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="srcWidth">需裁剪的宽度</param>
        /// <param name="srcHeight">需裁剪的高度</param>
        /// <param name="srcX">需裁剪的左上角点坐标</param>
        /// <param name="srcY">需裁剪的左上角点坐标</param>
        public static void CropAvatar(this IUserService userService, long userId, float srcWidth, float srcHeight, float srcX, float srcY)
        {
            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            IStoreFile iStoreFile = storeProvider.GetFile(GetAvatarRelativePath(userId), GetAvatarFileName(userId, AvatarSizeType.Original));
            if (iStoreFile == null)
                return;
            User user = GetFullUser(userService, userId);
            if (user == null)
                return;
            bool isFirst = true;
            string avatarRelativePath = GetAvatarRelativePath(userId).Replace(Path.DirectorySeparatorChar, '/');
            avatarRelativePath = avatarRelativePath.Substring(AvatarDirectory.Length + 1);
            //user.Avatar = avatarRelativePath + "/avatar_" + userId;

            IUserRepository userRepository = userService.GetUserRepository();
            userRepository.UpdateAvatar(user, user.HasAvatar);

            UserProfileSettings userProfileSettings = DIContainer.Resolve<ISettingsManager<UserProfileSettings>>().Get();

            using (Stream fileStream = iStoreFile.OpenReadStream())
            {
                Stream bigImage = ImageProcessor.Crop(fileStream, new Rectangle((int)srcX, (int)srcY, (int)srcWidth, (int)srcHeight), userProfileSettings.AvatarWidth, userProfileSettings.AvatarHeight);

                Stream smallImage = ImageProcessor.Resize(bigImage, userProfileSettings.SmallAvatarWidth, userProfileSettings.SmallAvatarHeight, ResizeMethod.KeepAspectRatio);

                storeProvider.AddOrUpdateFile(GetAvatarRelativePath(userId), GetAvatarFileNameForEdit(userId, AvatarSizeType.Big), bigImage);

                storeProvider.AddOrUpdateFile(GetAvatarRelativePath(userId), GetAvatarFileNameForEdit(userId, AvatarSizeType.Small), smallImage);

                bigImage.Dispose();
                smallImage.Dispose();
            }

            //触发用户更新头像事件
            EventBus<User, CropAvatarEventArgs>.Instance().OnAfter(user, new CropAvatarEventArgs(isFirst));

        }

        /// <summary>
        /// 等比例缩放头像(手机端使用)
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userId"></param>
        public static void ResizeAvatar(this IUserService userService, long userId, Stream postedFile)
        {
            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            IStoreFile iStoreFile = storeProvider.GetFile(GetAvatarRelativePath(userId), GetAvatarFileName(userId, AvatarSizeType.Original));
            if (iStoreFile == null)
                return;
            User user = GetFullUser(userService, userId);
            if (user == null)
                return;
            bool isFirst = true;
            string avatarRelativePath = GetAvatarRelativePath(userId).Replace(Path.DirectorySeparatorChar, '/');
            avatarRelativePath = avatarRelativePath.Substring(AvatarDirectory.Length + 1);
            //user.Avatar = avatarRelativePath + "/avatar_" + userId;

            IUserRepository userRepository = userService.GetUserRepository();
            userRepository.UpdateAvatar(user, user.HasAvatar);

            UserProfileSettings userProfileSettings = DIContainer.Resolve<ISettingsManager<UserProfileSettings>>().Get();

            using (postedFile)
            {
                Stream smallImage = ImageProcessor.Resize(postedFile, userProfileSettings.SmallAvatarWidth, userProfileSettings.SmallAvatarWidth, ResizeMethod.KeepAspectRatio);
                storeProvider.AddOrUpdateFile(GetAvatarRelativePath(userId), GetAvatarFileNameForEdit(userId, AvatarSizeType.Small), smallImage);

                smallImage.Dispose();
            }

            //触发用户更新头像事件
            EventBus<User, CropAvatarEventArgs>.Instance().OnAfter(user, new CropAvatarEventArgs(isFirst));
        }

        /// <summary>
        /// 删除用户头像
        /// </summary>
        public static void DeleteAvatar(this IUserService userService, long userId)
        {
            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();

            //删除文件系统的头像使用以下代码
            storeProvider.DeleteFolder(GetAvatarRelativePath(userId));
            IUserRepository userRepository = userService.GetUserRepository();
            var user = userRepository.Get(userId);
            user.HasAvatar = 0;
            userRepository.UpdateAvatar(user, user.HasAvatar);
        }

        /// <summary>
        /// 获取直连URL
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns></returns>
        public static string GetCoverDirectlyUrl(this IUserService userService, IUser user, bool enableClientCaching = true, AvatarSizeType avatarSizeType = AvatarSizeType.Small)
        {
            string url = string.Empty;

            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            string directlyRootUrl = storeProvider.DirectlyRootUrl;
            if (!string.IsNullOrEmpty(storeProvider.DirectlyRootUrl))
            {
                url += storeProvider.DirectlyRootUrl;
            }
            else
            {
                url += WebUtility.ResolveUrl("~/Uploads");  //本机存储时仅允许用~/Uploads/
            }

            if (user == null)
            {
                url += "/" + AvatarDirectory + "/usercover";
            }
            else
            {
                if (user.HasCover == 1)
                {
                    url += "/" + GetAvatarRelativePath(user.UserId).Replace(Path.DirectorySeparatorChar, '/') + "/cover_" + user.UserId;
                }
                else
                {
                    url += "/" + AvatarDirectory + "/usercover";
                }
            }
            switch (avatarSizeType)
            {
                case AvatarSizeType.Original:
                case AvatarSizeType.Big:
                case AvatarSizeType.Medium:
                    url += "_big." + AvatarFileExtension;
                    break;
                case AvatarSizeType.Small:
                case AvatarSizeType.Micro:
                    url += "." + AvatarFileExtension;
                    break;
                default:
                    url += "." + AvatarFileExtension;
                    break;
            }
            if (!enableClientCaching)
            {
                url += "?lq=" + DateTime.Now.Ticks;
            }
            return url;
        }

        /// <summary>
        /// 获取直连URL
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="avatarSizeType"><see cref="AvatarSizeType"/></param>
        /// <returns></returns>
        public static string GetAvatarDirectlyUrl(this IUserService userService, IUser user, AvatarSizeType avatarSizeType, bool enableClientCaching = true)
        {
            string url = string.Empty;

            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            string directlyRootUrl = storeProvider.DirectlyRootUrl;
            if (!string.IsNullOrEmpty(storeProvider.DirectlyRootUrl))
            {
                url += storeProvider.DirectlyRootUrl;
            }
            else
            {
                url += WebUtility.ResolveUrl("~/Uploads");  //本机存储时仅允许用~/Uploads/
            }

            if (user == null)
            {
                url += "/" + AvatarDirectory + "/anonymous";
            }
            else
            {
                if (user.HasAvatar == 1)
                {
                    url += "/" + GetAvatarRelativePath(user.UserId).Replace(Path.DirectorySeparatorChar, '/') + "/avatar_" + user.UserId;
                }
                else
                {
                    url += "/" + AvatarDirectory + "/anonymous";
                }
            }

            switch (avatarSizeType)
            {
                case AvatarSizeType.Original:
                    url += "_original." + AvatarFileExtension;
                    break;
                case AvatarSizeType.Big:
                case AvatarSizeType.Medium:
                    url += "_big." + AvatarFileExtension;
                    break;
                case AvatarSizeType.Small:
                case AvatarSizeType.Micro:
                    url += "." + AvatarFileExtension;
                    break;
                default:
                    url = string.Empty;
                    break;
            }
            if (!enableClientCaching)
            {
                url += "?lq=" + DateTime.Now.Ticks;
            }
            return url;
        }

        /// <summary>
        /// 获取直连URL(手机端)
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userId"></param>
        /// <param name="avatarSizeType"></param>
        /// <returns></returns>
        public static string GetAvatarDirectlyUrl(this IUserService userService, long userId, AvatarSizeType avatarSizeType = AvatarSizeType.Small)
        {
            string url = string.Empty;
            var user = userService.GetFullUser(userId);

            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            string directlyRootUrl = storeProvider.DirectlyRootUrl;
            if (!string.IsNullOrEmpty(storeProvider.DirectlyRootUrl))
            {
                url += storeProvider.DirectlyRootUrl;
            }
            else
            {
                url += WebUtility.ResolveUrl("~/Uploads");  //本机存储时仅允许用~/Uploads/
            }

            if (user == null)
            {
                url = string.Empty;
            }
            else
            {
                if (user.HasAvatar == 1)
                {
                    url += "/" + GetAvatarRelativePath(user.UserId).Replace(Path.DirectorySeparatorChar, '/') + "/avatar_" + user.UserId;

                    switch (avatarSizeType)
                    {
                        case AvatarSizeType.Original:
                            url += "_original." + AvatarFileExtension;
                            break;
                        case AvatarSizeType.Big:
                        case AvatarSizeType.Medium:
                            url += "_big." + AvatarFileExtension;
                            break;
                        case AvatarSizeType.Small:
                        case AvatarSizeType.Micro:
                            url += "." + AvatarFileExtension;
                            break;
                        default:
                            url = string.Empty;
                            break;
                    }
                }
                else
                {
                    url = string.Empty;
                }
            }

            return url;
        }

        /// <summary>
        /// 获取用户封面图
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="avatarSizeType">头像尺寸类型</param>
        /// <returns></returns>
        public static IStoreFile GetCover(this IUserService userService, long userId, AvatarSizeType avatarSizeType = AvatarSizeType.Small)
        {
            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            return storeProvider.GetFile(GetAvatarRelativePath(userId), GetCoverFileName(userId, avatarSizeType));
        }


        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="avatarSizeType">头像尺寸类型</param>
        /// <returns></returns>
        public static IStoreFile GetAvatar(this IUserService userService, long userId, AvatarSizeType avatarSizeType)
        {
            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            return storeProvider.GetFile(GetAvatarRelativePath(userId), GetAvatarFileName(userId, avatarSizeType));
        }

        /// <summary>
        /// 获取UserId头像存储的相对路径
        /// </summary>
        public static string GetAvatarRelativePath(long userId)
        {
            IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
            string idString = userId.ToString().PadLeft(15, '0');
            return storeProvider.JoinDirectory(AvatarDirectory, idString.Substring(0, 5), idString.Substring(5, 5), idString.Substring(10, 5));
        }


        /// <summary>
        /// 获取头像文件名称
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="avatarSizeType">头像尺寸类别</param>
        private static string GetAvatarFileNameForEdit(long userId, AvatarSizeType avatarSizeType)
        {
            string filename;
            switch (avatarSizeType)
            {
                case AvatarSizeType.Original:
                    filename = string.Format("avatar_{0}_original.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Big:
                    filename = string.Format("avatar_{0}_big.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Medium:
                    filename = string.Format("avatar_{0}_big.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Small:
                    filename = string.Format("avatar_{0}.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Micro:
                    filename = string.Format("avatar_{0}.{1}", userId, AvatarFileExtension);
                    break;
                default:
                    filename = string.Empty;
                    break;
            }
            return filename;
        }

        /// <summary>
        /// 获取头像文件名称
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="avatarSizeType">头像尺寸类别</param>
        private static string GetAvatarFileName(long userId, AvatarSizeType avatarSizeType)
        {
            string filename;
            switch (avatarSizeType)
            {
                case AvatarSizeType.Original:
                    filename = string.Format("avatar_{0}_original.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Big:
                    filename = string.Format("avatar_{0}_big.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Medium:
                    filename = string.Format("avatar_{0}_big.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Small:
                    filename = string.Format("avatar_{0}.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Micro:
                    filename = string.Format("avatar_{0}.{1}", userId, AvatarFileExtension);
                    break;
                default:
                    filename = string.Empty;
                    break;
            }
            return filename;
        }
        /// <summary>
        /// 获取封面图文件名称
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="avatarSizeType">头像尺寸类别</param>
        private static string GetCoverFileName(long userId, AvatarSizeType avatarSizeType = AvatarSizeType.Small)
        {
            string filename;
            switch (avatarSizeType)
            {
                case AvatarSizeType.Original:
                    filename = string.Format("cover_{0}_original.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Big:
                    filename = string.Format("cover_{0}_big.{1}", userId, AvatarFileExtension);
                    filename = string.Format("cover_{0}_big.{1}", userId, AvatarFileExtension);
                    break;
                case AvatarSizeType.Small:
                case AvatarSizeType.Micro:
                    filename = string.Format("cover_{0}.{1}", userId, AvatarFileExtension);
                    break;
                default:
                    filename = string.Empty;
                    break;
            }
            return filename;
            //return string.Format("cover_{0}.{1}", userId, AvatarFileExtension);
        }
        #endregion 头像处理


        /// <summary>
        /// 获取完整的用户实体
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userId">用户ID</param>        
        public static User GetFullUser(this IUserService userService, long userId)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            return userRepository.GetUser(userId);
        }

        /// <summary>
        /// 获取完整的用户实体
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userName">用户名</param>
        public static User GetFullUser(this IUserService userService, string userName)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            long userId = UserIdToUserNameDictionary.GetUserId(userName);
            return userRepository.GetUser(userId);
        }

        /// <summary>
        /// 获取前N个用户
        /// </summary>
        /// <param name="topNumber">获取用户数</param>
        /// <param name="sortBy">排序字段</param>
        /// <returns></returns>
        public static IEnumerable<IUser> GetTopUsers(this IUserService userService, int topNumber, SortBy_User sortBy)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            return userRepository.GetTopUsers(topNumber, sortBy);
        }

        /// <summary>
        /// 根据排序条件分页显示用户
        /// </summary>
        /// <param name="sortBy">排序条件</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录</param>
        /// <returns>根据排序条件倒排序分页显示用户</returns>
        public static PagingDataSet<User> GetPagingUsers(this IUserService userService, SortBy_User? sortBy, int pageIndex, int pageSize)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            return userRepository.GetPagingUsers(sortBy, pageIndex, pageSize);
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userQuery">查询用户条件</param>
        /// <param name="pageSize">页面显示条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public static PagingDataSet<User> GetUsers(this IUserService userService, UserQuery userQuery, int pageSize, int pageIndex)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            return userRepository.GetUsers(userQuery, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据用户Id集合组装用户集合
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public static IEnumerable<User> GetFullUsers(this IUserService userService, IEnumerable<long> userIds)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            return userRepository.PopulateEntitiesByEntityIds<long>(userIds);
        }

        /// <summary>
        /// 帐号邮箱通过验证
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userId">用户Id</param>
        public static void UserEmailVerified(this IUserService userService, long userId)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            User user = userRepository.Get(userId);
            if (user == null)
                return;
            user.IsEmailVerified = true;
            userRepository.Update(user);

            EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(EventOperationType.Instance().UserEmailVerified()));
        }

        /// <summary>
        /// 解除符合解除管制标准的用户（永久管制的用户不会自动解除管制）
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userId"></param>
        public static void NoModeratedUser(this IUserService userService, long userId)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            User user = userRepository.Get(userId);
            if (user == null)
                return;
            user.IsModerated = false;
            userRepository.Update(user);
            EventBus<User>.Instance().OnAfter(user, new CommonEventArgs(EventOperationType.Instance().AutoNoModeratedUser()));
        }

        ///// <summary>
        ///// 更换皮肤
        ///// </summary>
        ///// <param name="userService"></param>
        ///// <param name="userId">用户Id</param>
        ///// <param name="isUseCustomStyle">是否使用自定义皮肤</param>
        ///// <param name="themeAppearance">themeKey与appearanceKey用逗号关联</param>
        //public static void ChangeThemeAppearance(this IUserService userService, long userId, bool isUseCustomStyle, string themeAppearance)
        //{
        //    IUserRepository userRepository = userService.GetUserRepository();
        //    userRepository.ChangeThemeAppearance(userId, isUseCustomStyle, themeAppearance);
        //}

        /// <summary>
        /// 获取用户数据访问实例
        /// </summary>
        /// <param name="userService"></param>
        /// <returns></returns>
        private static IUserRepository GetUserRepository(this IUserService userService)
        {
            IUserRepository userRepository = DIContainer.Resolve<IUserRepository>();
            if (userRepository == null)
                userRepository = new UserRepository();
            return userRepository;
        }

        /// <summary>
        /// 根据用户状态获取用户数
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="status">用户账号状态(-1=已删除,1=已激活,0=未激活)</param>
        /// <param name="isBanned">是否封禁</param>
        /// <param name="isModerated">是否管制</param>
        public static Dictionary<UserManageableCountType, int> GetManageableCounts(this IUserService userService, UserStatus status, bool isBanned, bool isModerated)
        {
            IUserRepository userRepository = userService.GetUserRepository();
            return userRepository.GetManageableCounts(status, isBanned, isModerated);
        }



        /// <summary>
        /// 获取用户评论数计数And贴子回复数计数
        /// </summary>
        /// <param name="kvStore"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantTypeId">租户</param>
        /// <param name="isIgnoreAuditStatus">是否忽略审核,用户个人空间计数</param>
        /// <returns></returns>
        public static int GetUserCommentCount(this IUserService userService, long userId, string tenantTypeId, bool isIgnoreAuditStatus = false)
        {

            IKvStore kvStore = DIContainer.Resolve<IKvStore>();
            //待审核,需在审核,通过个数
            int pendingCount, againCount, successCount, returnCount = 0;
            if (isIgnoreAuditStatus)
                kvStore.TryGet<int>(KvKeys.Instance().UserCommentCount(userId, tenantTypeId, null), out returnCount);
            else
            {
                kvStore.TryGet<int>(KvKeys.Instance().UserCommentCount(userId, tenantTypeId, AuditStatus.Pending), out pendingCount);
                kvStore.TryGet<int>(KvKeys.Instance().UserCommentCount(userId, tenantTypeId, AuditStatus.Again), out againCount);
                kvStore.TryGet<int>(KvKeys.Instance().UserCommentCount(userId, tenantTypeId, AuditStatus.Success), out successCount);
                userService.CalculateCount(pendingCount, againCount, successCount, ref returnCount);
            }
            return returnCount;
        }

        /// <summary>
        /// 获取用户发布贴子数
        /// </summary>
        /// <param name="kvStore"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantTypeId">租户</param>
        /// <param name="isIgnoreAuditStatus">是否忽略审核,用户个人空间计数</param>
        /// <returns></returns>
        public static int GetUserThreadCount(this IUserService userService, long userId, string tenantTypeId, bool isIgnoreAuditStatus = false)
        {
            IKvStore kvStore = DIContainer.Resolve<IKvStore>();
            //待审核,需在审核,通过个数
            int pendingCount, againCount, successCount, returnCount = 0;
            if (isIgnoreAuditStatus)
                kvStore.TryGet<int>(KvKeys.Instance().UserThreadCount(userId, tenantTypeId, null), out returnCount);
            else
            {
                kvStore.TryGet<int>(KvKeys.Instance().UserThreadCount(userId, tenantTypeId, AuditStatus.Pending), out pendingCount);
                kvStore.TryGet<int>(KvKeys.Instance().UserThreadCount(userId, tenantTypeId, AuditStatus.Again), out againCount);
                kvStore.TryGet<int>(KvKeys.Instance().UserThreadCount(userId, tenantTypeId, AuditStatus.Success), out successCount);
                userService.CalculateCount(pendingCount, againCount, successCount, ref returnCount);

            }
            return returnCount;

        }
        /// <summary>
        /// 获取用户发布文章数
        /// </summary>
        /// <param name="kvStore"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="contentModelKey">模型key</param>
        /// <param name="isIgnoreAuditStatus">是否忽略审核,用户个人空间计数</param>
        /// <returns></returns>
        public static int GetUserContentItemCount(this IUserService userService, long userId, string contentModelKey, bool isIgnoreAuditStatus = false)
        {
            IKvStore kvStore = DIContainer.Resolve<IKvStore>();
            //待审核,需在审核,通过个数
            int pendingCount, againCount, successCount, returnCount = 0;
            if (isIgnoreAuditStatus)
                kvStore.TryGet<int>(KvKeys.Instance().UserContentItemCount(userId, contentModelKey, null), out returnCount);
            else
            {
                kvStore.TryGet<int>(KvKeys.Instance().UserContentItemCount(userId, contentModelKey, AuditStatus.Pending), out pendingCount);
                kvStore.TryGet<int>(KvKeys.Instance().UserContentItemCount(userId, contentModelKey, AuditStatus.Again), out againCount);
                kvStore.TryGet<int>(KvKeys.Instance().UserContentItemCount(userId, contentModelKey, AuditStatus.Success), out successCount);
                userService.CalculateCount(pendingCount, againCount, successCount, ref returnCount);

            }
            return returnCount;
        }

        /// <summary>
        /// 删除用户所有计数
        /// </summary>
        /// <param name="kvStore"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantTypeId">租户</param>
        /// <param name="isIgnoreAuditStatus">是否忽略审核,用户个人空间计数</param>
        /// <returns></returns>
        public static void DeleteUserCount(this IUserService userService, long userId)
        {
            //删除用户所有相关的kvstore里面的数据 目前包含计数
            IKvStore kvStore = DIContainer.Resolve<IKvStore>();
            var kvValues = kvStore.GetList(userId.ToString(), true);
            foreach (var item in kvValues)
            {
                kvStore.DeleteById(kvValues.Select(n => n.Id).ToArray());
            }

        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="pendingCount"></param>
        /// <param name="againCount"></param>
        /// <param name="successCount"></param>
        /// <param name="returnCount"></param>
        /// <returns></returns>
        public static void CalculateCount(this IUserService userService, int pendingCount, int againCount, int successCount, ref int returnCount)
        {
            SiteSettings siteSetting = DIContainer.Resolve<ISettingsManager<SiteSettings>>().Get();
            if (siteSetting.AuditStatus == PubliclyAuditStatus.Success)
                returnCount = successCount;
            else if (siteSetting.AuditStatus == PubliclyAuditStatus.Again_GreaterThanOrEqual)
                returnCount = againCount + successCount;
            else if (siteSetting.AuditStatus == PubliclyAuditStatus.Pending_GreaterThanOrEqual)
                returnCount = pendingCount + againCount + successCount;
        }

    }
}
