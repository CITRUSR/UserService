using Microsoft.EntityFrameworkCore;

namespace UserService.Application.Common.Paging;

public class PaginationList<T>
{
    public List<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int MaxPage => Convert.ToInt32(Math.Ceiling((double)TotalCount / PageSize));

    public PaginationList()
    {
    }

    private PaginationList(List<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static async Task<PaginationList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginationList<T>(items, page, pageSize, totalCount);
    }
}