using MediatR;
using UserService.Application.CQRS.SpecialityEntity.Responses;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpecialities;

public record DeleteSpecialitiesCommand(List<int> SpecialitiesId)
    : IRequest<List<SpecialityShortInfoDto>>;
