using MediatR;
using UserService.Application.CQRS.SpecialityEntity.Responses;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;

public record GetSpecialityByIdQuery(int Id) : IRequest<SpecialityDto>;
