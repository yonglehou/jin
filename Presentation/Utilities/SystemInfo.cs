//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using Microsoft.Win32;
using PetaPoco;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Configuration;
using System.Data.Common;


namespace Tunynet.Common
{
    public class SystemInfo
    {
        //操作系统名称
        private string osName;
        /// <summary>
        /// 操作系统名称
        /// </summary>
        public string OSName
        {
            get
            {
                //如果操作系统为空则获取一下
                if (string.IsNullOrEmpty(this.osName))
                {
                    RegistryKey rk;
                    rk = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion");
                    this.osName = rk.GetValue("ProductName").ToString();
                }
                return this.osName;
            }
        }

        //操作系统版本号
        private int osVersion;
        /// <summary>
        /// 操作系统版本号
        /// </summary>
        public int OSVersion
        {
            get
            {
                if (osVersion == 0)
                {
                    RegistryKey rk;
                    rk = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion");
                    int.TryParse(rk.GetValue("CurrentBuildNumber").ToString(), out this.osVersion);
                }

                return this.osVersion;
            }
        }

        //IIS版本号
        private string iis;
        /// <summary>
        /// IIS版本号
        /// </summary>
        public string IIS
        {
            get
            {
                //如果IIS为空则获取
                if (string.IsNullOrEmpty(this.iis))
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\InetStp");
                    this.iis = "IIS" + key.GetValue("MajorVersion").ToString();
                }
                return this.iis;
            }
        }

        //framework版本号
        private string framework;
        /// <summary>
        /// framework版本号
        /// </summary>
        public string Framework
        {
            get
            {
                //如果framework版本号为空则获取
                if (string.IsNullOrEmpty(this.framework))
                {
                    Version v = Environment.Version;
                    if (v != null)
                    {
                        this.framework = v.Major + "." + v.Minor;
                    }
                }
                return this.framework;
            }
        }

        //数据库类型
        private string dataBaseVersion;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DataBaseVersion
        {

            get
            {
                //如果数据库类型为空则获取
                if (string.IsNullOrEmpty(dataBaseVersion))
                {
                    string dbType = GetDBtype();
                    var database = Database.CreateInstance();
                    string DBversion = string.Empty;
                    if (dbType.StartsWith("MySql"))
                    {
                        DBversion = database.ExecuteScalar<object>(Sql.Builder.Select("version()")).ToString();
                    }
                    else
                    {
                        DBversion = database.ExecuteScalar<object>(Sql.Builder.Select("@@@version")).ToString();
                    }
                    this.dataBaseVersion = DBversion;
                    if (!string.IsNullOrEmpty(DBversion))
                    {
                        Match match = Regex.Match(DBversion, @"^(?<DBversion>.*)-");
                        if (match.Success)
                        {
                            //获得有效字符串
                            this.dataBaseVersion = match.Groups["DBversion"].Value;
                        }
                    }
                }
                return this.dataBaseVersion;
            }
        }

        //.NET信任级别
        private string netTrustLevel;
        /// <summary>
        /// .NET信任级别
        /// </summary>
        public string NetTrustLevel
        {

            get
            {
                TrustSection t = new TrustSection();
                this.netTrustLevel = t.Level;
                return this.netTrustLevel;
            }
        }

        //数据库占用
        private string getDBSize;
        /// <summary>
        /// 数据库占用
        /// </summary>
        public string GetDBSize
        {

            get
            {
                if (string.IsNullOrEmpty(getDBSize))
                {
                    string dbType = GetDBtype();
                    var database = Database.CreateInstance();
                    if (dbType.StartsWith("MySql"))
                    {
                        long dataSize = database.First<long>(Sql.Builder.Append("SELECT sum(DATA_LENGTH)+sum(INDEX_LENGTH) FROM information_schema.TABLES where TABLE_SCHEMA=database();"));
                        this.getDBSize = (dataSize / 1048576.0) + "M";
                    }
                    else
                    {
                        dynamic databaseInfo = database.FirstOrDefault<dynamic>("execute sp_spaceused");
                        if (databaseInfo != null)
                            this.getDBSize = databaseInfo.database_size;
                    }
                }
                return this.getDBSize;
            }
        }

        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <returns></returns>
        private string GetDBtype()
        {
            var providerName = "System.Data.SqlClient";
            int connectionStringsCount = ConfigurationManager.ConnectionStrings.Count;
            if (connectionStringsCount > 0)
                providerName = ConfigurationManager.ConnectionStrings[connectionStringsCount - 1].ProviderName;
            else
                throw new InvalidOperationException("Can't find a connection string '");
            DbProviderFactory _factory = null;
            if (!string.IsNullOrEmpty(providerName))
                _factory = DbProviderFactories.GetFactory(providerName);
            if (_factory != null)
                return _factory.GetType().Name;
            return string.Empty;
        }

    }
}