namespace UserService.Application.Common.Cache;

public static class CacheKeys
{
    public static string ById<T, K>(K id) => $"{typeof(T)}-{id}";

    public static string GetEntities<T>(int currentPage) => $"{typeof(T)}-Entities-{currentPage}";
}
