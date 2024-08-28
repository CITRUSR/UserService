using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;

public class DeleteStudentCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<DeleteStudentCommand, Guid> handler
) : IRequestHandler<DeleteStudentCommand, Guid>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<DeleteStudentCommand, Guid> _handler = handler;

    public async Task<Guid> Handle(
        DeleteStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var id = _handler.Handle(request, cancellationToken).Result;

        await _cacheService.RemoveAsync(CacheKeys.ById<Student, Guid>(id), cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Student>(), cancellationToken);

        return id;
    }
}
