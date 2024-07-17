using MediatR;
using Serilog;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;

public class CreateGroupCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<CreateGroupCommand, int>
{
    public async Task<int> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var speciality = await DbContext.Specialities.FindAsync(
            new object?[] { request.SpecialityId, cancellationToken }, cancellationToken: cancellationToken);

        if (speciality == null)
        {
            throw new SpecialityNotFoundException(request.SpecialityId);
        }

        var curator = await DbContext.Teachers.FindAsync(new object?[] { request.CuratorId, cancellationToken },
            cancellationToken: cancellationToken);

        if (curator == null)
        {
            throw new TeacherNotFoundException(request.CuratorId);
        }

        var group = new Group()
        {
            SpecialityId = request.SpecialityId,
            CuratorId = request.CuratorId,
            CurrentCourse = request.CurrentCourse,
            StartedAt = request.StartedAt,
            SubGroup = request.SubGroup,
            CurrentSemester = request.CurrentSemester,
        };

        await DbContext.Groups.AddAsync(group, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The group with id:{group.Id} is created");

        return group.Id;
    }
}