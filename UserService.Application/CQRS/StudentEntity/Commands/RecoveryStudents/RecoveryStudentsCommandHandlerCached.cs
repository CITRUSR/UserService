using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.RecoveryStudents;

public class RecoveryStudentsCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<RecoveryStudentsCommand, List<StudentShortInfoDto>> handler
) : IRequestHandler<RecoveryStudentsCommand, List<StudentShortInfoDto>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<RecoveryStudentsCommand, List<StudentShortInfoDto>> _handler =
        handler;

    public async Task<List<StudentShortInfoDto>> Handle(
        RecoveryStudentsCommand request,
        CancellationToken cancellationToken
    )
    {
        var students = await _handler.Handle(request, cancellationToken);

        foreach (var student in students)
        {
            var key = CacheKeys.ById<Student, Guid>(student.Id);

            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Student>(), cancellationToken);

        return students;
    }
}
