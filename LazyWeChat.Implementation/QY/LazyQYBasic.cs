using LazyWeChat.Abstract;
using LazyWeChat.Abstract.QY;
using LazyWeChat.Models;
using LazyWeChat.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.QY
{
    public static partial class CONSTANT
    {
        public const string ACCESSTOKENURL = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";

        public const string GETUSERINFOURL = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}";

        public const string AUTHORIZATIONURL = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state=STATE#wechat_redirect";
    }

    public partial class LazyQYBasic : ILazyQYBasic
    {
        private readonly IOptions<LazyWeChatConfiguration> _options;
        private readonly IHttpRepository _httpRepository;
        private readonly ILogger<LazyQYBasic> _logger;
        private string _accessToken;
        private DateTime _expireAccessToken;
        private string _contactAccessToken;
        private DateTime _contactExpireAccessToken;

        public LazyQYBasic(
            IOptions<LazyWeChatConfiguration> options,
            IHttpRepository httpRepository,
            ILogger<LazyQYBasic> logger)
        {
            _options = options;
            _logger = logger;
            _httpRepository = httpRepository;
        }

        async Task<dynamic> SendContactRequest(dynamic requestObject, string requestUrl, HttpMethod method, params string[] validationNames)
        {
            var accessToken = await GetContactAccessTokenAsync();
            return await _httpRepository.SendRequestAsync(requestObject, requestUrl, method, accessToken, validationNames);
        }

        async Task<dynamic> SendRequest(dynamic requestObject, string requestUrl, HttpMethod method, params string[] validationNames)
        {
            var accessToken = await GetAccessTokenAsync();
            return await _httpRepository.SendRequestAsync(requestObject, requestUrl, method, accessToken, validationNames);
        }

        public virtual string GetAuthorizationCode(HttpContext context)
        {
            if (!context.Request.Query.Keys.Contains("code"))
            {
                var redirectUrl = context.Request.ToAbsoluteUri();
                var url = string.Format(CONSTANT.AUTHORIZATIONURL,
                    _options.Value.CorpID,
                    redirectUrl,
                    SCOPE.snsapi_base);

                _logger.LogInformation("redirect url is :'{0}'", url);

                context.Response.Redirect(url);
            }
            var code = context.Request.Query["code"].ToString();
            _logger.LogInformation("code for authorization :'{0}'", code);
            return code;
        }

        public virtual async Task<dynamic> GetUserInfoAsync(string code)
        {
            var access_token = await GetAccessTokenAsync();
            var url = string.Format(CONSTANT.GETUSERINFOURL, access_token, code);
            return await _httpRepository.GetParseValidateAsync(url);
        }

        public virtual async Task<string> GetAccessTokenAsync()
        {
            if (!CheckAccessToken)
            {
                await SendRequestforAccessToken(_options.Value.AppSecret, false);
            }
            return _accessToken;
        }

        public virtual async Task<string> GetContactAccessTokenAsync()
        {
            if (!ContactCheckAccessToken)
            {
                await SendRequestforAccessToken(_options.Value.ContactSecret, true);
            }
            return _contactAccessToken;
        }

        async Task SendRequestforAccessToken(string secret, bool isContactServer)
        {
            var url = string.Format(CONSTANT.ACCESSTOKENURL,
                _options.Value.CorpID,
                secret);

            var returnObj = await _httpRepository.GetParseValidateAsync(url, "access_token");

            if (isContactServer)
            {
                _contactAccessToken = returnObj.access_token;
                int.TryParse(returnObj.expires_in, out int seconds);
                _contactExpireAccessToken = DateTime.Now.AddSeconds(seconds - 120);
            }
            else
            {
                _accessToken = returnObj.access_token;
                int.TryParse(returnObj.expires_in, out int seconds);
                _expireAccessToken = DateTime.Now.AddSeconds(seconds - 120);
            }
        }

        bool CheckAccessToken { get => (!string.IsNullOrEmpty(_accessToken) && _expireAccessToken != null && _expireAccessToken > DateTime.Now); }

        bool ContactCheckAccessToken { get => (!string.IsNullOrEmpty(_contactAccessToken) && _contactExpireAccessToken != null && _contactExpireAccessToken > DateTime.Now); }
    }
}
