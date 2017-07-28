//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Tunynet.Common;
using System.Collections.Concurrent;
using Tunynet.Utilities;
using PetaPoco;
using System.Xml.Linq;
using System.Configuration;

namespace Spacebuilder.Setup
{
    /// <summary>
    /// 
    /// InstallController
    /// </summary>
    /// <returns></returns>
    //[Themed(PresentAreaKeysOfBuiltIn.Channel, IsApplication = true)]
    public class SetupController : Controller
    {
        /// <summary>
        /// 
        /// 安装开始
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Setup()
        {
            //string htmlWord = "../Uploads";
            //object wordFileName = "vs格式设置与导入指南.doc";
            ////在此处放置用户代码以初始化页面 
            //ApplicationClass word = new ApplicationClass();
            //Type wordType = word.GetType();
            //Documents docs = word.Documents;
            ////打开文件 
            //Type docsType = docs.GetType();
            //Document doc = (Document)docsType.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, docs, new Object[] { wordFileName, true, true });
            ////转换格式，另存为 
            //string a = doc.Comments.ToString();
            //Type docType = doc.GetType();
            //string wordSaveFileName = wordFileName.ToString();



            //string strSaveFileName = htmlWord + "\\" + Path.GetFileNameWithoutExtension(wordSaveFileName) + ".html";

            //object saveFileName = strSaveFileName;
            //docType.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, null, doc, new object[] { saveFileName, WdSaveFormat.wdFormatFilteredHTML });
            //docType.InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod, null, doc, null);
            ////退出 Word 
            //wordType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, word, null);
            return View();
        }
        /// <summary>
        /// 安装协议
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SetupProtocol()
        {
            return View();
        }
        /// <summary>
        /// 第一步环境检查
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Setup_Step1()
        {
            Dictionary<string, bool> directoryPermissions = new Dictionary<string, bool>();
            directoryPermissions["App_Data"] = CheckFolderWriteable(Server.MapPath(@"~\App_Data"));
            directoryPermissions["Uploads"] = CheckFolderWriteable(Server.MapPath(@"~\Uploads"));
            directoryPermissions["Webconfig"] = CheckWebConfig();
            ViewData["DirectoryPermissions"] = directoryPermissions;
            return View();
        }

        /// <summary>
        /// 第二步填写数据库相关信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Setup_Step2()
        {
            if (!CheckStep1())
            {
                Response.Redirect(CachedUrlHelper.Action("Setup_Step1", "Setup"));
            }
            DataBaseInfoModel model = new DataBaseInfoModel();
            ViewData["dbtype"] = GetDBTypeList();
            return View(model);
        }
        /// <summary>
        /// 第三步数据初始化
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Setup_Step3()
        {
            if (!CheckStep1())
            {
                Response.Redirect(CachedUrlHelper.Action("Setup_Step1", "Setup"));
            }
            if (ConfigurationManager.ConnectionStrings["SqlServer"] == null&& ConfigurationManager.ConnectionStrings["MySql"] == null)
            {
                Response.Redirect(CachedUrlHelper.Action("Setup_Step2", "Setup"));
            }
            DataBaseInfoModel model = new DataBaseInfoModel();
            return View(model);
        }
        /// <summary>
        /// 第四步安装成功
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Setup_Step4()
        {
            if (!CheckStep1())
            {
                Response.Redirect(CachedUrlHelper.Action("Setup_Step1", "Setup"));
            }
            if (ConfigurationManager.ConnectionStrings["SqlServer"] == null && ConfigurationManager.ConnectionStrings["MySql"] == null)
            {
                Response.Redirect(CachedUrlHelper.Action("Setup_Step2", "Setup"));
            }
            if (Session["administrator"] == null)
            {
                Response.Redirect(CachedUrlHelper.Action("Setup_Step3", "Setup"));
            }
            ViewData["path"] = WebUtility.GetPhysicalFilePath("~/Applications/Setup");
            return View();
        }

