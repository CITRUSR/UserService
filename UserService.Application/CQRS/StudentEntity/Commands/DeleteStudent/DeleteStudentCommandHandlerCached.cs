using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;

public class DeleteStudentCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<DeleteStudentCommand, StudentShortInfoDto> handler
) : IRequestHandler<DeleteStudentCommand, StudentShortInfoDto>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<DeleteStudentCommand, StudentShortInfoDto> _handler = handler;

    public async Task<StudentShortInfoDto> Handle(
        DeleteStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var student = _handler.Handle(request, cancellationToken).Result;

        await _cacheService.RemoveAsync(
            CacheKeys.ById<Student, Guid>(student.Id),
            cancellationToken
        );

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Student>(), cancellationToken);

        return student;
    }
}
