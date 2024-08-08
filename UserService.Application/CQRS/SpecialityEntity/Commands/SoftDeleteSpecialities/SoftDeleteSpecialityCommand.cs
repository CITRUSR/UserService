using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;

public record SoftDeleteSpecialityCommand(List<int> SpecialitiesId) : IRequest<List<Speciality>>;
