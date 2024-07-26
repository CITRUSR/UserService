using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroup;

public class GraduateGroupCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GraduateGroupCommand, int>
{
    public async Task<int> Handle(GraduateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await DbContext.Groups.FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        group.GraduatedAt = request.GraduatedTime;

        foreach (var student in group.Students)
        {
            student.DroppedOutAt ??= request.GraduatedTime;
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}