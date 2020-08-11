using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract.WeChatPay.V3
{
    public interface ILazyBasicPayV3
    {
        Task<string> GenerateJsApiOrder();
    }
}
