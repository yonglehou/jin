//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System.Web;
using System.Text.RegularExpressions;
using Tunynet.Common;
using Tunynet.Caching;
using RestSharp;
using System.Net;
using System.Web.Routing;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 第三方帐号获取器
    /// </summary>
    public abstract class ThirdAccountGetter
    {
        private ICacheService cacheService = DIContainer.Resolve<ICacheService>();

        /// <summary>
        /// 帐号类型名称
        /// </summary>
        public abstract string AccountTypeName { get; }

        /// <summary>
        /// 帐号类型官方网站地址
        /// </summary>
        public abstract string AccountTypeUrl { get; }

        /// <summary>
        /// 帐号类型Key
        /// </summary>
        public abstract string AccountTypeKey { get; }

        /// <summary>
        /// 获取身份认证Url
        /// </summary>
        /// <returns></returns>
        public abstract string GetAuthorizationUrl();

        /// <summary>
        /// 获取身份认证Url(自定义返回页面)
        /// </summary>
        /// <returns></returns>
        public abstract string GetAuthorizationUrl(string url);

        /// <summary>
        /// 获取第三方网站用户空间地址Url
        /// </summary>
        /// <returns></returns>
        public abstract string GetSpaceHomeUrl(string identification);

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 获取当前第三方帐号上的用户标识
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="expires_in">有效期（单位：秒）</param>
        /// <returns></returns>
        public abstract string GetAccessToken(HttpRequestBase Request, out int expires_in);

        /// <summary>
        /// 获取当前第三方帐号上的用户
        /// </summary>
        /// <remarks>使用缓存</remarks>
        /// <param name="accessToken">访问授权</param>
        /// <param name="identification">用户标识</param>
        /// <param name="userCache">是否使用缓存</param>
        /// <returns></returns>
        public ThirdUser GetThirdUser(string accessToken, string identification, bool userCache)
        {
            if (!userCache)
                return GetThirdUser(accessToken, identification);

            string cacheKey = string.Format("ThirdUser::AccountTypeKey-{0}:AccessToken-{1}:Identification-{2}", AccountTypeKey, accessToken, identification ?? string.Empty);
            ThirdUser thirdUser = cacheService.Get<ThirdUser>(cacheKey);
            if (thirdUser == null)
            {
                thirdUser = GetThirdUser(accessToken, identification);
                cacheService.Set(cacheKey, thirdUser, CachingExpirationType.SingleObject);
            }
            return thirdUser;
        }

        /// <summary>
        /// 获取当前第三方帐号上的用户标识
        /// </summary>
        /// <param name="accessToken">访问授权</param>
        /// <returns></returns>
        public abstract ThirdUser GetThirdUser(string accessToken, string identification = null);

        /// <summary>
        /// 回调地址
        /// </summary>
        protected string CallbackUrl
        {
            get
            {
                return SiteUrls.FullUrl(CachedUrlHelper.Action("ThirdCallBack", "Account", null, new RouteValueDictionary { { "accountTypeKey", AccountTypeKey } }));
            }
        }

        /// <summary>
        /// 帐号类型
        /// </summary>
        protected AccountType AccountType
        {
            get
            {
                return new AccountBindingService().GetAccountType(AccountTypeKey);
            }
        }


        /// <summary>
        /// 从请求内容中获取AccessToken
        /// </summary>
        /// <remarks>适用于Oauth2.0</remarks>
        /// <param name="content"></param>
        /// <param name="regexPattern">正则表达式</param>
        /// <param name="parmName">参数名称（对应于正则中的分组名）</param>
        /// <returns></returns>
        protected string GetParmFromContent(string content, string regexPattern, string parmName)
        {
            Regex regex = new Regex(regexPattern);
            Match match = regex.Match(content);
            if (!match.Success)
                return string.Empty;
            return match.Groups[parmName].Value;
        }


        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IRestResponse Execute(RestClient _restClient, RestRequest request)
        {
            var response = _restClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ExceptionFacade(response.Content + "\n" + response.ResponseUri);
            }
            return response;
        }

    }
}