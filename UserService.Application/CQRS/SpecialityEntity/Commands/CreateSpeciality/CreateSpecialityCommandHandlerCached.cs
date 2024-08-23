using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;

public class CreateSpecialityCommandHandlerCached(
    ICacheService cacheService,
    CreateSpecialityCommandHandler handler
) : IRequestHandler<CreateSpecialityCommand, Speciality>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly CreateSpecialityCommandHandler _handler = handler;

    public async Task<Speciality> Handle(
        CreateSpecialityCommand request,
        CancellationToken cancellationToken
    )
    {
        var speciality = await _handler.Handle(request, cancellationToken);

        for (int i = 0; i < CacheConstants.PagesForCaching; i++)
        {
            await _cacheService.RemoveAsync(
                CacheKeys.GetEntities<Speciality>(i, 10),
                cancellationToken
            );
        }

        return speciality;
    }
}
