//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Tunynet.Common
{
    public class SendObject
    {
        public SendObject()
        {
        }
        public bool Submit(string Url, ref string webInfo)
        {
            bool flag = true;
            try
            {
                WebRequest request = WebRequest.Create(Url);
                request.Method = "POST";

                request.GetRequestStream().Close();
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                webInfo = sr.ReadToEnd();
                sr.Close();
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        public string MD5Encrypt(string strText)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strText, "MD5").ToLower();
        }
        public string URLEncoding(string str)
        {
            return System.Web.HttpUtility.UrlEncode(str, System.Text.Encoding.GetEncoding("UTF-8"));
        }
    }
}
