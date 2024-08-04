using Grpc.Core;
using MediatR;
using UserService.API.Mappers;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Domain.Entities;

namespace UserService.API.Services;

public class GroupService(
    IMediator mediator,
    IMapper<Group, GroupModel> mapper,
    IMapper<Group, ChangeGroupResponseModel> changeGroupResponseMapper
) : UserService.GroupService.GroupServiceBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper<Group, GroupModel> _mapper = mapper;
    private readonly IMapper<Group, ChangeGroupResponseModel> _changeGroupResponseMapper =
        changeGroupResponseMapper;

    public override async Task<CreateGroupResponse> CreateGroup(
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

        return new CreateGroupResponse { Group = _changeGroupResponseMapper.Map(group) };
    }

    public override async Task<DeleteGroupResponse> DeleteGroup(
        DeleteGroupRequest request,
        ServerCallContext context
    )
    {
        var command = new DeleteGroupCommand(request.Id);

        var id = await _mediator.Send(command);

        return new DeleteGroupResponse { Id = id };
    }

    public override async Task<GroupModel> EditGroup(
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
            (byte)request.SubGroup
        );

        var group = await _mediator.Send(command);

        return _mapper.Map(group);
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

        return new GraduateGroupsResponse
        {
            Groups = { groups.Select(x => _changeGroupResponseMapper.Map(x)) },
        };
    }

    public override async Task<TransferGroupsToNextSemesterResponse> TransferGroupsToNextSemester(
        TransferGroupsToNextSemesterRequest request,
        ServerCallContext context
    )
    {
        var command = new TransferGroupsToNextSemesterCommand(request.IdGroups.ToList());

        var groups = await _mediator.Send(command);

        return new TransferGroupsToNextSemesterResponse
        {
            Groups = { groups.Select(x => _changeGroupResponseMapper.Map(x)) }
        };
    }

    public override async Task<TransferGroupsToNextCourseResponse> TransferGroupsToNextCourse(
        TransferGroupsToNextCourseRequest request,
        ServerCallContext context
    )
    {
        var command = new TransferGroupsToNextCourseCommand(request.IdGroups.ToList());

        var groups = await _mediator.Send(command);

        return new TransferGroupsToNextCourseResponse
        {
            Groups = { groups.Select(x => _changeGroupResponseMapper.Map(x)) }
        };
    }

    public override async Task<GroupModel> GetGroupById(
        GetGroupByIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetGroupByIdQuery(request.Id);

        var group = await _mediator.Send(query);

        return _mapper.Map(group);
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
        };

        var groups = await _mediator.Send(query);

        return new GetGroupsResponse
        {
            Groups = { groups.Items.Select(x => _mapper.Map(x)) },
            LastPage = groups.MaxPage,
        };
    }
}
