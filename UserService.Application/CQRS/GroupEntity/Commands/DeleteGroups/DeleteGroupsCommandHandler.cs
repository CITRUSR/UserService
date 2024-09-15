using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;

public class DeleteGroupsCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DeleteGroupsCommand, List<GroupShortInfoDto>>
{
    public async Task<List<GroupShortInfoDto>> Handle(
        DeleteGroupsCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await DbContext
            .Groups.Include(x => x.Speciality)
            .Where(x => request.Ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (groups.Count != request.Ids.Count)
        {
            var notFoundIds = request.Ids.Except(groups.Select(x => x.Id));

            throw new GroupNotFoundException([.. notFoundIds]);
        }

        try
        {
            DbContext.Groups.RemoveRange(groups);

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