        /// <summary>
        /// 第二步-等待安装完成
        /// </summary>
        /// <remarks>主要处理数据库结构及</remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _Step2_Wait(DataBaseInfoModel model)
        {

            ConcurrentDictionary<string, string> messages = new ConcurrentDictionary<string, string>();
            DataBaseInfoModel m = model;
            string server = string.Format("{0}{1}{2}", model.Server
                                                     , !string.IsNullOrEmpty(model.Port) ? (model.DBType == DBType.MySql ? ";Port="+model.Port : "," + model.Port) : ""
                                                     , !string.IsNullOrEmpty(model.Instance) ? "\\" + model.Instance : "");
            string connectString = string.Format("server={0};uid={1};pwd={2};",
                                                 server, model.DataBaseUserName, model.DataBasePassword);

            if (model.DBType == DBType.MySql)
            {
                connectString += "Charset=utf8;";
            }

            Database db = CreateDatabase(connectString, model.DBType, ref messages);

            //尝试打开数据库链接，检查数据库是否能够链接上
            try
            {
                db.OpenSharedConnection();
                db.CloseSharedConnection();
            }
            catch (Exception e)
            {
                bool success = false;
                //如果是SQL Server数据库，则再次尝试打开SQLEXPRESS
                if (model.DBType == DBType.SqlServer && string.IsNullOrEmpty(model.Instance))
                {
                    try
                    {
                        connectString = string.Format("server={0};uid={1};pwd={2};",
                                                      model.Server + "\\SQLEXPRESS", model.DataBaseUserName, model.DataBasePassword);
                        db = CreateDatabase(connectString, model.DBType, ref messages);
                        db.OpenSharedConnection();
                        db.CloseSharedConnection();
                        success = true;
                    }
                    catch
                    {

                    }
                }
                if (!success)
                {
                    messages["数据库帐号或密码错误，无法登录数据库服务器"] = string.Empty;
                    return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                }
            }

            if (model.DBType == DBType.SqlServer)
            {
                #region SQL Server

                var val = db.FirstOrDefault<string>("select @@@@Version");

                if (string.IsNullOrEmpty(val))
                {
                    messages["要求数据库为Sql 2005及以上"] = string.Empty;
                    return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                }

                int dbVersion = Convert.ToInt32(val.Substring(21, 4));
                if (dbVersion < 2005)
                {
                    messages["要求数据库为Sql 2005及以上,当前为" + val] = string.Empty;
                    return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                }

                val = db.FirstOrDefault<string>("select 1 from master..sysdatabases where [name]=@0", model.DataBase);

                //创建空数据库
                if (string.IsNullOrEmpty(val))
                {
                    try
                    {
                        db.Execute(string.Format("create database {0}; ALTER DATABASE {0} SET RECOVERY SIMPLE; ", model.DataBase));
                    }
                    catch (Exception e)
                    {
                        messages[e.Message] = e.StackTrace;
                        return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                    }
                }
                else
                {
                    //检查当前数据库是否为本程序数据库或一个空库
                    string dbConnectString = connectString + ";database=" + model.DataBase;
                    db = CreateDatabase(dbConnectString, model.DBType, ref messages);

                    int tableCount = db.FirstOrDefault<int>("select COUNT(*) from sysobjects where xtype='U'");

                    string tableName = db.FirstOrDefault<string>("select name from sysobjects where name=tn_Settings");

                    if (tableCount > 0 && string.IsNullOrEmpty(tableName))
                    {
                        messages["当前数据库不是本程序数据库！<br>请先删除数据库或重新命名！"] = string.Empty;
                        return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                    }
                }
                #endregion
            }
            else if (model.DBType == DBType.MySql)
            {
                #region MySql
                string information_schema_ConnectString = connectString + "database=information_schema;";
                db = CreateDatabase(information_schema_ConnectString, model.DBType, ref messages);

                //检查数据库是否已创建
                string SCHEMA_NAME = db.FirstOrDefault<string>(Sql.Builder.Select("SCHEMA_NAME").From("SCHEMATA").Where("SCHEMA_NAME=@0", model.DataBase));
                if (string.IsNullOrEmpty(SCHEMA_NAME))
                {
                    //创建空数据库
                    db.Execute(string.Format("CREATE DATABASE `{0}` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci; ", model.DataBase));
                }
                else
                {
                    int tableCount = db.FirstOrDefault<int>("SELECT COUNT(*) FROM information_schema.TABLES where TABLE_SCHEMA = '@0';", model.DataBase);

                    //当前数据库不是本程序数据库或一个空库
                    string TABLE_NAME = db.FirstOrDefault<string>(Sql.Builder.Select("TABLE_NAME").From("TABLES").Where("TABLE_NAME=\"tn_SystemData\""));
                    if (tableCount > 0 && string.IsNullOrEmpty(SCHEMA_NAME))
                    {
                        messages["当前数据库不是本程序数据库！"] = string.Empty;
                        return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                    }
                }

                #endregion
            }
            //修改web.config中数据库链接字符串
            connectString += "database=" + model.DataBase;


            //安装数据库表结构

            List<string> fileList = SetupHelper.GetInstallFiles(model.DBType).Where(n => n.Contains("Schema")).ToList();
            string message = string.Empty;
            foreach (var file in fileList)
            {
                try
                {
                    db = CreateDatabase(connectString, model.DBType, ref messages);
                    SetupHelper.ExecuteInFile(db, file, out messages);
                }
                catch { }
                if (messages.Count > 0)
                {
                    WriteLogFile(messages);
                    messages["安装数据库表结构时出现错误，请查看安装日志！"] = string.Empty;
                    return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                }
            }
            SetWebConfig(connectString, model.DBType, out messages);
            return Json(new { success = true, connectString = connectString, DBType = model.DBType });
        }


