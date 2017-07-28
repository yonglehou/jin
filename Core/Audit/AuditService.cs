//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Tunynet.Common.Configuration;
using Tunynet.Common.Repositories;
using Tunynet.Events;
using Tunynet.Repositories;
using Tunynet.Settings;

namespace Tunynet.Common
{
    /// <summary>
    /// 审核业务逻辑类
    /// </summary>
    public class AuditService
    {
        #region 构造器
        private IRepository<AuditItem> auditItemRepository;
        private IAuditItemInUserRoleRepository auditItemInUserRoleRepository;
        private IUserService userService;
        private RoleService roleService;
     

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="auditItemRepository">AuditItem仓储</param>
        /// <param name="auditItemInUserRoleRepository">PermissionItemInUserRoleRepository仓储</param>
        public AuditService(IRepository<AuditItem> auditItemRepository, IAuditItemInUserRoleRepository auditItemInUserRoleRepository, IUserService userService, RoleService roleService)
        {
            this.auditItemRepository = auditItemRepository;
            this.auditItemInUserRoleRepository = auditItemInUserRoleRepository;
            this.userService = userService;
            this.roleService = roleService;
        }
        #endregion

        #region 审核项目

        /// <summary>
        /// 获取AuditItem
        /// </summary>
        /// <param name="itemKey">审核项标识</param>
        /// <returns></returns>
        public AuditItem GetAuditItem(string itemKey)
        {
            return auditItemRepository.Get(itemKey);
        }

        /// <summary>
        /// 获取用户角色对应的审核设置
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns>返回roleName对应的审核设置</returns>
        public IEnumerable<AuditItemInUserRole> GetAuditItemsInUserRole(long roleId)
        {
            return auditItemInUserRoleRepository.GetAuditItemsInUserRole(roleId);
        }

        /// <summary>
        /// 更新权限规则
        /// </summary>
        /// <param name="AuditItemInUserRoles">待更新的权限项目规则集合</param>
        public void UpdateAuditItemInUserRole(IEnumerable<AuditItemInUserRole> auditItemInUserRoles)
        {
            auditItemInUserRoleRepository.UpdateAuditItemInUserRole(auditItemInUserRoles);
            EventBus<AuditItemInUserRole, CommonEventArgs>.Instance().OnBatchAfter(auditItemInUserRoles, new CommonEventArgs(EventOperationType.Instance().Update()));
        }


        /// <summary>
        /// 创建实体时设置审核状态
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="auditable">可审核实体</param>
        /// <param name="isManager">是否有权限管理</param>
        public void ChangeAuditStatusForCreate(long userId, IAuditable auditable, bool isManager )
        {
            if (NeedAudit(userId, auditable, AuditStrictDegree.Create, isManager))
                auditable.ApprovalStatus = AuditStatus.Pending;
            else
                auditable.ApprovalStatus = AuditStatus.Success;
        }

        /// <summary>
        /// 更新实体时设置审核状态
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="auditable">可审核实体</param>
        /// <param name="isManager">是否有权限管理</param>
        public void ChangeAuditStatusForUpdate(long userId, IAuditable auditable, bool isManager)
        {
            if (auditable.ApprovalStatus == AuditStatus.Success)
            {
                if (NeedAudit(userId, auditable, AuditStrictDegree.Update, isManager))
                    auditable.ApprovalStatus = AuditStatus.Again;
            }
        }

        /// <summary>
        /// 判断是否需要在一定的严格程度上需要审核
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="auditable">可审核实体</param>
        /// <param name="strictDegree">审核严格程度</param>
        /// <param name="isManager">是否有权限管理</param>
        /// <returns></returns>
        private bool NeedAudit(long userId, IAuditable auditable, AuditStrictDegree strictDegree, bool isManager)
        {
            var user = userService.GetUser(userId);
            //匿名用户需要审核
            if (user == null)
                return true;

            UserSettings userSettings = DIContainer.Resolve<ISettingsManager<UserSettings>>().Get();

            //需审核
            if (auditable.ApprovalStatus == AuditStatus.Pending)
            {
                return true;
            }
            //不启用审核
            if (!userSettings.EnableAudit)
                return false;
            //判断用户是否有权限管理此模块,如果有权限直接通过审核
            if (isManager)
                return false;

            //如果用户处于免审核角色，则直接通过
            if (roleService.IsUserInRoles(userId, userSettings.NoAuditedRoleNames.ToArray()))
                return false;

            //获取用户所属的角色，并附加上注册用户角色
            IList<long> roleIdsOfUser = roleService.GetRoleIdsOfUser(userId).ToList();
            //roleIdsOfUser.Add(RoleIds.Instance().RegisteredUsers());
            if (user.IsModerated)
                roleIdsOfUser.Add(RoleIds.Instance().ModeratedUser());

            //判断每个用户角色的设置是否可用
            foreach (var roleId in roleIdsOfUser)
            {
                IEnumerable<AuditItemInUserRole> auditItemInUserRoles = GetAuditItemsInUserRole(roleId);
                foreach (var auditItemInUserRole in auditItemInUserRoles)
                {
                    if (auditItemInUserRole.ItemKey.Equals(auditable.AuditItemKey))
                    {
                        if (auditItemInUserRole.StrictDegree == AuditStrictDegree.None)
                            return false;
                        else if (auditItemInUserRole.StrictDegree == AuditStrictDegree.NotSet)
                            break;
                        else if ((int)auditItemInUserRole.StrictDegree >= (int)strictDegree)
                            return true;
                    }
                }
            }

            //如果用户处于免审核用户等级，也直接通过
            if (user.Rank >= userSettings.MinNoAuditedUserRank)
                return false;

            return true;
        }

        /// <summary>
        /// 获取所有用户审核规则
        /// </summary>
        /// <returns>返回roleName对应的权限设置</returns>
        public Dictionary<long, IEnumerable<AuditItemInUserRole>> GetAllAuditItemsInUserRole()
        {
            return auditItemInUserRoleRepository.GetAllAuditItemsInUserRole();
        }

        /// <summary>
        /// 获取所有审核规则
        /// </summary>
        public IEnumerable<AuditItem> GetAll()
        {
            return auditItemRepository.GetAll();
        }

        #endregion

        /// <summary>
        /// 解析审核状态变化前后是否会对其他数据产生正向还负向的影响（例如：是该加积分，还是减积分）
        /// </summary>
        /// <remarks>该方法仅针对于管理员通过审核或不通过审核的情况</remarks>
        /// <param name="oldAuditStatus">变化前的审核状态（若是创建操作，请赋值为null）</param>
        /// <param name="newAuditStatus">变化后的审核状态（若是删除操作，请赋值为null）</param>
        /// <returns>true-正影响，false-负影响，null-未产生影响</returns>
        public bool? ResolveAuditDirection(AuditStatus? oldAuditStatus, AuditStatus? newAuditStatus)
        {
            if (oldAuditStatus == null && newAuditStatus == AuditStatus.Success)
                return true;
            if (oldAuditStatus == AuditStatus.Pending && newAuditStatus == AuditStatus.Success)
                return true;
            if (oldAuditStatus == AuditStatus.Fail && newAuditStatus == AuditStatus.Success)
                return true;
            if (oldAuditStatus == AuditStatus.Success && newAuditStatus == AuditStatus.Fail)
                return false;
            if (oldAuditStatus == AuditStatus.Again && newAuditStatus == null)
                return false;
            if (oldAuditStatus == AuditStatus.Success && newAuditStatus == null)
                return false;
            return null;
        }

    }
}
