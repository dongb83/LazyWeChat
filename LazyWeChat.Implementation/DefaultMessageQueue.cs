using LazyWeChat.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Implementation
{
    public class DefaultMessageQueue : IMessageQueue
    {
        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        public DefaultMessageQueue()
        {

        }

        public DefaultMessageQueue(string connectionString)
        {

        }

        public async Task<string> Pop()
        {
            var returnObj = "";
            await Task.Run(() =>
            {
                if (queue.TryDequeue(out string workItem))
                {
                    returnObj = workItem;
                }
                else
                {
                    throw new ArgumentException(nameof(workItem));
                }
            });
            return returnObj;
        }

        public async Task Push(string item)
        {
            await Task.Run(() => queue.Enqueue(item));
        }
    }
}
