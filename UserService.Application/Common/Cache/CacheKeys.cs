namespace UserService.Application.Common.Cache;

public static class CacheKeys
{
    public static string GroupById(int groupId) => $"group-{groupId}";
    public static string GetGroups(int currentPage, int pageSize) => $"groups-{currentPage}-{pageSize}";
}