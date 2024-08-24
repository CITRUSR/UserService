using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.EditGroup;

public class EditGroupCommandHandlerCached(
    EditGroupCommandHandler handler,
    ICacheService cacheService
) : IRequestHandler<EditGroupCommand, Group>
{
    private readonly EditGroupCommandHandler _handler = handler;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Group> Handle(EditGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _handler.Handle(request, cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.ById<Group, int>(request.Id), cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Group>(), cancellationToken);

        return group;
    }
}
