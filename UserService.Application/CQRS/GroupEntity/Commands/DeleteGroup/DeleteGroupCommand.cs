using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;

public record DeleteGroupsCommand(List<int> Ids) : IRequest<List<Group>>;
