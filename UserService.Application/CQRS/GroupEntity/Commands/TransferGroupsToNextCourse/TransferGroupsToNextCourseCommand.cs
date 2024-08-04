using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;

public record TransferGroupsToNextCourseCommand(List<int> IdGroups) : IRequest<List<Group>>;
