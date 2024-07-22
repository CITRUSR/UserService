using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace UserService.Persistance.Extensions;

public static class RedisExtension
{
    public static async Task<TResponse> GetOrCreateAsync<TQuery, TResponse>(this IDistributedCache cache, string key,
        Func<TQuery, CancellationToken, Task<TResponse>> fact,
        TQuery request,
        CancellationToken cancellationToken = default) where TQuery : IRequest<TResponse>
    {
        var cachedValue = await cache.GetStringAsync(key, cancellationToken);

        TResponse? value;
        if (!string.IsNullOrWhiteSpace(cachedValue))
        {
            value = JsonConvert.DeserializeObject<TResponse>(cachedValue);

            if (value != null)
            {
                return value;
            }
        }

        value = await fact(request, cancellationToken);

        await cache.SetStringAsync(key, JsonConvert.SerializeObject(value), cancellationToken);

        return value;
    }
}