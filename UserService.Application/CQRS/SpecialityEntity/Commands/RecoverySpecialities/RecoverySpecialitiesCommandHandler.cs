using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Responses;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.RecoverySpecialities;

public class RecoverySpecialitiesCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<RecoverySpecialitiesCommand, List<SpecialityShortInfoDto>>
{
    public async Task<List<SpecialityShortInfoDto>> Handle(
        RecoverySpecialitiesCommand request,
        CancellationToken cancellationToken
    )
    {
        var specialities = await DbContext
            .Specialities.Where(x => request.SpecialityIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (request.SpecialityIds.Count != specialities.Count)
        {
            var notFoundIds = request.SpecialityIds.Except(specialities.Select(x => x.Id));

            throw new SpecialityNotFoundException([.. notFoundIds]);
        }

        try
        {
            await DbContext.BeginTransactionAsync();

            foreach (var speciality in specialities)
            {
                speciality.IsDeleted = false;
            }

            await DbContext.SaveChangesAsync(cancellationToken);

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        return specialities.Adapt<List<SpecialityShortInfoDto>>();
    }
}
