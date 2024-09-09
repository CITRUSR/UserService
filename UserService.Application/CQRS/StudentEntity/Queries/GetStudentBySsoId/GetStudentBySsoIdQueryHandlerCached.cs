using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;

public class GetStudentBySsoIdQueryHandlerCached(
    IRequestHandler<GetStudentBySsoIdQuery, StudentDto> handler,
    ICacheService cacheService
) : IRequestHandler<GetStudentBySsoIdQuery, StudentDto>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetStudentBySsoIdQuery, StudentDto> _handler = handler;

    public async Task<StudentDto> Handle(
        GetStudentBySsoIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var student = await _cacheService.GetOrCreateAsync<StudentDto>(
            CacheKeys.ById<Student, Guid>(request.SsoId),
            async () => await _handler.Handle(request, cancellationToken),
            cancellationToken
        );

        return student;
    }
}
