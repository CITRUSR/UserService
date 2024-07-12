using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupsToNextCourse;

public class TransferGroupsToNextCourseCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<TransferGroupsToNextCourseCommand, List<int>>
{
    public async Task<List<int>> Handle(TransferGroupsToNextCourseCommand request, CancellationToken cancellationToken)
    {
        var groups = await DbContext.Groups.ToListAsync(cancellationToken);

        if (request.IdGroups != null && request.IdGroups.Any())
        {
            groups = groups.Where(x => request.IdGroups.Contains(x.Id)).ToList();

            foreach (var group in groups)
            {
                group.CurrentCourse++;
            }

            return groups.Select(x => x.Id).ToList();
        }

        foreach (var group in groups)
        {
            group.CurrentCourse++;
        }

        return groups.Select(x => x.Id).ToList();
    }
}