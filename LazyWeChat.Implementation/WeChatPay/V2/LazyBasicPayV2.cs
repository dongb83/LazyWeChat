using LazyWeChat.Abstract;
using LazyWeChat.Abstract.WeChatPay.V2;
using LazyWeChat.Models;
using LazyWeChat.Models.WeChatPay.V2;
using LazyWeChat.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.WeChatPay.V2
{
    public static partial class CONSTANT
    {
        public const string UNIFIEDORDERURL = "https://api.mch.weixin.qq.com/pay/unifiedorder";

        public const string MICROPAYURL = "https://api.mch.weixin.qq.com/pay/micropay";

        public const string ORDERQUERYURL = "https://api.mch.weixin.qq.com/pay/orderquery";

        public const string ORDERCLOSEURL = "https://api.mch.weixin.qq.com/pay/closeorder";
    }

    public partial class LazyBasicPayV2 : ILazyBasicPayV2
    {
        private readonly IHttpRepository _httpRepository;
        private readonly IOptions<LazyWeChatConfiguration> _options;
        private readonly IQRGenerator _qrGenerator;

        public LazyBasicPayV2(
            IHttpRepository httpRepository,
            IQRGenerator qrGenerator,
            IOptions<LazyWeChatConfiguration> options)
        {
            _httpRepository = httpRepository;
            _qrGenerator = qrGenerator;
            _options = options;
        }

        public virtual async Task<dynamic> UnifiedOrderAsync(UnifiedOrderModel unifiedOrderModel)
        {
            var xml = unifiedOrderModel.XML;
            var response = await _httpRepository.PostParseValidateAsync(CONSTANT.UNIFIEDORDERURL, xml, "prepay_id");
            return response;
        }

        public virtual async Task<string> GetJsApiScriptAsync(string out_trade_no, string body, double total_fee, string openid, string notify_url)
        {
            JsApiModel jsApiModel = new JsApiModel(_options.Value);
            var unifiedOrderModel = new UnifiedOrderModel(_options.Value);
            unifiedOrderModel.out_trade_no = out_trade_no;
            unifiedOrderModel.body = body;
            unifiedOrderModel.total_fee = total_fee;
            unifiedOrderModel.openid = openid;
            unifiedOrderModel.trade_type = TradeType.JSAPI.ToString();
            unifiedOrderModel.notify_url = notify_url;

            var unifiedOrderResult = await UnifiedOrderAsync(unifiedOrderModel);
            var package = "prepay_id=" + unifiedOrderResult.prepay_id;
            jsApiModel.package = package;

            var scriptWeixinJSBridge = @"
                            function onBridgeReady()
                            {{
                                WeixinJSBridge.invoke(
                                    'getBrandWCPayRequest',
                                    {0},
                                    function (res)
                                    {{
                                        WeixinJSBridge.log(res.err_msg);
                                    }}
                                );
                            }}
                            if (typeof WeixinJSBridge == 'undefined'){{
                               if (document.addEventListener)
                                {{
                                        document.addEventListener('WeixinJSBridgeReady', onBridgeReady, false);
                                }}
                                else if (document.attachEvent)
                                {{
                                        document.attachEvent('WeixinJSBridgeReady', onBridgeReady);
                                        document.attachEvent('onWeixinJSBridgeReady', onBridgeReady);
                                }}
                            }}
                            else{{
                               onBridgeReady();
                            }}";

            var script = string.Format(scriptWeixinJSBridge, jsApiModel.JSON);
            return script;
        }

        public virtual string ToNotifyUrl(HttpContext httpContext)
        {
            var httpRequest = httpContext.Request;
            return new StringBuilder()
                .Append(httpRequest.Scheme)
                .Append("://")
                .Append(httpRequest.Host)
                .Append($"/{_options.Value.LazyWechatListener}")
                .ToString();
        }

        private string GetPrePayUrl(string out_trade_no)
        {
            NativePayModel model = new NativePayModel(_options.Value);
            model.product_id = out_trade_no;
            return model.URL;
        }

        public virtual byte[] GetPrePayQRCode(string out_trade_no) => _qrGenerator.Generate(GetPrePayUrl(out_trade_no));

        public virtual async Task<string> GetPayUrl(string out_trade_no, string body, double total_fee, string notify_url)
        {
            var unifiedOrderModel = new UnifiedOrderModel(_options.Value);
            unifiedOrderModel.out_trade_no = out_trade_no;
            unifiedOrderModel.product_id = out_trade_no;
            unifiedOrderModel.body = body;
            unifiedOrderModel.total_fee = total_fee;
            unifiedOrderModel.trade_type = TradeType.NATIVE.ToString();
            unifiedOrderModel.notify_url = notify_url;
            unifiedOrderModel.time_start = DateTime.Now.ToString("yyyyMMddHHmmss");
            unifiedOrderModel.time_expire = DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss");
            unifiedOrderModel.spbill_create_ip = "";

            //非必填字段
            //payModel.attach = attach;
            //payModel.goods_tag = goods_tag;

            var result = await UnifiedOrderAsync(unifiedOrderModel);

            string url = result.code_url.ToString();//获得统一下单接口返回的二维码链接
            return url;
        }

        public virtual async Task<byte[]> GetPayQRCodeAsync(string out_trade_no, string body, double total_fee, string notify_url) => _qrGenerator.Generate(await GetPayUrl(out_trade_no, body, total_fee, notify_url));

        public virtual async Task<dynamic> RunMicroPayAsync(string out_trade_no, string body, double total_fee, string auth_code)
        {
            MicroPayModel microPayModel = new MicroPayModel(_options.Value);
            microPayModel.out_trade_no = out_trade_no;
            microPayModel.body = body;
            microPayModel.total_fee = total_fee;
            microPayModel.auth_code = auth_code;

            var requestXml = microPayModel.Xml;
            var returnObject = await _httpRepository.PostParseValidateAsync(CONSTANT.MICROPAYURL, requestXml);
            return returnObject;
        }

        public virtual async Task<dynamic> RunH5PayAsync(H5PayModel h5PayModel)
        {
            var unifiedOrderModel = new UnifiedOrderModel(_options.Value);
            unifiedOrderModel.out_trade_no = h5PayModel.out_trade_no;
            unifiedOrderModel.body = h5PayModel.body;
            unifiedOrderModel.total_fee = h5PayModel.total_fee;
            unifiedOrderModel.trade_type = TradeType.MWEB.ToString();
            unifiedOrderModel.notify_url = h5PayModel.notify_url;
            unifiedOrderModel.spbill_create_ip = h5PayModel.spbill_create_ip;
            unifiedOrderModel.scene_info = new ExpandoObject();
            unifiedOrderModel.scene_info.h5_info = h5PayModel.h5_info;

            var unifiedOrderResult = await UnifiedOrderAsync(unifiedOrderModel);
            return unifiedOrderResult;
        }

        public virtual async Task<dynamic> RunMiniPayAsync(string out_trade_no, string openid, string body, double total_fee, string spbill_create_ip, string notify_url)
        {
            var unifiedOrderModel = new UnifiedOrderModel(_options.Value);
            unifiedOrderModel.out_trade_no = out_trade_no;
            unifiedOrderModel.body = body;
            unifiedOrderModel.total_fee = total_fee;
            unifiedOrderModel.trade_type = TradeType.JSAPI.ToString();
            unifiedOrderModel.spbill_create_ip = spbill_create_ip;
            unifiedOrderModel.openid = openid;
            unifiedOrderModel.notify_url = notify_url;

            var unifiedOrderResult = await UnifiedOrderAsync(unifiedOrderModel);
            MiniPayModel miniPayModel = new MiniPayModel(_options.Value);
            miniPayModel.package= $"prepay_id={unifiedOrderResult.prepay_id}";

            return miniPayModel.Parameters.ToDynamic();
        }
    }
}
