using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;

public class TransferGroupsToNextCourseCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<TransferGroupsToNextCourseCommand, List<int>>
{
    public async Task<List<int>> Handle(TransferGroupsToNextCourseCommand request, CancellationToken cancellationToken)
    {
        var groups = await DbContext.Groups.ToListAsync(cancellationToken);

        List<int> invalidGroupsId = new List<int>();

        if (request.IdGroups != null && request.IdGroups.Any())
        {
            groups = groups.Where(x => request.IdGroups.Contains(x.Id)).ToList();
        }

        IncreaseCourse(groups, invalidGroupsId);

        if (invalidGroupsId.Any())
        {
            throw new GroupCourseOutOfRangeException(invalidGroupsId.ToArray());
        }

        return groups.Select(x => x.Id).ToList();
    }

    private void IncreaseCourse(List<Group> groups, List<int> invalidGroupsId)
    {
        foreach (var group in groups)
        {
            var maxCourse = Math.Ceiling(group.Speciality.DurationMonths / 12.0);

            if (group.CurrentCourse + 1 > maxCourse)
            {
                invalidGroupsId.Add(group.Id);
            }
            else
            {
                group.CurrentCourse++;
            }
        }
    }
}