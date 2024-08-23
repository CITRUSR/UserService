using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;

public class CreateGroupCommandHandlerCached(
    ICacheService cacheService,
    CreateGroupCommandHandler handler
) : IRequestHandler<CreateGroupCommand, Group>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly CreateGroupCommandHandler _handler = handler;

    public async Task<Group> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _handler.Handle(request, cancellationToken);

        for (int i = 0; i < CacheConstants.PagesForCaching; i++)
        {
            await _cacheService.RemoveAsync(CacheKeys.GetEntities<Group>(i), cancellationToken);
        }

        return group;
    }
}
