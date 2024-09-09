using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;

public class CreateGroupCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<CreateGroupCommand, GroupShortInfoDto> handler
) : IRequestHandler<CreateGroupCommand, GroupShortInfoDto>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<CreateGroupCommand, GroupShortInfoDto> _handler = handler;

    public async Task<GroupShortInfoDto> Handle(
        CreateGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var group = await _handler.Handle(request, cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Group>(), cancellationToken);

        return group;
    }
}
