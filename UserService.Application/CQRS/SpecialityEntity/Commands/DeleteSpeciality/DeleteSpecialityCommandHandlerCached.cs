using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;

public class DeleteSpecialityCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<DeleteSpecialityCommand, List<Speciality>> handler
) : IRequestHandler<DeleteSpecialityCommand, List<Speciality>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<DeleteSpecialityCommand, List<Speciality>> _handler = handler;

    public async Task<List<Speciality>> Handle(
        DeleteSpecialityCommand request,
        CancellationToken cancellationToken
    )
    {
        var specialities = await _handler.Handle(request, cancellationToken);

        foreach (var speciality in specialities)
        {
            await _cacheService.RemoveAsync(
                CacheKeys.ById<Speciality, int>(speciality.Id),
                cancellationToken
            );
        }

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Speciality>(), cancellationToken);

        return specialities;
    }
}
