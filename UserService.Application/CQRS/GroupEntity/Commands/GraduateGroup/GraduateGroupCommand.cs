using MediatR;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroup;

public record GraduateGroupCommand(int GroupId, DateTime GraduatedTime) : IRequest<int>;