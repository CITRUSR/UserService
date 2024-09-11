using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.RecoveryGroups;

public class RecoveryGroupsCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<RecoveryGroupsCommand, List<GroupShortInfoDto>> handler
) : IRequestHandler<RecoveryGroupsCommand, List<GroupShortInfoDto>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<RecoveryGroupsCommand, List<GroupShortInfoDto>> _handler =
        handler;

    public async Task<List<GroupShortInfoDto>> Handle(
        RecoveryGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await _handler.Handle(request, cancellationToken);

        foreach (var group in groups)
        {
            var key = CacheKeys.ById<Group, int>(group.Id);

            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Group>(), cancellationToken);

        return groups;
    }
}
