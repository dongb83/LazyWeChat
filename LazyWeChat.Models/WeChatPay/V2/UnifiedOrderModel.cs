using System;
using System.Collections.Generic;

namespace LazyWeChat.Models.WeChatPay.V2
{

    #region 统一下单模型
    /// <summary>
    /// 调用该类的Parameter属性时，程序会为所有传递过来的属性生成签名;该类的XML属性则直接将Parameter属性转换成XML的形式，
    /// </summary>
    public class UnifiedOrderModel : BaseWeChatPay
    {
        /// <summary>
        /// out_trade_no,body,total_fee,trade_type,notify_url,openid,product_id为必填属性
        /// </summary>
        /// <param name="weChatConfiguration"></param>
        public UnifiedOrderModel(LazyWeChatConfiguration weChatConfiguration) : base(weChatConfiguration)
        {
            m_values.Add(nameof(appid), appid);
            m_values.Add(nameof(mch_id), mch_id);
            m_values.Add(nameof(nonce_str), nonce_str);
        }

        /// <summary>
        /// 设备号 device_info : String(32) 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB" (不是必填字段)
        /// </summary>
        public string device_info
        {
            get
            {
                return m_values.GetValue("device_info").ToString();
            }
            set
            {
                m_values.SetValue("appid", value);
            }
        }

        /// <summary>
        /// 签名 sign : String(32) 签名，详见签名生成算法 
        /// </summary>
        public string sign
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

        /// <summary>
        /// 商品描述 body : String(128) 商品或支付单简要描述 
        /// </summary>
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

        /// <summary>
        /// 商品详情 detail : String(8192) 商品名称明细列表 (不是必填字段)
        /// </summary>
        public string detail
        {
            get
            {
                return m_values.GetValue("detail").ToString();
            }
            set
            {
                m_values.SetValue("detail", value);
            }
        }

        /// <summary>
        /// 附加数据 attach : String(127) 在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据 (不是必填字段)
        /// </summary>
        public string attach
        {
            get
            {
                return m_values.GetValue("attach").ToString();
            }
            set
            {
                m_values.SetValue("attach", value);
            }
        }

        /// <summary>
        /// 商户订单号 out_trade_no : String(32) 商户系统内部的订单号,32个字符内、可包含字母, 其他说明见商户订单号 
        /// </summary>
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

        /// <summary>
        /// 货币类型 fee_type : String(16)  符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型 (不是必填字段)
        /// </summary>
        public string fee_type
        {
            get
            {
                return m_values.GetValue("fee_type").ToString();
            }
            set
            {
                m_values.SetValue("fee_type", value);
            }
        }

        /// <summary>
        /// 总金额 total_fee : Int 订单总金额，单位为分，详见支付金额 
        /// </summary>
        public double total_fee
        {
            get
            {
                return double.Parse(m_values.GetValue("total_fee").ToString());
            }
            set
            {
                m_values.SetValue("total_fee", value);
            }
        }

        /// <summary>
        /// 终端IP spbill_create_ip : String(16) APP和网页支付提交用户端ip，Native支付填调用微信支付API的机器IP。 
        /// </summary>
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

        /// <summary>
        /// 交易起始时间 time_start : String(14) 订单生成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。 (不是必填字段)
        /// </summary>
        public string time_start
        {
            get
            {
                return m_values.GetValue("time_start").ToString();
            }
            set
            {
                m_values.SetValue("time_start", value);
            }
        }

        /// <summary>
        /// 交易结束时间 time_expire : String(14) 20091227091010 订单失效时间，格式为yyyyMMddHHmmss，如2009年12月27日9点10分10秒表示为20091227091010。最短失效时间间隔必须大于5分钟 (不是必填字段)
        /// </summary>
        public string time_expire
        {
            get
            {
                return m_values.GetValue("time_expire").ToString();
            }
            set
            {
                m_values.SetValue("time_expire", value);
            }
        }

        /// <summary>
        /// 商品标记 goods_tag : String(32) 商品标记，代金券或立减优惠功能的参数，说明详见代金券或立减优惠 (不是必填字段)
        /// </summary>
        public string goods_tag
        {
            get
            {
                return m_values.GetValue("goods_tag").ToString();
            }
            set
            {
                m_values.SetValue("goods_tag", value);
            }
        }

        /// <summary>
        /// 通知地址 notify_url : String(256) 接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数。 
        /// </summary>
        public string notify_url
        {
            get
            {
                return m_values.GetValue("notify_url").ToString();
            }
            set
            {
                m_values.SetValue("notify_url", value);
            }
        }

        /// <summary>
        /// 交易类型 trade_type : String(16) JSAPI 取值如下：JSAPI，NATIVE，APP，详细说明见参数规定 
        /// </summary>
        public string trade_type
        {
            get
            {
                return m_values.GetValue("trade_type").ToString();
            }
            set
            {
                m_values.SetValue("trade_type", value);
            }
        }

        /// <summary>
        /// 商品ID product_id : String(32) 12235413214070356458058 trade_type=NATIVE，此参数必传。此id为二维码中包含的商品ID，商户自行定义。 (其他情况不是必填字段)
        /// </summary>
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

        /// <summary>
        /// 指定支付方式 limit_pay : String(32) no_credit no_credit--指定不能使用信用卡支付 (不是必填字段)
        /// </summary>
        public string limit_pay
        {
            get
            {
                return m_values.GetValue("limit_pay").ToString();
            }
            set
            {
                m_values.SetValue("limit_pay", value);
            }
        }

        /// <summary>
        /// 用户标识 openid : String(128) trade_type=JSAPI，此参数必传，用户在商户appid下的唯一标识。openid如何获取，可参考【获取openid】。企业号请使用【企业号OAuth2.0接口】获取企业号内成员userid，再调用【企业号userid转openid接口】进行转换 (其他情况不是必填字段)
        /// </summary>
        public string openid
        {
            get
            {
                return m_values.GetValue("openid").ToString();
            }
            set
            {
                m_values.SetValue("openid", value);
            }
        }

        /// <summary>
        /// 场景信息
        /// </summary>
        public dynamic scene_info { get; set; }

        public SortedDictionary<string, object> Parameters
        {
            get
            {
                //检测必填参数
                if (!m_values.IsSet("out_trade_no"))
                    throw new ArgumentNullException(nameof(out_trade_no));

                if (!m_values.IsSet("body"))
                    throw new ArgumentNullException(nameof(body));

                if (!m_values.IsSet("total_fee"))
                    throw new ArgumentNullException(nameof(total_fee));

                if (!m_values.IsSet("trade_type"))
                    throw new ArgumentNullException(nameof(trade_type));

                if (!m_values.IsSet("notify_url"))
                    throw new ArgumentNullException(nameof(notify_url));

                //关联参数
                if (m_values.GetValue("trade_type").ToString() == "JSAPI" && !m_values.IsSet("openid"))
                    throw new ArgumentNullException(nameof(openid));

                if (m_values.GetValue("trade_type").ToString() == "NATIVE" && !m_values.IsSet("product_id"))
                    throw new ArgumentNullException(nameof(product_id));

                if (!m_values.IsSet("sign"))
                    m_values.SetValue("sign", m_values.MakeSign(key));
                return m_values;
            }
        }

        public string XML
        {
            get
            {
                return Parameters.ToXml();
            }
        }
    }
    #endregion
}
