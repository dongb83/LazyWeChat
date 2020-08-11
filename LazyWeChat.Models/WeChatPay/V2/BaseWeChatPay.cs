using System.Collections.Generic;

namespace LazyWeChat.Models.WeChatPay.V2
{
    public class BaseWeChatPay
    {
        protected readonly LazyWeChatConfiguration _weChatConfiguration;

        protected SortedDictionary<string, object> m_values;

        /// <summary>
        /// out_trade_no,body,total_fee,trade_type,notify_url,openid,product_id为必填属性
        /// </summary>
        /// <param name="weChatConfiguration"></param>
        public BaseWeChatPay(LazyWeChatConfiguration weChatConfiguration)
        {
            _weChatConfiguration = weChatConfiguration;
            m_values = new SortedDictionary<string, object>();
        }

        /// <summary>
        /// 公众账号ID : String(32) 微信分配的公众账号ID（企业号corpid即为此appId） 
        /// </summary>
        public string appid { get => _weChatConfiguration.AppID; }

        /// <summary>
        /// 商户号 mch_id : String(32) 微信支付分配的商户号 
        /// </summary>
        public string mch_id { get => _weChatConfiguration.MCHID; }

        /// <summary>
        /// 随机字符串 nonce_str : String(32) 随机字符串，不长于32位。推荐随机数生成算法 
        /// </summary>
        public string nonce_str { get => _weChatConfiguration.NonceStr; }

        /// <summary>
        /// 商户支付密钥
        /// </summary>
        protected string key { get => _weChatConfiguration.Key; }
    }
}
