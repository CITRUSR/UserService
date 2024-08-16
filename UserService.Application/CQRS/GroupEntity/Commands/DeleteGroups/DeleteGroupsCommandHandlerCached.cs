using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;

public class DeleteGroupsCommandHandlerCached(
    DeleteGroupsCommandHandler handler,
    ICacheService cacheService
) : IRequestHandler<DeleteGroupsCommand, List<Group>>
{
    private readonly DeleteGroupsCommandHandler _handler = handler;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<List<Group>> Handle(
        DeleteGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await _handler.Handle(request, cancellationToken);

        foreach (var group in groups)
        {
            var key = CacheKeys.ById<Group, int>(group.Id);

            await _cacheService.RemoveAsync(key, cancellationToken);

            await _cacheService.RemovePagesWithObjectAsync<Group, int>(
                CacheKeys.GetEntities<Group>,
                group.Id,
                (group, i) => group.Id == i,
                cancellationToken
            );
        }

        return groups;
    }
}
