using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;

public class DropOutStudentCommandHandlerCached(
    IRequestHandler<DropOutStudentCommand, StudentShortInfoDto> handler,
    ICacheService cacheService
) : IRequestHandler<DropOutStudentCommand, StudentShortInfoDto>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<DropOutStudentCommand, StudentShortInfoDto> _handler = handler;

    public async Task<StudentShortInfoDto> Handle(
        DropOutStudentCommand request,
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
