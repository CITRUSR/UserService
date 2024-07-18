using MediatR;

namespace UserService.Application.CQRS.SpecialityEntity.Commands;

public record CreateSpecialityCommand(
    string Name,
    string Abbreavation,
    Decimal Cost,
    byte DurationMonths) : IRequest<int>;