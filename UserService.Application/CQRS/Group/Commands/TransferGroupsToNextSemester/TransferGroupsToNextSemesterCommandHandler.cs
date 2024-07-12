using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupsToNextSemester;

public class TransferGroupsToNextSemesterCommandHandler(IAppDbContext dbContext) : HandlerBase(dbContext),
    IRequestHandler<TransferGroupsToNextSemesterCommand, List<int>>
{
    public async Task<List<int>> Handle(TransferGroupsToNextSemesterCommand request,
        CancellationToken cancellationToken)
    {
        var groups = await DbContext.Groups.ToListAsync(cancellationToken);

        if (request.IdGroups != null && request.IdGroups.Any())
        {
            groups = await DbContext.Groups.Where(x => request.IdGroups.Contains(x.Id))
                .ToListAsync(cancellationToken);

            foreach (var group in groups)
            {
                group.CurrentSemester++;
            }

            return groups.Select(x => x.Id).ToList();
        }

        foreach (var group in DbContext.Groups)
        {
            group.CurrentSemester++;
        }

        return groups.Select(x => x.Id).ToList();
    }
}