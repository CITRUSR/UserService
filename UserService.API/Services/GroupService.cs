using Grpc.Core;
using MediatR;
using UserService.Application.CQRS.Group.Commands.CreateGroup;
using UserService.Application.CQRS.Group.Commands.GraduateGroup;

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

    public override async Task<GraduateGroupResponse> GraduateGroup(GraduateGroupRequest request,
        ServerCallContext context)
    {
        var command = new GraduateGroupCommand(request.Id, request.GraduatedTime.ToDateTime());

        var id = await _mediator.Send(command);

        return new GraduateGroupResponse
        {
            Id = id,
        };
    }
}