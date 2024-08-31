using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.EditGroup;

public record EditGroupCommand(
    int Id,
    int SpecialityId,
    Guid CuratorId,
    byte CurrentCourse,
    byte CurrentSemester,
    byte SubGroup,
    bool IsDeleted
) : IRequest<Group>;
