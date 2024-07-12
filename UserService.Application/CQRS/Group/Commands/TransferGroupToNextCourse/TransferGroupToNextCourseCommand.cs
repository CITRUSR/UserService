using MediatR;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupToNextCourse;

public record TransferGroupToNextCourseCommand(int GroupId) : IRequest<int>;