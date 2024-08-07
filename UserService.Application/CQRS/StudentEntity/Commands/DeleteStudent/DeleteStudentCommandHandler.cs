﻿using MediatR;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;

public class DeleteStudentCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DeleteStudentCommand, Guid>
{
    public async Task<Guid> Handle(
        DeleteStudentCommand request,
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

        var group = await DbContext.Groups.FindAsync(
            new object?[] { student.GroupId, cancellationToken },
            cancellationToken: cancellationToken
        );

        group?.Students.Remove(student);

        DbContext.Students.Remove(student);
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The student with Id:{request.Id} is deleted");

        return request.Id;
    }
}
