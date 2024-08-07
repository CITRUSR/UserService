﻿using MediatR;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;

public class CreateSpecialityCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<CreateSpecialityCommand, Speciality>
{
    public async Task<Speciality> Handle(
        CreateSpecialityCommand request,
        CancellationToken cancellationToken
    )
    {
        var speciality = new Speciality()
        {
            DurationMonths = request.DurationMonths,
            Cost = request.Cost,
            Name = request.Name,
            Abbreavation = request.Abbreavation,
        };

        await DbContext.Specialities.AddAsync(speciality, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The speciality with id{speciality.Id} is created");

        return speciality;
    }
}
