using Grpc.Core;
using MediatR;
using UserService.Application.CQRS.Group.Commands.CreateGroup;

namespace UserService.API.Services;

public class GroupService(IMediator mediator) : Group.GroupBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<CreateGroupResponse> CreateGroup(CreateGroupRequest request, ServerCallContext context)
    {
        var command = new CreateGroupCommand(request.SpecialityId, Guid.Parse(request.CuratorId),
            (byte)request.CurrentCourse,
            (byte)request.CurrentSemester, (byte)request.SubGroup, request.StartedAt.ToDateTime());

        var id = await _mediator.Send(command);

        return new CreateGroupResponse
        {
            Id = id,
        };
    }
}