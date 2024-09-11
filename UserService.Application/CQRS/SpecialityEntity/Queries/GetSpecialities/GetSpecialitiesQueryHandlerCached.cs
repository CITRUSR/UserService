using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;

public class GetSpecialitiesQueryHandlerCached(
    ICacheService cacheService,
    IRequestHandler<GetSpecialitiesQuery, GetSpecialitiesResponse> handler,
    ICacheOptions cacheOptions
) : IRequestHandler<GetSpecialitiesQuery, GetSpecialitiesResponse>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetSpecialitiesQuery, GetSpecialitiesResponse> _handler =
        handler;
    private readonly ICacheOptions _cacheOptions = cacheOptions;

    public async Task<GetSpecialitiesResponse> Handle(
        GetSpecialitiesQuery request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.Page <= _cacheOptions.PagesForCaching
            && request
                is {
                    SortState: SpecialitySortState.NameAsc,
                    DeletedStatus: DeletedStatus.OnlyActive
                }
        )
        {
            var key = CacheKeys.GetEntities<Speciality>();

            var entities = await _cacheService.GetOrCreateAsync<GetSpecialitiesResponse>(
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

            entities.Specialities = [.. entities.Specialities.Take(request.PageSize)];

            return entities;
        }

        return await _handler.Handle(request, cancellationToken);
    }
}
