using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;

public class GetGroupByIdQueryHandlerCached(
    IRequestHandler<GetGroupByIdQuery, GroupDto> handler,
    ICacheService cacheService
) : IRequestHandler<GetGroupByIdQuery, GroupDto>
{
    private readonly IRequestHandler<GetGroupByIdQuery, GroupDto> _handler = handler;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GroupDto> Handle(
        GetGroupByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var key = CacheKeys.ById<Group, int>(request.Id);

        GroupDto group = await _cacheService.GetOrCreateAsync<GroupDto>(
            key,
            async () => await _handler.Handle(request, cancellationToken),
            cancellationToken
        );

        return group;
    }
}
