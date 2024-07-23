using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;
using UserService.Persistance.Extensions;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroups;

public class GetGroupsQueryHandlerCached(IDistributedCache cache, GetGroupsQueryHandler handler)
    : IRequestHandler<GetGroupsQuery, PaginationList<Group>>
{
    private readonly IDistributedCache _cache = cache;
    private readonly GetGroupsQueryHandler _handler = handler;

    public async Task<PaginationList<Group>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        int[] pagesForCaching = [1, 2, 3];

        if (pagesForCaching.Contains(request.Page) &&
            request is { SortState: GroupSortState.GroupAsc, GraduatedStatus: GroupGraduatedStatus.OnlyActive })
        {
            return await _cache.GetOrCreateAsync(CacheKeys.GetGroups(request.Page, request.PageSize),
                _handler.Handle,
                request, cancellationToken);
        }

        return await _handler.Handle(request, cancellationToken);
    }
}