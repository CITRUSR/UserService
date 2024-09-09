using MediatR;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;

public record TransferGroupsToNextCourseCommand(List<int> IdGroups)
    : IRequest<List<GroupShortInfoDto>>;
