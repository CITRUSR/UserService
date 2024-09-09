using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Responses;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpecialities;

public class DeleteSpecialitiesCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DeleteSpecialitiesCommand, List<SpecialityShortInfoDto>>
{
    public async Task<List<SpecialityShortInfoDto>> Handle(
        DeleteSpecialitiesCommand request,
        CancellationToken cancellationToken
    )
    {
        var specialities = await DbContext
            .Specialities.Where(x => request.SpecialitiesId.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (specialities.Count != request.SpecialitiesId.Count)
        {
            var notFoundIds = request.SpecialitiesId.Except(specialities.Select(x => x.Id));

            throw new SpecialityNotFoundException([.. notFoundIds]);
        }

        try
        {
            await DbContext.BeginTransactionAsync();

            DbContext.Specialities.RemoveRange(specialities);

            await DbContext.SaveChangesAsync(cancellationToken);

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        Log.Information(
            $"The specialities with id:{string.Join(", ", request.SpecialitiesId)} is deleted"
        );

        return specialities.Adapt<List<SpecialityShortInfoDto>>();
    }
}
