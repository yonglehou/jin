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

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// UserInRole数据访问接口
    /// </summary>
    public interface IUserInRoleRepository : IRepository<UserInRole>
    {

        /// <summary>
        /// 把用户加入到一组角色中
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleIds">赋予用户的用户角色</param>
        void AddUserToRoles(long userId, List<long> roleIds);
        /// <summary>
        /// 获取用户的角色ID
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="onlyPublic">是否仅获取对外公开的角色</param>
        /// <returns>用户的所有角色，如果该用户没有用户角色返回空集合</returns>
        IEnumerable<long> GetRoleIdsOfUser(long userId, bool onlyPublic = false);

        /// <summary>
        /// 获取用户的角色名称
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="onlyPublic">是否仅获取对外公开的角色</param>
        /// <returns>返回用户的所有角色，如果该用户没有用户角色返回空集合</returns>
        IEnumerable<string> GetRoleNamesOfUser(long userId, bool onlyPublic = false);

        /// <summary>
        /// 移除用户的所有角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        void RemoveUserRoles(long userId);

        /// <summary>
        /// 删除用户的一个角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        void Delete(long userId, long roleId);

        /// <summary>
        /// 查询拥有管理员角色的用户Id集合
        /// </summary>
        /// <param name="administratorRoleId">管理员角色Id</param>
        /// <returns></returns>
        IEnumerable<long> GetUserIdsOfRole(long administratorRoleId);


        /// <summary>
        /// 根据角色ID除所有关联
        /// </summary>
        /// <param name="roleId">角色Id</param>
        void RemoveRoles(long roleId);
        

    }


}
