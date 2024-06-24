using MediatR;
using Serilog;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS;

namespace UserService.Application.Student.Commands.CreateStudent;

public class CreateStudentCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<CreateStudentCommand, long>
{
    public async Task<long> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var group = await DbContext.Groups.FindAsync(new object?[] { request.GroupId, cancellationToken },
            cancellationToken: cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        var student = new Domain.Entities.Student
        {
            Id = request.Id,
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