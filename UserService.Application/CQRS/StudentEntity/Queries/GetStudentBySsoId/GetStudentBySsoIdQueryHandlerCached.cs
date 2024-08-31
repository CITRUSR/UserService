using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;

public class GetStudentBySsoIdQueryHandlerCached(
    IRequestHandler<GetStudentBySsoIdQuery, Student> handler,
    ICacheService cacheService
) : IRequestHandler<GetStudentBySsoIdQuery, Student>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetStudentBySsoIdQuery, Student> _handler = handler;

    public async Task<Student> Handle(
        GetStudentBySsoIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var student = await _cacheService.GetOrCreateAsync<Student>(
            CacheKeys.ById<Student, Guid>(request.SsoId),
            async () => await _handler.Handle(request, cancellationToken),
            cancellationToken
        );

        return student;
    }
}
