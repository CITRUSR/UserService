using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.SoftDeleteStudents;

public class SoftDeleteStudentsCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<SoftDeleteStudentsCommand, List<StudentShortInfoDto>> handler
) : IRequestHandler<SoftDeleteStudentsCommand, List<StudentShortInfoDto>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<
        SoftDeleteStudentsCommand,
        List<StudentShortInfoDto>
    > _handler = handler;

    public async Task<List<StudentShortInfoDto>> Handle(
        SoftDeleteStudentsCommand request,
        CancellationToken cancellationToken
    )
    {
        var students = await _handler.Handle(request, cancellationToken);

        foreach (var student in students)
        {
            await _cacheService.RemoveAsync(
                CacheKeys.ById<Student, Guid>(student.Id),
                cancellationToken
            );
        }

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Student>(), cancellationToken);

        return students;
    }
}
