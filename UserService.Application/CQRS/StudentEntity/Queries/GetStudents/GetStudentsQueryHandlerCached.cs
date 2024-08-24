using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudents;

public class GetStudentsQueryHandlerCached(
    GetStudentsQueryHandler handler,
    ICacheService cacheService
) : IRequestHandler<GetStudentsQuery, PaginationList<Student>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly GetStudentsQueryHandler _handler = handler;

    public async Task<PaginationList<Student>> Handle(
        GetStudentsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.Page <= CacheConstants.PagesForCaching
            && request
                is {
                    SortState: SortState.LastNameAsc,
                    DroppedOutStatus: StudentDroppedOutStatus.OnlyActive,
                    DeletedStatus: DeletedStatus.OnlyActive
                }
        )
        {
            var key = CacheKeys.GetEntities<Student>();

            return await _cacheService.GetOrCreateAsync<PaginationList<Student>>(
                key,
                async () => await _handler.Handle(request, cancellationToken),
                cancellationToken
            );
        }

        return await _handler.Handle(request, cancellationToken);
    }
}
