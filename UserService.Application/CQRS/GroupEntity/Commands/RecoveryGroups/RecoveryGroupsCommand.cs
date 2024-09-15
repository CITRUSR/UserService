using MediatR;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.RecoveryGroups;

public record RecoveryGroupsCommand(List<int> GroupIds) : IRequest<List<GroupShortInfoDto>>;
