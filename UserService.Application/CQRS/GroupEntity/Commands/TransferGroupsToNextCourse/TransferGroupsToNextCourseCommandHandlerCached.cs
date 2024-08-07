using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;

public class TransferGroupsToNextCourseCommandHandlerCached(
    ICacheService cacheService,
    TransferGroupsToNextCourseCommandHandler handler
) : IRequestHandler<TransferGroupsToNextCourseCommand, List<Group>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly TransferGroupsToNextCourseCommandHandler _handler = handler;

    public async Task<List<Group>> Handle(
        TransferGroupsToNextCourseCommand request,
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
                group.Id,
                (group1, i) => group1.Id == i,
                cancellationToken
            );
        }

        return groups;
    }
}
