using LazyWeChat.Abstract;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models;
using LazyWeChat.Models.OfficialAccount;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.OfficialAccount
{
    public static partial class CONSTANT
    {
        public const string SENDKFMESSAGEURL = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";
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

        public async Task<dynamic> SendKFMessage(WeChatKFTextMessage text)
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

        public async Task<dynamic> SendKFMessage(WeChatKFImageMessage image) => await Send(image);

        public async Task<dynamic> SendKFMessage(WeChatKFVoiceMessage voice) => await Send(voice);

        public async Task<dynamic> SendKFMessage(WeChatKFVideoMessage video) => await Send(video);

        public async Task<dynamic> SendKFMessage(WeChatKFMusicMessage music) => await Send(music);

        public async Task<dynamic> SendKFMessage(WeChatKFExternalNewsMessage externalNews) => await Send(externalNews);

        public async Task<dynamic> SendKFMessage(WeChatKFNewsMessage news) => await Send(news);

        public async Task<dynamic> SendKFMessage(WeChatKFMenuMessage menu) => await Send(menu);

        public async Task<dynamic> SendKFMessage(WeChatKFMiniMessage mini) => await Send(mini);

        public async Task<dynamic> SendKFMessage(WeChatKFCardMessage card) => await Send(card);

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
    }
}
