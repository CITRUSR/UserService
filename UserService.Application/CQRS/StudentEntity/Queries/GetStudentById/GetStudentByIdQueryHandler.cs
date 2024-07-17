using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;

public class GetStudentByIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetStudentByIdQuery, Student>
{
    public async Task<Student> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await DbContext.Students.FindAsync(new object?[] { request.Id, cancellationToken },
            cancellationToken: cancellationToken);

        if (student == null)
        {
            throw new StudentNotFoundException(request.Id);
        }

        return student;
    }
}