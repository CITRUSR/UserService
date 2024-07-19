using MediatR;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;

public class EditSpecialityCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<EditSpecialityCommand, int>
{
    public async Task<int> Handle(EditSpecialityCommand request, CancellationToken cancellationToken)
    {
        var speciality = await DbContext.Specialities.FindAsync(new object?[] { request.Id, cancellationToken },
            cancellationToken: cancellationToken);

        if (speciality == null)
        {
            throw new SpecialityNotFoundException(request.Id);
        }

        speciality.Name = request.Name;
        speciality.Abbreavation = request.Abbrevation;
        speciality.Cost = request.Cost;
        speciality.DurationMonths = request.DurationMonths;
        speciality.IsDeleted = request.IsDeleted;

        await DbContext.SaveChangesAsync(cancellationToken);

        return speciality.Id;
    }
}