using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;

public class TransferGroupsToNextSemesterCommandHandlerCached(
    IRequestHandler<TransferGroupsToNextSemesterCommand, List<GroupShortInfoDto>> handler,
    ICacheService cacheService
) : IRequestHandler<TransferGroupsToNextSemesterCommand, List<GroupShortInfoDto>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<
        TransferGroupsToNextSemesterCommand,
        List<GroupShortInfoDto>
    > _handler = handler;

    public async Task<List<GroupShortInfoDto>> Handle(
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
        }

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Group>(), cancellationToken);

        return groups;
    }
}
