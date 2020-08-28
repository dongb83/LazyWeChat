using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models;
using LazyWeChat.Models.OfficialAccount;
using LazyWeChat.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.OfficialAccount
{
    public static partial class CONSTANT
    {
        public const string SENDKFMESSAGEURL = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";

        public const string ADDKFACCOUNTURL = "https://api.weixin.qq.com/customservice/kfaccount/add?access_token={0}";

        public const string EDITKFACCOUNTURL = "https://api.weixin.qq.com/customservice/kfaccount/update?access_token={0}";

        public const string DELETEKFACCOUNTURL = "https://api.weixin.qq.com/customservice/kfaccount/del?access_token={0}";

        public const string GETKFACCOUNTSURL = "https://api.weixin.qq.com/cgi-bin/customservice/getkflist?access_token={0}";

        public const string UPLOADKFAVATARURL = "https://api.weixin.qq.com/customservice/kfaccount/uploadheadimg?access_token={0}&kf_account={1}";
    }

    public partial class LazyMessager : ILazyMessager
    {
        private readonly IHttpRepository _httpRepository;
        private readonly ILazyWeChatBasic _lazyWeChatBasic;
        private readonly ILogger<LazyMessager> _logger;

        public LazyMessager(IHttpRepository httpRepository,
            ILazyWeChatBasic lazyWeChatBasic,
            ILogger<LazyMessager> logger)
        {
            _httpRepository = httpRepository;
            _lazyWeChatBasic = lazyWeChatBasic;
            _logger = logger;
        }

        #region Send

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFTextMessage text)
        {
            var requestJson = "";
            if (!string.IsNullOrEmpty(text.customservice.kf_account))
            {
                requestJson = JsonConvert.SerializeObject(text);
            }
            else
            {
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                string[] props = { "customservice" };
                jSetting.ContractResolver = new LimitPropsContractResolver(props, false);
                requestJson = JsonConvert.SerializeObject(text, jSetting);
            }
            dynamic returnObject = await Send(requestJson);
            return returnObject;
        }

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFImageMessage image) => await Send(image);

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFVoiceMessage voice) => await Send(voice);

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFVideoMessage video) => await Send(video);

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFMusicMessage music) => await Send(music);

        public async Task<dynamic> SendKFMessageAsync(WeChatKFExternalNewsMessage externalNews) => await Send(externalNews);

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFNewsMessage news) => await Send(news);

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFMenuMessage menu) => await Send(menu);

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFMiniMessage mini) => await Send(mini);

        public virtual async Task<dynamic> SendKFMessageAsync(WeChatKFCardMessage card) => await Send(card);

        private async Task<dynamic> Send(string requestJson)
        {
            var access_token = await _lazyWeChatBasic.GetAccessTokenAsync();
            var url = string.Format(CONSTANT.SENDKFMESSAGEURL, access_token);
            _logger.LogInformation($"Request Url:{url}, Request Json : {requestJson}");
            var returnObject = await _httpRepository.PostParseValidateAsync(url, requestJson);
            _logger.LogInformation($"Response Json:{JsonConvert.SerializeObject(returnObject)}");
            return returnObject;
        }

        private async Task<dynamic> Send(WeChatKFMessage message)
        {
            var requestJson = JsonConvert.SerializeObject(message);
            var access_token = await _lazyWeChatBasic.GetAccessTokenAsync();
            var url = string.Format(CONSTANT.SENDKFMESSAGEURL, access_token);
            _logger.LogInformation($"Request Url:{url}, Request Json : {requestJson}");
            var returnObject = await _httpRepository.PostParseValidateAsync(url, requestJson);
            _logger.LogInformation($"Response Json:{JsonConvert.SerializeObject(returnObject)}");
            return returnObject;
        }
        #endregion

        public virtual async Task<dynamic> AddKFAccountAsync(string kf_account, string nickname, string password)
        {
            var requestObject = new { kf_account = kf_account, nickname = nickname, password = password };
            var res = await SendRequest(requestObject, CONSTANT.ADDKFACCOUNTURL, HttpMethod.Post);
            return res;
        }

        public virtual async Task<dynamic> EditKFAccountAsync(string kf_account, string nickname, string password)
        {
            var requestObject = new { kf_account = kf_account, nickname = nickname, password = password };
            var res = await SendRequest(requestObject, CONSTANT.EDITKFACCOUNTURL, HttpMethod.Post);
            return res;
        }

        public virtual async Task<dynamic> DeleteKFAccountAsync(string kf_account)
        {
            var requestObject = new { kf_account = kf_account };
            var res = await SendRequest(requestObject, CONSTANT.DELETEKFACCOUNTURL, HttpMethod.Post);
            return res;
        }

        public virtual async Task<dynamic> GetKFAccountsAsync()
        {
            var res = await SendRequest("", CONSTANT.GETKFACCOUNTSURL, HttpMethod.Get, "kf_list");
            return res;
        }

        public virtual async Task<dynamic> UploadKFAvatarAsync(string kf_account, string avatarFilePath)
        {
            var access_token = await _lazyWeChatBasic.GetAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.UPLOADKFAVATARURL, access_token, kf_account);
            var returnJson = await _httpRepository.UploadFileAsync(requestUrl, avatarFilePath);
            var returnObject = UtilRepository.ParseAPIResult(returnJson);
            return returnObject;
        }
    }
}
