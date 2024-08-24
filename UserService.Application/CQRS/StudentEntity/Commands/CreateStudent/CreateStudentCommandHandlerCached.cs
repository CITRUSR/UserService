using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;

public class CreateStudentCommandHandlerCached(
    ICacheService cache,
    CreateStudentCommandHandler handler
) : IRequestHandler<CreateStudentCommand, Guid>
{
    private readonly ICacheService _cacheService = cache;
    private readonly CreateStudentCommandHandler _handler = handler;

    public async Task<Guid> Handle(
        CreateStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var id = await _handler.Handle(request, cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Student>(), cancellationToken);

        return id;
    }
}
