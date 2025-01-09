namespace UserService.Application.Common.Cache;

public static class CacheKeys
{
    public static string ById<T, K>(K id) => $"{typeof(T)}-{id}";

    public static string EntitiesByGroupId<T>(int groupId) => $"{typeof(T)}-group-{groupId}";

    public static string GetEntities<T>() => $"{typeof(T)}-pages";
}
