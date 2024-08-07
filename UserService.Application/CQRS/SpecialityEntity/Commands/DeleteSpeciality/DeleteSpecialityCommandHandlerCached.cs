using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;

public class DeleteSpecialityCommandHandlerCached(
    ICacheService cacheService,
    DeleteSpecialityCommandHandler handler
) : IRequestHandler<DeleteSpecialityCommand, List<Speciality>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly DeleteSpecialityCommandHandler _handler = handler;

    public async Task<List<Speciality>> Handle(
        DeleteSpecialityCommand request,
        CancellationToken cancellationToken
    )
    {
        var specialities = await _handler.Handle(request, cancellationToken);

        foreach (var speciality in specialities)
        {
            await _cacheService.RemovePagesWithObjectAsync<Speciality, int>(
                speciality.Id,
                (speciality, i) => speciality.Id == i,
                cancellationToken
            );

            await _cacheService.RemoveAsync(
                CacheKeys.ById<Speciality, int>(speciality.Id),
                cancellationToken
            );
        }

        return specialities;
    }
}
