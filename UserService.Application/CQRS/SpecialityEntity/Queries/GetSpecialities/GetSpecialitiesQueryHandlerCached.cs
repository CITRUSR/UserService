using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;

public class GetSpecialitiesQueryHandlerCached(
    ICacheService cacheService,
    IRequestHandler<GetSpecialitiesQuery, PaginationList<Speciality>> handler
) : IRequestHandler<GetSpecialitiesQuery, PaginationList<Speciality>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetSpecialitiesQuery, PaginationList<Speciality>> _handler =
        handler;

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
            var key = CacheKeys.GetEntities<Speciality>();

            var entities = await _cacheService.GetOrCreateAsync<PaginationList<Speciality>>(
                key,
                async () =>
                    await _handler.Handle(
                        request with
                        {
                            PageSize =
                                CacheConstants.PagesForCaching * CacheConstants.EntitiesPerPage
                        },
                        cancellationToken
                    ),
                cancellationToken
            );

            entities.Items = [.. entities.Items.Take(request.PageSize)];

            return entities;
        }

        return await _handler.Handle(request, cancellationToken);
    }
}
