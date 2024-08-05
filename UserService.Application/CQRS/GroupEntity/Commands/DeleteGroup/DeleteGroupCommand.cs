using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;

public record DeleteGroupCommand(int Id) : IRequest<Group>;
