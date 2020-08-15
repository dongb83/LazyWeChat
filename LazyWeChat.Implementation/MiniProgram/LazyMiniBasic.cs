using LazyWeChat.Abstract;
using LazyWeChat.Abstract.MiniProgram;
using LazyWeChat.Abstract.OfficialAccount;
using LazyWeChat.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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

        public async Task<dynamic> Code2SessionAsync(string js_code)
        {
            var url = string.Format(CONSTANT.CODE2SESSIONURL, _options.Value.AppID, _options.Value.AppSecret, js_code);
            return await _httpRepository.GetParseValidateAsync(url, "openid");
        }

        public async Task<string> GetAccessTokenAsync() => await _lazyWeChatBasic.GetAccessTokenAsync();

        /// <summary>
        /// 根据微信小程序平台提供的解密算法解密数据
        /// </summary>
        /// <param name="encryptedData">加密数据</param>
        /// <param name="iv">初始向量</param>
        /// <param name="sessionKey">从服务端获取的SessionKey</param>
        /// <returns></returns>
        public string Decrypt(string encryptedData, string iv, string sessionKey)
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
    }
}
