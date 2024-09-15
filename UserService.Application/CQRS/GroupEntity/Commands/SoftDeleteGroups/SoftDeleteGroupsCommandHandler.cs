using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;

public class SoftDeleteGroupsCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<SoftDeleteGroupsCommand, List<GroupShortInfoDto>>
{
    public async Task<List<GroupShortInfoDto>> Handle(
        SoftDeleteGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await DbContext
            .Groups.Include(x => x.Speciality)
            .Where(x => request.GroupsId.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (request.GroupsId.Count != groups.Count)
        {
            var notFoundIds = request.GroupsId.Except(groups.Select(x => x.Id));
            throw new GroupNotFoundException([.. notFoundIds]);
        }

        try
        {
            await DbContext.BeginTransactionAsync();

            foreach (var group in groups)
            {
                group.IsDeleted = true;
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
