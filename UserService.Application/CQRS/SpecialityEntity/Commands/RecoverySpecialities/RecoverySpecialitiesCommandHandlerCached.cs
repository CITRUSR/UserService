using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.RecoverySpecialities;

public class RecoverySpecialitiesCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<RecoverySpecialitiesCommand, List<SpecialityShortInfoDto>> handler
) : IRequestHandler<RecoverySpecialitiesCommand, List<SpecialityShortInfoDto>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<
        RecoverySpecialitiesCommand,
        List<SpecialityShortInfoDto>
    > _handler = handler;

    public async Task<List<SpecialityShortInfoDto>> Handle(
        RecoverySpecialitiesCommand request,
        CancellationToken cancellationToken
    )
    {
        var specialities = await _handler.Handle(request, cancellationToken);

        foreach (var speciality in specialities)
        {
            var key = CacheKeys.ById<Speciality, int>(speciality.Id);

            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Speciality>(), cancellationToken);

        return specialities;
    }
}
