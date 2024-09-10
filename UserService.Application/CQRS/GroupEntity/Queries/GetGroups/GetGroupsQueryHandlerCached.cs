using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroups;

public class GetGroupsQueryHandlerCached(
    IRequestHandler<GetGroupsQuery, GetGroupsResponse> handler,
    ICacheService cacheService,
    ICacheOptions cacheOptions
) : IRequestHandler<GetGroupsQuery, GetGroupsResponse>
{
    private readonly IRequestHandler<GetGroupsQuery, GetGroupsResponse> _handler = handler;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ICacheOptions _cacheOptions = cacheOptions;

    public async Task<GetGroupsResponse> Handle(
        GetGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.Page <= _cacheOptions.PagesForCaching
            && request
                is {
                    SortState: GroupSortState.GroupAsc,
                    GraduatedStatus: GroupGraduatedStatus.OnlyActive,
                    DeletedStatus: DeletedStatus.OnlyActive
                }
        )
        {
            var key = CacheKeys.GetEntities<Group>();

            var entities = await _cacheService.GetOrCreateAsync<GetGroupsResponse>(
                key,
                async () =>
                    await _handler.Handle(
                        request with
                        {
                            PageSize = _cacheOptions.PagesForCaching * _cacheOptions.EntitiesPerPage
                        },
                        cancellationToken
                    ),
                cancellationToken
            );

            entities.Groups = [.. entities.Groups.Take(request.PageSize)];

            return entities;
        }

        return await _handler.Handle(request, cancellationToken);
    }
}
