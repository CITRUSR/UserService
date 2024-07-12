using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupToNextCourse;

public class TransferGroupToNextCourseCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<TransferGroupToNextCourseCommand, int>
{
    public async Task<int> Handle(TransferGroupToNextCourseCommand request, CancellationToken cancellationToken)
    {
        var group = await DbContext.Groups.FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        group.CurrentCourse++;
        await DbContext.SaveChangesAsync(cancellationToken);

        return request.GroupId;
    }
}