using Grpc.Core;
using Mapster;
using MediatR;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Application.CQRS.GroupEntity.Commands.RecoveryGroups;
using UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;

namespace UserService.API.Services;

public class GroupService(IMediator mediator) : UserService.GroupService.GroupServiceBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<GroupShortInfo> CreateGroup(
        CreateGroupRequest request,
        ServerCallContext context
    )
    {
        var command = new CreateGroupCommand(
            request.SpecialityId,
            Guid.Parse(request.CuratorId),
            (byte)request.CurrentCourse,
            (byte)request.CurrentSemester,
            (byte)request.SubGroup,
            request.StartedAt.ToDateTime()
        );

        var group = await _mediator.Send(command);

        return group.Adapt<GroupShortInfo>();
    }

    public override async Task<DeleteGroupsResponse> DeleteGroups(
        DeleteGroupsRequest request,
        ServerCallContext context
    )
    {
        var command = new DeleteGroupsCommand([.. request.Ids]);

        var groups = await _mediator.Send(command);

        return groups.Adapt<DeleteGroupsResponse>();
    }

    public override async Task<SoftDeleteGroupsResponse> SoftDeleteGroups(
        SoftDeleteGroupsRequest request,
        ServerCallContext context
    )
    {
        var command = new SoftDeleteGroupsCommand([.. request.Ids]);

        var groups = await _mediator.Send(command);

        return groups.Adapt<SoftDeleteGroupsResponse>();
    }

    public override async Task<RecoveryGroupsResponse> RecoveryGroups(
        RecoveryGroupsRequest request,
        ServerCallContext context
    )
    {
        var command = new RecoveryGroupsCommand([.. request.Ids]);

        var groups = await _mediator.Send(command);

        return groups.Adapt<RecoveryGroupsResponse>();
    }

    public override async Task<GroupShortInfo> EditGroup(
        EditGroupRequest request,
        ServerCallContext context
    )
    {
        var command = new EditGroupCommand(
            request.Id,
            request.SpecialityId,
            Guid.Parse(request.CuratorId),
            (byte)request.CurrentCourse,
            (byte)request.CurrentSemester,
            (byte)request.SubGroup,
            request.IsDeleted
        );

        var group = await _mediator.Send(command);

        return group.Adapt<GroupShortInfo>();
    }

    public override async Task<GraduateGroupsResponse> GraduateGroups(
        GraduateGroupsRequest request,
        ServerCallContext context
    )
    {
        var command = new GraduateGroupsCommand(
            request.GroupsId.ToList(),
            request.GraduatedTime.ToDateTime()
        );

        var groups = await _mediator.Send(command);

        return groups.Adapt<GraduateGroupsResponse>();
    }

    public override async Task<TransferGroupsToNextSemesterResponse> TransferGroupsToNextSemester(
        TransferGroupsToNextSemesterRequest request,
        ServerCallContext context
    )
    {
        var command = new TransferGroupsToNextSemesterCommand(request.IdGroups.ToList());

        var groups = await _mediator.Send(command);

        return groups.Adapt<TransferGroupsToNextSemesterResponse>();
    }

    public override async Task<TransferGroupsToNextCourseResponse> TransferGroupsToNextCourse(
        TransferGroupsToNextCourseRequest request,
        ServerCallContext context
    )
    {
        var command = new TransferGroupsToNextCourseCommand(request.IdGroups.ToList());

        var groups = await _mediator.Send(command);

        return groups.Adapt<TransferGroupsToNextCourseResponse>();
    }

    public override async Task<GroupModel> GetGroupById(
        GetGroupByIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetGroupByIdQuery(request.Id);

        var group = await _mediator.Send(query);

        return group.Adapt<GroupModel>();
    }

    public override async Task<GetGroupsResponse> GetGroups(
        GetGroupsRequest request,
        ServerCallContext context
    )
    {
        var query = new GetGroupsQuery
        {
            PageSize = request.PageSize,
            Page = request.Page,
            SortState = (Application.CQRS.GroupEntity.Queries.GetGroups.GroupSortState)
                request.SortState,
            SearchString = request.SearchString,
            DeletedStatus = (Application.Enums.DeletedStatus)request.DeletedStatus,
        };

        var groups = await _mediator.Send(query);

        return groups.Adapt<GetGroupsResponse>();
    }
}
