using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Student.Quereis;

namespace UserService.Application.CQRS.Student.Queries.GetStudent;

public class GetStudentByIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetStudentByIdQuery, Domain.Entities.Student>
{
    public async Task<Domain.Entities.Student> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
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