using MediatR;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;

public record GraduateGroupsCommand(List<int> GroupsId, DateTime GraduatedTime)
    : IRequest<List<GroupShortInfoDto>>;
