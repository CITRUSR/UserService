using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;

public class TransferGroupsToNextSemesterCommandHandler(IAppDbContext dbContext) : HandlerBase(dbContext),
    IRequestHandler<TransferGroupsToNextSemesterCommand, List<Group>>
{
    public async Task<List<Group>> Handle(TransferGroupsToNextSemesterCommand request,
        CancellationToken cancellationToken)
    {
        var groups = await DbContext.Groups.Where(x => request.IdGroups.Contains(x.Id))
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

            List<Group> invalidGroups = new List<Group>();

            IncreaseSemester(groups, invalidGroups);

            if (invalidGroups.Any())
            {
                throw new GroupSemesterOutOfRangeException(invalidGroups.ToArray());
            }

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }


        return groups;
    }

    private void IncreaseSemester(List<Group> groups, List<Group> invalidGroupsId)
    {
        foreach (var group in groups)
        {
            var maxSemester = Math.Ceiling(group.Speciality.DurationMonths / 6.0);

            if (group.CurrentSemester + 1 > maxSemester)
            {
                invalidGroupsId.Add(group);
            }
            else
            {
                group.CurrentSemester++;
            }
        }
    }
}