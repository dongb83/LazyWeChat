using LazyWeChat.Utility;
using System;

namespace LazyWeChat.Models
{
    public enum APIType
    {
        OfficialAccount,
        MiniProgram,
        QY,
    }

    public class LazyWeChatConfiguration
    {
        /// <summary>
        /// 公众号或者小程序 AppID或者企业微信App的AgentID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 公众号或者小程序 AppSecret或者是企业微信App的Secret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 企业微信Contact的Secret
        /// </summary>
        public string ContactSecret { get; set; }

        /// <summary>
        /// 企业微信CorpID
        /// </summary>
        public string CorpID { get; set; }

        /// <summary>
        /// 公众号或者小程序 EncodingAESKey
        /// </summary>
        public string EncodingAESKey { get; set; }

        /// <summary>
        /// 企业微信Contact的EncodingAESKey
        /// </summary>
        public string ContactEncodingAESKey { get; set; }

        /// <summary>
        /// API类型:OfficialAccount或者MiniProgram
        /// </summary>
        public string APIType { get; set; }

        public APIType Type { get => (APIType)Enum.Parse(typeof(APIType), APIType); }

        /// <summary>
        /// 公众号或者小程序 Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 企业微信Contact的Token
        /// </summary>
        public string ContactToken { get; set; }

        /// <summary>
        /// 微信支付商户号
        /// </summary>
        public string MCHID { get; set; }

        /// <summary>
        /// 支付秘钥
        /// </summary>
        public string Key { get; set; }

        public string NonceStr { get => Guid.NewGuid().ToString().Replace("-", ""); }

        public string Timestamp { get => UtilRepository.GetUTCTicks().ToString(); }

        public string SignType { get => "MD5"; }

        public string QYContactListener { get => "QYContactListener"; }

        public string LazyWechatListener { get => "LazyWechatListener"; }

        public string NativeNotifyListener { get => "NativeNotifyListener"; }

        /// <summary>
        /// 消息队列的链接字符串, 如使用默认消息队列，则无需配置
        /// </summary>
        public string MQConnectionString { get; set; }

    }
}
