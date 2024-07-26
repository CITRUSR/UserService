using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;

public class GetStudentBySsoIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetStudentBySsoIdQuery, Student>
{
    public async Task<Student> Handle(GetStudentBySsoIdQuery request,
        CancellationToken cancellationToken)
    {
        var student = await DbContext.Students.FirstOrDefaultAsync(x => x.SsoId == request.SsoId, cancellationToken);

        if (student == null)
        {
            throw new StudentNotFoundException(request.SsoId);
        }

        return student;
    }
}