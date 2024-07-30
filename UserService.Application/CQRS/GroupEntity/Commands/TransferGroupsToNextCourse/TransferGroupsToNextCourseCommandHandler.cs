using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;

public class TransferGroupsToNextCourseCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<TransferGroupsToNextCourseCommand, List<Group>>
{
    public async Task<List<Group>> Handle(TransferGroupsToNextCourseCommand request,
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

            List<Group> invalidGroups = [];

            IncreaseCourse(groups, invalidGroups);

            if (invalidGroups.Any())
            {
                throw new GroupCourseOutOfRangeException(invalidGroups.ToArray());
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

    private void IncreaseCourse(List<Group> groups, ICollection<Group> invalidGroups)
    {
        foreach (var group in groups)
        {
            var maxCourse = Math.Ceiling(group.Speciality.DurationMonths / 12.0);

            if (group.CurrentCourse + 1 > maxCourse)
            {
                invalidGroups.Add(group);
            }
            else
            {
                group.CurrentCourse++;
            }
        }
    }
}