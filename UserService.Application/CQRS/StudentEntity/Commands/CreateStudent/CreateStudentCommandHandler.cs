﻿using Mapster;
using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;

public class CreateStudentCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<CreateStudentCommand, StudentShortInfoDto>
{
    public async Task<StudentShortInfoDto> Handle(
        CreateStudentCommand request,
        CancellationToken cancellationToken
    )
    {
        var group = await DbContext.Groups.FindAsync(
            new object?[] { request.GroupId, cancellationToken },
            cancellationToken: cancellationToken
        );

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

        return student.Adapt<StudentShortInfoDto>();
    }
}
