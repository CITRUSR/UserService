using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;

public record DeleteSpecialityCommand(List<int> SpecialitiesId) : IRequest<List<Speciality>>;
