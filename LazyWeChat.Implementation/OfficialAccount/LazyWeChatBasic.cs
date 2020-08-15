using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models;
using LazyWeChat.Models.Exception;
using LazyWeChat.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.OfficialAccount
{
    public static partial class CONSTANT
    {
        public const string ACCESSTOKENURL = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";

        public const string WEBACCESSTOKENURL = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        public const string REFRESHWEBACCESSTOKENURL = "https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}";

        public const string VALIDATEWEBACCESSTOKENURL = "https://api.weixin.qq.com/sns/auth?access_token={0}&openid={1}";

        public const string AUTHORIZATIONURL = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state=STATE#wechat_redirect ";

        public const string GETUSERINFOURL = " https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang={2}";

        public const string IPLISTURL = "https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token={0}";

        public const string APIIPLISTURL = "https://api.weixin.qq.com/cgi-bin/get_api_domain_ip?access_token={0}";

        public const string GENERATEQRCODETICKETURL = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";

        public const string GENERATEQRCODEURL = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}";

        public const string SHORTURL = "https://api.weixin.qq.com/cgi-bin/shorturl?access_token={0}";
    }

    public partial class LazyWeChatBasic : ILazyWeChatBasic
    {
        private readonly IOptions<LazyWeChatConfiguration> _options;
        private readonly IHttpRepository _httpRepository;
        private readonly ILogger<LazyWeChatBasic> _logger;
        private string _accessToken;
        private DateTime _expireAccessToken;

        public LazyWeChatBasic(
            IOptions<LazyWeChatConfiguration> options,
            IHttpRepository httpRepository,
            ILogger<LazyWeChatBasic> logger)
        {
            _options = options;
            _logger = logger;
            _httpRepository = httpRepository;
        }

        async Task<dynamic> SendRequest(dynamic requestObject, string requestUrl, HttpMethod method, params string[] validationNames)
        {
            var accessToken = await GetAccessTokenAsync();
            return await _httpRepository.SendRequest(requestObject, requestUrl, method, accessToken, validationNames);
        }

        #region GetAccessToken

        public string GetAuthorizationCode(HttpContext context, SCOPE scope = SCOPE.snsapi_userinfo)
        {
            if (!context.Request.Query.Keys.Contains("code"))
            {
                var redirectUrl = context.Request.ToAbsoluteUri();
                var url = string.Format(CONSTANT.AUTHORIZATIONURL,
                    _options.Value.AppID,
                    redirectUrl,
                    scope.ToString());

                _logger.LogInformation("redirect url is :'{0}'", url);

                context.Response.Redirect(url);
            }
            var code = context.Request.Query["code"].ToString();
            _logger.LogInformation("code for authorization :'{0}'", code);
            return code;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!CheckAccessToken)
            {
                await SendRequestforAccessToken();
            }
            return _accessToken;
        }

        async Task SendRequestforAccessToken()
        {
            var url = string.Format(CONSTANT.ACCESSTOKENURL,
                _options.Value.AppID,
                _options.Value.AppSecret);

            var returnObj = await _httpRepository.GetParseValidateAsync(url, "access_token");

            _accessToken = returnObj.access_token;
            int.TryParse(returnObj.expires_in, out int seconds);
            _expireAccessToken = DateTime.Now.AddSeconds(seconds - 120);
        }

        bool CheckAccessToken { get => (!string.IsNullOrEmpty(_accessToken) && _expireAccessToken != null && _expireAccessToken > DateTime.Now); }

        #endregion

        #region GetWebAccessToken
        public async Task<dynamic> GetWebAccessTokenAsync(string code)
        {
            var url = string.Format(CONSTANT.WEBACCESSTOKENURL, _options.Value.AppID, _options.Value.AppSecret, code);
            return await _httpRepository.GetParseValidateAsync(url, "openid");
        }

        public async Task<dynamic> RefreshWebAccessTokenAsync(string refresh_token)
        {
            var url = string.Format(CONSTANT.REFRESHWEBACCESSTOKENURL, _options.Value.AppID, refresh_token);
            return await _httpRepository.GetParseValidateAsync(url, "openid");
        }

        public async Task<bool> ValidateWebAccessTokenAsync(string access_token, string openid)
        {
            var url = string.Format(CONSTANT.VALIDATEWEBACCESSTOKENURL, access_token, openid);

            var validated = false;
            var returnObj = await _httpRepository.GetParseAsync(url);
            int.TryParse(returnObj.errcode, out int errcode);
            if (errcode != 0)
            {
                throw new BadResultException(returnObj);
            }
            validated = true;
            return validated;
        }
        #endregion

        #region GetUserInfo

        public async Task<dynamic> GetUserInfoAsync(string access_token, string openid, string lang = "zh_CN")
        {
            var url = string.Format(CONSTANT.GETUSERINFOURL, access_token, openid, lang);
            return await _httpRepository.GetParseValidateAsync(url, "openid");
        }
        #endregion

        #region GetIPList
        public async Task<List<string>> GetWeChatCallbackIPListAsync() => await GetIPListAsync(CONSTANT.IPLISTURL);

        public async Task<List<string>> GetWeChatAPIIPListAsync() => await GetIPListAsync(CONSTANT.APIIPLISTURL);

        private async Task<List<string>> GetIPListAsync(string apiAddr)
        {
            var returnObject = await SendRequest(null, apiAddr, HttpMethod.Get, "ip_list");

            var list = new List<string>();
            foreach (var ip in returnObject.ip_list)
                list.Add(ip.ToString());

            return list;
        }
        #endregion

        #region Account Management
        async Task<dynamic> GenerateQRCodeTicketAsync<T>(bool tempOrNot, int expire_seconds, T scene_value)
        {
            if (!(typeof(T) == typeof(int)|| typeof(T) == typeof(string)))
                throw new InvalidCastException(nameof(scene_value));

            dynamic requestObject = new ExpandoObject();
            if (tempOrNot)
            {
                requestObject.expire_seconds = expire_seconds;
                requestObject.action_name = "QR_SCENE";
            }
            else
            {
                requestObject.action_name = "QR_STR_SCENE";
            }
            requestObject.action_info = new ExpandoObject();
            requestObject.action_info.scene = new ExpandoObject();
            if (scene_value is int scene_id)
                requestObject.action_info.scene.scene_id = scene_id;
            else
                requestObject.action_info.scene.scene_str = scene_value.ToString();

            var returnObject = await SendRequest(requestObject, CONSTANT.GENERATEQRCODETICKETURL, HttpMethod.Post, "ticket");
            return returnObject;
        }

        public async Task<string> GenerateQRCodeAsync<T>(bool tempOrNot, int expire_seconds, T scene_value)
        {
            var ticketObject = await GenerateQRCodeTicketAsync(tempOrNot, expire_seconds, scene_value);

            var ticketEncoded = WebUtility.UrlEncode(ticketObject.ticket);

            var url = string.Format(CONSTANT.GENERATEQRCODEURL, ticketEncoded);
            return url;
        }

        public async Task<string> ShortUrlAsync(string long_url)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.long_url = long_url;
            requestObject.action = "long2short";

            var returnObject = await SendRequest(requestObject, CONSTANT.SHORTURL, HttpMethod.Post, "short_url");
            return returnObject.short_url;
        }

        #endregion
    }
}