        /// <summary>
        /// 数据库初始化及创建系统管理员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _Step2_Install_InitialData(DataBaseInfoModel model)
        {

            ConcurrentDictionary<string, string> messages = new ConcurrentDictionary<string, string>();
            DBType dbType = DBType.SqlServer;
            string connectString = "";
            //获取上一步设置的数据库连接字符串和数据库类型
            if (System.Configuration.ConfigurationManager.ConnectionStrings[dbType.ToString()] == null)
            {
                dbType = DBType.MySql;
                if (System.Configuration.ConfigurationManager.ConnectionStrings[dbType.ToString()] != null)
                {
                    connectString = System.Configuration.ConfigurationManager.ConnectionStrings[dbType.ToString()].ConnectionString.ToString();
                }
                else {
                    messages["请勿跨步骤操作！<br>请先进行第2步安装数据库结构！"] = "";
                    return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                }
            }
            else {
                connectString = System.Configuration.ConfigurationManager.ConnectionStrings[dbType.ToString()].ConnectionString.ToString();
            }

            //打开数据库连接
            Database db = CreateDatabase(connectString, dbType, ref messages);
            if (messages.Keys.Count > 0)
            {
                return Json(new StatusMessageData(StatusMessageType.Error, "连接字符串不对！"));
            }
            string administrator = model.Administrator;//站点管理员账号
            string userPassword = UserPasswordHelper.EncodePassword(model.UserPassword, Tunynet.Common.UserPasswordFormat.MD5);//站点管理员密码
            KeyValuePair<string, string> adminInfo = new KeyValuePair<string, string>(administrator, userPassword);
            string SiteName = model.SiteName;//站点名称
            List<string> fileList;
            //是否安装示例数据
            if (model.isInstallSampleData)
            {
                fileList = SetupHelper.GetInstallFiles(dbType).Where(n => n.Contains("InitialData") || n.Contains("SampleData")).ToList();
            }
            else {
                fileList = SetupHelper.GetInstallFiles(dbType).Where(n => n.Contains("InitialData")).ToList();
            }
            string message = string.Empty;
            foreach (var file in fileList)
            {
                try
                {
                    SetupHelper.ExecuteInFile(db, file, out messages, adminInfo, SiteName);
                }
                catch { }
                if (messages.Count > 0)
                {
                    WriteLogFile(messages);
                    messages["执行数据库初始化脚本时出现错误，请查看安装日志！"] = StatusMessageType.Error.ToString();
                    return Json(new { errorkey = messages.Keys.FirstOrDefault(), errorvalue = messages.Values.FirstOrDefault() });
                }
            }

            Session["administrator"] = administrator;
            Session["userPassword"] = model.UserPassword;
            return Json(new StatusMessageData(StatusMessageType.Success, "安装数据库表结构成功！"));
        }

        /// <summary>
        /// 安装日志
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FileResult InstallLog()
        {
            return File(GetLogFileName(), "text/plain", "install.log");
        }

        /// <summary>
        /// 获取安装日志文件名
        /// </summary>
        /// <returns></returns>
        private string GetLogFileName()
        {
            string currentDirectory = WebUtility.GetPhysicalFilePath("~/Uploads");
            return currentDirectory + "\\install.log";
        }

