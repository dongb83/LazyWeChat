using LazyWeChat.Abstract;
using LazyWeChat.Abstract.WeChatPay.V2;
using LazyWeChat.Models;
using LazyWeChat.Models.WeChatPay.V2;
using LazyWeChat.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation.WeChatPay.V2
{
    public partial class LazyBasicPayV2 : ILazyBasicPayV2
    {
        public async Task<dynamic> OrderQueryAsync(string out_trade_no, string transaction_id)
        {
            OrderQueryModel orderQueryModel = new OrderQueryModel(_options.Value);

            if (!string.IsNullOrEmpty(out_trade_no))
                orderQueryModel.out_trade_no = out_trade_no;
            else
                orderQueryModel.transaction_id = transaction_id;

            var requestXml = orderQueryModel.Xml;
            var returnObject = await _httpRepository.PostParseValidateAsync(CONSTANT.ORDERQUERYURL, requestXml);
            return returnObject;
        }
    }
}
