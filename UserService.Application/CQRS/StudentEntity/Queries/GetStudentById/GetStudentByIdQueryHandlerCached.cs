using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;

public class GetStudentByIdQueryHandlerCached(
    ICacheService cacheService,
    IRequestHandler<GetStudentByIdQuery, StudentDto> handler
) : IRequestHandler<GetStudentByIdQuery, StudentDto>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetStudentByIdQuery, StudentDto> _handler = handler;

    public async Task<StudentDto> Handle(
        GetStudentByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var student = await _cacheService.GetOrCreateAsync<StudentDto>(
            CacheKeys.ById<Student, Guid>(request.Id),
            async () => await _handler.Handle(request, cancellationToken),
            cancellationToken
        );

        return student;
    }
}
