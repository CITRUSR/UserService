using Mapster;
using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.EditStudent;

public class EditStudentCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<EditStudentCommand, StudentShortInfoDto>
{
    public async Task<StudentShortInfoDto> Handle(
        EditStudentCommand request,
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

        var newGroup = await DbContext.Groups.FindAsync(
            new object?[] { request.GroupId, cancellationToken },
            cancellationToken: cancellationToken
        );

        if (newGroup == null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        var oldGroup = await DbContext.Groups.FindAsync(
            new object?[] { student.GroupId, cancellationToken },
            cancellationToken: cancellationToken
        );

        oldGroup?.Students.Remove(student);

        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.PatronymicName = request.PatronymicName;
        student.GroupId = request.GroupId;
        student.Group = newGroup;
        student.IsDeleted = request.IsDeleted;

        newGroup.Students.Add(student);

        await DbContext.SaveChangesAsync(cancellationToken);

        return student.Adapt<StudentShortInfoDto>();
    }
}
