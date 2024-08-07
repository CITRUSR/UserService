using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;

public class DeleteSpecialityCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DeleteSpecialityCommand, List<Speciality>>
{
    public async Task<List<Speciality>> Handle(
        DeleteSpecialityCommand request,
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

        return specialities;
    }
}
