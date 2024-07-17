using MediatR;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;

public class TransferGroupsToNextSemesterCommand : IRequest<List<int>>
{
    public List<int>? IdGroups { get; set; }
}