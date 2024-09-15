using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;

public class DropOutStudentsCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>>
{
    public async Task<List<StudentShortInfoDto>> Handle(
        DropOutStudentsCommand request,
        CancellationToken cancellationToken
    )
    {
        var students = await DbContext
            .Students.Where(x => request.StudentIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (request.StudentIds.Count != students.Count)
        {
            var notFoundStudentIds = request.StudentIds.Except(students.Select(x => x.Id));

            throw new StudentNotFoundException([.. notFoundStudentIds]);
        }

        try
        {
            await DbContext.BeginTransactionAsync();

            if (!TryDropOutStudents(students, request.DroppedOutTime, out var invalidStudentIds))
            {
                throw new StudentAlreadyDroppedOutException([.. invalidStudentIds]);
            }

            await DbContext.SaveChangesAsync(cancellationToken);

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        return students.Adapt<List<StudentShortInfoDto>>();
    }

    private bool TryDropOutStudents(
        List<Student> students,
        DateTime droppedTime,
        out List<Student> invalidStudentIds
    )
    {
        invalidStudentIds = [];

        foreach (var student in students)
        {
            if (student.DroppedOutAt != null)
            {
                invalidStudentIds.Add(student);
                continue;
            }

            student.DroppedOutAt = droppedTime;
        }

        return invalidStudentIds.Count == 0;
    }
}
