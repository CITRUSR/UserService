using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;

namespace UserService.Application.Common.Cache;

public class CacheService(IDistributedCache cache) : ICacheService
{
    private readonly IDistributedCache _cache = cache;

    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };

    public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory,
        CancellationToken cancellationToken = default) where T : class
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

    public async Task<T?> GetObjectAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
        where T : class
    {
        var objectString = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (string.IsNullOrWhiteSpace(objectString))
        {
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(objectString, _settings);
    }

    public Task<string?> GetStringAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        return _cache.GetStringAsync(cacheKey, cancellationToken);
    }

    public async Task SetObjectAsync<T>(string cacheKey, T value, CancellationToken cancellationToken = default)
        where T : class
    {
        var t = JsonConvert.SerializeObject(value, _settings);
        await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(value, _settings), cancellationToken);
    }

    public async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(cacheKey, cancellationToken);
    }

    public async Task RemovePagesWithObjectAsync<T, K>(K id, Func<T, K, bool> pred,
        CancellationToken cancellationToken = default) where T : class
    {
        for (int i = 1; i <= CacheConstants.PagesForCaching; i++)
        {
            var page = await GetObjectAsync<PaginationList<T>>(CacheKeys.GetEntities<T>(i, 10), cancellationToken);

            if (page == null)
            {
                continue;
            }

            if (ObjectExistsInPage<T, K>(page, id, pred))
            {
                await RemovePagesStartingFrom<T>(i);
            }
        }
    }

    private bool ObjectExistsInPage<T, K>(PaginationList<T> page, K id, Func<T, K, bool> pred)
    {
        return page.Items.Any(x => pred(x, id));
    }

    private async Task RemovePagesStartingFrom<T>(int startPage)
    {
        for (int j = startPage; j <= CacheConstants.PagesForCaching; j++)
        {
            await _cache.RemoveAsync(CacheKeys.GetEntities<T>(j, 10));
        }
    }
}