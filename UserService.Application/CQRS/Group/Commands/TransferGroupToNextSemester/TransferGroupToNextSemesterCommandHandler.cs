using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupToNextSemester;

public class TransferGroupToNextSemesterCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<TransferGroupToNextSemesterCommand, int>
{
    public async Task<int> Handle(TransferGroupToNextSemesterCommand request, CancellationToken cancellationToken)
    {
        var group = await DbContext.Groups.FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        group.CurrentSemester++;
        await DbContext.SaveChangesAsync(cancellationToken);

        return request.GroupId;
    }
}