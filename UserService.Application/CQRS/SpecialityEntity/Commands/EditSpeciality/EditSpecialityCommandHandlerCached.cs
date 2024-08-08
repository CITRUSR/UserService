using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;

public class EditSpecialityCommandHandlerCached(
    EditSpecialityCommandHandler handler,
    ICacheService cacheService
) : IRequestHandler<EditSpecialityCommand, Speciality>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly EditSpecialityCommandHandler _handler = handler;

    public async Task<Speciality> Handle(
        EditSpecialityCommand request,
        CancellationToken cancellationToken
    )
    {
        var speciality = await _handler.Handle(request, cancellationToken);

        await _cacheService.SetObjectAsync<Speciality>(
            CacheKeys.ById<Speciality, int>(speciality.Id),
            speciality,
            cancellationToken
        );

        await _cacheService.RemovePagesWithObjectAsync<Speciality, int>(
            speciality.Id,
            (spec, i) => spec.Id == i,
            cancellationToken
        );

        return speciality;
    }
}
