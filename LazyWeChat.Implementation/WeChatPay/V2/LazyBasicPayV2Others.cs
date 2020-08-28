using LazyWeChat.Abstract.WeChatPay.V2;
using LazyWeChat.Models.WeChatPay.V2;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.WeChatPay.V2
{
    public partial class LazyBasicPayV2 : ILazyBasicPayV2
    {
        public virtual async Task<dynamic> OrderQueryAsync(string transaction_id, string out_trade_no)
        {
            OrderQueryModel orderQueryModel = new OrderQueryModel(_options.Value);

            if (string.IsNullOrEmpty(transaction_id))
                orderQueryModel.out_trade_no = out_trade_no;
            else
                orderQueryModel.transaction_id = transaction_id;

            var requestXml = orderQueryModel.Xml;
            var returnObject = await _httpRepository.PostParseValidateAsync(CONSTANT.ORDERQUERYURL, requestXml);
            return returnObject;
        }

        public virtual async Task<dynamic> CloseOrderAsync(string out_trade_no)
        {
            OrderQueryModel closeOrderModel = new OrderQueryModel(_options.Value);
            closeOrderModel.out_trade_no = out_trade_no;

            var requestXml = closeOrderModel.Xml;
            var returnObject = await _httpRepository.PostParseValidateAsync(CONSTANT.ORDERCLOSEURL, requestXml);
            return returnObject;
        }
    }
}
