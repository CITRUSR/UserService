using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;

public class GetStudentByIdQueryHandlerCached(
    ICacheService cacheService,
    IRequestHandler<GetStudentByIdQuery, Student> handler
) : IRequestHandler<GetStudentByIdQuery, Student>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetStudentByIdQuery, Student> _handler = handler;

    public async Task<Student> Handle(
        GetStudentByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var student = await _cacheService.GetOrCreateAsync<Student>(
            CacheKeys.ById<Student, Guid>(request.Id),
            async () => await _handler.Handle(request, cancellationToken),
            cancellationToken
        );

        return student;
    }
}
