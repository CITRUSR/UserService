using MediatR;
using UserService.Application.CQRS.SpecialityEntity.Responses;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;

public record SoftDeleteSpecialitiesCommand(List<int> SpecialitiesId)
    : IRequest<List<SpecialityShortInfoDto>>;
