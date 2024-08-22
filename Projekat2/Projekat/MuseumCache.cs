using System;
using System.Collections.Generic;

class MuseumCache
{
    public static int cacheIsEmpty = 0;
    public static readonly int Capacity = 1000; 
    public static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

    public static readonly System.Timers.Timer cacheCleanupTimer = new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);

    static MuseumCache()
    {
        cacheCleanupTimer.Elapsed += (sender, e) => CacheCleanup();
    }

    public static void CacheCleanup()
    {
        Console.WriteLine("Brisanje iz kesa:");
        foreach (var key in cache.Keys)
        {
            Console.WriteLine($" - {key}");
        }
        cache.Clear();
        cacheIsEmpty = 0;
        cacheCleanupTimer.Stop();
        Console.WriteLine("Stopiran je tajmer");
    }

    public static void AddToCache(string key, string value)
    {
        if (cacheIsEmpty == 0)
        {
            cacheCleanupTimer.Start();
            cacheIsEmpty = 1;
            Console.WriteLine("Startovan je tajmer");
        }

        if (cache.Count >= Capacity)
        {
            var firstKey = new List<string>(cache.Keys)[0];
            cache.Remove(firstKey);
            Console.WriteLine($"Izbaceno iz kesa: {firstKey}");
        }

        cache[key] = value;
        Console.WriteLine($"Dodato u kes: {key}");
    }

    public static bool TryGetFromCache(string key, out string value)
    {
        bool isFound = cache.TryGetValue(key, out value);
        if (isFound)
        {
            Console.WriteLine($"Pronadjeno u kesu: {key}");
        }
        return isFound;
    }

    public static bool IsInCache(string key)
    {
        bool isInCache = cache.ContainsKey(key);
        if (isInCache)
        {
            Console.WriteLine($"Ključ je u kešu: {key}");
        }
        return isInCache;
    }
}
