using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UserService.Application.Abstraction;

namespace UserService.Persistance.Cache;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly CacheOptions _cacheOptions;

    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };

    private readonly DistributedCacheEntryOptions _cacheOptionsSettings;

    public CacheService(IDistributedCache cache, IOptions<CacheOptions> cacheOptions)
    {
        _cache = cache;
        _cacheOptions = cacheOptions.Value;
        _cacheOptionsSettings = new DistributedCacheEntryOptions().SetSlidingExpiration(
            TimeSpan.FromMinutes(_cacheOptions.SlidingExpirationTime)
        );
    }

    public async Task<T> GetOrCreateAsync<T>(
        string cacheKey,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        string? cachedString = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrWhiteSpace(cachedString))
        {
            return JsonConvert.DeserializeObject<T>(cachedString, _settings);
        }

        var value = await factory();

        await SetObjectAsync<T>(cacheKey, value, cancellationToken);

        return value;
    }

    public async Task<T?> GetObjectAsync<T>(
        string cacheKey,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        var objectString = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (string.IsNullOrWhiteSpace(objectString))
        {
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(objectString, _settings);
    }

    public Task<string?> GetStringAsync(
        string cacheKey,
        CancellationToken cancellationToken = default
    )
    {
        return _cache.GetStringAsync(cacheKey, cancellationToken);
    }

    public async Task SetObjectAsync<T>(
        string cacheKey,
        T value,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        await _cache.SetStringAsync(
            cacheKey,
            JsonConvert.SerializeObject(value, _settings),
            _cacheOptionsSettings,
            cancellationToken
        );
    }

    public async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(cacheKey, cancellationToken);
    }
}
