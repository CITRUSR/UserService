using MediatR;
using Serilog;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.Student.Commands.DropOutStudent;

public class DropOutStudentCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<DropOutStudentCommand, long>
{
    public async Task<long> Handle(DropOutStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await DbContext.Students.FindAsync(new object?[] { request.Id, cancellationToken },
            cancellationToken: cancellationToken);

        if (student == null)
        {
            throw new NotFoundException($"The user with Id:{request.Id} is not found");
        }

        student.DroppedOutAt = request.DroppedOutTime;
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The user with Id:{request.Id} is dropped out");

        return request.Id;
    }
}