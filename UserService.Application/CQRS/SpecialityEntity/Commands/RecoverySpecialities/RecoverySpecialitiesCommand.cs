using MediatR;
using UserService.Application.CQRS.SpecialityEntity.Responses;

namespace UserService.Application.CQRS.SpecialityEntity.Commands.RecoverySpecialities;

public record RecoverySpecialitiesCommand(List<int> SpecialityIds)
    : IRequest<List<SpecialityShortInfoDto>>;
