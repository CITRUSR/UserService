using MediatR;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpeciality;

public record SoftDeleteSpecialityCommand(int Id) : IRequest<int>;
