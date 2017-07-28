//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Tunynet.Common;

namespace Spacebuilder.Setup
{
    /// <summary>
    /// 数据库实体类
    /// </summary>
    public class DataBaseInfoModel
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        private DBType dbType = DBType.SqlServer;
        [Display(Name = "数据库服务器类型")]
        [Required(ErrorMessage = "请选择数据库服务器")]
        public DBType DBType
        {
            get
            {
                return dbType;
            }
            set
            {
                dbType = value;
            }
        }
        /// <summary>
        /// 数据库服务器
        /// </summary>
        private string server = "localhost";
        [Display(Name = "数据库服务器")]
        [Required(ErrorMessage = "请输入数据库服务器")]
        public string Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
            }
        }
        /// <summary>
        /// 实例名
        /// </summary>
        [Display(Name = "实例名")]
        public string Instance { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        [Display(Name = "端口号")]
        public string Port { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        private string database = "Spacebuilder5";
        [Display(Name = "数据库名称")]
        [Required(ErrorMessage = "请输入数据库名称")]
        public string DataBase
        {
            get
            {
                return database;
            }
            set
            {
                database = value;
            }
        }
        /// <summary>
        /// 数据库用户帐号
        /// </summary>
        private string databaseUserName = "sa";
        [Display(Name = "数据库用户帐号")]
        [Required(ErrorMessage = "请输入数据库用户帐号")]
        public string DataBaseUserName
        {
            get
            {
                return databaseUserName;
            }
            set
            {
                databaseUserName = value;
            }
        }
        /// <summary>
        /// 数据库用户密码
        /// </summary>
        [Display(Name = "数据库用户密码")]
        [Required(ErrorMessage = "请输入数据库用户密码")]
        public string DataBasePassword { get; set; }

        /// <summary>
        /// 管理员帐号
        /// </summary>
        private string administrator = "admin";
        [Display(Name = "管理员帐号")]
        [Required(ErrorMessage = "请输入管理员帐号")]
        public string Administrator
        {
            get
            {
                return administrator;
            }
            set
            {
                administrator = value;
            }
        }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码")]
        public string UserPassword { get; set; }
        /// <summary>
        /// 站点域名
        /// </summary>
        [Display(Name = "站点名称")]
        [Required(ErrorMessage = "请输入站点名称")]
        public string SiteName { get; set; } = "近乎";
        /// <summary>
        /// 是否安装示例数据
        /// </summary>
        [Display(Name = "是否安装示例数据")]
        public bool isInstallSampleData { get; set; }
    }
}
