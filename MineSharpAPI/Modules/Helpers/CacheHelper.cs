using MineSharpAPI.Modules.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace MineSharpAPI.Modules.Helpers;

public class CacheHelper
{
    /*
     * Helper per settare al cache di redis, IDIstributedcache può essere inizializzato ad inizio file, cachekey anche
     * però poi aggiungendogli qualcosa di unico dopo, T è la classe da passare da serializzare a json e timespan è per la
     * scadenza della cache
     * N.B.:Qquando si chiama CacheObject bisogna includere nelle <> cosa va serializzato
     */
    public static void CacheObject<T>(IDistributedCache cache, string cacheKey, T obj, TimeSpan expiration)
    {
        if (cache == null || obj == null || string.IsNullOrWhiteSpace(cacheKey))
        {
            throw new ArgumentException("Invalid cache parameters");
        }

        var serializedObj = JsonConvert.SerializeObject(obj);

        var cacheOptions = new DistributedCacheEntryOptions
        {
             AbsoluteExpirationRelativeToNow = expiration
        };

        cache.SetStringAsync(cacheKey, serializedObj, cacheOptions);
    }
    
    /*
     * Più o meno stesso concetto di sopra però prendo dalla cache
     */
    public static void GetCachedObj<T>(IDistributedCache cache, string cacheKey, out T? obj)
    {
        var cachedData =  cache.GetString(cacheKey);
        if (string.IsNullOrEmpty(cachedData))
        {
            obj = default;
            return;
        }
        obj = JsonConvert.DeserializeObject<T>(cachedData);
    }
}