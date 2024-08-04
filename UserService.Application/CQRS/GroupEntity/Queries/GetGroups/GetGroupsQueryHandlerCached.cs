using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroups;

public class GetGroupsQueryHandlerCached(GetGroupsQueryHandler handler, ICacheService cacheService)
    : IRequestHandler<GetGroupsQuery, PaginationList<Group>>
{
    private readonly GetGroupsQueryHandler _handler = handler;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<PaginationList<Group>> Handle(
        GetGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.Page <= CacheConstants.PagesForCaching
            && request
                is {
                    SortState: GroupSortState.GroupAsc,
                    GraduatedStatus: GroupGraduatedStatus.OnlyActive
                }
        )
        {
            var key = CacheKeys.GetEntities<Group>(request.Page, request.PageSize);

            return await _cacheService.GetOrCreateAsync<PaginationList<Group>>(
                key,
                async () => await _handler.Handle(request, cancellationToken),
                cancellationToken
            );
        }

        return await _handler.Handle(request, cancellationToken);
    }
}
