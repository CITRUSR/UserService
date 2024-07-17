using MediatR;
using Serilog;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;

public class CreateStudentCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<CreateStudentCommand, Guid>
{
    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var group = await DbContext.Groups.FindAsync(new object?[] { request.GroupId, cancellationToken },
            cancellationToken: cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        var student = new Student
        {
            Id = Guid.NewGuid(),
            SsoId = request.SsoId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PatronymicName = request.PatronymicName,
            GroupId = request.GroupId,
            Group = group,
        };

        await DbContext.Students.AddAsync(student, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The student with Id:{student.Id} is created");

        return student.Id;
    }
}