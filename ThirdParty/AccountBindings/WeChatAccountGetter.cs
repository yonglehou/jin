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
    /// 微信帐号获取器
    /// </summary>
    public class WeChatAccountGetter : ThirdAccountGetter
    {

        private RestClient _restClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WeChatAccountGetter()
        {
            _restClient = new RestClient("https://api.weixin.qq.com");
        }

        /// <summary>
        /// 名称
        /// </summary>
        public override string AccountTypeName
        {
            get { return "微信帐号"; }
        }

        /// <summary>
        /// 官方网站地址
        /// </summary>
        public override string AccountTypeUrl
        {
            get { return "http://weixin.qq.com/"; }
        }

        /// <summary>
        /// 帐号类型Key
        /// </summary>
        public override string AccountTypeKey
        {
            get { return AccountTypeKeys.Instance().WeChat(); }
        }

        /// <summary>
        /// 获取第三方网站空间主页地址
        /// </summary>
        /// <param name="identification"></param>
        /// <returns></returns>
        public override string GetSpaceHomeUrl(string identification)
        {
            return string.Format("http://weixin.qq.com/{0}", identification);
        }

        /// <summary>
        /// 获取身份认证Url
        /// </summary>
        /// <returns></returns>
        public override string GetAuthorizationUrl()
        {
            string getAuthorizationCodeUrlPattern = "https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect";
            return string.Format(getAuthorizationCodeUrlPattern, AccountType.AppKey, WebUtility.UrlEncode(CallbackUrl), Utility.GenerateRndNonce());
        }

        /// <summary>
        /// 获取身份认证Url(自定义返回页面)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public override string GetAuthorizationUrl(string url)
        {
            string getAuthorizationCodeUrlPattern = "https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect";
            return string.Format(getAuthorizationCodeUrlPattern, AccountType.AppKey, WebUtility.UrlEncode(url), Utility.GenerateRndNonce());
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
            request.Resource = "sns/oauth2/access_token?appid={appid}&secret={secret}&code={code}&grant_type=authorization_code";
            request.AddParameter("appid", AccountType.AppKey, ParameterType.UrlSegment);
            request.AddParameter("secret", AccountType.AppSecret, ParameterType.UrlSegment);
            request.AddParameter("code", code, ParameterType.UrlSegment);
            var response = Execute(_restClient, request);
            var responseJson = Json.Decode(response.Content);
            string access_token = responseJson.access_token;
            this.OpenId = responseJson.openid;
            expires_in = responseJson.expires_in;
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
            _restClient.Authenticator = null;
            var request = new RestRequest(Method.GET);

            //调用OpenAPI，获取用户信息
            _restClient.Authenticator = new QQOAuthAuthenticator(identification, accessToken, AccountType.AppKey);
            request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;
            //request.AddHeader("Content-Type", "application/json");
            request.Resource = "/sns/userinfo";
            var getUserInfoResponse = Execute(_restClient, request);
            var weChatUser = Json.Decode(getUserInfoResponse.Content);
            return new ThirdUser
            {
                AccountTypeKey = AccountType.AccountTypeKey,
                Identification = weChatUser.unionid,
                AccessToken = accessToken,
                NickName = weChatUser.nickname,
                Gender = weChatUser.sex == 1 ? GenderType.Male : GenderType.FeMale,
                UserAvatarUrl = weChatUser.headimgurl
            };
        }
    }
}