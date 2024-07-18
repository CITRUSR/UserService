using MediatR;

namespace UserService.Application.CQRS.GroupEntity.Commands.EditGroup;

public record EditGroupCommand(
    int Id,
    int SpecialityId,
    Guid CuratorId,
    byte CurrentCourse,
    byte CurrentSemester,
    byte SubGroup) : IRequest<int>;