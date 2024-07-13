using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupsToNextSemester;

public class TransferGroupsToNextSemesterCommandHandler(IAppDbContext dbContext) : HandlerBase(dbContext),
    IRequestHandler<TransferGroupsToNextSemesterCommand, List<int>>
{
    public async Task<List<int>> Handle(TransferGroupsToNextSemesterCommand request,
        CancellationToken cancellationToken)
    {
        var groups = await DbContext.Groups.ToListAsync(cancellationToken);
        List<int> invalidGroupsId = new List<int>();

        if (request.IdGroups != null && request.IdGroups.Any())
        {
            groups = await DbContext.Groups.Where(x => request.IdGroups.Contains(x.Id))
                .ToListAsync(cancellationToken);
        }

        IncreaseSemester(groups, invalidGroupsId);

        if (invalidGroupsId.Any())
        {
            throw new GroupSemesterOutOfRangeException(invalidGroupsId.ToArray());
        }

        return groups.Select(x => x.Id).ToList();
    }

    private void IncreaseSemester(List<Domain.Entities.Group> groups, List<int> invalidGroupsId)
    {
        foreach (var group in DbContext.Groups)
        {
            var maxSemester = Math.Ceiling(group.Speciality.DurationMonths / 6.0);

            if (group.CurrentSemester + 1 > maxSemester)
            {
                invalidGroupsId.Add(group.Id);
            }
            else
            {
                group.CurrentSemester++;
            }
        }
    }
}