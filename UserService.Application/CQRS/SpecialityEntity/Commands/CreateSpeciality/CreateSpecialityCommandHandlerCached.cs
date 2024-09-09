using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;

public class CreateSpecialityCommandHandlerCached(
    ICacheService cacheService,
    IRequestHandler<CreateSpecialityCommand, SpecialityShortInfoDto> handler
) : IRequestHandler<CreateSpecialityCommand, SpecialityShortInfoDto>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<CreateSpecialityCommand, SpecialityShortInfoDto> _handler =
        handler;

    public async Task<SpecialityShortInfoDto> Handle(
        CreateSpecialityCommand request,
        CancellationToken cancellationToken
    )
    {
        var speciality = await _handler.Handle(request, cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Speciality>(), cancellationToken);

        return speciality;
    }
}
