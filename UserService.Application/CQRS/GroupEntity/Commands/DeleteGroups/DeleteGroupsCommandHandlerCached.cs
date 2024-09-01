using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;

public class DeleteGroupsCommandHandlerCached(
    IRequestHandler<DeleteGroupsCommand, List<Group>> handler,
    ICacheService cacheService
) : IRequestHandler<DeleteGroupsCommand, List<Group>>
{
    private readonly IRequestHandler<DeleteGroupsCommand, List<Group>> _handler = handler;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<List<Group>> Handle(
        DeleteGroupsCommand request,
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
