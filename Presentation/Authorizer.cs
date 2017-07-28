//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

namespace Tunynet.Common
{
    /// <summary>
    /// 权限验证服务类
    /// </summary>
    public class Authorizer
    {
        public TenantTypeService tenantTypeService = DIContainer.Resolve<TenantTypeService>();
        private static readonly object lockObject = new object();
        public IKvStore ikvstore = DIContainer.Resolve<IKvStore>();
        public IUserService userService = DIContainer.Resolve<IUserService>();
        public IAuthorizationService authorizationService = DIContainer.Resolve<IAuthorizationService>();
        public RoleService roleService = DIContainer.Resolve<RoleService>();
        public CategoryManagerService categoryManagerService = DIContainer.Resolve<CategoryManagerService>();

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        /// <returns></returns>
        public bool IsSuperAdministrator(IUser user)
        {
            return user.IsSuperAdministrator();
        }

        /// <summary>
        /// 是不是信任用户
        /// </summary>
        public bool IsTrustedUser(IUser user)
        {
            return user.IsTrustedUser();
        }

        /// <summary>
        /// 是否具有删除评论的权限
        /// </summary>        
        /// <returns></returns>
        public bool Comment_Delete(Comment comment, IUser user)
        {
            if (user == null)
                return false;
            TenantType tenantType = tenantTypeService.Get(comment.TenantTypeId);
            if (tenantType == null)
                return false;

            if (authorizationService.IsOwner(user, comment.UserId, comment.OwnerId))
                return true;
            if (authorizationService.IsTenantManager(user, comment.TenantTypeId, comment.OwnerId))
                return true;
            return false;
        }

