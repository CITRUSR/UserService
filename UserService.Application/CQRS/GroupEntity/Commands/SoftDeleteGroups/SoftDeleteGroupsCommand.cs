using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;

public record SoftDeleteGroupsCommand(List<int> GroupsId) : IRequest<List<Group>> { }
