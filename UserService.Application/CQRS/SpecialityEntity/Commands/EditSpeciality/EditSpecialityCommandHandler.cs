using Mapster;
using MediatR;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Responses;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;

public class EditSpecialityCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<EditSpecialityCommand, SpecialityShortInfoDto>
{
    public async Task<SpecialityShortInfoDto> Handle(
        EditSpecialityCommand request,
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

        speciality.Name = request.Name;
        speciality.Abbreviation = request.Abbrevation;
        speciality.Cost = request.Cost;
        speciality.DurationMonths = request.DurationMonths;
        speciality.IsDeleted = request.IsDeleted;

        await DbContext.SaveChangesAsync(cancellationToken);

        return speciality.Adapt<SpecialityShortInfoDto>();
    }
}
