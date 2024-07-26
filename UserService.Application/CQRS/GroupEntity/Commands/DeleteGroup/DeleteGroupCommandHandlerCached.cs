using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;

public class DeleteGroupCommandHandlerCached(
    DeleteGroupCommandHandler handler,
    ICacheService cacheService)
    : IRequestHandler<DeleteGroupCommand, int>
{
    private readonly DeleteGroupCommandHandler _handler = handler;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<int> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var id = await _handler.Handle(request, cancellationToken);

        var key = CacheKeys.ById<Group, int>(request.Id);

        await _cacheService.RemoveAsync(key, cancellationToken);

        await _cacheService.RemovePagesWithObjectAsync<Group, int>(request.Id, (group, i) => group.Id == i,
            cancellationToken);

        return id;
    }
}