using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudents;

public class GetStudentsQueryHandlerCached(
    IRequestHandler<GetStudentsQuery, GetStudentsResponse> handler,
    ICacheService cacheService,
    ICacheOptions cacheOptions
) : IRequestHandler<GetStudentsQuery, GetStudentsResponse>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetStudentsQuery, GetStudentsResponse> _handler = handler;
    private readonly ICacheOptions _cacheOptions = cacheOptions;

    public async Task<GetStudentsResponse> Handle(
        GetStudentsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.Page <= _cacheOptions.PagesForCaching
            && request
                is {
                    SortState: SortState.LastNameAsc,
                    DroppedOutStatus: StudentDroppedOutStatus.OnlyActive,
                    DeletedStatus: DeletedStatus.OnlyActive
                }
        )
        {
            var key = CacheKeys.GetEntities<Student>();

            var entities = await _cacheService.GetOrCreateAsync<GetStudentsResponse>(
                key,
                async () =>
                    await _handler.Handle(
                        request with
                        {
                            PageSize = _cacheOptions.PagesForCaching * _cacheOptions.EntitiesPerPage
                        },
                        cancellationToken
                    ),
                cancellationToken
            );

            entities.Students = [.. entities.Students.Take(request.PageSize)];

            return entities;
        }

        return await _handler.Handle(request, cancellationToken);
    }
}
