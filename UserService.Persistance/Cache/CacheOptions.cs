using UserService.Application.Abstraction;

namespace UserService.Persistance.Cache;

public class CacheOptions : ICacheOptions
{
    public int SlidingExpirationTime { get; init; }
    public int PagesForCaching { get; init; }
    public int EntitiesPerPage { get; init; }
}
