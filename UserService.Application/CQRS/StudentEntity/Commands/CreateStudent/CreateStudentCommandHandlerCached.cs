using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;

public class CreateStudentCommandHandlerCached(
    ICacheService cache,
    IRequestHandler<CreateStudentCommand, StudentShortInfoDto> handler
) : IRequestHandler<CreateStudentCommand, StudentShortInfoDto>
{
    private readonly ICacheService _cacheService = cache;
    private readonly IRequestHandler<CreateStudentCommand, StudentShortInfoDto> _handler = handler;

    public async Task<StudentShortInfoDto> Handle(
        CreateStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var id = await _handler.Handle(request, cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Student>(), cancellationToken);

        return id;
    }
}
