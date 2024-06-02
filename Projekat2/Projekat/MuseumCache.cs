using System;
using System.Collections.Concurrent;

namespace Projekat
{
    internal class MuseumCache
    {
        private static readonly ConcurrentDictionary<string, CacheItem> cache = new ConcurrentDictionary<string, CacheItem>();
        private const int MaxCapacity = 1000;
        private const int DefaultExpirationMinutes = 30;
        private static readonly object lockObject = new object();

        public static string Get(string key)
        {
            if (cache.TryGetValue(key, out CacheItem item))
            {
                if (item.ExpiresAt >= DateTime.Now)
                {
                    return item.Data;
                }
                else
                {
                    lock (lockObject)
                    {
                        cache.TryRemove(key, out _);
                    }
                }
            }
            return null;
        }

        public static void Add(string key, string data, int expirationMinutes = DefaultExpirationMinutes)
        {
            var newItem = new CacheItem(data, DateTime.Now.AddMinutes(expirationMinutes));

            lock (lockObject)
            {
                while (cache.Count >= MaxCapacity)
                {
                    RemoveOldestItem();
                }

                cache.TryAdd(key, newItem);
            }
        }

        private static void RemoveOldestItem()
        {
            DateTime oldestExpiration = DateTime.MaxValue;
            string keyToRemove = null;

            foreach (var kvp in cache)
            {
                if (kvp.Value.ExpiresAt < oldestExpiration)
                {
                    oldestExpiration = kvp.Value.ExpiresAt;
                    keyToRemove = kvp.Key;
                }
            }

            if (keyToRemove != null)
            {
                lock (lockObject)
                {
                    cache.TryRemove(keyToRemove, out _);
                }
            }
        }

        private class CacheItem
        {
            public string Data { get; }
            public DateTime ExpiresAt { get; }

            public CacheItem(string data, DateTime expiresAt)
            {
                Data = data;
                ExpiresAt = expiresAt;
            }
        }
    }
}
