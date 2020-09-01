using LazyWeChat.Abstract;
using LazyWeChat.Abstract.QY;
using LazyWeChat.Models;
using LazyWeChat.Models.Exception;
using LazyWeChat.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.QY
{
    public static partial class CONSTANT
    {
        public const string ACCESSTOKENURL = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";

        public const string GETUSERINFOURL = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}";

        public const string AUTHORIZATIONURL = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state=STATE#wechat_redirect";

        public const string GETTICKETURL = "https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token={0}";

        public const string GETAGENTTICKETURL = "https://qyapi.weixin.qq.com/cgi-bin/ticket/get?access_token={0}&type=agent_config";
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

        private string _jsTicket;
        private DateTime _expireJSTicket;
        private string _jsAgentTicket;
        private DateTime _expireAgentJSTicket;

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
                    _options.Value.AppID,
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
                _options.Value.AppID,
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

        public virtual async Task<string> GenerateWXConfigScriptAsync(
            string requestUrl,
            bool debug,
            params string[] jsApiList)
        {
            if (jsApiList == null || jsApiList.Length == 0)
            {
                throw new ArgumentNullException(nameof(jsApiList));
            }

            string script = @"
                            wx.config({{
                                  beta: true,
                                  debug: {0},
                                  appId: '{1}',
                                  timestamp: {2},
                                  nonceStr: '{3}',
                                  signature: '{4}',
                                  jsApiList: [
                                    {5}
                                  ]
                              }});
                            ";

            StringBuilder api = new StringBuilder();
            jsApiList.ToList().ForEach(i => api.Append($"'{i}',"));
            var apiList = api.ToString();

            var appId = _options.Value.AppID;
            var noncestr = _options.Value.NonceStr;
            var timestamp = _options.Value.Timestamp;
            var jsTicket = await GetJSTicketAsync();

            var signature = UtilRepository.GenerateSignature(noncestr, timestamp, requestUrl, jsTicket, out string outString);

            _logger.LogInformation($"noncestr:{noncestr},timestamp:{timestamp},requestUrl:{requestUrl},jsTicket:{jsTicket},signature:{signature},outString:{outString}");

            script = string.Format(script, debug.ToString().ToLower(), appId, timestamp, noncestr, signature, apiList.Substring(0, apiList.Length - 1));
            return script;
        }

        public virtual async Task<string> GenerateWXConfigScriptAsync(
            HttpContext context,
            bool debug,
            params string[] jsApiList) => await GenerateWXConfigScriptAsync(context.Request.ToAbsoluteUri(), debug, jsApiList);

        public virtual async Task<Dictionary<string, string>> GetAgentConfigParameters(string requestUrl)
        {
            var corpid = _options.Value.AppID;
            var agentid = _options.Value.AgentID;
            var timestamp = _options.Value.Timestamp;
            var noncestr = _options.Value.NonceStr;
            var jsTicket = await GetAgentJSTicketAsync();

            var dict = new Dictionary<string, string>();
            dict.Add("corpid", corpid);
            dict.Add("agentid", agentid);
            dict.Add("timestamp", timestamp);
            dict.Add("nonceStr", noncestr);
            var signature = UtilRepository.GenerateSignature(noncestr, timestamp, requestUrl, jsTicket, out string outString);
            dict.Add("signature", signature);

            _logger.LogInformation($"noncestr:{noncestr},timestamp:{timestamp},requestUrl:{requestUrl},jsTicket:{jsTicket},signature:{signature},outString:{outString}");
            return dict;
        }

        public virtual async Task<Dictionary<string, string>> GetAgentConfigParameters(HttpContext context) => await GetAgentConfigParameters(context.Request.ToAbsoluteUri());

        #region GetJSTicket

        public virtual async Task<string> GetJSTicketAsync()
        {
            if (!CheckJSTicket)
            {
                var access_token = await GetAccessTokenAsync();
                var url = string.Format(CONSTANT.GETTICKETURL, access_token);
                SendRequestforJSTicket(url, ref _jsTicket, ref _expireJSTicket);
            }
            return _jsTicket;
        }

        public virtual async Task<string> GetAgentJSTicketAsync()
        {
            if (!CheckAgentJSTicket)
            {
                var access_token = await GetAccessTokenAsync();
                var url = string.Format(CONSTANT.GETAGENTTICKETURL, access_token);
                SendRequestforJSTicket(url, ref _jsAgentTicket, ref _expireAgentJSTicket);
            }
            return _jsAgentTicket;
        }

        void SendRequestforJSTicket(string url, ref string jsTicket, ref DateTime expireJSTicket)
        {
            var returnObj = _httpRepository.GetParseAsync(url).Result;
            int.TryParse(returnObj.errcode, out int errcode);
            if (errcode != 0)
            {
                throw new BadResultException(returnObj);
            }

            jsTicket = returnObj.ticket;
            int.TryParse(returnObj.expires_in, out int seconds);
            expireJSTicket = DateTime.Now.AddSeconds(seconds - 120);
        }

        bool CheckJSTicket { get => (!string.IsNullOrEmpty(_jsTicket) && _expireJSTicket != null && _expireJSTicket > DateTime.Now); }

        bool CheckAgentJSTicket { get => (!string.IsNullOrEmpty(_jsAgentTicket) && _expireAgentJSTicket != null && _expireAgentJSTicket > DateTime.Now); }

        #endregion
    }
}
