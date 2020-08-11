using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWeChat.Models.WeChatPay.V3
{
    public class JsApiOrderModel
    {
        public JsApiOrderModel()
        {
            amount = new JsApiOrderAmountModel();
            payer = new JsApiOrderPayerModel();
            detail = new JsApiOrderDetailModel();
            scene_info = new JsApiOrderSceneInfoModel();
        }

        /// <summary>
        /// 公众号ID,示例值:wxd678efh567hg6787
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 直连商户号,示例值:1230000109
        /// </summary>
        public string mchid { get; set; }

        /// <summary>
        /// 商品描述,示例值:Image形象店-深圳腾大-QQ公仔
        /// </summary>
        public string description { get; set; }
                /// <summary>
        /// 商户订单号(商户系统内部订单号，只能是数字、大小写字母_-*且在同一个商户号下唯一),示例值:1217752501201407033233368018
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 交易结束时间,示例值:2018-06-08T10:34:56+08:00
        /// </summary>
        public string time_expire { get; set; }

        /// <summary>
        /// 附加数据,示例值:自定义数据 
        /// </summary>
        public string attach { get; set; }

        /// <summary>
        /// 通知地址,示例值:https://www.weixin.qq.com/wxpay/pay.php
        /// </summary>
        public string notify_url { get; set; }

        /// <summary>
        /// 订单优惠标记,示例值:WXG
        /// </summary>
        public string goods_tag { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public JsApiOrderAmountModel amount { get; set; }

        /// <summary>
        /// 支付者
        /// </summary>
        public JsApiOrderPayerModel payer { get; set; }

        /// <summary>
        /// 优惠功能
        /// </summary>
        public JsApiOrderDetailModel detail { get; set; }

        /// <summary>
        /// 场景信息
        /// </summary>
        public JsApiOrderSceneInfoModel scene_info { get; set; }
    }

    public class JsApiOrderAmountModel
    {
        /// <summary>
        /// 总金额(分),示例值:100
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 货币类型,示例值:CNY
        /// </summary>
        public string currency { get; set; }
    }

    public class JsApiOrderPayerModel
    {
        /// <summary>
        /// 用户标识,示例值:oUpF8uMuAJO_M2pxb1Q9zNjWeS6o
        /// </summary>
        public string openid { get; set; }
    }

    public class JsApiOrderDetailModel
    {
        public JsApiOrderDetailModel()
        {
            goods_detail = new List<JsApiOrderGoodsDetailModel>();
        }

        /// <summary>
        /// 订单原价,示例值:608800
        /// </summary>
        public int cost_price { get; set; }

        /// <summary>
        /// 商品小票ID,示例值:微信123
        /// </summary>
        public string invoice_id { get; set; }

        /// <summary>
        /// 单品列表
        /// </summary>
        public List<JsApiOrderGoodsDetailModel> goods_detail { get; set; }
    }

    public class JsApiOrderGoodsDetailModel
    {
        /// <summary>
        /// 商户侧商品编码,示例值:商品编码
        /// </summary>
        public string merchant_goods_id { get; set; }

        /// <summary>
        /// 微信侧商品编码,示例值:1001
        /// </summary>
        public string wechatpay_goods_id { get; set; }

        /// <summary>
        /// 商品名称,示例值:iPhoneX 256G
        /// </summary>
        public string goods_name { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// 商品单价(分)
        /// </summary>
        public int unit_price { get; set; }
    }

    public class JsApiOrderSceneInfoModel
    {
        public JsApiOrderSceneInfoModel()
        {
            store_info = new JsApiOrderSceneInfoStoreInfoModel();
        }

        /// <summary>
        /// 用户终端IP,示例值:14.23.150.211
        /// </summary>
        public string payer_client_ip { get; set; }

        /// <summary>
        /// 商户端设备号,示例值:013467007045764
        /// </summary>
        public string device_id { get; set; }

        /// <summary>
        /// 商户门店信息
        /// </summary>
        public JsApiOrderSceneInfoStoreInfoModel store_info { get; set; }
    }

    public class JsApiOrderSceneInfoStoreInfoModel
    {
        /// <summary>
        /// 门店编号,示例值:0001
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 商户端设备号,示例值:013467007045764
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 地区编码,示例值:440305
        /// </summary>
        public string area_code { get; set; }

        /// <summary>
        /// 详细地址	
        /// </summary>
        public string address { get; set; }
    }
}
