using System;
using System.Collections.Generic;

namespace LazyWeChat.Models.WeChatPay.V2
{
    public class OrderQueryModel : BaseWeChatPay
    {
        public OrderQueryModel(LazyWeChatConfiguration weChatConfiguration) : base(weChatConfiguration)
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

        public string transaction_id
        {
            get
            {
                return m_values.GetValue("transaction_id").ToString();
            }
            set
            {
                m_values.SetValue("transaction_id", value);
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

                if (!m_values.IsSet("out_trade_no") && !m_values.IsSet("transaction_id"))
                    throw new ArgumentNullException("out_trade_no或者transaction_id不能同时为空");

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
}