        /// <summary>
        /// 确保文件已被创建
        /// </summary>
        /// <param name="fileName">带路径的文件名</param>
        /// <returns></returns>
        private bool EnsureFileExist(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                return true;
            }
            else
            {
                try
                {
                    FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                    fs.Close();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 将升级信息写入升级日志中
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private bool WriteLogFile(ConcurrentDictionary<string, string> messages)
        {

            string fileName = GetLogFileName();
            if (!EnsureFileExist(fileName))
                return false;

            StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8);   //该编码类型不会改变已有文件的编码类型
            foreach (var message in messages)
            {
                sw.WriteLine(DateTime.Now.ToString() + "：" + string.Format("{0}:{1}", message.Key, message.Value));
            }
            sw.Close();
            return true;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetSite()
        {
            CheckWebConfig();
            return new EmptyResult();
        }


        #region Helper Method

        private bool CheckFolderWriteable(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Server.MapPath(path));
                return true;
            }

            try
            {
                string testFilePath = string.Format("{0}/test{1}{2}{3}{4}.txt", path, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                FileStream TestFile = System.IO.File.Create(testFilePath);
                TestFile.WriteByte(Convert.ToByte(true));
                TestFile.Close();
                System.IO.File.Delete(testFilePath);
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 检测web.config的权限
        /// </summary>
        /// <returns></returns>
        private bool CheckWebConfig()
        {
            FileInfo FileInfo = new FileInfo(Server.MapPath("~/Web.config"));
            if (!FileInfo.Exists)
                return false;

            System.Xml.XmlDocument xmldocument = new System.Xml.XmlDocument();
            xmldocument.Load(FileInfo.FullName);
            try
            {
                XmlNode moduleNode = xmldocument.SelectSingleNode("//httpModules");
                if (moduleNode.HasChildNodes)
                {
                    for (int i = 0; i < moduleNode.ChildNodes.Count; i++)
                    {
                        XmlNode node = moduleNode.ChildNodes[i];
                        if (node.Name == "add")
                        {
                            if (node.Attributes.GetNamedItem("name").Value == "SpaceBuilderModule")
                            {
                                moduleNode.RemoveChild(node);
                                break;
                            }
                        }
                    }
                }
                xmldocument.Save(FileInfo.FullName);
            }
            catch
            {
                return false;
            }

            return true;

        }

        /// <summary>
        /// 设置文件权限
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="username">需要设置权限的用户名</param>
        private bool SetAccount(string filePath, string username)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            FileSecurity fileSecurity = fileInfo.GetAccessControl();

            try
            {
                fileSecurity.AddAccessRule(new FileSystemAccessRule(username, FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置文件夹访问权限
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="userName">需要设置权限的用户名</param>
        /// <param name="rights">访问权限</param>
        /// <param name="allowOrDeny">允许拒绝访问</param>
        private bool SetFolderACL(string folderPath, string userName, FileSystemRights rights, AccessControlType allowOrDeny)
        {

            InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            return SetFolderACL(folderPath, userName, rights, allowOrDeny, inherits, PropagationFlags.None, AccessControlModification.Add);

        }

        /// <summary>
        /// 设置文件夹访问权限
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="userName">需要设置权限的用户名</param>
        /// <param name="rights">访问权限</param>
        /// <param name="allowOrDeny">允许拒绝访问</param>
        /// <param name="inherits">继承标志指定访问控制项 (ACE) 的继承语义</param>
        /// <param name="propagateToChildren">指定如何将访问面控制项 (ACE) 传播到子对象。仅当存在继承标志时，这些标志才有意义</param>
        /// <param name="addResetOrRemove">指定要执行的访问控制修改的类型。此枚举由 System.Security.AccessControl.ObjectSecurity 类及其子类的方法使用</param>
        private bool SetFolderACL(string folderPath, string userName, FileSystemRights rights, AccessControlType allowOrDeny, InheritanceFlags inherits, PropagationFlags propagateToChildren, AccessControlModification addResetOrRemove)
        {
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            DirectorySecurity dSecurity = folder.GetAccessControl(AccessControlSections.All);
            FileSystemAccessRule accRule = new FileSystemAccessRule(userName, rights, inherits, propagateToChildren, allowOrDeny);

            bool modified;
            dSecurity.ModifyAccessRule(addResetOrRemove, accRule, out modified);
            folder.SetAccessControl(dSecurity);

            return modified;
        }

        //设置web.config
        private void SetWebConfig(string connectionString, DBType dbType, out ConcurrentDictionary<string, string> messages)
        {
            messages = new ConcurrentDictionary<string, string>();
            System.IO.FileInfo FileInfo = new FileInfo(Server.MapPath("~/web.config"));

            if (!FileInfo.Exists)
            {
                messages[string.Format("文件 : {0} 不存在", Server.MapPath("~/web.config"))] = "";
            }

            XElement rootElement = XElement.Load(FileInfo.FullName);

            XElement connectionStringsElement = rootElement.Descendants("connectionStrings").FirstOrDefault();
            if (connectionStringsElement != null && connectionStringsElement.HasElements)
            {
                XElement element = connectionStringsElement.Elements("add").LastOrDefault(n => n.NodeType != XmlNodeType.Comment);
                if (element != null)
                {
                    try
                    {
                        element.Attribute("name").Value = dbType.ToString();
                        element.Attribute("connectionString").Value = connectionString;
                        element.SetAttributeValue("providerName", GetProviderName(dbType));
                    }
                    catch (Exception e)
                    {
                        messages[e.Message] = e.StackTrace;
                    }
                }
            }
            else {
                XNamespace ns = connectionStringsElement.Name.NamespaceName;
                XElement node = new XElement(ns + "add",
                    new XAttribute("name", dbType.ToString()),
                    new XAttribute("connectionString", connectionString),
                    new XAttribute("providerName", GetProviderName(dbType)));
                connectionStringsElement.Add(node);
            }

            rootElement.Save(FileInfo.FullName);
        }

        /// <summary>
        /// 从web.config中获取连接字符串
        /// </summary>
        /// <returns></returns>
        private string GetConnectionStringFromWebConfig()
        {
            string connectionString = string.Empty;
            System.IO.FileInfo FileInfo = new FileInfo(Server.MapPath("~/web.config"));

            if (!FileInfo.Exists)
                return string.Empty;

            XElement rootElement = XElement.Load(FileInfo.FullName);
            XElement connectionStringsElement = rootElement.Descendants("connectionStrings").FirstOrDefault();
            if (connectionStringsElement != null && connectionStringsElement.HasElements)
            {
                XElement element = connectionStringsElement.Elements("add").LastOrDefault(n => n.NodeType != XmlNodeType.Comment);
                if (element != null)
                {
                    try
                    {
                        connectionString = element.Attribute("connectionString").Value;
                    }
                    catch { }
                }
            }
            return connectionString;
        }

        /// <summary>
        /// 获取数据库链接提供者
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private string GetProviderName(DBType dbType)
        {
            var providerName = string.Empty;
            switch (dbType)
            {
                case DBType.MySql:
                    providerName = "MySql.Data.MySqlClient";
                    break;
                //case DBType.SqlCE:
                //    providerName = "System.Data.EntityClient";
                //    break;
                case DBType.SqlServer:
                default:
                    providerName = "System.Data.SqlClient";
                    break;
            }
            return providerName;
        }

        /// <summary>
        /// 创建数据库访问对象
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbType"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        private Database CreateDatabase(string connectionString, DBType dbType, ref ConcurrentDictionary<string, string> messages)
        {
            if (messages == null)
                messages = new ConcurrentDictionary<string, string>();

            //DbProviderFactory factory = DbProviderFactories.GetFactory(GetProviderName(dbType));
            string providerName = GetProviderName(dbType);
            try
            {
                return new Database(connectionString, providerName);
            }
            catch (Exception e)
            {
                messages[e.Message] = e.StackTrace;
                return null;
            }
        }
        /// <summary>
        /// 获取数据库类型集合
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetDBTypeList()
        {
            List<SelectListItem> dbtypeList = new List<SelectListItem>();
            foreach (DBType type in Enum.GetValues(typeof(DBType)))
            {
                dbtypeList.Add(new SelectListItem { Text = type.ToString(), Value = type.ToString() });
            }
            return dbtypeList;
        }
        /// <summary>
        /// 检查环境
        /// </summary>
        /// <returns></returns>
        private bool CheckStep1()
        {
            SystemInfo sysInfo = new SystemInfo();
            bool sussess = true;
            if (!CheckFolderWriteable(Server.MapPath(@"~\App_Data")))
            {
                return false;
            }
            if (!CheckFolderWriteable(Server.MapPath(@"~\Uploads")))
            {
                return false;
            }
            if (!CheckWebConfig())
            {
                return false;
            }

            if (Convert.ToDouble(sysInfo.Framework) < 4)
            {
                return false;
            }
            int iis = Convert.ToInt32(sysInfo.IIS.Substring(3));

            if (iis < 7)
            {
                return false;
            }

            return sussess;
        }
        #endregion
    }
}
