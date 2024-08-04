using MediatR;
using UserService.Application.Abstraction;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.TeacherEntity.Commands.CreateTeacher;

public class CreateTeacherCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<CreateTeacherCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateTeacherCommand request,
        CancellationToken cancellationToken
    )
    {
        //Todo: check for null room when schedule service be ready
        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            SsoId = request.SsoId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PatronymicName = request.PatronymicName,
            RoomId = request.RoomId,
        };

        await DbContext.Teachers.AddAsync(teacher, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return teacher.Id;
    }
}
