using System;
using System.Collections.Concurrent;

namespace Projekat
{
    internal class MuseumCache
    {
        private static readonly ConcurrentDictionary<string, CacheItem> cache = new ConcurrentDictionary<string, CacheItem>();
        private const int MaxCapacity = 1000; // Maksimalni kapacitet cache-a
        private const int DefaultExpirationMinutes = 30; // Podrazumevano vreme isteka stavki u cache-u

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
                    // Stavka je istekla, ukloni je iz cache-a
                    cache.TryRemove(key, out _);
                }
            }
            return null; // Stavka ne postoji u cache-u ili je istekla
        }

        public static void Add(string key, string data, int expirationMinutes = DefaultExpirationMinutes)
        {
            var newItem = new CacheItem(data, DateTime.Now.AddMinutes(expirationMinutes));

            // Proveri da li je cache prekoračio kapacitet i ukloni najstarije stavke ako je potrebno
            while (cache.Count >= MaxCapacity)
            {
                RemoveOldestItem();
            }

            cache.TryAdd(key, newItem);
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
                cache.TryRemove(keyToRemove, out _);
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
