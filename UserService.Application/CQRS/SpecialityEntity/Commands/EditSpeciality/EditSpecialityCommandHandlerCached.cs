using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;

public class EditSpecialityCommandHandlerCached(
    IRequestHandler<EditSpecialityCommand, SpecialityShortInfoDto> handler,
    ICacheService cacheService,
    IAppDbContext appDbContext
) : IRequestHandler<EditSpecialityCommand, SpecialityShortInfoDto>
{
    private readonly IAppDbContext _appDbContext = appDbContext;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IRequestHandler<EditSpecialityCommand, SpecialityShortInfoDto> _handler =
        handler;

    public async Task<SpecialityShortInfoDto> Handle(
        EditSpecialityCommand request,
        CancellationToken cancellationToken
    )
    {
        var speciality = await _handler.Handle(request, cancellationToken);

        var groupsWithSpeciality = await _appDbContext
            .Groups.Where(x => x.SpecialityId == request.Id)
            .ToListAsync(cancellationToken);

        foreach (var group in groupsWithSpeciality)
        {
            await _cacheService.RemoveAsync(
                CacheKeys.ById<Group, int>(group.Id),
                cancellationToken
            );
        }

        await _cacheService.RemoveAsync(
            CacheKeys.ById<Speciality, int>(speciality.Id),
            cancellationToken
        );

        await _cacheService.RemoveAsync(CacheKeys.GetEntities<Speciality>(), cancellationToken);

        return speciality;
    }
}
