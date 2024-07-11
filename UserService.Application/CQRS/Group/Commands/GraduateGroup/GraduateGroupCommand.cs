using MediatR;

namespace UserService.Application.CQRS.Group.Commands.GraduateGroup;

public record GraduateGroupCommand(int GroupId, DateTime GraduatedTime) : IRequest<int>;