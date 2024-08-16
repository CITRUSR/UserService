using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;

public class TransferGroupsToNextSemesterCommandHandlerCached(
    TransferGroupsToNextSemesterCommandHandler handler,
    ICacheService cacheService
) : IRequestHandler<TransferGroupsToNextSemesterCommand, List<Group>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly TransferGroupsToNextSemesterCommandHandler _handler = handler;

    public async Task<List<Group>> Handle(
        TransferGroupsToNextSemesterCommand request,
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
