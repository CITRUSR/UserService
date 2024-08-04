using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;

public record CreateGroupCommand(
    int SpecialityId,
    Guid CuratorId,
    byte CurrentCourse,
    byte CurrentSemester,
    byte SubGroup,
    DateTime StartedAt
) : IRequest<Group>;
