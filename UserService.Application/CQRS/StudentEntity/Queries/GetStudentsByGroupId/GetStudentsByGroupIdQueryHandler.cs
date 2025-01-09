using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentsByGroupId;

public class GetStudentsByGroupIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GetStudentsByGroupIdQuery, List<StudentViewModel>>
{
    public async Task<List<StudentViewModel>> Handle(
        GetStudentsByGroupIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var group = await DbContext.Groups.FindAsync(
            new object?[] { request.GroupId, cancellationToken },
            cancellationToken: cancellationToken
        );

        if (group == null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        var students = await DbContext
            .Students.Include(x => x.Group)
            .ThenInclude(x => x.Speciality)
            .Where(x => x.GroupId == request.GroupId)
            .ToListAsync(cancellationToken);

        return students.Adapt<List<StudentViewModel>>();
    }
}
