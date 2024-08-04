namespace UserService.Application.Common.Cache;

public static class CacheKeys
{
    public static string ById<T, K>(K id) => $"{typeof(T)}-{id}";

    public static string GetEntities<T>(int currentPage, int pageSize) =>
        $"{typeof(T)}-{currentPage}-{pageSize}";
}
