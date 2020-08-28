using LazyWeChat.Abstract;
using LazyWeChat.Abstract.WeChatPay.V3;
using LazyWeChat.Models;
using LazyWeChat.Models.WeChatPay.V3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.WeChatPay
{
    public static partial class CONSTANT
    {
        public const string GENERATEJSAPIORDERURL = "https://api.mch.weixin.qq.com/v3/pay/transactions/jsapi";
    }

    public class LazyBasicPayV3 : ILazyBasicPayV3
    {
        private readonly IOptions<LazyWeChatConfiguration> _options;
        private readonly IHttpRepository _httpRepository;
        private readonly ILogger<LazyBasicPayV3> _logger;

        public LazyBasicPayV3(
            IHttpRepository httpRepository,
            IOptions<LazyWeChatConfiguration> options,
            ILogger<LazyBasicPayV3> logger)
        {
            _httpRepository = httpRepository;
            _options = options;
            _logger = logger;
        }

        public virtual async Task<string> GenerateJsApiOrder()
        {
            JsApiOrderModel jsApiOrderModel = new JsApiOrderModel();
			jsApiOrderModel.out_trade_no = "1217752501201407033233368018";
            jsApiOrderModel.appid = _options.Value.AppID;
            jsApiOrderModel.time_expire = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-ddTHH:mm:ss");
            jsApiOrderModel.description = "LazyWeChat框架代码";
            jsApiOrderModel.notify_url = "http://test.lazywechat.cn/LazyWechatListener";
            jsApiOrderModel.mchid = _options.Value.MCHID;
            jsApiOrderModel.amount.currency = "CNY";
            jsApiOrderModel.amount.total = 1;
			jsApiOrderModel.payer.openid = "oNDiC0d-r7Su5mYCU-mXFSXuhmtQ";

            var requestJson = JsonConvert.SerializeObject(jsApiOrderModel);

            var a = @"{
	""time_expire"": ""2020-08-08T10:34:56+08:00"",
	""amount"": {
		""total"": 100,
		""currency"": ""CNY""
	},
	""mchid"": ""1501396621"",
	""description"": ""Image形象店-深圳腾大-QQ公仔"",
	""notify_url"": ""http://test.lazywechat.cn/LazyWechatListener"",
	""payer"": {
		""openid"": ""oNDiC0d-r7Su5mYCU-mXFSXuhmtQ""
	},
	""out_trade_no"": ""1217752501201407033233368018"",
	""goods_tag"": ""WXG"",
	""appid"": ""wxbb23a029883b991d"",
	""attach"": ""自定义数据说明"",
	""detail"": {
		""invoice_id"": ""wx123"",
		""goods_detail"": [{
			""goods_name"": ""iPhoneX 256G"",
			""wechatpay_goods_id"": ""1001"",
			""quantity"": 1,
			""merchant_goods_id"": ""商品编码"",
			""unit_price"": 828800
		}, {
			""goods_name"": ""iPhoneX 256G"",
			""wechatpay_goods_id"": ""1001"",
			""quantity"": 1,
			""merchant_goods_id"": ""商品编码"",
			""unit_price"": 828800
		}],
		""cost_price"": 608800
	},
	""scene_info"": {
		""store_info"": {
			""address"": ""广东省深圳市南山区科技中一道10000号"",
			""area_code"": ""440305"",
			""name"": ""腾讯大厦分店"",
			""id"": ""0001""
		},
		""device_id"": ""013467007045764"",
		""payer_client_ip"": ""14.23.150.211""
	}
}";
            var returnObject = await _httpRepository.PostParseValidateAsync(CONSTANT.GENERATEJSAPIORDERURL, a, "prepay_id");
            return returnObject.prepay_id;
        }
    }
}
