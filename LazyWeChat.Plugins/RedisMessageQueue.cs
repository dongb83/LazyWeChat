using LazyWeChat.Abstract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Plugins
{
    public class RedisMessageQueue : IMessageQueue
    {
        private readonly string _connectionString;
        private static readonly object thisLock = new object();
        private static ConnectionMultiplexer redisMultiplexer;
        IDatabase db;
        private const string redisKey = "LazyWeChatRedisMQList";

        public RedisMessageQueue(string connectionString)
        {
            _connectionString = connectionString;

            if (redisMultiplexer == null)
            {
                lock (thisLock)
                {
                    if (redisMultiplexer == null)
                    {
                        redisMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
                        db = redisMultiplexer.GetDatabase();
                    }
                }
            }
        }

        public async Task<string> Pop() => await db.ListLeftPopAsync(redisKey);

        public async Task Push(string item) => await db.ListLeftPushAsync(redisKey, item);
    }
}
