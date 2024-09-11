namespace UserService.Application.Abstraction;

public interface ICacheOptions
{
    public int SlidingExpirationTime { get; init; }
    public int PagesForCaching { get; init; }
    public int EntitiesPerPage { get; init; }
}
