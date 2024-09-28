using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;

public class TransferGroupsToNextSemesterCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<TransferGroupsToNextSemesterCommand, List<GroupShortInfoDto>>
{
    public async Task<List<GroupShortInfoDto>> Handle(
        TransferGroupsToNextSemesterCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await DbContext
            .Groups.Where(x => request.IdGroups.Contains(x.Id))
            .Include(x => x.Speciality)
            .ToListAsync(cancellationToken);

        if (groups.Count < request.IdGroups.Count)
        {
            var notFoundGroups = groups.Where(x => !request.IdGroups.Contains(x.Id));
            throw new GroupNotFoundException(notFoundGroups.Select(x => x.Id).ToArray());
        }

        try
        {
            await DbContext.BeginTransactionAsync();

            if (!TryIncreaseSemester(groups, out var invalidGroups))
            {
                throw new GroupSemesterOutOfRangeException([.. invalidGroups]);
            }

            await DbContext.SaveChangesAsync(cancellationToken);

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        return groups.Adapt<List<GroupShortInfoDto>>();
    }

    private bool TryIncreaseSemester(List<Group> groups, out List<Group> invalidGroups)
    {
        invalidGroups = [];

        foreach (var group in groups)
        {
            var maxSemester = Math.Ceiling(group.Speciality.DurationMonths / 6.0);

            if (group.CurrentSemester + 1 > maxSemester)
            {
                invalidGroups.Add(group);
            }
            else
            {
                group.CurrentSemester++;
            }
        }

        if (invalidGroups.Count != 0)
        {
            return false;
        }

        return true;
    }
}
