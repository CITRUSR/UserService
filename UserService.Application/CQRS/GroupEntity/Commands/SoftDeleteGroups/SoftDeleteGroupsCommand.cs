using MediatR;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;

public record SoftDeleteGroupsCommand(List<int> GroupsId) : IRequest<List<GroupShortInfoDto>> { }
