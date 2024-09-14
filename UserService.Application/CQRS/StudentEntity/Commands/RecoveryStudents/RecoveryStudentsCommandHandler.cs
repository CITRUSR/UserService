using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.RecoveryStudents;

public class RecoveryStudentsCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<RecoveryStudentsCommand, List<StudentShortInfoDto>>
{
    public async Task<List<StudentShortInfoDto>> Handle(
        RecoveryStudentsCommand request,
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

            foreach (var student in students)
            {
                student.IsDeleted = false;
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
}
