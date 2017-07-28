//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Tunynet.Common;
using Tunynet.Utilities;
using System.Web.Helpers;
using System.Web;
using RestSharp;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// QQ帐号获取器
    /// </summary>
    public class QQAccountGetter : ThirdAccountGetter
    {

        private RestClient _restClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        public QQAccountGetter()
        {
            _restClient = new RestClient("https://graph.qq.com");
        }

        /// <summary>
        /// 名称
        /// </summary>
        public override string AccountTypeName
        {
            get { return "QQ帐号"; }
        }

        /// <summary>
        /// 官方网站地址
        /// </summary>
        public override string AccountTypeUrl
        {
            get { return "http://i.qq.com/"; }
        }

        /// <summary>
        /// 帐号类型Key
        /// </summary>
        public override string AccountTypeKey
        {
            get { return AccountTypeKeys.Instance().QQ(); }
        }

        /// <summary>
        /// 获取第三方网站空间主页地址
        /// </summary>
        /// <param name="identification"></param>
        /// <returns></returns>
        public override string GetSpaceHomeUrl(string identification)
        {
            return string.Format("http://user.qzone.qq.com/{0}", identification);
        }

        /// <summary>
        /// 获取身份认证Url
        /// </summary>
        /// <returns></returns>
        public override string GetAuthorizationUrl()
        {
            string getAuthorizationCodeUrlPattern = "https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope=add_idol,add_t,add_pic_t";
            return string.Format(getAuthorizationCodeUrlPattern, AccountType.AppKey, WebUtility.UrlEncode(CallbackUrl));
        }

        /// <summary>
        /// 获取身份认证Url(自定义返回页面)
        /// </summary>
        /// <returns></returns>
        public override string GetAuthorizationUrl(string url)
        {
            string getAuthorizationCodeUrlPattern = "https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope=add_idol,add_t,add_pic_t";
            return string.Format(getAuthorizationCodeUrlPattern, AccountType.AppKey, WebUtility.UrlEncode(url));
        }

        /// <summary>
        /// 获取当前第三方帐号上的访问授权
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="expires_in">有效期（单位：秒）</param>
        /// <returns></returns>
        public override string GetAccessToken(HttpRequestBase Request, out int expires_in)
        {
            //Step1：通过Authorization Code获取Access Token
            _restClient.Authenticator = null;
            string code = Request.QueryString.GetString("code", string.Empty);
            var request = new RestRequest(Method.GET);
            request.Resource = "oauth2.0/token?grant_type=authorization_code&client_id={appkey}&client_secret={appsecret}&code={code}&redirect_uri={callbackurl}";
            request.AddParameter("appkey", AccountType.AppKey, ParameterType.UrlSegment);
            request.AddParameter("appsecret", AccountType.AppSecret, ParameterType.UrlSegment);
            request.AddParameter("code", code, ParameterType.UrlSegment);
            request.AddParameter("callbackurl", CallbackUrl, ParameterType.UrlSegment);
            var response = Execute(_restClient, request);
            string access_token = GetParmFromContent(response.Content, @"access_token=(?<accessToken>[^&]+)&expires_in", "accessToken");
            string expiresString = GetParmFromContent(response.Content, @"&expires_in=(?<expires_in>[^&]+)", "expires_in");
            expires_in = 0;
            int.TryParse(expiresString, out expires_in);
            return access_token;
        }

        /// <summary>
        /// 获取当前第三方帐号上的用户
        /// </summary>
        /// <param name="accessToken">访问授权</param>
        /// <param name="identification">标识</param>
        /// <returns></returns>
        public override ThirdUser GetThirdUser(string accessToken, string identification = null)
        {
            //Step1：通根据access_token获得对应用户身份的openid
            _restClient.Authenticator = null;
            var request = new RestRequest(Method.GET);
            if (string.IsNullOrEmpty(identification))
            {
                request.Resource = "oauth2.0/me?access_token={accesstoken}";
                request.AddParameter("accesstoken", accessToken, ParameterType.UrlSegment);
                var response = Execute(_restClient, request);
                identification = GetParmFromContent(response.Content, @"""openid"":""(?<openId>[^""]+)""", "openId");
                if (string.IsNullOrEmpty(identification))
                    return null;
            }

            //Step2：调用OpenAPI，获取用户信息
            _restClient.Authenticator = new QQOAuthAuthenticator(identification, accessToken, AccountType.AppKey);
            request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;
            //request.AddHeader("Content-Type", "application/json");
            request.Resource = "user/get_user_info";
            var getUserInfoResponse = Execute(_restClient, request);
            var qqUser = Json.Decode(getUserInfoResponse.Content);
            return new ThirdUser
            {
                AccountTypeKey = AccountType.AccountTypeKey,
                Identification = identification,
                AccessToken = accessToken,
                NickName = qqUser.nickname,
                Gender = qqUser.gender == "男" ? GenderType.Male : GenderType.FeMale,
                UserAvatarUrl = qqUser.figureurl_qq_2
            };
        }
    }
}