using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;

public record GetSpecialityByIdQuery(int Id) : IRequest<Speciality>;