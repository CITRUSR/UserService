using MediatR;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;

public record CreateSpecialityCommand(
    string Name,
    string Abbreavation,
    Decimal Cost,
    byte DurationMonths
) : IRequest<int>;
