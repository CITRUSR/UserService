using MediatR;

namespace UserService.Application.CQRS.Group.Commands.CreateGroup;

public record CreateGroupCommand(
    int SpecialityId,
    Guid CuratorId,
    byte CurrentCourse,
    byte CurrentSemester,
    byte SubGroup,
    DateTime StartedAt) : IRequest<int>;