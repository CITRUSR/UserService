using Grpc.Core;
using MediatR;
using UserService.Application.CQRS.Group.Commands.CreateGroup;
using UserService.Application.CQRS.Group.Commands.DeleteGroup;
using UserService.Application.CQRS.Group.Commands.GraduateGroup;
using UserService.Application.CQRS.Group.Commands.TransferGroupsToNextCourse;
using UserService.Application.CQRS.Group.Commands.TransferGroupsToNextSemester;

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

    public override async Task<DeleteGroupResponse> DeleteGroup(DeleteGroupRequest request, ServerCallContext context)
    {
        var command = new DeleteGroupCommand(request.Id);

        var id = await _mediator.Send(command);

        return new DeleteGroupResponse
        {
            Id = id
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

    public override async Task<TransferGroupsToNextSemesterResponse> TransferGroupsToNextSemester(
        TransferGroupsToNextSemesterRequest request, ServerCallContext context)
    {
        var command = new TransferGroupsToNextSemesterCommand
        {
            IdGroups = request.IdGroups.ToList(),
        };

        var ids = await _mediator.Send(command);

        return new TransferGroupsToNextSemesterResponse
        {
            IdGroups = { ids }
        };
    }

    public override async Task<TransferGroupsToNextCourseResponse> TransferGroupsToNextCourse(
        TransferGroupsToNextCourseRequest request, ServerCallContext context)
    {
        var command = new TransferGroupsToNextCourseCommand
        {
            IdGroups = request.IdGroups.ToList(),
        };

        var ids = await _mediator.Send(command);

        return new TransferGroupsToNextCourseResponse
        {
            IdGroups = { ids }
        };
    }
}