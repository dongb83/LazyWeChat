using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models.Exception;
using LazyWeChat.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.OfficialAccount
{
    public static partial class CONSTANT
    {
        public const string GETTICKETURL = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type={1}";
    }

    public partial class LazyWeChatBasic : ILazyWeChatBasic
    {
        private string _jsTicket;
        private DateTime _expireJSTicket;

        public async Task<string> GenerateWXConfigScriptAsync(
            HttpContext context,
            bool debug,
            params string[] jsApiList)
        {
            if (jsApiList == null || jsApiList.Length == 0)
            {
                throw new ArgumentNullException(nameof(jsApiList));
            }

            string script = @"
                            wx.config({{
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
            var requestUrl = context.Request.ToAbsoluteUri();
            var jsTicket = await GetJSTicketAsync();

            var signature = UtilRepository.GenerateSignature(noncestr, timestamp, requestUrl, jsTicket, out string outString);

            _logger.LogInformation($"noncestr:{noncestr},timestamp:{timestamp},requestUrl:{requestUrl},jsTicket:{jsTicket},signature:{signature},outString:{outString}");

            script = string.Format(script, debug.ToString().ToLower(), appId, timestamp, noncestr, signature, apiList.Substring(0, apiList.Length - 1));
            return script;
        }

        public async Task<string> GenerateWXConfigScriptAsync(
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

        #region GetJSTicket

        public async Task<string> GetJSTicketAsync()
        {
            if (!CheckJSTicket)
            {
                await SendRequestforJSTicket();
            }
            return _jsTicket;
        }

        async Task SendRequestforJSTicket()
        {
            var accessToken = await GetAccessTokenAsync();
            var url = string.Format(CONSTANT.GETTICKETURL, accessToken, "jsapi");

            var returnObj = await _httpRepository.GetParseAsync(url);
            int.TryParse(returnObj.errcode, out int errcode);
            if (errcode != 0)
            {
                throw new BadResultException(returnObj);
            }

            _jsTicket = returnObj.ticket;
            int.TryParse(returnObj.expires_in, out int seconds);
            _expireJSTicket = DateTime.Now.AddSeconds(seconds - 120);
        }

        bool CheckJSTicket { get => (!string.IsNullOrEmpty(_jsTicket) && _expireJSTicket != null && _expireJSTicket > DateTime.Now); }

        #endregion
    }
}
