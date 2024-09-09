using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;

public class GetSpecialityByIdQueryHandlerCached(
    ICacheService cacheService,
    IRequestHandler<GetSpecialityByIdQuery, SpecialityDto> handler
) : IRequestHandler<GetSpecialityByIdQuery, SpecialityDto>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetSpecialityByIdQuery, SpecialityDto> _handler = handler;

    public async Task<SpecialityDto> Handle(
        GetSpecialityByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var key = CacheKeys.ById<Speciality, int>(request.Id);

        var speciality = await _cacheService.GetOrCreateAsync<SpecialityDto>(
            key,
            async () => await _handler.Handle(request, cancellationToken),
            cancellationToken
        );

        return speciality;
    }
}
