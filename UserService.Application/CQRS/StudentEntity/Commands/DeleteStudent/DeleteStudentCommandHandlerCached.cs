using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;

public class DeleteStudentCommandHandlerCached(
    ICacheService cacheService,
    DeleteStudentCommandHandler handler
) : IRequestHandler<DeleteStudentCommand, Guid>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly DeleteStudentCommandHandler _handler = handler;

    public async Task<Guid> Handle(
        DeleteStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var id = _handler.Handle(request, cancellationToken).Result;

        await _cacheService.RemoveAsync(CacheKeys.ById<Student, Guid>(id), cancellationToken);

        for (int i = 0; i < CacheConstants.PagesForCaching; i++)
        {
            await _cacheService.RemoveAsync(
                CacheKeys.GetEntities<Student>(i, 10),
                cancellationToken
            );
        }

        return id;
    }
}
