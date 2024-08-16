using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;

public class GraduateGroupsCommandHandlerCached(
    ICacheService cacheService,
    GraduateGroupsCommandHandler handler
) : IRequestHandler<GraduateGroupsCommand, List<Group>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly GraduateGroupsCommandHandler _handler = handler;

    public async Task<List<Group>> Handle(
        GraduateGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await _handler.Handle(request, cancellationToken);

        foreach (var group in groups)
        {
            await _cacheService.SetObjectAsync<Group>(
                CacheKeys.ById<Group, int>(group.Id),
                group,
                cancellationToken
            );
            await _cacheService.RemovePagesWithObjectAsync<Group, int>(
                CacheKeys.GetEntities<Group>,
                group.Id,
                (gr, i) => gr.Id == i,
                cancellationToken
            );
        }

        return groups;
    }
}
