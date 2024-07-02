using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace UserService.Persistance.Extensions;

public static class RedisExtension
{
    public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache cache, string key,
        Func<IRequest, CancellationToken, T> fact,
        IRequest request,
        CancellationToken cancellationToken = default)
    {
        var cachedValue = await cache.GetStringAsync(key, cancellationToken);

        T? value;
        if (!string.IsNullOrWhiteSpace(cachedValue))
        {
            value = JsonSerializer.Deserialize<T>(cachedValue);

            if (value != null)
            {
                return value;
            }
        }

        value = fact(request, cancellationToken);

        await cache.SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
        
        return value;
    }
}