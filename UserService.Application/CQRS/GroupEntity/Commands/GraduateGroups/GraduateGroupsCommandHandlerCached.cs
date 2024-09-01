using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;

public class GraduateGroupsCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<GraduateGroupsCommand, List<GroupShortInfoDto>> handler
) : IRequestHandler<GraduateGroupsCommand, List<GroupShortInfoDto>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GraduateGroupsCommand, List<GroupShortInfoDto>> _handler =
        handler;

    public async Task<List<GroupShortInfoDto>> Handle(
        GraduateGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await _handler.Handle(request, cancellationToken);

        foreach (var group in groups)
        {
            await _cacheService.RemoveAsync(
                CacheKeys.ById<Group, int>(group.Id),
                cancellationToken
            );
        }

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Group>(), cancellationToken);

        return groups;
    }
}
