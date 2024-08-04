using MediatR;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpeciality;

public class SoftDeleteSpecialityCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<SoftDeleteSpecialityCommand, int>
{
    public async Task<int> Handle(
        SoftDeleteSpecialityCommand request,
        CancellationToken cancellationToken
    )
    {
        var speciality = await DbContext.Specialities.FindAsync(
            new object?[] { request.Id, cancellationToken },
            cancellationToken: cancellationToken
        );

        if (speciality == null)
        {
            throw new SpecialityNotFoundException(request.Id);
        }

        speciality.IsDeleted = true;
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The speciality with id:{request.Id} is soft deleted");

        return request.Id;
    }
}
