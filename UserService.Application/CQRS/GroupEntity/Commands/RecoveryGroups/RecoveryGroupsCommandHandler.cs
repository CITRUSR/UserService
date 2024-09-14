using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.RecoveryGroups;

public class RecoveryGroupsCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<RecoveryGroupsCommand, List<GroupShortInfoDto>>
{
    public async Task<List<GroupShortInfoDto>> Handle(
        RecoveryGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await DbContext
            .Groups.Include(x => x.Speciality)
            .Where(x => request.GroupIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (request.GroupIds.Count != groups.Count)
        {
            var notFoundIds = request.GroupIds.Except(groups.Select(x => x.Id));

            throw new GroupNotFoundException([.. notFoundIds]);
        }

        try
        {
            await DbContext.BeginTransactionAsync();

            foreach (var group in groups)
            {
                group.IsDeleted = false;
            }

            await DbContext.SaveChangesAsync(cancellationToken);

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        return groups.Adapt<List<GroupShortInfoDto>>();
    }
}
