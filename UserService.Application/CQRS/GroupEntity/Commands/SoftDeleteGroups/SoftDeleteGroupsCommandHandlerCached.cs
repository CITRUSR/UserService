using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;

public class SoftDeleteGroupsCommandHandlerCached(
    ICacheService cacheService,
    SoftDeleteGroupsCommandHandler handler
) : IRequestHandler<SoftDeleteGroupsCommand, List<Group>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly SoftDeleteGroupsCommandHandler _handler = handler;

    public async Task<List<Group>> Handle(
        SoftDeleteGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await _handler.Handle(request, cancellationToken);

        foreach (var group in groups)
        {
            var key = CacheKeys.ById<Group, int>(group.Id);

            await _cacheService.SetObjectAsync<Group>(key, group, cancellationToken);

            await _cacheService.RemovePagesWithObjectAsync<Group, int>(
                group.Id,
                (group1, i) => group1.Id == i,
                cancellationToken
            );
        }

        return groups;
    }
}