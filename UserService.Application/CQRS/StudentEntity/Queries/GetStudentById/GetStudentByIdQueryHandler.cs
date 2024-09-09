using Mapster;
using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;

public class GetStudentByIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GetStudentByIdQuery, StudentDto>
{
    public async Task<StudentDto> Handle(
        GetStudentByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var student = await DbContext.Students.FindAsync(
            new object?[] { request.Id, cancellationToken },
            cancellationToken: cancellationToken
        );

        if (student == null)
        {
            throw new StudentNotFoundException(request.Id);
        }

        return student.Adapt<StudentDto>();
    }
}
