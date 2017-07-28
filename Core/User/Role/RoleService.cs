//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tunynet.Repositories;
using Tunynet.Common.Repositories;
using Tunynet.Events;
using System.IO;

namespace Tunynet.Common
{
    /// <summary>
    /// 用户角色业务逻辑类
    /// </summary>
    public class RoleService
    {

        private Repository<Role> roleRepository = new Repository<Role>();
        private IUserInRoleRepository userInRoleRepository;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="roleRepository">Role仓储</param>
        /// <param name="userInRoleRepository"><see cref="IUserInRoleRepository"/></param>
        public RoleService( IUserInRoleRepository userInRoleRepository)
        {
            this.userInRoleRepository = userInRoleRepository;
        }

        #region Role

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"><see cref="Role"/>要添加的角色</param>
        /// <param name="stream">输入流</param>
        public bool Create(Role role)
        {
            if (!roleRepository.Exists(role.RoleId))
            {
                EventBus<Role>.Instance().OnBefore(role, new CommonEventArgs(EventOperationType.Instance().Create()));
                roleRepository.Insert(role);
                EventBus<Role>.Instance().OnAfter(role, new CommonEventArgs(EventOperationType.Instance().Create()));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role"><see cref="Role"/>要更新的角色</param>
        /// <param name="stream">输入流</param>
        public  void Update(Role role)
        {
            if (roleRepository.Exists(role.RoleId))
            {
                EventBus<Role>.Instance().OnBefore(role, new CommonEventArgs(EventOperationType.Instance().Update()));
                //暂时停止
                //role.RoleImage = ImageService.UploadImage(role.RoleName, stream);
                roleRepository.Update(role);
                EventBus<Role>.Instance().OnAfter(role, new CommonEventArgs(EventOperationType.Instance().Update()));
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        public   void Delete(long roleId)
        {
            Role role = Get(roleId);
            EventBus<Role>.Instance().OnBefore(role, new CommonEventArgs(EventOperationType.Instance().Delete()));
            roleRepository.DeleteByEntityId(roleId);
            userInRoleRepository.RemoveRoles(roleId);
            EventBus<Role>.Instance().OnAfter(role, new CommonEventArgs(EventOperationType.Instance().Delete()));

        }

        /// <summary>
        /// 获取Role
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns><see cref="Role"/></returns>
        public Role Get(long roleId)
        {
            return roleRepository.Get(roleId);
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <remarks>
        /// 按是否内置及角色名称排序
        /// </remarks>
        /// <returns>符合查询条件的Role集合</returns>
        public IEnumerable<Role> GetRoles()
        {
            return roleRepository.GetAll("IsBuiltIn desc,RoleId");
        }

        /// <summary>
        /// 根据条件获取Role
        /// </summary>
        /// <param name="connectToUser">是否可关联到用户</param>
        /// <returns>符合查询条件的Role集合</returns>
        public IEnumerable<Role> GetRoles(bool? connectToUser)
        {
            return GetRoles().Where(n => (connectToUser.HasValue ? n.ConnectToUser == connectToUser.Value : true));
        }

        /// <summary>
        /// 根据角色Id组装角色实体
        /// </summary>
        /// <param name="roleNames"></param>
        /// <returns>Role集合</returns>
        public IEnumerable<Role> GetRoles(IEnumerable<long> roleId)
        {
            return roleRepository.PopulateEntitiesByEntityIds(roleId);
        }

        #endregion


        #region UsersInRoles

        /// <summary>
        /// 把用户加入到一组角色中
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleIds">角色id集合</param>
        public void AddUserToRoles(long userId, List<long> roleIds)
        {
            if (roleIds == null)
                return;

            IEnumerable<long> oldRoleNames = GetRoleIdsOfUser(userId);
            bool nameIsChange = false;
   
            //以下为nameIsChange赋值的代码为郑伟添加
            nameIsChange = roleIds.Except(oldRoleNames).Count() > 0;

            if (nameIsChange)
            {
                userInRoleRepository.AddUserToRoles(userId, roleIds);
                List<UserInRole> newUsersInRoles = new List<UserInRole>();
                foreach (var r in roleIds)
                {
                    newUsersInRoles.Add(new UserInRole() { UserId = userId, RoleId = r });
                }
                EventBus<UserInRole>.Instance().OnBatchAfter(newUsersInRoles, new CommonEventArgs(EventOperationType.Instance().Update()));
            }
        }

        /// <summary>
        /// 给用户添加角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleId">角色名称</param>
        public void AddUserToRole(long userId, long roleId)
        {
            Role role = Get(roleId);
            if (role != null)
            {
                if (!role.ConnectToUser)
                    return;

                UserInRole userInRole = new UserInRole()
                {
                    UserId = userId,
                    RoleId = role.RoleId
                };

                userInRoleRepository.Insert(userInRole);

                EventBus<UserInRole>.Instance().OnAfter(userInRole, new CommonEventArgs(EventOperationType.Instance().Create()));
            }
        }

        /// <summary>
        /// 移除用户的一个角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleId">角色名称</param>
        public void RemoveUserFromRole(long userId, long roleId)
        {
            Role role = Get(roleId);
            if (role != null)
            {
                UserInRole userInRole = new UserInRole()
                {
                    UserId = userId,
                    RoleId = role.RoleId
                };

                userInRoleRepository.Delete(userId, roleId);

                EventBus<UserInRole>.Instance().OnAfter(userInRole, new CommonEventArgs(EventOperationType.Instance().Delete()));
            }
        }

        /// <summary>
        /// 移除用户的所有角色
        /// </summary>
        /// <remarks>
        /// 删除用户时使用
        /// </remarks>
        public void RemoveUserRoles(long userId)
        {
            userInRoleRepository.RemoveUserRoles(userId);
        }
        
        /// <summary>
        /// 获取用户的角色Id 
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="onlyPublic">是否仅获取对外公开的角色</param>
        /// <returns>返回用户的所有角色，如果该用户没有用户角色返回空集合</returns>
        public IEnumerable<long> GetRoleIdsOfUser(long userId, bool onlyPublic = false)
        {
            return userInRoleRepository.GetRoleIdsOfUser(userId, onlyPublic);
        }
        /// <summary>
        /// 获取用户的角色名称
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="onlyPublic">是否仅获取对外公开的角色</param>
        /// <returns>返回用户的所有角色，如果该用户没有用户角色返回空集合</returns>
        public IEnumerable<string> GetRoleNamesOfUser(long userId, bool onlyPublic = false)
        {
            return userInRoleRepository.GetRoleNamesOfUser(userId, onlyPublic);
        }

        /// <summary>
        /// 判断UserId是否至少拥有roleNames的一个用户角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleIds">用户角色集合</param>
        /// <returns></returns>
        public bool IsUserInRoles(long userId, params long[] roleIds)
        {
            IEnumerable<long> userRoleIds = GetRoleIdsOfUser(userId);
            return userRoleIds.Any(r => roleIds.Contains(r));
        }


        #endregion


    }
}
