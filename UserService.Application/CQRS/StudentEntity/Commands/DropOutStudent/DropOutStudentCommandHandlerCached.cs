using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;

public class DropOutStudentCommandHandlerCached(
    DropOutStudentCommandHandler handler,
    ICacheService cacheService
) : IRequestHandler<DropOutStudentCommand, Guid>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly DropOutStudentCommandHandler _handler = handler;

    public async Task<Guid> Handle(
        DropOutStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var id = await _handler.Handle(request, cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.ById<Student, Guid>(id), cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Student>(), cancellationToken);

        return id;
    }
}
