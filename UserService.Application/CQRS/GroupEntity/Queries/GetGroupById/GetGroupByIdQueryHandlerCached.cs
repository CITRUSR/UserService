using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;

public class GetGroupByIdQueryHandlerCached(GetGroupByIdQueryHandler handler, ICacheService cacheService)
    : IRequestHandler<GetGroupByIdQuery, Group>
{
    private readonly GetGroupByIdQueryHandler _handler = handler;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Group> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var key = CacheKeys.ById<Group, int>(request.Id);

        Group group = await _cacheService.GetOrCreateAsync<Group>(key,
            async () => await _handler.Handle(request, cancellationToken), cancellationToken);

        return group;
    }
}