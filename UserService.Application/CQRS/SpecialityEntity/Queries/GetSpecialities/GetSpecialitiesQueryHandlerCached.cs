using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;

public class GetSpecialitiesQueryHandlerCached(
    ICacheService cacheService,
    GetSpecialitiesQueryHandler handler
) : IRequestHandler<GetSpecialitiesQuery, PaginationList<Speciality>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly GetSpecialitiesQueryHandler _handler = handler;

    public async Task<PaginationList<Speciality>> Handle(
        GetSpecialitiesQuery request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.Page <= CacheConstants.PagesForCaching
            && request
                is {
                    SortState: SpecialitySortState.NameAsc,
                    DeletedStatus: DeletedStatus.OnlyActive
                }
        )
        {
            var key = CacheKeys.GetEntities<Speciality>(request.Page);

            return await _cacheService.GetOrCreateAsync<PaginationList<Speciality>>(
                key,
                async () => await _handler.Handle(request, cancellationToken),
                cancellationToken
            );
        }

        return await _handler.Handle(request, cancellationToken);
    }
}
