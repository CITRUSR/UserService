using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;

public record GraduateGroupsCommand(List<int> GroupsId, DateTime GraduatedTime)
    : IRequest<List<Group>>;
