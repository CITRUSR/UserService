using MediatR;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupsToNextCourse;

public class TransferGroupsToNextCourseCommand : IRequest<List<int>>
{
    public List<int>? IdGroups { get; set; }
}