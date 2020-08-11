using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Abstract
{
    public interface IMessageQueue
    {
        Task<string> Pop();

        Task Push(string item);
    }
}
