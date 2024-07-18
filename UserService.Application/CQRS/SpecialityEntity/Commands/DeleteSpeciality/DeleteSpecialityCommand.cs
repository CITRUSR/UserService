using MediatR;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;

public record DeleteSpecialityCommand(int Id) : IRequest<int>;