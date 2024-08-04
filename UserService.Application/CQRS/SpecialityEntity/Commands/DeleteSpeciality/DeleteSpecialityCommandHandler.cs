using MediatR;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;

public class DeleteSpecialityCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DeleteSpecialityCommand, int>
{
    public async Task<int> Handle(
        DeleteSpecialityCommand request,
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

        DbContext.Specialities.Remove(speciality);
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The speciality with id:{request.Id} is deleted");

        return request.Id;
    }
}
