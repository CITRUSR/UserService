﻿using Grpc.Core;
using MediatR;
using UserService.API.Mappers;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroup;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Domain.Entities;

namespace UserService.API.Services;

public class GroupService(IMediator mediator, IMapper<Group, GroupModel> mapper)
    : UserService.GroupService.GroupServiceBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper<Group, GroupModel> _mapper = mapper;

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

    public override async Task<EditGroupResponse> EditGroup(EditGroupRequest request, ServerCallContext context)
    {
        var command = new EditGroupCommand(request.Id, request.SpecialityId, Guid.Parse(request.CuratorId),
            (byte)request.CurrentCourse,
            (byte)request.CurrentSemester,
            (byte)request.SubGroup);

        var id = await _mediator.Send(command);

        return new EditGroupResponse
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

    public override async Task<GroupModel> GetGroupById(GetGroupByIdRequest request, ServerCallContext context)
    {
        var query = new GetGroupByIdQuery(request.Id);

        var group = await _mediator.Send(query);

        return _mapper.Map(group);
    }

    public override async Task<GetGroupsResponse> GetGroups(GetGroupsRequest request, ServerCallContext context)
    {
        var query = new GetGroupsQuery
        {
            PageSize = request.PageSize,
            Page = request.Page,
            SortState = (Application.CQRS.GroupEntity.Queries.GetGroups.GroupSortState)request.SortState,
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