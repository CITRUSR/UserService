using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;

public class DeleteGroupCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DeleteGroupCommand, int>
{
    public async Task<int> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await DbContext.Groups.FirstOrDefaultAsync(
            x => x.Id == request.Id,
            cancellationToken
        );

        if (group == null)
        {
            throw new GroupNotFoundException(request.Id);
        }

        DbContext.Groups.Remove(group);
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The group with id:{request.Id} is deleted");

        return request.Id;
    }
}
