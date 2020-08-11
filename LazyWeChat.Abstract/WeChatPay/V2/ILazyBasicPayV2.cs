using LazyWeChat.Models.WeChatPay.V2;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.WeChatPay.V2
{
    public interface ILazyBasicPayV2
    {
        Task<dynamic> UnifiedOrderAsync(UnifiedOrderModel unifiedOrderModel);

        /// <summary>
        /// 把该方法的返回值直接放在html页面即可触发JsApi支付(页面需引用jssdk)
        /// <script type="text/javascript" src="http://res.wx.qq.com/open/js/jweixin-1.6.0.js"></script>
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <param name="body"></param>
        /// <param name="total_fee"></param>
        /// <param name="openid"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<string> GetJsApiScriptAsync(string out_trade_no, string body, double total_fee, string openid, string notify_url);

        /// <summary>
        /// 以第一种扫码方式支付，该方式只在生成URL的时候只初始化product_id以及appid,mch_id等参数，详细的参数以及prepay_id将在监听程序中根据product_id生成并返回
        /// </summary>
        /// <returns></returns>
        string GetPrePayUrl(string out_trade_no);

        byte[] GetPrePayQRCode(string out_trade_no);

        /// <summary>
        /// 以第二种扫码方式支付，该方式只在生成URL的时候通过product_id获取到商品详细信息，并直接调用统一支付接口生产URL，无需监听程序
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <param name="body"></param>
        /// <param name="total_fee"></param>
        /// <param name="notify_url"></param>
        /// <returns></returns>
        Task<string> GetPayUrl(string out_trade_no, string body, double total_fee, string notify_url);

        Task<byte[]> GetPayQRCodeAsync(string out_trade_no, string body, double total_fee, string notify_url);

        /// <summary>
        /// 微信中付款码支付,该接口可能不会立刻获取到支付结果(如果是免密支付的情况下，可以直接获得结果),需要调用OrderQueryAsync查看订单付款状态
        /// </summary>
        /// <param name="body">商品描述</param>
        /// <param name="total_fee">商品价格,单位：分</param>
        /// <param name="auth_code">授权码:微信->我->钱包->付款 中的18位纯数字，以11、12、13、14、15开头的授权码</param>
        /// <returns></returns>
        Task<dynamic> RunMicroPayAsync(string out_trade_no, string body, double total_fee, string auth_code);

        Task<dynamic> RunH5PayAsync(H5PayModel h5PayModel);

        Task<dynamic> RunMiniPayAsync(string out_trade_no, string openid, string body, double total_fee, string spbill_create_ip, string notify_url);

        Task<dynamic> OrderQueryAsync(string out_trade_no, string transaction_id);

        string ToNotifyUrl(HttpContext httpContext);
    }

    public class H5PayModel
    {
        public string out_trade_no { get; set; }

        public string body { get; set; }

        public int total_fee { get; set; }

        public string notify_url { get; set; }

        public string spbill_create_ip { get; set; }

        public dynamic h5_info { get; set; }
    }
}
