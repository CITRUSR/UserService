using MediatR;
using Serilog;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.Student.Commands.EditStudent;

public class EditStudentCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<EditStudentCommand, Guid>
{
    public async Task<Guid> Handle(EditStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await DbContext.Students.FindAsync(new object?[] { request.Id, cancellationToken },
            cancellationToken: cancellationToken);

        if (student == null)
        {
            throw new StudentNotFoundException(request.Id);
        }

        var newGroup = await DbContext.Groups.FindAsync(new object?[] { request.GroupId, cancellationToken },
            cancellationToken: cancellationToken);

        if (newGroup == null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        var oldStudent = new Domain.Entities.Student
        {
            Id = student.Id,
            SsoId = student.SsoId,
            FirstName = student.FirstName,
            LastName = student.LastName,
            PatronymicName = student.PatronymicName,
            GroupId = student.GroupId,
            Group = student.Group,
            DroppedOutAt = student.DroppedOutAt,
        };

        var oldGroup = await DbContext.Groups.FindAsync(new object?[] { student.GroupId, cancellationToken },
            cancellationToken: cancellationToken);

        oldGroup?.Students.Remove(student);

        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.PatronymicName = request.PatronymicName;
        student.GroupId = request.GroupId;
        student.Group = newGroup;

        newGroup.Students.Add(student);

        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information(
            $"The student with Id:{student.Id} is edit." + "Old state: {@oldStudent}. New state: {@student}");

        return student.Id;
    }
}