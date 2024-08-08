using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;

public class SoftDeleteSpecialitiesCommandHandlerCached(
    SoftDeleteSpecialitiesCommandHandler handler,
    ICacheService cacheService
) : IRequestHandler<SoftDeleteSpecialitiesCommand, List<Speciality>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly SoftDeleteSpecialitiesCommandHandler _handler = handler;

    public async Task<List<Speciality>> Handle(
        SoftDeleteSpecialitiesCommand request,
        CancellationToken cancellationToken
    )
    {
        var specialities = await _handler.Handle(request, cancellationToken);

        foreach (var speciality in specialities)
        {
            await _cacheService.SetObjectAsync<Speciality>(
                CacheKeys.ById<Speciality, int>(speciality.Id),
                speciality,
                cancellationToken
            );

            await _cacheService.RemovePagesWithObjectAsync<Speciality, int>(
                speciality.Id,
                (speciality, i) => speciality.Id == i,
                cancellationToken
            );
        }

        return specialities;
    }
}
