using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;

public class GraduateGroupsCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GraduateGroupsCommand, List<GroupShortInfoDto>>
{
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

            if (!TryGraduateGroups(groups, request.GraduatedTime, out var invalidGroups))
            {
                throw new GroupAlreadyGraduatedException([.. invalidGroups]);
            }

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        return groups.Adapt<List<GroupShortInfoDto>>();
    }

    private bool TryGraduateGroups(
        IEnumerable<Group> groups,
        DateTime graduatedTime,
        out List<Group> invalidGroups
    )
    {
        invalidGroups = new List<Group>();

        foreach (var group in groups)
        {
            if (group.GraduatedAt == null)
            {
                group.GraduatedAt = graduatedTime;
                DropOutStudents(group, graduatedTime);
            }
            else
            {
                invalidGroups.Add(group);
            }
        }

        if (invalidGroups.Count != 0)
        {
            return false;
        }

        return true;
    }

    private void DropOutStudents(Group group, DateTime graduatedTime)
    {
        foreach (var student in group.Students)
        {
            student.DroppedOutAt ??= graduatedTime;
        }
    }
}
