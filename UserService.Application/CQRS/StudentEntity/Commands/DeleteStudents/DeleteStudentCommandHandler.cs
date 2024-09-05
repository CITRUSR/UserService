using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;

public class DeleteStudentsCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<DeleteStudentsCommand, List<StudentShortInfoDto>>
{
    public async Task<List<StudentShortInfoDto>> Handle(
        DeleteStudentsCommand request,
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

            DbContext.Students.RemoveRange(students);

            await DbContext.SaveChangesAsync(cancellationToken);

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        Log.Information(
            $"The students with Id:{string.Join(", ", students.Select(x => x.Id))} are deleted"
        );

        return students.Adapt<List<StudentShortInfoDto>>();
    }
}
