using Mapster;
using MediatR;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Domain.Entities;

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

        var oldSpeciality = new Speciality
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Abbreviation = speciality.Abbreviation,
            Cost = speciality.Cost,
            DurationMonths = speciality.DurationMonths,
            IsDeleted = speciality.IsDeleted,
        };

        speciality.Name = request.Name;
        speciality.Abbreviation = request.Abbrevation;
        speciality.Cost = request.Cost;
        speciality.DurationMonths = request.DurationMonths;
        speciality.IsDeleted = request.IsDeleted;

        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information(
            $"The speciality with id:{request.Id} is updated"
                + "old state:{@oldSpeciality} new state:{@speciality}"
        );

        return speciality.Adapt<SpecialityShortInfoDto>();
    }
}
