using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LazyWeChat.Models.WeChatPay.V2
{
    public enum TradeType
    {
        JSAPI,
        NATIVE,
        MWEB,
    }

    #region 支付码支付模型
    public class MicroPayModel : BaseWeChatPay
    {
        public MicroPayModel(LazyWeChatConfiguration weChatConfiguration) : base(weChatConfiguration)
        {
            m_values.Add(nameof(appid), appid);
            m_values.Add(nameof(mch_id), mch_id);
            m_values.Add(nameof(nonce_str), nonce_str);
        }

        public string out_trade_no
        {
            get
            {
                return m_values.GetValue("out_trade_no").ToString();
            }
            set
            {
                m_values.SetValue("out_trade_no", value);
            }
        }

        public string auth_code
        {
            get
            {
                return m_values.GetValue("auth_code").ToString();
            }
            set
            {
                m_values.SetValue("auth_code", value);
            }
        }

        public string body
        {
            get
            {
                return m_values.GetValue("body").ToString();
            }
            set
            {
                m_values.SetValue("body", value);
            }
        }

        public double total_fee
        {
            get
            {
                return long.Parse(m_values.GetValue("total_fee").ToString());
            }
            set
            {
                m_values.SetValue("total_fee", value);
            }
        }

        public string spbill_create_ip
        {
            get
            {
                return m_values.GetValue("spbill_create_ip").ToString();
            }
            set
            {
                m_values.SetValue("spbill_create_ip", value);
            }
        }

        private string sign
        {
            get
            {
                return m_values.GetValue("sign").ToString();
            }
            set
            {
                m_values.SetValue("sign", value);
            }
        }

        public SortedDictionary<string, object> Parameters
        {
            get
            {
                //检测必填参数
                if (!m_values.IsSet("appid"))
                    throw new ArgumentNullException(nameof(appid));

                if (!m_values.IsSet("mch_id"))
                    throw new ArgumentNullException(nameof(mch_id));

                if (!m_values.IsSet("nonce_str"))
                    throw new ArgumentNullException(nameof(nonce_str));

                if (!m_values.IsSet("auth_code"))
                    throw new ArgumentNullException(nameof(auth_code));

                if (!m_values.IsSet("body"))
                    throw new ArgumentNullException(nameof(body));

                if (!m_values.IsSet("out_trade_no"))
                    throw new ArgumentNullException(nameof(out_trade_no));

                if (!m_values.IsSet("total_fee"))
                    throw new ArgumentNullException(nameof(total_fee));

                if (!m_values.IsSet("sign"))
                    m_values.SetValue("sign", m_values.MakeSign(key));
                return m_values;
            }
        }

        public string Xml
        {
            get => Parameters.ToXml();
        }

    }

    #endregion

    #region JSAPI支付模型
    public class JsApiModel : BaseWeChatPay
    {
        public JsApiModel(LazyWeChatConfiguration weChatConfiguration) : base(weChatConfiguration)
        {
            m_values.Add(nameof(appId), appId);
            m_values.Add(nameof(timeStamp), timeStamp);
            m_values.Add(nameof(nonceStr), nonceStr);
            m_values.Add(nameof(signType), signType);
        }

        public string appId { get => _weChatConfiguration.AppID; }

        public string timeStamp { get => _weChatConfiguration.Timestamp; }

        public string nonceStr { get => _weChatConfiguration.NonceStr; }

        public string signType { get => _weChatConfiguration.SignType; }

        public string package
        {
            get
            {
                return m_values.GetValue("package").ToString();
            }
            set
            {
                m_values.SetValue("package", value);
            }
        }

        private string paySign
        {
            get
            {
                return m_values.GetValue("paySign").ToString();
            }
            set
            {
                m_values.SetValue("paySign", value);
            }
        }

        public SortedDictionary<string, object> Parameters
        {
            get
            {
                //检测必填参数
                if (!m_values.IsSet("appId"))
                    throw new ArgumentNullException(nameof(appId));

                if (!m_values.IsSet("timeStamp"))
                    throw new ArgumentNullException(nameof(timeStamp));

                if (!m_values.IsSet("nonceStr"))
                    throw new ArgumentNullException(nameof(nonceStr));

                if (!m_values.IsSet("package"))
                    throw new ArgumentNullException(nameof(package));

                if (!m_values.IsSet("signType"))
                    throw new ArgumentNullException(nameof(signType));

                if (!m_values.IsSet("paySign"))
                    m_values.SetValue("paySign", m_values.MakeSign(key));
                return m_values;
            }
        }

        public string JSON { get => JsonConvert.SerializeObject(Parameters); }

        public string Xml
        {
            get => Parameters.ToXml();
        }
    }
    #endregion

    #region 扫码支付模型
    public class NativePayModel : BaseWeChatPay
    {
        public NativePayModel(LazyWeChatConfiguration weChatConfiguration) : base(weChatConfiguration)
        {
            m_values.Add(nameof(appid), appid);
            m_values.Add(nameof(mch_id), mch_id);
            m_values.Add(nameof(time_stamp), time_stamp);
            m_values.Add(nameof(nonce_str), nonce_str);
        }

        public string time_stamp { get => _weChatConfiguration.Timestamp; }

        public string product_id
        {
            get
            {
                return m_values.GetValue("product_id").ToString();
            }
            set
            {
                m_values.SetValue("product_id", value);
            }
        }

        private string sign
        {
            get
            {
                return m_values.GetValue("sign").ToString();
            }
            set
            {
                m_values.SetValue("sign", value);
            }
        }

        public SortedDictionary<string, object> Parameters
        {
            get
            {
                //检测必填参数
                if (!m_values.IsSet("appid"))
                    throw new ArgumentNullException(nameof(appid));

                if (!m_values.IsSet("mch_id"))
                    throw new ArgumentNullException(nameof(mch_id));

                if (!m_values.IsSet("time_stamp"))
                    throw new ArgumentNullException(nameof(time_stamp));

                if (!m_values.IsSet("nonce_str"))
                    throw new ArgumentNullException(nameof(nonce_str));

                if (!m_values.IsSet("product_id"))
                    throw new ArgumentNullException(nameof(product_id));

                if (!m_values.IsSet("sign"))
                    m_values.SetValue("sign", m_values.MakeSign(key));
                return m_values;
            }
        }

        string ToUrlParams(SortedDictionary<string, object> map)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in map)
            {
                buff += pair.Key + "=" + pair.Value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }

        public string URL
        {
            get
            {
                var p = Parameters;
                string str = ToUrlParams(m_values);
                string url = "weixin://wxpay/bizpayurl?" + str;
                return url;
            }
        }

        public string Xml
        {
            get => Parameters.ToXml();
        }
    }

    #endregion

    #region 小程序支付模型
    public class MiniPayModel : BaseWeChatPay
    {
        public MiniPayModel(LazyWeChatConfiguration weChatConfiguration) : base(weChatConfiguration)
        {
            m_values = new SortedDictionary<string, object>();
            m_values.Add(nameof(appId), appId);
            m_values.Add(nameof(timeStamp), timeStamp);
            m_values.Add(nameof(nonceStr), nonceStr);
            m_values.Add(nameof(signType), signType);
        }

        private string appId { get => _weChatConfiguration.AppID; }

        public string timeStamp { get => _weChatConfiguration.Timestamp; }

        public string nonceStr { get => _weChatConfiguration.NonceStr; }

        public string signType { get => _weChatConfiguration.SignType; }

        public string package
        {
            get
            {
                return m_values.GetValue("package").ToString();
            }
            set
            {
                m_values.SetValue("package", value);
            }
        }

        private string paySign
        {
            get
            {
                return m_values.GetValue("paySign").ToString();
            }
            set
            {
                m_values.SetValue("paySign", value);
            }
        }

        public SortedDictionary<string, object> Parameters
        {
            get
            {
                //检测必填参数
                if (!m_values.IsSet("appId"))
                    throw new ArgumentNullException(nameof(appId));

                if (!m_values.IsSet("timeStamp"))
                    throw new ArgumentNullException(nameof(timeStamp));

                if (!m_values.IsSet("nonceStr"))
                    throw new ArgumentNullException(nameof(nonceStr));

                if (!m_values.IsSet("package"))
                    throw new ArgumentNullException(nameof(package));

                if (!m_values.IsSet("signType"))
                    throw new ArgumentNullException(nameof(signType));

                if (!m_values.IsSet("paySign"))
                    m_values.SetValue("paySign", m_values.MakeSign(key));
                return m_values;
            }
        }

    }
    #endregion
}