        /// <summary>
        /// 是否具有贴子管理的权限
        /// </summary>
        /// <returns></returns>
        public bool IsPostManager(IUser user)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().Post()))
                return true;

            return false;
        }

        /// <summary>
        /// 是否具有用户管理的权限
        /// </summary>
        /// <returns></returns>
        public bool IsUserManager(IUser user)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().User()))
                return true;

            return false;
        }

        /// <summary>
        /// 是否具有公共内容管理的权限
        /// </summary>
        /// <returns></returns>
        public bool IsGlobalContentManager(IUser user)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().GlobalContent()))
                return true;

            return false;
        }

        /// <summary>
        /// 是否具有站点设置管理的权限
        /// </summary>
        /// <returns></returns>
        public bool IsSiteManager(IUser user)
        {
            if (authorizationService.Check(user, PermissionItemKeys.Instance().SiteManage()))
                return true;
            return false;
        }

        /// <summary>
        /// 是否为栏目管理员
        /// </summary>
        /// <returns></returns>
        public bool IsCategoryManager(string tenantTypeId, IUser user, long? categoryId)
        {
           
            TenantType tenantType = tenantTypeService.Get(tenantTypeId);
            if (tenantType == null)
                return false;
            if (authorizationService.Check(user, PermissionItemKeys.Instance().CMS()))
                return true;
            return categoryManagerService.IsCategoryManager(tenantTypeId, user.UserId, categoryId);
        }

        #region 暂时用不到
        ///// <summary>
        ///// 是否具有删除附件的权限
        ///// </summary>        
        ///// <returns></returns>
        //public bool Attachment_Delete(Attachment attachment)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    TenantType tenantType = tenantTypeService.Get(attachment.TenantTypeId);
        //    if (tenantType == null)
        //        return false;
        //    if (IsAdministrator(tenantType.ApplicationId))
        //        return true;
        //    if (AuthorizationService.IsOwner(currentUser, attachment.UserId))
        //        return true;
        //    if (AuthorizationService.IsTenantManager(currentUser, attachment.TenantTypeId, attachment.OwnerId))
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// 是否具有编辑附件的权限
        ///// </summary>        
        ///// <returns></returns>
        //public bool Attachment_Edit(Attachment attachment)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    TenantType tenantType = tenantTypeService.Get(attachment.TenantTypeId);
        //    if (tenantType == null)
        //        return false;
        //    if (IsAdministrator(tenantType.ApplicationId))
        //        return true;
        //    if (AuthorizationService.IsOwner(currentUser, attachment.UserId))
        //        return true;
        //    if (AuthorizationService.IsTenantManager(currentUser, attachment.TenantTypeId, attachment.OwnerId))
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// 是否拥有下载的权限
        ///// </summary>
        ///// <param name="attachment"></param>
        ///// <returns></returns>
        //public bool Attachment_Download(Attachment attachment)
        //{
        //    //处理仅允许注册用户下载
        //    //if (thread.OnlyAllowRegisteredUserDownload && currentUser == null)
        //    //    return false;

        //    //处理售价
        //    if (attachment.Price <= 0)
        //        return true;

        //    //if (DIContainer.Resolve<Authorizer>().Attachment_Edit(attachment))
        //    //    return true;
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    if (AuthorizationService.IsOwner(currentUser, attachment.UserId))
        //        return true;

        //    AttachmentDownloadService attachementDownloadService = new AttachmentDownloadService();
        //    if (UserContext.CurrentUser != null && attachementDownloadService.IsDownloaded(UserContext.CurrentUser.UserId, attachment.AttachmentId))
        //        return true;

        //    return false;
        //}

        ///// <summary>
        ///// 是否允许购买（包含已经下载过、或者不需要购买）
        ///// </summary>
        ///// <param name="attachment"></param>
        ///// <returns></returns>
        //public bool Attachment_Buy(Attachment attachment)
        //{
        //    if (Attachment_Download(attachment))
        //        return true;
        //    if (UserContext.CurrentUser != null && attachment.Price <= UserContext.CurrentUser.TradePoints)
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// 是否具有删除标签的权限
        ///// </summary>        
        ///// <returns></returns>
        //public bool Tag_Delete(Tag tag)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    TenantType tenantType = tenantTypeService.Get(tag.TenantTypeId);
        //    if (tenantType == null)
        //        return false;
        //    if (IsAdministrator(tenantType.ApplicationId))
        //        return true;
        //    if (AuthorizationService.IsTenantManager(currentUser, tag.TenantTypeId, tag.OwnerId))
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// 是否具有编辑标签的权限
        ///// </summary>        
        ///// <returns></returns>
        //public bool Tag_Edit(Tag tag)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    TenantType tenantType = tenantTypeService.Get(tag.TenantTypeId);
        //    if (tenantType == null)
        //        return false;
        //    if (IsAdministrator(tenantType.ApplicationId))
        //        return true;
        //    if (AuthorizationService.IsTenantManager(currentUser, tag.TenantTypeId, tag.OwnerId))
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// 是否具有编辑标签的权限
        ///// </summary>        
        ///// <param name="tenantTypeId">租户类型Id</param>
        ///// <param name="tenantOwnerId">租户的OwnerId</param>
        ///// <returns></returns>
        //public bool Category_Create(string tenantTypeId, long tenantOwnerId)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    TenantType tenantType = tenantTypeService.Get(tenantTypeId);
        //    if (tenantType == null)
        //        return false;
        //    if (IsAdministrator(tenantType.ApplicationId))
        //        return true;
        //    if (AuthorizationService.IsTenantManager(currentUser, tenantTypeId, tenantOwnerId))
        //        return true;
        //    return false;
        //}
        ///// <summary>
        ///// 是否具有编辑分类的权限
        ///// </summary>        
        ///// <returns></returns>
        //public bool Category_Edit(Category category)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    TenantType tenantType = tenantTypeService.Get(category.TenantTypeId);
        //    if (tenantType == null)
        //        return false;
        //    if (IsAdministrator(tenantType.ApplicationId))
        //        return true;
        //    if (AuthorizationService.IsTenantManager(currentUser, category.TenantTypeId, category.OwnerId))
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// 是否具有删除分类的权限
        ///// </summary>        
        ///// <returns></returns>
        //public bool Category_Delete(Category category)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    TenantType tenantType = tenantTypeService.Get(category.TenantTypeId);
        //    if (tenantType == null)
        //        return false;
        //    if (IsAdministrator(tenantType.ApplicationId))
        //        return true;
        //    if (AuthorizationService.IsTenantManager(currentUser, category.TenantTypeId, category.OwnerId))
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// 是否具有加关注的权限
        ///// </summary>        
        ///// <returns></returns>
        //public bool Follow(long userId)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;

        //    return new PrivacyService().Validate(userId, currentUser.UserId, PrivacyItemKeys.Instance().Follow());
        //}

        ///// <summary>
        ///// 是否具有发私信的权限
        ///// </summary>        
        ///// <returns></returns>
        //public bool Message(long userId)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;

        //    return new PrivacyService().Validate(userId, currentUser.UserId, PrivacyItemKeys.Instance().Message());
        //}

        ///// <summary>
        ///// 是否具有管理推荐内容的权限
        ///// </summary>
        ///// <returns></returns>
        //public bool RecommendItem_Manage(string tenantTypeId)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;

        //    TenantType tenantType = tenantTypeService.Get(tenantTypeId);
        //    if (tenantType == null)
        //        return false;

        //    if (IsAdministrator(tenantType.ApplicationId))
        //        return true;

        //    if (currentUser.IsContentAdministrator())
        //        return true;

        //    return false;
        //}

        ///// <summary>
        ///// 是否有删除帐号绑定信息的权限
        ///// </summary>
        ///// <param name="userId">用户id</param>
        ///// <returns>是否用户删除帐号绑定的权限</returns>
        //public bool DeleteAccountBinding(long userId)
        //{
        //    if (UserContext.CurrentUser == null)
        //        return false;

        //    if (UserContext.CurrentUser.UserId == userId)
        //        return true;

        //    if (UserContext.CurrentUser.IsSuperAdministrator())
        //        return true;

        //    return false;
        //}

        //#region 友情链接
        ///// <summary>
        ///// 是否具有管理友情链接的权限
        ///// </summary>
        ///// <param name="link">链接实体</param>
        ///// <returns></returns>
        //public bool Link_Manage(int ownerType, long ownerId)
        //{
        //    IUser currentUser = UserContext.CurrentUser;
        //    if (currentUser == null)
        //        return false;
        //    if (AuthorizationService.IsSuperAdministrator(currentUser))
        //        return true;
        //    if (ownerType == OwnerTypes.Instance().User())
        //    {
        //        if (AuthorizationService.IsOwner(currentUser, ownerId))
        //            return true;
        //    }
        //    else if (ownerType == OwnerTypes.Instance().Group())
        //    {
        //        if (AuthorizationService.IsTenantManager(currentUser, TenantTypeIds.Instance().Group(), ownerId))
        //            return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 是否具有编辑友情链接的权限
        ///// </summary>
        ///// <param name="link">链接实体</param>
        ///// <returns></returns>
        //public bool Link_Edit(LinkEntity link)
        //{
        //    return Link_Manage(link.OwnerType, link.OwnerId);
        //}

        ///// <summary>
        ///// 是否具有删除友情链接的权限
        ///// </summary>
        ///// <param name="link">链接实体</param>
        ///// <returns></returns>
        //public bool Link_Delete(LinkEntity link)
        //{
        //    return Link_Edit(link);
        //}
        //#endregion
        #endregion
    }
}
