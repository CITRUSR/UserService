using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;
using UserService.Persistance.Extensions;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;

public class GetGroupsByIdQueryHandlerCached(GetGroupByIdQueryHandler handler, IDistributedCache cache)
    : IRequestHandler<GetGroupByIdQuery, Group>
{
    private readonly GetGroupByIdQueryHandler _handler = handler;
    private readonly IDistributedCache _cache = cache;

    public async Task<Group> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        Group group = await _cache.GetOrCreateAsync<GetGroupByIdQuery, Group>(CacheKeys.GroupById(request.Id),
            _handler.Handle, request,
            cancellationToken);

        return group;
    }
}