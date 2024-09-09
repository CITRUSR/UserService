using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;

public class DeleteStudentsCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<DeleteStudentsCommand, List<StudentShortInfoDto>> handler
) : IRequestHandler<DeleteStudentsCommand, List<StudentShortInfoDto>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<DeleteStudentsCommand, List<StudentShortInfoDto>> _handler =
        handler;

    public async Task<List<StudentShortInfoDto>> Handle(
        DeleteStudentsCommand request,
        CancellationToken cancellationToken
    )
    {
        var students = _handler.Handle(request, cancellationToken).Result;

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
