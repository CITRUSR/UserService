namespace UserService.Application.Abstraction;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string cacheKey,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default
    )
        where T : class;

    Task<T?> GetObjectAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
        where T : class;

    Task<string?> GetStringAsync(string cacheKey, CancellationToken cancellationToken = default);

    Task SetObjectAsync<T>(string cacheKey, T value, CancellationToken cancellationToken = default)
        where T : class;

    Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);

    Task RemovePagesWithObjectAsync<T, K>(
        Func<int, string> pageKey,
        K id,
        Func<T, K, bool> pred,
        CancellationToken cancellationToken = default
    )
        where T : class;
}
