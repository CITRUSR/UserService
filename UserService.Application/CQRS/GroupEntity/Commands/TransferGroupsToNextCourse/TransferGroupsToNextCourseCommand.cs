using MediatR;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;

public class TransferGroupsToNextCourseCommand : IRequest<List<int>>
{
    public List<int>? IdGroups { get; set; }
}