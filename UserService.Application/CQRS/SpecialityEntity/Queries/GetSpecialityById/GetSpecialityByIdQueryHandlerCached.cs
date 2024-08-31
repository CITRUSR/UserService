using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;

public class GetSpecialityByIdQueryHandlerCached(
    ICacheService cacheService,
    IRequestHandler<GetSpecialityByIdQuery, Speciality> handler
) : IRequestHandler<GetSpecialityByIdQuery, Speciality>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<GetSpecialityByIdQuery, Speciality> _handler = handler;

    public async Task<Speciality> Handle(
        GetSpecialityByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var key = CacheKeys.ById<Speciality, int>(request.Id);

        var speciality = await _cacheService.GetOrCreateAsync<Speciality>(
            key,
            async () => await _handler.Handle(request, cancellationToken),
            cancellationToken
        );

        return speciality;
    }
}
