using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;

public class GraduateGroupsCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GraduateGroupsCommand, List<Group>>
{
    public async Task<List<Group>> Handle(GraduateGroupsCommand request, CancellationToken cancellationToken)
    {
        var groups = DbContext.Groups.Where(x => request.GroupsId.Contains(x.Id))
            .Include(x => x.Speciality);

        if (groups.Count() < request.GroupsId.Count)
        {
            var notFoundGroups = groups.Where(x => !request.GroupsId.Contains(x.Id));
            throw new GroupNotFoundException(notFoundGroups.Select(x => x.Id).ToArray());
        }

        foreach (var group in groups)
        {
            group.GraduatedAt = request.GraduatedTime;
        }

        foreach (var group in groups)
        {
            foreach (var student in group.Students)
            {
                student.DroppedOutAt ??= request.GraduatedTime;
            }
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return await groups.ToListAsync(cancellationToken);
    }
}