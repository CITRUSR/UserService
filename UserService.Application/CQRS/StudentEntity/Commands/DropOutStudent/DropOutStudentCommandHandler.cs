using Mapster;
using MediatR;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;

public class DropOutStudentCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DropOutStudentCommand, StudentShortInfoDto>
{
    public async Task<StudentShortInfoDto> Handle(
        DropOutStudentCommand request,
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

        student.DroppedOutAt = request.DroppedOutTime;
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The student with Id:{request.Id} is dropped out");

        return student.Adapt<StudentShortInfoDto>();
    }
}
