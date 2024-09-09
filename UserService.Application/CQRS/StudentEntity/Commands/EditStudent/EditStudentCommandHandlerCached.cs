using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.EditStudent;

public class EditStudentCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<EditStudentCommand, StudentShortInfoDto> handler
) : IRequestHandler<EditStudentCommand, StudentShortInfoDto>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<EditStudentCommand, StudentShortInfoDto> _handler = handler;

    public async Task<StudentShortInfoDto> Handle(
        EditStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var student = await _handler.Handle(request, cancellationToken);

        await _cacheService.RemoveAsync(
            CacheKeys.ById<Student, Guid>(student.Id),
            cancellationToken
        );

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Student>(), cancellationToken);

        return student;
    }
}
