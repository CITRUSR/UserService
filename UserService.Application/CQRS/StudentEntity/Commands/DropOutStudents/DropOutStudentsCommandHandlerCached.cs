using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;

public class DropOutStudentsCommandHandlerCached(
    IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>> handler,
    ICacheService cacheService
) : IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>> _handler =
        handler;

    public async Task<List<StudentShortInfoDto>> Handle(
        DropOutStudentsCommand request,
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
