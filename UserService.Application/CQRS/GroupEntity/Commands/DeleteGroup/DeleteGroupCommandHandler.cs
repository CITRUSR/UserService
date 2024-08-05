using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;

public class DeleteGroupCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DeleteGroupCommand, Group>
{
    public async Task<Group> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await DbContext
            .Groups.Include(x => x.Speciality)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(request.Id);
        }

        DbContext.Groups.Remove(group);
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The group {group.ToString()} is deleted");

        return group;
    }
}
