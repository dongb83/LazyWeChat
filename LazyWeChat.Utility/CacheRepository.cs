using Microsoft.Extensions.Caching.Memory;
using System;

namespace LazyWeChat.Utility
{
    internal class CacheRepository
    {
        private readonly IMemoryCache _memoryCache;

        public CacheRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void CacheInsertAddSeconds(string key, object value, int second)
        {
            if (value == null) return;
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(second)));
        }

        public void CacheInsertFromSeconds(string key, object value, int second)
        {
            if (value == null) return;
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(second)));
        }

        public void CacheInsert(string key, object value)
        {
            _memoryCache.Set(key, value);
        }

        public void CacheNull(string key)
        {
            _memoryCache.Remove(key);
        }

        public object CacheValue(string key)
        {
            return _memoryCache.Get(key);
        }
    }
}
