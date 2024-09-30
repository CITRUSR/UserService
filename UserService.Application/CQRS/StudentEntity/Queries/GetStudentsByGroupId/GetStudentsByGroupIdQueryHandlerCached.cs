using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentsByGroupId;

public class GetStudentsByGroupIdQueryHandlerCached(
    IRequestHandler<GetStudentsByGroupIdQuery, List<StudentViewModel>> handler,
    ICacheService cacheService
) : IRequestHandler<GetStudentsByGroupIdQuery, List<StudentViewModel>>
{
    private readonly IRequestHandler<GetStudentsByGroupIdQuery, List<StudentViewModel>> _handler =
        handler;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<List<StudentViewModel>> Handle(
        GetStudentsByGroupIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var students = await _cacheService.GetOrCreateAsync<List<StudentViewModel>>(
            CacheKeys.EntitiesByGroupId<Student>(request.GroupId),
            async () => await _handler.Handle(request, cancellationToken),
            cancellationToken
        );

        return students;
    }
}
