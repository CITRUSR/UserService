using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;

public class GraduateGroupsCommandHandler(
    IAppDbContext dbContext,
    IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>> dropOutStudentsHandler
) : HandlerBase(dbContext), IRequestHandler<GraduateGroupsCommand, List<GroupShortInfoDto>>
{
    private readonly IRequestHandler<
        DropOutStudentsCommand,
        List<StudentShortInfoDto>
    > _dropOutStudentsHandler = dropOutStudentsHandler;

    public async Task<List<GroupShortInfoDto>> Handle(
        GraduateGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await DbContext
            .Groups.Where(x => request.GroupsId.Contains(x.Id))
            .Include(x => x.Speciality)
            .ToListAsync(cancellationToken);

        if (groups.Count < request.GroupsId.Count)
        {
            var notFoundGroups = groups.Where(x => !request.GroupsId.Contains(x.Id));
            throw new GroupNotFoundException(notFoundGroups.Select(x => x.Id).ToArray());
        }

        try
        {
            await DbContext.BeginTransactionAsync();

            await GraduateGroups(groups, request.GraduatedTime);

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        return groups.Adapt<List<GroupShortInfoDto>>();
    }

    private async Task GraduateGroups(IEnumerable<Group> groups, DateTime graduatedTime)
    {
        var invalidGroups = new List<Group>();

        foreach (var group in groups)
        {
            if (group.GraduatedAt == null)
            {
                group.GraduatedAt = graduatedTime;

                await DropOutStudentsInGroup(group, graduatedTime);
            }
            else
            {
                invalidGroups.Add(group);
            }
        }

        if (invalidGroups.Count != 0)
        {
            throw new GroupAlreadyGraduatedException([.. invalidGroups]);
        }
    }

    private async Task DropOutStudentsInGroup(Group group, DateTime droppedOutTime)
    {
        var studentsIds = group
            .Students.Where(x => x.DroppedOutAt == null)
            .Select(x => x.Id)
            .ToList();

        if (studentsIds.Count != 0)
        {
            var dropOutStudentsCommand = new DropOutStudentsCommand(studentsIds, droppedOutTime);

            await _dropOutStudentsHandler.Handle(dropOutStudentsCommand, default);
        }
    }
}
