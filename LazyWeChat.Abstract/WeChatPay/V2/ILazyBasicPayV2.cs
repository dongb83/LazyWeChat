using LazyWeChat.Models.WeChatPay.V2;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.WeChatPay.V2
{
    public interface ILazyBasicPayV2
    {
        Task<dynamic> UnifiedOrderAsync(UnifiedOrderModel unifiedOrderModel);

        /// <summary>
        /// JsApi支付
        /// </summary>
        /// <param name="out_trade_no">商户订单号</param>
        /// <param name="body">商品描述</param>
        /// <param name="total_fee">商品价格,单位：分</param>
        /// <param name="openid">购买者openid</param>
        /// <param name="notify_url">通知地址</param>
        /// <returns></returns>
        Task<string> GetJsApiScriptAsync(string out_trade_no, string body, double total_fee, string openid, string notify_url);

        /// <summary>
        /// 扫码方式支付模式一
        /// </summary>
        /// <returns></returns>
        byte[] GetPrePayQRCode(string out_trade_no);

        /// <summary>
        /// 以第二种扫码方式支付
        /// </summary>
        /// <param name="out_trade_no">商品ID</param>
        /// <param name="body">商品描述</param>
        /// <param name="total_fee">总金额</param>
        /// <param name="notify_url">通知地址</param>
        /// <returns></returns>
        Task<byte[]> GetPayQRCodeAsync(string out_trade_no, string body, double total_fee, string notify_url);

        /// <summary>
        /// 付款码支付
        /// </summary>
        /// <param name="body">商品描述</param>
        /// <param name="total_fee">商品价格,单位：分</param>
        /// <param name="auth_code">授权码:微信->我->钱包->付款 中的18位纯数字，以11、12、13、14、15开头的授权码</param>
        /// <returns></returns>
        Task<dynamic> RunMicroPayAsync(string out_trade_no, string body, double total_fee, string auth_code);

        /// <summary>
        /// H5支付
        /// </summary>
        /// <param name="h5PayModel">H5支付的支付模型</param>
        /// <returns></returns>
        Task<dynamic> RunH5PayAsync(H5PayModel h5PayModel);

        /// <summary>
        /// 小程序支付
        /// </summary>
        /// <param name="out_trade_no">商户订单号</param>
        /// <param name="openid">购买者openid</param>
        /// <param name="body">商品描述</param>
        /// <param name="total_fee">商品价格,单位：分</param>
        /// <param name="spbill_create_ip">支付设备IP地址</param>
        /// <param name="notify_url">通知地址</param>
        /// <returns></returns>
        Task<dynamic> RunMiniPayAsync(string out_trade_no, string openid, string body, double total_fee, string spbill_create_ip, string notify_url);

        /// <summary>
        /// 查询支付结果
        /// </summary>
        /// <param name="transaction_id">微信订单号, 优先使用</param>
        /// <param name="out_trade_no">商户订单号</param>
        /// <returns></returns>
        Task<dynamic> OrderQueryAsync(string transaction_id, string out_trade_no);

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="out_trade_no">商户订单号</param>
        /// <returns></returns>
        Task<dynamic> CloseOrderAsync(string out_trade_no);

        string ToNotifyUrl(HttpContext httpContext);
    }

    public class H5PayModel
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string body { get; set; }

        /// <summary>
        /// 商品价格,单位：分
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        public string notify_url { get; set; }

        /// <summary>
        /// 设备IP地址
        /// </summary>
        public string spbill_create_ip { get; set; }

        /// <summary>
        /// 场景信息
        /// </summary>
        public dynamic h5_info { get; set; }
    }
}
