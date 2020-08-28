using LazyWeChat.Abstract;
using LazyWeChat.Abstract.MiniProgram;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.MiniProgram
{
    public static partial class CONSTANT
    {
        public const string CODE2SESSIONURL = "https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code";

        public const string GETTEMPMEDIAURL = "https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";

        public const string UPLOADTEMPMEDIAURL = "https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";

        public const string SENDKFMESSAGEURL = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";

        public const string UNIFORMMESSAGEURL = "https://api.weixin.qq.com/cgi-bin/message/wxopen/template/uniform_send?access_token={0}";

        public const string GETPAIDUNIONIDWTTRANSACTIONIDURL = "https://api.weixin.qq.com/wxa/getpaidunionid?access_token={0}&openid={1}&transaction_id={2}";

        public const string GETPAIDUNIONIDWTOUTTRADENOURL = "https://api.weixin.qq.com/wxa/getpaidunionid?access_token={0}&openid={1}&mch_id={2}&out_trade_no={3}";

        public const string CREATEACTIVITYIDURL = "https://api.weixin.qq.com/cgi-bin/message/wxopen/activityid/create?access_token={0}";

        public const string SETUPDATABLEMSGURL = "https://api.weixin.qq.com/cgi-bin/message/wxopen/updatablemsg/send?access_token={0}";

        public const string CREATEQRCODEURL = "https://api.weixin.qq.com/cgi-bin/wxaapp/createwxaqrcode?access_token={0}";
    }

    public partial class LazyMiniBasic : ILazyMiniBasic
    {
        private readonly IOptions<LazyWeChatConfiguration> _options;
        private readonly IHttpRepository _httpRepository;
        private readonly ILazyWeChatBasic _lazyWeChatBasic;
        private readonly ILazyMaterials _lazyMaterials;
        private readonly ILogger<LazyMiniBasic> _logger;

        public LazyMiniBasic(
            IOptions<LazyWeChatConfiguration> options,
            IHttpRepository httpRepository,
            ILazyWeChatBasic lazyWeChatBasic,
            ILazyMaterials lazyMaterials,
            ILogger<LazyMiniBasic> logger)
        {
            _options = options;
            _httpRepository = httpRepository;
            _lazyWeChatBasic = lazyWeChatBasic;
            _lazyMaterials = lazyMaterials;
            _logger = logger;
        }

        public virtual async Task<dynamic> Code2SessionAsync(string js_code)
        {
            var url = string.Format(CONSTANT.CODE2SESSIONURL, _options.Value.AppID, _options.Value.AppSecret, js_code);
            return await _httpRepository.GetParseValidateAsync(url, "openid");
        }

        public virtual async Task<string> GetAccessTokenAsync() => await _lazyWeChatBasic.GetAccessTokenAsync();

        public virtual string Decrypt(string encryptedData, string iv, string sessionKey)
        {
            //创建解密器生成工具实例
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            //设置解密器参数
            aes.Mode = CipherMode.CBC;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            //格式化待处理字符串
            byte[] byte_encryptedData = Convert.FromBase64String(encryptedData);
            byte[] byte_iv = Convert.FromBase64String(iv);
            byte[] byte_sessionKey = Convert.FromBase64String(sessionKey);

            aes.IV = byte_iv;
            aes.Key = byte_sessionKey;
            //根据设置好的数据生成解密器实例
            ICryptoTransform transform = aes.CreateDecryptor();

            //解密
            byte[] final = transform.TransformFinalBlock(byte_encryptedData, 0, byte_encryptedData.Length);

            //生成结果
            string result = Encoding.UTF8.GetString(final);

            return result;
        }

        public virtual async Task<dynamic> getPaidUnionIdwtTransactionIdAsync(string openid, string transaction_id)
        {
            var access_token = await GetAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.GETPAIDUNIONIDWTTRANSACTIONIDURL, access_token, openid, transaction_id);
            var returnObject = await _httpRepository.GetParseValidateAsync(requestUrl, "unionid");
            return returnObject;
        }

        public virtual async Task<dynamic> getPaidUnionIdwtOutTradeNoAsync(string openid, string out_trade_no)
        {
            var access_token = await GetAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.GETPAIDUNIONIDWTOUTTRADENOURL, access_token, openid, _options.Value.MCHID, out_trade_no);
            var returnObject = await _httpRepository.GetParseValidateAsync(requestUrl, "unionid");
            return returnObject;
        }

        public virtual async Task<dynamic> CreateActivityIdAsync()
        {
            var access_token = await GetAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.CREATEACTIVITYIDURL, access_token);
            var returnObject = await _httpRepository.GetParseValidateAsync(requestUrl, "activity_id");
            return returnObject;
        }

        public virtual async Task<dynamic> SetUpdatableMsgAsync(string activity_id, int target_state, Dictionary<ParameterType, string> parameters)
        {
            dynamic requestObject = new ExpandoObject();
            requestObject.activity_id = activity_id;
            requestObject.target_state = target_state;
            requestObject.template_info = new ExpandoObject();
            var parameter_list = new List<dynamic>();
            foreach (var item in parameters)
            {
                dynamic parameter = new ExpandoObject();
                parameter.name = item.Key.ToString();
                parameter.value = item.Value.ToString();
                parameter_list.Add(parameter);
            }
            requestObject.template_info.parameter_list = parameter_list;

            var returnObject = await SendRequest(requestObject, CONSTANT.SETUPDATABLEMSGURL, HttpMethod.Post);
            return returnObject;
        }

        public virtual async Task<byte[]> CreateQRCodeAsync(string path, int width)
        {
            var access_token = await GetAccessTokenAsync();
            var requestUrl = string.Format(CONSTANT.CREATEQRCODEURL, access_token);
            var requestContent = JsonConvert.SerializeObject(new { path = path, width = width });
            var res = await _httpRepository.PostAsync(requestUrl, requestContent);
            return Encoding.Default.GetBytes(res);
        }
    }
}
