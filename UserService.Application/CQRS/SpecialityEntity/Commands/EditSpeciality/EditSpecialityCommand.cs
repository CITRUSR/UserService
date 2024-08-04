﻿using MediatR;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;

public record EditSpecialityCommand(
    int Id,
    string Name,
    string Abbrevation,
    Decimal Cost,
    byte DurationMonths,
    bool IsDeleted
) : IRequest<int>;
