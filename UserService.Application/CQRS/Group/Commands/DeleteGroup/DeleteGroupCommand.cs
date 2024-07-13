using MediatR;

namespace UserService.Application.CQRS.Group.Commands.DeleteGroup;

public record DeleteGroupCommand(int Id) : IRequest<int>;