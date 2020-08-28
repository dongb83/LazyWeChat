using LazyWeChat.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Plugins
{
    public class KafkaMessageQueue : IMessageQueue
    {
        public Task<string> Pop()
        {
            throw new NotImplementedException();
        }

        public Task Push(string item)
        {
            throw new NotImplementedException();
        }
    }
}
