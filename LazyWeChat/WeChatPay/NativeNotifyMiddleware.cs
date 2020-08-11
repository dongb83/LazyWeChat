using LazyWeChat.Abstract;
using LazyWeChat.Abstract.WeChatPay.V2;
using LazyWeChat.Models;
using LazyWeChat.Models.WeChatPay.V2;
using LazyWeChat.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.WeChatPay
{
    public class NativeNotifyMiddleware
    {
        private readonly RequestDelegate _next;
        private IMessageQueue _messageQueue;
        private ILazyBasicPayV2 _lazyBasicPayV2;
        private readonly ILogger<NativeNotifyMiddleware> _logger;
        private readonly IOptions<LazyWeChatConfiguration> _options;
        private Func<string, (string, int)> _onGetProductInfo;

        public NativeNotifyMiddleware(
            RequestDelegate next,
            ILogger<NativeNotifyMiddleware> logger,
            IOptions<LazyWeChatConfiguration> options,
            ILazyBasicPayV2 lazyBasicPayV2,
            Func<string, (string, int)> onGetProductInfo,
            Type implementation)
        {
            _next = next;
            _logger = logger;
            _lazyBasicPayV2 = lazyBasicPayV2;
            _options = options;
            _onGetProductInfo = onGetProductInfo;
            _messageQueue = (IMessageQueue)Activator.CreateInstance(implementation);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            if (request.Path.Value.Contains(_options.Value.NativeNotifyListener))
            {
                #region 以stream的方式获取微信post到监听程序的数据:数据类型为XML
                var inputXml = "";
                using (StreamReader stream = new StreamReader(request.Body, Encoding.UTF8))
                {
                    inputXml = await stream.ReadToEndAsync();
                }
                #endregion

                var info = "inputXml:{0} received from wechat at {1}";
                _logger.LogInformation(info, inputXml, DateTime.Now);

                if (!string.IsNullOrEmpty(inputXml))
                {
                    //转换数据格式并验证签名
                    var notifyData = inputXml.FromXml();

                    //第一种扫码支付方式只能在input stream中获取到product_id，并且在之前已经规定product_id中传递out_trade_no
                    var product_id = notifyData.GetValue("product_id").ToString();

                    (string body, int total_fee) = _onGetProductInfo(product_id);

                    var unifiedOrderModel = new UnifiedOrderModel(_options.Value);
                    unifiedOrderModel.out_trade_no = product_id;
                    unifiedOrderModel.product_id = product_id;
                    unifiedOrderModel.body = body;
                    unifiedOrderModel.total_fee = total_fee;
                    unifiedOrderModel.trade_type = TradeType.NATIVE.ToString();
                    unifiedOrderModel.openid = notifyData.GetValue("openid").ToString();
                    unifiedOrderModel.notify_url = _lazyBasicPayV2.ToNotifyUrl(context);
                    var unifiedOrderResult = await _lazyBasicPayV2.UnifiedOrderAsync(unifiedOrderModel);

                    var return_code = notifyData.CheckSign(_options.Value.Key, out string return_msg) ? "SUCCESS" : "FAIL";
                    SortedDictionary<string, object> data = new SortedDictionary<string, object>();
                    data.SetValue("return_code", return_code);
                    data.SetValue("return_msg", return_msg);
                    data.SetValue("appid", _options.Value.AppID);
                    data.SetValue("mch_id", _options.Value.MCHID);
                    data.SetValue("nonce_str", _options.Value.NonceStr);
                    string prepay_id = unifiedOrderResult.prepay_id.ToString();
                    data.SetValue("prepay_id", prepay_id);
                    data.SetValue("result_code", return_code);
                    data.SetValue("err_code_des", return_msg);
                    data.SetValue("sign", data.MakeSign(_options.Value.Key));
                    await context.Response.WriteAsync(data.ToXml());
                }
                else
                {
                    await context.Response.WriteAsync("PINGPONG");
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
