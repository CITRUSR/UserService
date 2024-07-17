using MediatR;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;

public record DeleteGroupCommand(int Id) : IRequest<int>;